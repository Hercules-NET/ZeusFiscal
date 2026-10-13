using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Serialization;
using DFe.Utils;
using DFe.Utils.Assinatura;
using NFe.Utils.Assinatura;
using Xunit;
using SignatureZeus = DFe.Classes.Assinatura.Signature;

namespace NFe.Utils.Testes.Assinatura
{
    public class AssinaturaDigitalTestes
    {
        private const string Id = "ID41180678393592000146558900000006041028190697";

        [FatoWindows]
        public void AssinaturaDigital_com_chave_csp_gera_assinatura_valida()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCsp())
                AssinarDuasVezes(certificado.Certificado, AssinarComAssinaturaDigital);
        }

        [FatoWindows]
        public void AssinaturaDigital_com_chave_cng_gera_assinatura_valida()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCng())
                AssinarDuasVezes(certificado.Certificado, AssinarComAssinaturaDigital);
        }

        [FatoWindows]
        public void Assinador_com_chave_csp_gera_assinatura_valida()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCsp())
                AssinarDuasVezes(certificado.Certificado, AssinarComAssinador);
        }

        [FatoWindows]
        public void Assinador_com_chave_cng_gera_assinatura_valida()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCng())
                AssinarDuasVezes(certificado.Certificado, AssinarComAssinador);
        }

        [Fact]
        public void Assinador_com_certificado_em_memoria_gera_assinatura_valida()
        {
            using (var certificado = CertificadoDeTeste.CriarEmMemoria())
                AssertAssinaturaValida(AssinarComAssinador(certificado), certificado);
        }

        private static SignatureZeus AssinarComAssinaturaDigital(X509Certificate2 certificado)
        {
            return AssinaturaDigital.Assina(CriarDocumento(), Id, certificado);
        }

        private static SignatureZeus AssinarComAssinador(X509Certificate2 certificado)
        {
            return Assinador.ObterAssinatura(CriarDocumento(), Id, certificado);
        }

        /// <summary>
        /// A chave obtida é descartada a cada assinatura; isso não pode remover a chave persistida do certificado
        /// </summary>
        private static void AssinarDuasVezes(X509Certificate2 certificado, Func<X509Certificate2, SignatureZeus> assinar)
        {
            for (var i = 0; i < 2; i++)
                AssertAssinaturaValida(assinar(certificado), certificado);
        }

        private static DocumentoTeste CriarDocumento()
        {
            return new DocumentoTeste { infDocumento = new InfDocumentoTeste { Id = Id, valor = "Operação com acentuação" } };
        }

        /// <summary>
        /// Monta o XML como é enviado à SEFAZ (documento + Signature) e valida a assinatura com a chave pública
        /// </summary>
        private static void AssertAssinaturaValida(SignatureZeus assinatura, X509Certificate2 certificado)
        {
            var xml = new XmlDocument { PreserveWhitespace = true };
            xml.LoadXml(FuncoesXml.ClasseParaXmlString(CriarDocumento()));

            var xmlAssinatura = new XmlDocument { PreserveWhitespace = true };
            xmlAssinatura.LoadXml(FuncoesXml.ClasseParaXmlString(assinatura));
            var elementoAssinatura = (XmlElement)xml.ImportNode(xmlAssinatura.DocumentElement, true);
            xml.DocumentElement.AppendChild(elementoAssinatura);

            var signedXml = new SignedXml(xml);
            signedXml.LoadXml(elementoAssinatura);

            Assert.True(signedXml.CheckSignature(certificado, true));
        }
    }

    [XmlRoot("documentoTeste", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class DocumentoTeste
    {
        public InfDocumentoTeste infDocumento { get; set; }
    }

    public class InfDocumentoTeste
    {
        [XmlAttribute]
        public string Id { get; set; }

        public string valor { get; set; }
    }
}
