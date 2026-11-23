using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using DCe.Classes.Servicos.Evento;
using DCe.Utils.Extensoes;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Utils;

/// <summary>Assinatura de XML usando os modelos fiscais serializáveis.</summary>
public static class DceAssinatura
{
    public static string Assinar(string xml, X509Certificate2 certificado)
    {
        var documento = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
        documento.LoadXml(xml);
        var raiz = documento.DocumentElement!;
        if (raiz.NamespaceURI != "http://www.portalfiscal.inf.br/dce")
            throw new ArgumentException("Esperado XML de DC-e ou eventoDCe.", nameof(xml));
        if (raiz.LocalName == "DCe")
            return new DCeDocumento().CarregarDeXmlString(xml).Assina(certificado).ObterXmlString();
        if (raiz.LocalName == "eventoDCe")
            return new eventoDCe().CarregarDeXmlString(xml).Assina(certificado).ObterXmlString();
        throw new ArgumentException("Esperado XML de DC-e ou eventoDCe.", nameof(xml));
    }
}
