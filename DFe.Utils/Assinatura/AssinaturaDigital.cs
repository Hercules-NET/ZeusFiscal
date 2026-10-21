using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Shared.DFe.Utils;
using SignatureZeus = DFe.Classes.Assinatura.Signature;

namespace DFe.Utils.Assinatura
{
    public class AssinaturaDigital
    {
        public static SignatureZeus Assina<T>(T objeto, string id, X509Certificate2 certificado,
            string signatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            string digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1",
            bool cfgServicoRemoverAcentos = false) where T : class
        {
            var xml = FuncoesXml.ClasseParaXmlString(objeto);
            if (cfgServicoRemoverAcentos)
                xml = xml.RemoverAcentos();

            return AssinaXml(xml, id, certificado, signatureMethod, digestMethod);
        }

        /// <summary>
        /// Assina (XMLDSig envelopado, C14N) o elemento de Id [id] do XML e retorna a Signature
        /// </summary>
        public static SignatureZeus AssinaXml(string xml, string id, X509Certificate2 certificado,
            string signatureMethod, string digestMethod)
        {
            if (id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var documento = new XmlDocument { PreserveWhitespace = true };
            documento.LoadXml(xml);

            var docXml = new SignedXml(documento);

            docXml.SignedInfo.SignatureMethod = signatureMethod;
            var reference = new Reference { Uri = "#" + id, DigestMethod = digestMethod };

            // adicionando EnvelopedSignatureTransform a referencia
            var envelopedSigntature = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(envelopedSigntature);

            var c14Transform = new XmlDsigC14NTransform();
            reference.AddTransform(c14Transform);

            docXml.AddReference(reference);

            // carrega o certificado em KeyInfoX509Data para adicionar a KeyInfo
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));

            docXml.KeyInfo = keyInfo;

            // a chave legada (.NET Framework) fica guardada no certificado e não é descartada; a moderna é
            var chaveLegada = CertificadoDigital.ObterChavePrivadaLegada(certificado);
            using (var chaveModerna = chaveLegada == null ? certificado.ObterChavePrivadaRsa() : null)
            {
                docXml.SigningKey = chaveLegada ?? chaveModerna;
                docXml.ComputeSignature();
            }

            //// recuperando a representacao do XML assinado
            var xmlDigitalSignature = docXml.GetXml();
            var assinatura = FuncoesXml.XmlStringParaClasse<Classes.Assinatura.Signature>(xmlDigitalSignature.OuterXml);
            return assinatura;
        }
    }
}