using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DCe.Testes.Fixtures.Entradas;
using DCe.Servicos;
using DCe.Classes.Retorno;
using DCe.Utils.Extensoes;
using DCe.Wsdl.Soap;
using DCe.Testes.Fixtures;
using DCe.Utils;
using DCe.Utils.Abstractions;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceServiceTest
{
    private const string Ns = "http://www.portalfiscal.inf.br/dce";
    private const string SoapNs = "http://www.w3.org/2003/05/soap-envelope";
    private const string Chave = "26260899999999000191990010000000011300123455";

    private sealed class SoapEspiao : ISoapClient
    {
        public string? Endpoint { get; private set; }
        public string? Action { get; private set; }
        public string? Envelope { get; private set; }
        public X509Certificate2? Certificado { get; private set; }
        public int TimeOut { get; private set; }
        public string Resposta { get; set; } = string.Empty;
        public int Chamadas { get; private set; }

        public Task<string> SendAsync(string endpoint, string soapAction, string soapEnvelope, X509Certificate2 certificado, int timeOut)
        {
            (Endpoint, Action, Envelope, Certificado, TimeOut) = (endpoint, soapAction, soapEnvelope, certificado, timeOut);
            Chamadas++;
            return Task.FromResult(Resposta);
        }
    }

    private sealed class RelogioFixo : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class CodigoFixo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }

    internal static X509Certificate2 Autoassinado()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=Teste DCe", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return pedido.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
    }

    private static DCeConfiguracao Configuracao(X509Certificate2 certificado, TipoAmbiente ambiente = TipoAmbiente.Homologacao)
        => new() { Certificado = certificado, TipoAmbiente = ambiente, TimeOut = 12345 };

    private static DceService Servico(DCeConfiguracao configuracao, ISoapClient transporte)
        => new(configuracao, transporte);

    private static CancelamentoDce Cancelamento(TipoAmbiente ambiente = TipoAmbiente.Homologacao) => new()
    {
        Ambiente = ambiente, ChaveAcesso = Chave, CodigoOrgao = Estado.PE,
        CnpjAutor = "99999999000191", CpfUsuarioEmitente = "12345678909",
        NumeroProtocolo = "1262600000000001", Justificativa = "TRANSPORTE CANCELADO A PEDIDO DO DECLARANTE"
    };

    private static string Retorno(string servico) => servico switch
    {
        "Autorizacao" => $"""<retDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo><cUF>26</cUF><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto></retDCe>""",
        "Consulta" => $"""<retConsSitDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo><cUF>26</cUF><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto><chDCe>{Chave}</chDCe></retConsSitDCe>""",
        "StatusServico" => $"""<retConsStatServDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cStat>107</cStat><xMotivo>Servico em Operacao</xMotivo><cUF>26</cUF><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto></retConsStatServDCe>""",
        "RecepcaoEvento" => $"""<retEventoDCe xmlns="{Ns}" versao="1.00"><infEvento><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cOrgao>26</cOrgao><cStat>135</cStat><xMotivo>Evento registrado e vinculado a DCe</xMotivo><chDCe>{Chave}</chDCe><tpEvento>110111</tpEvento><nSeqEvento>1</nSeqEvento><nProt>1262600000000002</nProt></infEvento></retEventoDCe>""",
        _ => throw new ArgumentException("Serviço de teste desconhecido.", nameof(servico))
    };

    private static async Task<RetornoBase> Executar(DceService servico, string operacao, TipoAmbiente ambiente)
    {
        switch (operacao)
        {
            case "Autorizacao":
                var dc = DceFixture.Padrao();
                dc.Ambiente = ambiente;
                var autorizacao = await servico.AutorizarAsync(dc);
                Assert.IsTrue(autorizacao.Autorizada);
                Assert.AreEqual("100", autorizacao.CodigoStatus);
                return autorizacao;
            case "Consulta":
                var consulta = await servico.ConsultarAsync(Chave);
                Assert.AreEqual(Chave, consulta.ChaveAcesso);
                Assert.AreEqual("100", consulta.CodigoStatus);
                return consulta;
            case "StatusServico":
                var status = await servico.StatusServicoAsync(Estado.PE);
                Assert.IsTrue(status.EmOperacao);
                Assert.AreEqual("107", status.CodigoStatus);
                return status;
            case "RecepcaoEvento":
                var evento = await servico.CancelarAsync(Cancelamento(ambiente));
                Assert.IsTrue(evento.Registrado);
                Assert.AreEqual("1262600000000002", evento.NumeroProtocolo);
                return evento;
            default: throw new ArgumentException("Operação de teste desconhecida.", nameof(operacao));
        }
    }

    [DataTestMethod]
    [DataRow("Autorizacao", TipoAmbiente.Homologacao, "DCe")]
    [DataRow("Autorizacao", TipoAmbiente.Producao, "DCe")]
    [DataRow("Consulta", TipoAmbiente.Homologacao, "consSitDCe")]
    [DataRow("Consulta", TipoAmbiente.Producao, "consSitDCe")]
    [DataRow("StatusServico", TipoAmbiente.Homologacao, "consStatServDCe")]
    [DataRow("StatusServico", TipoAmbiente.Producao, "consStatServDCe")]
    [DataRow("RecepcaoEvento", TipoAmbiente.Homologacao, "eventoDCe")]
    [DataRow("RecepcaoEvento", TipoAmbiente.Producao, "eventoDCe")]
    public async Task FluxoReal_SelecionaEndpointActionEnvelopeEParseia(string operacao, TipoAmbiente ambiente, string raiz)
    {
        using var cert = Autoassinado();
        var espiao = new SoapEspiao { Resposta = Retorno(operacao) };
        await Executar(Servico(Configuracao(cert, ambiente), espiao), operacao, ambiente);

        var dominio = ambiente == TipoAmbiente.Producao ? "dce.fazenda.pr.gov.br" : "homologacao.dce.fazenda.pr.gov.br";
        Assert.AreEqual($"https://{dominio}/dce/DCe{operacao}", espiao.Endpoint);
        Assert.AreEqual($"http://www.portalfiscal.inf.br/dce/wsdl/DCe{operacao}/dce{operacao}", espiao.Action);
        Assert.AreSame(cert, espiao.Certificado);
        Assert.AreEqual(12345, espiao.TimeOut);
        var envelope = XDocument.Parse(espiao.Envelope!).Root!;
        Assert.AreEqual(XName.Get("Envelope", SoapNs), envelope.Name);
        var dados = envelope.Element(XName.Get("Body", SoapNs))!.Elements().Single();
        Assert.AreEqual(XName.Get("dceDadosMsg", $"http://www.portalfiscal.inf.br/dce/wsdl/DCe{operacao}"), dados.Name);
        var mensagem = dados.Elements().Single();
        Assert.AreEqual(XName.Get(raiz, Ns), mensagem.Name);
        Assert.AreEqual(((int)ambiente).ToString(), mensagem.Descendants(XName.Get("tpAmb", Ns)).Single().Value);
        if (operacao == "Consulta")
        {
            Assert.AreEqual("CONSULTAR", mensagem.Element(XName.Get("xServ", Ns))!.Value);
            Assert.AreEqual(Chave, mensagem.Element(XName.Get("chDCe", Ns))!.Value);
        }
        if (operacao == "StatusServico")
            Assert.AreEqual("STATUS", mensagem.Element(XName.Get("xServ", Ns))!.Value);
        if (operacao == "RecepcaoEvento")
        {
            Assert.AreEqual("110111", mensagem.Descendants(XName.Get("tpEvento", Ns)).Single().Value);
            Assert.AreEqual("12345678909", mensagem.Descendants(XName.Get("CPFUsEmit", Ns)).Single().Value);
            Assert.AreEqual("26", mensagem.Descendants(XName.Get("cOrgao", Ns)).Single().Value);
        }
        if (operacao is "Autorizacao" or "RecepcaoEvento")
        {
            // Verifica a assinatura no envelope efetivamente enviado, incluindo namespaces herdados.
            var documento = new XmlDocument { PreserveWhitespace = true };
            documento.LoadXml(espiao.Envelope!);
            var assinatura = new SignedXml(documento);
            assinatura.LoadXml((XmlElement)documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!);
            Assert.IsTrue(assinatura.CheckSignature(cert, true), "A assinatura deve permanecer válida dentro do SOAP.");
        }
    }

    [DataTestMethod]
    [DataRow("Autorizacao", 0)]
    [DataRow("Autorizacao", 1)]
    [DataRow("Autorizacao", 2)]
    [DataRow("Autorizacao", 3)]
    [DataRow("Consulta", 0)]
    [DataRow("Consulta", 1)]
    [DataRow("Consulta", 2)]
    [DataRow("Consulta", 3)]
    [DataRow("StatusServico", 0)]
    [DataRow("StatusServico", 1)]
    [DataRow("StatusServico", 2)]
    [DataRow("StatusServico", 3)]
    [DataRow("RecepcaoEvento", 0)]
    [DataRow("RecepcaoEvento", 1)]
    [DataRow("RecepcaoEvento", 2)]
    [DataRow("RecepcaoEvento", 3)]
    public async Task Retornos_ConservamXmlLiteralEnviadoRecebidoEEnvelope(string operacao, int formato)
    {
        using var cert = Autoassinado();
        var fiscal = Retorno(operacao).Replace("versao=\"1.00\"", "versao='1.00'")
            .Replace("><", ">\r\n  <").Replace("PR-1.0", "PR&#45;1.0");
        var resposta = formato switch
        {
            0 => "<?xml version='1.0' encoding='UTF-8'?>" + fiscal,
            1 => $"<s:Envelope xmlns:s='{SoapNs}'><s:Body>\r\n{fiscal}\r\n</s:Body></s:Envelope>",
            2 => $"<s:Envelope xmlns:s='{SoapNs}'><s:Body><wrapper xmlns='urn:teste'>{fiscal}</wrapper></s:Body></s:Envelope>",
            _ => $"<s:Envelope xmlns:s='{SoapNs}'><s:Header><cabecalho xmlns='{Ns}'>metadados</cabecalho></s:Header><s:Body>{fiscal}</s:Body></s:Envelope>"
        };
        var espiao = new SoapEspiao { Resposta = resposta };
        var resultado = await Executar(Servico(Configuracao(cert), espiao), operacao, TipoAmbiente.Homologacao);
        var inicioDados = espiao.Envelope!.IndexOf("<dceDadosMsg ", StringComparison.Ordinal);
        var inicioFiscal = espiao.Envelope.IndexOf('>', inicioDados) + 1;
        var fimFiscal = espiao.Envelope.IndexOf("</dceDadosMsg>", inicioFiscal, StringComparison.Ordinal);
        Assert.AreEqual(espiao.Envelope.Substring(inicioFiscal, fimFiscal - inicioFiscal), resultado.EnvioStr);
        Assert.AreEqual(formato == 0 ? resposta : fiscal, resultado.RetornoStr);
        Assert.AreEqual(resposta, resultado.RetornoCompletoStr);
        using var serializado = new StringWriter();
        new System.Xml.Serialization.XmlSerializer(resultado.GetType()).Serialize(serializado, resultado);
        Assert.IsFalse(serializado.ToString().Contains("EnvioStr", StringComparison.Ordinal));
        Assert.IsFalse(serializado.ToString().Contains("RetornoStr", StringComparison.Ordinal));
        Assert.IsFalse(serializado.ToString().Contains("RetornoCompletoStr", StringComparison.Ordinal));
    }

    [DataTestMethod]
    [DataRow("Autorizacao", false)]
    [DataRow("Autorizacao", true)]
    [DataRow("Consulta", false)]
    [DataRow("Consulta", true)]
    [DataRow("StatusServico", false)]
    [DataRow("StatusServico", true)]
    [DataRow("RecepcaoEvento", false)]
    [DataRow("RecepcaoEvento", true)]
    public async Task FluxoReal_ExtraiRespostaDoBodyComOuSemWrapper(string operacao, bool wrapper)
    {
        using var cert = Autoassinado();
        var retorno = wrapper ? $"<dceDadosMsgRetorno xmlns='http://www.portalfiscal.inf.br/dce/wsdl/DCe{operacao}'>{Retorno(operacao)}</dceDadosMsgRetorno>" : Retorno(operacao);
        var espiao = new SoapEspiao { Resposta = $"<s:Envelope xmlns:s='{SoapNs}'><s:Body>{retorno}</s:Body></s:Envelope>" };
        await Executar(Servico(Configuracao(cert), espiao), operacao, TipoAmbiente.Homologacao);
    }

    [DataTestMethod]
    [DataRow("Autorizacao")]
    [DataRow("Consulta")]
    [DataRow("StatusServico")]
    [DataRow("RecepcaoEvento")]
    public async Task SchemaInvalido_ImpedeQualquerTransmissao(string operacao)
    {
        using var cert = Autoassinado();
        var espiao = new SoapEspiao();
        var servico = Servico(Configuracao(cert), espiao);
        await Assert.ThrowsExceptionAsync<XmlSchemaValidationException>(async () =>
        {
            switch (operacao)
            {
                case "Autorizacao":
                    var dc = DceFixture.Padrao();
                    dc.VersaoAplicativo = new string('X', 100);
                    await servico.AutorizarAsync(dc);
                    break;
                case "Consulta": await servico.ConsultarAsync("123"); break;
                case "StatusServico": await servico.StatusServicoAsync((Estado)99); break;
                case "RecepcaoEvento":
                    var evento = Cancelamento();
                    evento.Justificativa = "curta";
                    await servico.CancelarAsync(evento);
                    break;
            }
        });
        Assert.AreEqual(0, espiao.Chamadas);
    }

    [DataTestMethod]
    [DataRow("<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope'/>", "sem elemento Body")]
    [DataRow("<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope'><s:Body/></s:Envelope>", "está vazio")]
    [DataRow("<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope'><s:Body><wrapper/></s:Body></s:Envelope>", "sem mensagem")]
    [DataRow("<s:Envelope xmlns:s='http://www.w3.org/2003/05/soap-envelope'><s:Body><s:Fault><s:Reason>Rejeitado</s:Reason></s:Fault></s:Body></s:Envelope>", "Rejeitado")]
    public async Task RespostaSoapInvalida_ProduzErroClaro(string resposta, string mensagem)
    {
        using var cert = Autoassinado();
        var servico = Servico(Configuracao(cert), new SoapEspiao { Resposta = resposta });
        var erro = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => servico.StatusServicoAsync(Estado.PE));
        StringAssert.Contains(erro.Message, mensagem);
    }

    [TestMethod]
    public async Task ConfiguracaoInconsistente_ImpedeEnvioSemAlterarDocumento()
    {
        using var cert = Autoassinado();
        var espiao = new SoapEspiao();
        Assert.ThrowsException<ArgumentNullException>(() => new DceService(null!));
        var config = Configuracao(cert, TipoAmbiente.Producao);
        var servico = Servico(config, espiao);
        var dc = DceFixture.Padrao();
        Assert.ThrowsException<ArgumentException>(() => servico.AutorizarAsync(dc));
        Assert.ThrowsException<ArgumentException>(() => servico.CancelarAsync(Cancelamento()));
        Assert.AreEqual(TipoAmbiente.Homologacao, dc.Ambiente);
        Assert.ThrowsException<ArgumentNullException>(() => servico.AutorizarAsync(null!));
        Assert.ThrowsException<ArgumentNullException>(() => servico.CancelarAsync(null!));
        config.TipoAmbiente = (TipoAmbiente)99;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => servico.ConsultarAsync(Chave));
        config.TipoAmbiente = TipoAmbiente.Homologacao;
        config.TimeOut = 0;
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => servico.StatusServicoAsync(Estado.PE));
        Assert.AreEqual(0, espiao.Chamadas);
    }

    [TestMethod]
    public async Task CertificadoDaConfiguracaoZeus_UsadoNaAssinaturaENoMtls()
    {
        using var cert = Autoassinado();
        var config = new DCeConfiguracao
        {
            ConfiguracaoCertificado = new ConfiguracaoCertificado
            {
                TipoCertificado = TipoCertificado.A1ByteArray,
                ArrayBytesArquivo = cert.Export(X509ContentType.Pfx, "teste"), Senha = "teste",
                KeyStorageFlags = X509KeyStorageFlags.Exportable
            }
        };
        var espiao = new SoapEspiao { Resposta = Retorno("Autorizacao") };
        await Servico(config, espiao).AutorizarAsync(DceFixture.Padrao());
        Assert.IsNotNull(espiao.Certificado);
        // O certificado carregado sem cache é liberado ao concluir a chamada.
        Assert.AreEqual(IntPtr.Zero, espiao.Certificado.Handle);
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(espiao.Envelope!);
        var assinatura = new SignedXml(doc);
        assinatura.LoadXml((XmlElement)doc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!);
        Assert.IsTrue(assinatura.CheckSignature(cert, true));
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task SalvarXml_RespeitaOpcaoEPreservaEnvioRetorno(bool salvar)
    {
        using var cert = Autoassinado();
        var pasta = Path.Combine(Path.GetTempPath(), "dce-servicos-" + Guid.NewGuid().ToString("N"));
        var config = Configuracao(cert);
        config.IsSalvarXml = salvar;
        config.CaminhoSalvarXml = pasta;
        var espiao = new SoapEspiao { Resposta = Retorno("Autorizacao") };
        try
        {
            await Servico(config, espiao).AutorizarAsync(DceFixture.Padrao());
            Assert.AreEqual(salvar, Directory.Exists(pasta));
            if (!salvar) return;
            var arquivos = Directory.GetFiles(pasta);
            Assert.AreEqual(2, arquivos.Length);
            var envio = File.ReadAllText(arquivos.Single(p => p.EndsWith("-env.xml", StringComparison.Ordinal)));
            var mensagem = XDocument.Parse(espiao.Envelope!).Descendants(XName.Get("DCe", Ns)).Single();
            Assert.IsTrue(XNode.DeepEquals(XElement.Parse(envio), mensagem));
            var retorno = File.ReadAllText(arquivos.Single(p => p.EndsWith("-ret.xml", StringComparison.Ordinal)));
            Assert.IsTrue(XNode.DeepEquals(XElement.Parse(Retorno("Autorizacao")), XElement.Parse(retorno)));
        }
        finally { if (Directory.Exists(pasta)) Directory.Delete(pasta, true); }
    }

    [DataTestMethod]
    [DataRow("Autorizacao")]
    [DataRow("Consulta")]
    [DataRow("StatusServico")]
    [DataRow("RecepcaoEvento")]
    public async Task CaminhoSchemasInexistente_ImpedeEnvioNosQuatroServicos(string operacao)
    {
        using var cert = Autoassinado();
        var config = Configuracao(cert);
        config.CaminhoSchemas = Path.Combine(Path.GetTempPath(), "dce-ausente-" + Guid.NewGuid().ToString("N"));
        var espiao = new SoapEspiao();
        var erro = await Assert.ThrowsExceptionAsync<DirectoryNotFoundException>(() =>
            Executar(Servico(config, espiao), operacao, TipoAmbiente.Homologacao));
        StringAssert.Contains(erro.Message, config.CaminhoSchemas);
        Assert.AreEqual(0, espiao.Chamadas);
    }

    [TestMethod]
    public async Task SalvarXmlSemPasta_ImpedeEnvio()
    {
        using var cert = Autoassinado();
        var config = Configuracao(cert);
        config.IsSalvarXml = true;
        var espiao = new SoapEspiao();
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => Servico(config, espiao).StatusServicoAsync(Estado.PE));
        Assert.AreEqual(0, espiao.Chamadas);
    }
}
