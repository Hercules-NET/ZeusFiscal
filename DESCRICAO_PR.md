# .NET 8 + .NET 10, fim dos warnings de obsoleto e correção de certificados A3 (PR #126 / issue #42)

## TL;DR

O diff é grande (TFMs em ~30 `.csproj`, nuspecs, CI e a pasta `DFe.Utils/Assinatura` reorganizada), mas **tecnicamente nada público foi quebrado**:

- **API pública:** comparamos os metadados das DLLs do `master` com os desta branch em **todas as bibliotecas × todos os TFMs em comum (102 comparações)**: **0 membros públicos removidos ou alterados** (incluindo nomes de parâmetros e valores padrão). Houve apenas **2 adições**.
- **.NET Framework:** quem usa `net462` e hoje funciona **executa o mesmo código de certificado de antes**. O código novo só entra onde o antigo lançava exceção (chave CNG — o bug do PR #126).
- **.NET 8 / .NET 10:** APIs modernas e **zero warnings de API obsoleta** (SYSLIB0014/0021/0028/0057 e CS0618).

---

## Contexto

- **.NET 8 e .NET 9 saem de suporte em 10/11/2026.** O .NET 8 é LTS, então segue suportado até lá; o .NET 9 (STS) foi removido e o .NET 10 (LTS) passa a ser o alvo moderno. Apps em .NET 9 usam os builds `net8.0`, que são compatíveis.
- **PR #126:** certificados A3 mais novos (chave em provedor CNG/KSP) falhavam no .NET Framework com `CryptographicException: Tipo de provedor inválido especificado`, porque a assinatura usava `X509Certificate2.PrivateKey`. O PR corrigia só a NF-e e não compilava; o mesmo problema existia na assinatura de CT-e, MDF-e, eventos e inutilização.
- **Issue #42:** construtores de `X509Certificate2` que carregam PFX estão obsoletos no .NET 9+ (SYSLIB0057).
- **Vulnerabilidades:** `System.Security.Cryptography.Xml 6.0.1` trazia `System.Security.Cryptography.Pkcs 6.0.1` e `System.Formats.Asn1 6.0.0` com vulnerabilidade **alta** para todos os TFMs.

---

## Certificados: .NET Framework × .NET 8 / 10

### O que cada API devolve no .NET Framework 4.8 (medido)

| Certificado | `PrivateKey` (master) | `GetRSAPrivateKey()` |
|---|---|---|
| A1 de PFX (flags padrão da lib) | `RSACryptoServiceProvider` (CAPI) | `RSACng` (CNG) |
| A1 de PFX (flags do `CertificadoDigitalUtils`) | `RSACryptoServiceProvider` | `RSACng` |
| Chave em CSP da Microsoft | `RSACryptoServiceProvider` | `RSACng` |
| Chave em CNG/KSP (A3 novo, caso do PR #126) | **exceção "Tipo de provedor inválido"** | `RSACng` |

Trocar tudo para `GetRSAPrivateKey()` (como no PR #126) mudaria silenciosamente o caminho da CryptoAPI de **todos** os usuários de .NET Framework. Por isso a regra adotada foi: **no .NET Framework, o código de antes roda primeiro; o novo só entra onde o antigo falhava.**

### Como ficou

| Ponto | .NET Framework (`net462`) | .NET 8 / 10 (e `netstandard2.0`) |
|---|---|---|
| **Assinatura XML** | `PrivateKey` como no master, sem Dispose. Se der "Tipo de provedor inválido" (chave CNG) → `GetRSAPrivateKey()` | `GetRSAPrivateKey()` + Dispose |
| **PIN do A3** | Código do master: `PrivateKey` → `CryptAcquireContext`/`CryptSetProvParam`. Se falhar por ser CNG → ramo CNG novo (`SmartCardPin`) | Provedor lido do certificado (`CRYPT_KEY_PROV_INFO`): CSP → mesmo caminho do master; CNG → ramo CNG |
| **`IsA3`** | Código do master (`CspKeyContainerInfo`). Se falhar → checagem CNG nova | CSP: `CspKeyContainerInfo`; CNG: `NCRYPT_IMPL_TYPE_PROPERTY` |
| **Carga do PFX** | Construtor do master (sem mudança) | `X509CertificateLoader` preservando provedor, nome da chave e alias gravados no PFX (como o construtor fazia) |
| **QR Code (PKCS#1)** | O master já usava `GetRSAPrivateKey()` aqui. Mantido; os bytes gerados são idênticos (há teste comparando com o formatter antigo) | Igual |

### Resultado para cada público

- **Quem usa .NET Framework e hoje funciona:** executa exatamente o mesmo código de antes.
- **Quem tem A3 CNG (o bug do PR #126):** passa a funcionar pelo fallback — na NF-e **e** no CT-e/MDF-e.
- **Quem usa .NET 8/10:** APIs modernas e zero warnings de obsoleto.

---

## Target frameworks

| Projetos | Antes | Agora |
|---|---|---|
| Libs NF-e/CT-e/MDF-e/DF-e | `net462;netstandard2.0;net8.0;net9.0;net10.0` | `net462;netstandard2.0;net8.0;net10.0` |
| QuestPdf, FastReport.Skia (NF-e/CT-e/MDF-e), PdfClown | `net8.0;net9.0;net10.0` | `net8.0;net10.0` |
| NFe.Danfe.Nativo | `net462;netstandard2.0;net8.0;net9.0-windows;net10.0-windows` | `net462;netstandard2.0;net8.0;net10.0-windows` |
| Apps de teste e projetos de teste | `net8.0`/`net9.0` | `net10.0` (testes de assinatura também em `net48`) |

---

## Outras mudanças

### API pública adicionada (nada removido)
- `AssinaturaDigital.AssinaXml(string xml, string id, X509Certificate2, string signatureMethod, string digestMethod)` — núcleo único da assinatura XML, usado por NF-e (`Assinador`), CT-e e MDF-e.
- `CertificadoDigital.ObterAssinaturaPkcs1(X509Certificate2, byte[])` — substitui 3 cópias privadas de `CreateSignaturePkcs1` (CT-e, CT-e OS, MDF-e).

Os demais helpers novos (chave RSA compatível com CSP/CNG, chave legada do .NET Framework, TLS dos serviços CT-e/MDF-e antes repetido em 17 construtores) são `internal`.

### HTTP / TLS
- **Transporte igual ao de antes:** `RequestSefazDefault`/HttpWebRequest antes do .NET 9 e `RequestSefazHttpClientHandler`/HttpClient no .NET 9+. A escolha passou a considerar a versão do runtime, porque apps .NET 9 agora consomem o build `net8.0`.
- **`ServicePointManager` mantido como no master em todos os TFMs** (com `#pragma` para o SYSLIB0014): além do HttpWebRequest, ele também é lido pelo `SmtpClient` (envio de e-mail).
- **Única correção de comportamento:** em `RequestSefazHttpClientHandler`, cada requisição fazia `ServicePointManager.ServerCertificateValidationCallback +=`, acumulando delegates globais e fazendo o processo inteiro aceitar qualquer certificado de servidor. Isso foi removido; a validação continua definida no próprio handler, como antes.

### Pacotes e segurança
- `System.Security.Cryptography.Xml` 6.0.1 → **10.0.12** (remove as vulnerabilidades altas de `Pkcs`/`Asn1`).
- `System.Security.Cryptography.Cng` 5.0.0 explícito no `netstandard2.0` (já vinha transitivo) para suportar A3 CNG nesse build.
- `SixLabors.ImageSharp` 2.1.13 no `NFe.Danfe.Html` (o `NetBarcode` trazia 2.1.1 vulnerável).
- `System.Net.Http` 4.3.4 só no .NET Framework.
- Testes: `Microsoft.NET.Test.Sdk` 17.14.1, `MSTest` 3.11.1, `xunit` 2.9.3, `xunit.runner.visualstudio` 3.1.5, `coverlet.collector` 6.0.4, com os usos obsoletos do MSTest corrigidos (`DataTestMethod`, `ThrowsException`, mensagens com formato).

### NuGet / CI
- Os `.nuspec` **não publicavam `net10.0`** e **não declaravam dependências**. Agora publicam `net462`, `netstandard2.0`, `net8.0` e `net10.0`, com as dependências de criptografia declaradas.
- O pacote `Hercules.NET.Impressao.NFCe.QuestPdf` usava a pasta `src\` em vez de `lib\` (saía sem DLL utilizável); corrigido, com descrição e tags ajustadas.
- CI: SDKs `10.0.x` e `8.0.x` (antes `9.0.x`/`6.0.x`, que não compilavam `net10.0`). `global.json` exige SDK 10+.

### Organização
- `DFe.Utils/Assinatura`: `MetodosNativos` e `ExtensaoCertificadoDigital` foram para arquivos próprios (mesmo namespace, mesma API), e `CertificadoDigital` foi organizado em regiões Públicos / Internos / Privados.
- A assinatura XML de NF-e (`Assinador`), CT-e e MDF-e passa por um único núcleo (`AssinaturaDigital.AssinaXml`).
- README atualizado com os frameworks suportados.

---

## Verificação

| Item | master | branch |
|---|---|---|
| Membros públicos removidos/alterados (102 comparações assembly × TFM) | — | **0** |
| SYSLIB0014 / 0021 / 0028 / 0057 | 58 / 12 / 12 / 8 | **0 / 0 / 0 / 0** |
| CS0618 (API obsoleta) | 33 | **0** |
| Vulnerabilidades NU1903 / NU1902 | 125 / 6 | 1 / 1 (`SixLabors.ImageSharp` 1.0.4 via `PdfSharpCore`, no `NFe.Danfe.Nativo` — já existia) |
| Erros de build | apps WPF antigos (`CTe/MDFe/NFe.AppTeste`, `NFe.Danfe.AppTeste.Fast`) | os mesmos (não compilam via `dotnet build`, não estão no CI) |
| `NFe.Utils.Testes` | — | net10.0: 286 aprovados · net48: 287 aprovados (+1 teste só do .NET Framework) · 6 falhas pré-existentes em cada (SVRS em `EnderecadorTestes`/`ExtinfNFeSuplTestes`) |
| `DFe.Testes` (net10.0) | — | 253 aprovados |

Testes novos de assinatura (net10.0 **e** net48): assinatura XML com chave CSP e CNG (duas vezes seguidas, garantindo que o Dispose não remove a chave), certificado em memória, carga de PFX, PKCS#1 idêntico ao formatter antigo, hashCSRT com o exemplo publicado, `IsA3` e — no net48 — que A1/CSP continuam usando o `PrivateKey` (`RSACryptoServiceProvider`) e só a chave CNG usa o fallback. No net48 os testes com chave CNG **reproduzem o erro do PR #126** no código antigo.

Também validado com um app .NET 9 real consumindo a biblioteca: usa o build `net8.0`, mantém o HttpClient e o ramo CNG funciona.

---

## Pontos de atenção / como testar

- **Testar com A1 e A3 reais antes do merge**, principalmente o **PIN em token A3 CNG** (caminho novo, sem hardware disponível nos testes automatizados).
- `ValidarCertificadoDoServidorNetCore` continua sem efeito no caminho HttpClient (.NET 9+) — comportamento já existente, não alterado.
- As 6 falhas de SVRS nos testes já existiam no `master`.

Substitui o #126 (crédito ao autor pela investigação) e resolve o #42.

🤖 Generated with [Claude Code](https://claude.com/claude-code)

https://claude.ai/code/session_01RDoBLt98yv9URAbus9nyNS
