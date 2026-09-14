using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DFe.Utils.Assinatura
{
    public static class ExtensaoCertificadoDigital
    {
        /// <summary>
        /// Obtém a chave privada RSA do certificado, compatível com chaves em provedores CSP e CNG (ex.: tokens A3 mais novos).
        /// <para>A chave retornada deve ser descartada (Dispose) por quem a obteve.</para>
        /// </summary>
        /// <exception cref="CryptographicException">Quando o certificado não possui chave privada RSA acessível</exception>
        internal static RSA ObterChavePrivadaRsa(this X509Certificate2 certificado)
        {
            var chave = certificado.GetRSAPrivateKey();
            if (chave == null)
                throw new CryptographicException("O certificado digital não possui chave privada RSA acessível.");

            return chave;
        }

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
                // Aqui vale o tipo do objeto .NET (e não o provedor registrado, como no PIN): a informação de
                // hardware removível vem do provedor que efetivamente abriu a chave. A legada (.NET Framework) não é descartada
                var chaveLegada = CertificadoDigital.ObterChavePrivadaLegada(x509Certificate2);
                using (var chaveModerna = chaveLegada == null ? x509Certificate2.GetRSAPrivateKey() : null)
                {
                    var chave = chaveLegada ?? chaveModerna;

                    if (chave is RSACryptoServiceProvider service)
                        return service.CspKeyContainerInfo.Removable && service.CspKeyContainerInfo.HardwareDevice;

                    if (chave is RSACng chaveCng)
                    {
                        var tipo = BitConverter.ToInt32(chaveCng.Key.GetProperty(MetodosNativos.NcryptImplTypeProperty, CngPropertyOptions.None).GetValue(), 0);
                        return (tipo & MetodosNativos.NcryptImplHardwareRemovivel) == MetodosNativos.NcryptImplHardwareRemovivel;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
