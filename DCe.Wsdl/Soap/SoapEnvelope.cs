using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DCe.Wsdl.Soap;

/// <summary>Envelope SOAP 1.2 e extração da mensagem fiscal, sem alterar seu conteúdo.</summary>
public static class SoapEnvelope
{
    private static readonly XNamespace DceNs = "http://www.portalfiscal.inf.br/dce";
    private static readonly XNamespace SoapNs = "http://www.w3.org/2003/05/soap-envelope";
    public const string NamespaceWsdl = "http://www.portalfiscal.inf.br/dce/wsdl/";

    public static string XmlFiscal(string xml)
    {
        // A declaração XML pertence ao documento, não pode aparecer dentro do Body.
        if (xml.StartsWith("<?xml", StringComparison.Ordinal))
            return xml.Substring(xml.IndexOf("?>", StringComparison.Ordinal) + 2);
        return xml;
    }

    public static string Criar(string xmlFiscal, string servico) =>
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><Envelope xmlns=\"" + SoapNs +
        "\"><Body><dceDadosMsg xmlns=\"" + NamespaceWsdl + servico + "\">" +
        XmlFiscal(xmlFiscal) + "</dceDadosMsg></Body></Envelope>";

    /// <summary>Recorta o elemento fiscal original, preservando espaços, entidades e aspas.</summary>
    public static string ExtrairXmlFiscal(string resposta)
    {
        if (string.IsNullOrWhiteSpace(resposta)) return resposta;
        var raiz = XElement.Parse(resposta);
        if (raiz.Name.Namespace != SoapNs) return resposta;
        // Valida Fault/Body/wrapper com os mesmos critérios da projeção tipada.
        Desembrulhar(resposta);
        using var texto = new StringReader(resposta);
        using var reader = XmlReader.Create(texto, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        var profundidadeBody = -1;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 &&
                reader.NamespaceURI == SoapNs.NamespaceName && reader.LocalName == "Body")
                profundidadeBody = reader.Depth;
            if (profundidadeBody < 0 || reader.Depth <= profundidadeBody ||
                reader.NodeType != XmlNodeType.Element || reader.NamespaceURI != DceNs.NamespaceName) continue;
            var inicio = Posicao(resposta, (IXmlLineInfo)reader) - 1;
            if (reader.IsEmptyElement)
                return resposta.Substring(inicio, FimTag(resposta, inicio) - inicio);
            var profundidade = reader.Depth;
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == profundidade)
                {
                    var fim = FimTag(resposta, Posicao(resposta, (IXmlLineInfo)reader));
                    return resposta.Substring(inicio, fim - inicio);
                }
        }
        throw new InvalidOperationException("Envelope SOAP sem mensagem da DC-e.");
    }

    private static int Posicao(string xml, IXmlLineInfo info)
    {
        var indice = 0;
        for (var linha = 1; linha < info.LineNumber; linha++)
        {
            while (xml[indice] != '\r' && xml[indice] != '\n') indice++;
            if (xml[indice++] == '\r' && indice < xml.Length && xml[indice] == '\n') indice++;
        }
        return indice + info.LinePosition - 1;
    }

    private static int FimTag(string xml, int inicio)
    {
        char aspas = '\0';
        for (var i = inicio; i < xml.Length; i++)
        {
            var c = xml[i];
            if (aspas != '\0') { if (c == aspas) aspas = '\0'; }
            else if (c == '\'' || c == '"') aspas = c;
            else if (c == '>') return i + 1;
        }
        throw new XmlException("Tag XML incompleta.");
    }

    public static string Desembrulhar(string resposta)
    {
        if (string.IsNullOrWhiteSpace(resposta)) return resposta;
        var raiz = XElement.Parse(resposta, LoadOptions.PreserveWhitespace);
        if (raiz.Name.Namespace != SoapNs) return resposta;
        var body = raiz.Element(SoapNs + "Body") ?? throw new InvalidOperationException("Envelope SOAP sem elemento Body.");
        var conteudo = body.Elements().FirstOrDefault() ?? throw new InvalidOperationException("Body do envelope SOAP está vazio.");
        if (conteudo.Name == SoapNs + "Fault") throw new InvalidOperationException("Falha SOAP retornada pela SEFAZ: " + conteudo.Value);
        if (conteudo.Name.Namespace != DceNs)
            conteudo = conteudo.Elements().FirstOrDefault(e => e.Name.Namespace == DceNs)
                ?? throw new InvalidOperationException("Wrapper de retorno sem mensagem da DC-e.");
        return Serializar(conteudo);
    }

    private static string Serializar(XElement raiz)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false), Indent = false, OmitXmlDeclaration = false,
                NewLineHandling = NewLineHandling.None
            })) raiz.Save(writer);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
