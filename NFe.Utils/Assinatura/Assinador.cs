using System.Security.Cryptography.X509Certificates;
using DFe.Utils;
using DFe.Utils.Assinatura;
using NFe.Utils.Evento;
using Signature = DFe.Classes.Assinatura.Signature;

namespace NFe.Utils.Assinatura
{
    public static class Assinador
    {
        /// <summary>
        ///     Obtém a assinatura de um objeto serializável
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser assinado</typeparam>
        /// <param name="objeto">Objeto a ser assinado</param>
        /// <param name="id">Id para URI do objeto <see cref="Signature"/></param>
        /// <param name="configuracaoServico">Configuração do serviço</param>
        /// <returns>Retorna um objeto do tipo Classes.Assinatura.Signature, contendo a assinatura do objeto passado como parâmetro</returns>
        public static Signature ObterAssinatura<T>(T objeto, string id, ConfiguracaoServico configuracaoServico = null) where T : class
        {
            var cfgServico = configuracaoServico ?? ConfiguracaoServico.Instancia;

            X509Certificate2 certificadoDigital = null;
            try
            {
                certificadoDigital = CertificadoDigital.ObterCertificado(cfgServico.Certificado);
                return ObterAssinatura<T>(objeto, id, certificadoDigital, cfgServico.Certificado.ManterDadosEmCache, cfgServico.Certificado.SignatureMethodSignedXml, cfgServico.Certificado.DigestMethodReference, cfgServico.RemoverAcentos);
            }
            finally
            {
                if (!cfgServico.Certificado.ManterDadosEmCache)
                    certificadoDigital?.Reset();
            }
        }

        /// <summary>
        ///     Obtém a assinatura de um objeto serializável
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <param name="id"></param>
        /// <param name="certificadoDigital">Informe o certificado digital</param>
        /// <param name="manterDadosEmCache">Validador para manter o certificado em cache</param>
        /// <param name="signatureMethod"></param>
        /// <param name="digestMethod"></param>
        /// <param name="cfgServicoRemoverAcentos"></param>
        /// <returns>Retorna um objeto do tipo Classes.Assinatura.Signature, contendo a assinatura do objeto passado como parâmetro</returns>
        public static Signature ObterAssinatura<T>(T objeto, string id, X509Certificate2 certificadoDigital,
            bool manterDadosEmCache = false, string signatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            string digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1", bool cfgServicoRemoverAcentos = false) where T : class
        {
            var xml = FuncoesXml.ClasseParaXmlString(objeto);
            if (cfgServicoRemoverAcentos)
                xml = xml.RemoverAcentosPreservandoDescEvento();

            return AssinaturaDigital.AssinaXml(xml, id, certificadoDigital, signatureMethod, digestMethod);
        }
    }
}