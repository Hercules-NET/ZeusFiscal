using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DCe.Classes.Retorno;
using DCe.Classes.Servicos.Evento;
using DCe.Utils.Validacao;
using DFe.Utils;
using DFe.Utils.Assinatura;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Utils.Extensoes;

/// <summary>Leitura, escrita, assinatura e validação dos objetos fiscais DC-e.</summary>
public static class ExtDCe
{
    // A declaração XML da DC-e usa UTF-8 em maiúsculas e não tem quebra de linha.
    public static string ObterXmlString<T>(this T documento) where T : class =>
        "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + FuncoesXml.ClasseParaXmlString(documento);

    public static T CarregarDeXmlString<T>(this T documento, string xml) where T : class
    {
        if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentException("XML vazio.", nameof(xml));
        var resultado = FuncoesXml.XmlStringParaClasse<T>(xml);
        ValidarRetornoLido(resultado);
        return resultado;
    }

    public static T CarregarDeArquivoXml<T>(this T documento, string arquivo) where T : class =>
        documento.CarregarDeXmlString(File.ReadAllText(arquivo));

    public static void SalvarXmlEmDisco<T>(this T documento, string arquivo) where T : class =>
        File.WriteAllText(arquivo, documento.ObterXmlString(), new UTF8Encoding(false));

    public static DCeDocumento Assina(this DCeDocumento documento, X509Certificate2 certificado)
    {
        if (documento == null) throw new ArgumentNullException(nameof(documento));
        if (documento.Signature != null) throw new ArgumentException("O XML já contém assinatura.", nameof(documento));
        if (string.IsNullOrWhiteSpace(documento.infDCe.Id)) throw new ArgumentException("O XML deve conter a informação fiscal com atributo Id.", nameof(documento));
        documento.Signature = AssinaturaDigital.Assina(documento, documento.infDCe.Id, certificado);
        return documento;
    }

    public static eventoDCe Assina(this eventoDCe evento, X509Certificate2 certificado)
    {
        if (evento == null) throw new ArgumentNullException(nameof(evento));
        if (evento.Signature != null) throw new ArgumentException("O XML já contém assinatura.", nameof(evento));
        if (string.IsNullOrWhiteSpace(evento.infEvento.Id)) throw new ArgumentException("O XML deve conter a informação fiscal com atributo Id.", nameof(evento));
        evento.Signature = AssinaturaDigital.Assina(evento, evento.infEvento.Id, certificado);
        return evento;
    }

    public static void Valida(this DCeDocumento documento, string? caminhoSchemas = null)
    {
        documento.ValidarDadosLocais();
        Validador.Valida(documento.ObterXmlString(), "dce", "dce_v1.00.xsd", caminhoSchemas);
    }

    public static void Valida(this eventoDCe evento, string? caminhoSchemas = null)
    {
        evento.ValidarDadosLocais();
        // O schema do envelope tem processContents=skip: conferir também o detalhe 110111.
        Validador.Valida(evento.infEvento.detEvento.evCancDCe.ObterXmlString(), "evento", "evCancDCe_v1.00.xsd", caminhoSchemas);
        Validador.Valida(evento.ObterXmlString(), "evento", "eventoDCe_v1.00.xsd", caminhoSchemas);
    }
    // Recusa retornos incompletos sem exigir dados opcionais
    // nem classificar cStat por uma lista fechada.
    private static void Obrigatorio(bool presente, string elemento, string pai)
    {
        if (!presente) throw new InvalidOperationException($"Elemento obrigatório '{elemento}' ausente em '{pai}'.");
    }

    private static void Cabecalho(bool ambiente, bool versao, bool status, bool motivo, bool uf, string pai, string nomeUf = "cUF")
    {
        Obrigatorio(ambiente, "tpAmb", pai);
        Obrigatorio(versao, "verAplic", pai);
        Obrigatorio(status, "cStat", pai);
        Obrigatorio(motivo, "xMotivo", pai);
        Obrigatorio(uf, nomeUf, pai);
    }

    private static void Protocolo(infProt? protocolo)
    {
        if (protocolo == null) return;
        Obrigatorio(protocolo.tpAmbSpecified, "tpAmb", "infProt");
        Obrigatorio(protocolo.verAplicSpecified, "verAplic", "infProt");
        Obrigatorio(protocolo.chDCeSpecified, "chDCe", "infProt");
        Obrigatorio(protocolo.cStatSpecified, "cStat", "infProt");
        Obrigatorio(protocolo.xMotivoSpecified, "xMotivo", "infProt");
    }

    private static void ValidarRetornoLido<T>(T documento) where T : class
    {
        switch (documento)
        {
            case retDCe r:
                Cabecalho(r.tpAmbSpecified, r.verAplicSpecified, r.cStatSpecified, r.xMotivoSpecified, r.cUFSpecified, "retDCe");
                Protocolo(r.protDCe?.infProt);
                break;
            case retConsSitDCe r:
                Cabecalho(r.tpAmbSpecified, r.verAplicSpecified, r.cStatSpecified, r.xMotivoSpecified, r.cUFSpecified, "retConsSitDCe");
                Obrigatorio(r.chDCeSpecified, "chDCe", "retConsSitDCe");
                Protocolo(r.protDCe?.infProt);
                break;
            case retConsStatServDCe r:
                Cabecalho(r.tpAmbSpecified, r.verAplicSpecified, r.cStatSpecified, r.xMotivoSpecified, r.cUFSpecified, "retConsStatServDCe");
                break;
            case retEventoDCe r:
                var inf = r.infEvento ?? throw new InvalidOperationException("retEventoDCe sem infEvento.");
                Cabecalho(inf.tpAmbSpecified, inf.verAplicSpecified, inf.cStatSpecified, inf.xMotivoSpecified, inf.cOrgaoSpecified, "infEvento", "cOrgao");
                break;
        }
    }
}
