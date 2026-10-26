using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DFe.Utils.Assinatura
{
    public static class ExtensaoCertificadoDigital
    {
        /// <summary>
        /// Extenção para certificado digital
        /// <para>Verificar validade do certificado digital, se vencido dispara ArgumentException</para>
        /// </summary>
        /// <param name="x509Certificate2"></param>
        public static void VerificaValidade(this X509Certificate2 x509Certificate2)
        {
            DateTime dataExpiracao = Convert.ToDateTime(x509Certificate2.GetExpirationDateString());

            if (dataExpiracao <= DateTime.Now)
            {
                throw new ArgumentException("Certificado digital vencido na data => " + dataExpiracao);
            }
        }

        /// <summary>
        /// Extensão para retornar o número de dias válidos do certificado
        /// </summary>
        /// <param name="x509Certificate2"></param>
        /// <returns>Número de dias válidos</returns>
        public static int VerificaDiasValidade(this X509Certificate2 x509Certificate2)
        {
            DateTime dtExp = Convert.ToDateTime(x509Certificate2.GetExpirationDateString().Substring(0, 10));
            TimeSpan dt = dtExp.Subtract(DateTime.Today);

            return dt.Days;
        }

        /// <summary>
        /// Extenção para certificado digital
        /// <para>Se usado ele retorna true se for um hardware, se for PenDriver ou SmartCard</para>
        /// </summary>
        /// <remarks>
        /// Limitações:
        /// <para>- Só reconhece chaves em provedor CSP (CryptoAPI) que se declara hardware removível. Chaves em provedor CNG/KSP,
        /// comuns nos tokens e cartões A3 mais novos (ex.: "Microsoft Smart Card Key Storage Provider"), retornam false mesmo sendo A3.
        /// No .NET 8 ou superior, tokens com minidriver da Microsoft também são abertos via CNG e retornam false.</para>
        /// <para>- Retorna false em qualquer erro ao abrir a chave (ex.: sem chave privada, token removido).</para>
        /// <para>- Só funciona no Windows; nas demais plataformas lança <see cref="NotSupportedException"/>.</para>
        /// <para>Não interfere na assinatura nem no PIN. Para saber se o certificado é A3, prefira a configuração usada para
        /// obtê-lo (<see cref="ConfiguracaoCertificado.TipoCertificado"/>).</para>
        /// </remarks>
        /// <param name="x509Certificate2"></param>
        /// <returns>bool</returns>
        public static bool IsA3(this X509Certificate2 x509Certificate2)
        {
            if (x509Certificate2 == null)
                return false;

            if (!MetodosNativos.EhWindows())
                throw new NotSupportedException("Metodo IsA3 com suporte apenas no Windows atualmente!");

            try
            {
                var service = CertificadoDigital.ObterChavePrivadaAssinatura(x509Certificate2) as RSACryptoServiceProvider;
                return service != null && service.CspKeyContainerInfo.Removable && service.CspKeyContainerInfo.HardwareDevice;
            }
            catch
            {
                return false;
            }
        }
    }
}
