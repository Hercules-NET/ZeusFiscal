using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;

// Os provedores CSP/CNG só existem no Windows; os testes que os usam são marcados com [FatoWindows]
#pragma warning disable CA1416

namespace NFe.Utils.Testes.Assinatura
{
    public sealed class FatoWindowsAttribute : FactAttribute
    {
        public FatoWindowsAttribute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Skip = "Requer Windows (provedores de chave CSP/CNG)";
        }
    }

    /// <summary>
    /// Certificado autoassinado cuja chave privada fica persistida em um provedor do Windows,
    /// simulando certificados reais (A1 instalado no repositório ou A3).
    /// </summary>
    internal sealed class CertificadoDeTeste : IDisposable
    {
        internal const string Senha = "zeus-teste";

        private readonly Action _removerChave;

        private CertificadoDeTeste(X509Certificate2 certificado, Action removerChave)
        {
            Certificado = certificado;
            _removerChave = removerChave;
        }

        public X509Certificate2 Certificado { get; }

        /// <summary>
        /// Chave no provedor legado (CryptoAPI/CSP)
        /// </summary>
        public static CertificadoDeTeste ComChaveCsp()
        {
            const int provRsaAes = 24;
            var rsa = new RSACryptoServiceProvider(2048, new CspParameters(provRsaAes, null, NomeContainer()));
            return new CertificadoDeTeste(CriarAutoAssinado(rsa), () =>
            {
                rsa.PersistKeyInCsp = false;
                rsa.Dispose();
            });
        }

        /// <summary>
        /// Chave no provedor moderno (CNG/KSP), como nos tokens A3 mais novos (ver PR #126)
        /// </summary>
        public static CertificadoDeTeste ComChaveCng()
        {
            var parametros = new CngKeyCreationParameters { Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider };
            parametros.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(2048), CngPropertyOptions.None));
            var chave = CngKey.Create(CngAlgorithm.Rsa, NomeContainer(), parametros);

            using (var rsa = new RSACng(chave))
                return new CertificadoDeTeste(CriarAutoAssinado(rsa), chave.Delete);
        }

        /// <summary>
        /// Certificado com chave efêmera (funciona em qualquer plataforma)
        /// </summary>
        public static X509Certificate2 CriarEmMemoria()
        {
            using (var rsa = RSA.Create(2048))
                return CriarAutoAssinado(rsa);
        }

        /// <summary>
        /// Conteúdo de um arquivo .pfx protegido por <see cref="Senha"/>, como um certificado A1
        /// </summary>
        public static byte[] CriarPfx()
        {
            using (var certificado = CriarEmMemoria())
                return certificado.Export(X509ContentType.Pfx, Senha);
        }

        public void Dispose()
        {
            Certificado.Dispose();
            _removerChave();
        }

        private static X509Certificate2 CriarAutoAssinado(RSA rsa)
        {
            var requisicao = new CertificateRequest("CN=ZeusFiscal Teste", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return requisicao.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(1));
        }

        private static string NomeContainer()
        {
            return "ZeusFiscal-Teste-" + Guid.NewGuid().ToString("N");
        }
    }
}
