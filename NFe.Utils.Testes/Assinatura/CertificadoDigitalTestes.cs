using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using DFe.Utils;
using DFe.Utils.Assinatura;
using Xunit;

namespace NFe.Utils.Testes.Assinatura
{
    public class CertificadoDigitalTestes
    {
        [Fact]
        public void ObterCertificado_A1ByteArray_carrega_chave_privada()
        {
            using (var certificado = CertificadoDigital.ObterCertificado(ConfiguracaoA1()))
                AssertConsegueAssinar(certificado);
        }

        [Fact]
        public void ObterCertificado_A1Arquivo_carrega_chave_privada()
        {
            var arquivo = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pfx");
            File.WriteAllBytes(arquivo, CertificadoDeTeste.CriarPfx());
            try
            {
                var configuracao = new ConfiguracaoCertificado
                {
                    TipoCertificado = TipoCertificado.A1Arquivo,
                    Arquivo = arquivo,
                    Senha = CertificadoDeTeste.Senha
                };

                using (var certificado = CertificadoDigital.ObterCertificado(configuracao))
                    AssertConsegueAssinar(certificado);
            }
            finally
            {
                File.Delete(arquivo);
            }
        }

        [Fact]
        public void ObterDosBytes_carrega_chave_privada()
        {
            using (var certificado = CertificadoDigitalUtils.ObterDosBytes(CertificadoDeTeste.CriarPfx(), CertificadoDeTeste.Senha, X509KeyStorageFlags.DefaultKeySet))
                AssertConsegueAssinar(certificado);
        }

        [Fact]
        public void ObterCertificado_com_senha_errada_falha()
        {
            Assert.ThrowsAny<Exception>(() => CertificadoDigital.ObterCertificado(ConfiguracaoA1("senha-errada")));
        }

        [Fact]
        public void ObterAssinaturaPkcs1_gera_os_mesmos_bytes_do_formatter_sha1()
        {
            var dados = Encoding.UTF8.GetBytes("41180678393592000146558900000006041028190697");
            using (var certificado = CertificadoDeTeste.CriarEmMemoria())
            using (var rsa = certificado.GetRSAPrivateKey())
            using (var sha1 = SHA1.Create())
            {
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA1");
                var esperado = formatter.CreateSignature(sha1.ComputeHash(dados));

                Assert.Equal(esperado, CertificadoDigital.ObterAssinaturaPkcs1(certificado, dados));
            }
        }

        [Fact]
        public void ObterChavePrivadaRsa_sem_chave_privada_lanca_CryptographicException()
        {
            using (var certificado = CertificadoDeTeste.CriarEmMemoria())
            using (var somenteChavePublica = X509CertificateLoader.LoadCertificate(certificado.RawData))
                Assert.Throws<CryptographicException>(() => somenteChavePublica.ObterChavePrivadaRsa());
        }

#if NETFRAMEWORK
        [FatoWindows]
        public void ObterChavePrivadaLegada_mantem_PrivateKey_no_NetFramework_e_deixa_CNG_para_o_fallback()
        {
            using (var csp = CertificadoDeTeste.ComChaveCsp())
            using (var cng = CertificadoDeTeste.ComChaveCng())
            using (var a1 = CertificadoDigital.ObterCertificado(ConfiguracaoA1()))
            {
                // o mesmo objeto de chave (CAPI) que era usado para assinar antes desta mudança
                Assert.IsType<RSACryptoServiceProvider>(CertificadoDigital.ObterChavePrivadaLegada(a1));
                Assert.IsType<RSACryptoServiceProvider>(CertificadoDigital.ObterChavePrivadaLegada(csp.Certificado));

                // chave CNG: o PrivateKey falha ("Tipo de provedor inválido") e o GetRSAPrivateKey assume
                Assert.Null(CertificadoDigital.ObterChavePrivadaLegada(cng.Certificado));
            }
        }
#endif

        [FatoWindows]
        public void ObterInfoProvedorChave_distingue_chave_csp_de_cng()
        {
            using (var csp = CertificadoDeTeste.ComChaveCsp())
            using (var cng = CertificadoDeTeste.ComChaveCng())
            {
                var infoCsp = MetodosNativos.ObterInfoProvedorChave(csp.Certificado);
                Assert.NotEqual(0, infoCsp.ProviderType);
                Assert.StartsWith("ZeusFiscal-Teste-", infoCsp.ContainerName);

                var infoCng = MetodosNativos.ObterInfoProvedorChave(cng.Certificado);
                Assert.Equal(0, infoCng.ProviderType);
                Assert.Equal(CngProvider.MicrosoftSoftwareKeyStorageProvider.Provider, infoCng.ProviderName);
            }
        }

        [FatoWindows]
        public void IsA3_retorna_false_para_chave_csp_em_software()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCsp())
                Assert.False(certificado.Certificado.IsA3());
        }

        [FatoWindows]
        public void IsA3_retorna_false_para_chave_cng_em_software()
        {
            using (var certificado = CertificadoDeTeste.ComChaveCng())
                Assert.False(certificado.Certificado.IsA3());
        }

        private static ConfiguracaoCertificado ConfiguracaoA1(string senha = CertificadoDeTeste.Senha)
        {
            return new ConfiguracaoCertificado
            {
                TipoCertificado = TipoCertificado.A1ByteArray,
                ArrayBytesArquivo = CertificadoDeTeste.CriarPfx(),
                Senha = senha
            };
        }

        private static void AssertConsegueAssinar(X509Certificate2 certificado)
        {
            Assert.True(certificado.HasPrivateKey);

            var dados = new byte[] { 1, 2, 3 };
            var assinatura = CertificadoDigital.ObterAssinaturaPkcs1(certificado, dados);
            using (var chavePublica = certificado.GetRSAPublicKey())
                Assert.True(chavePublica.VerifyData(dados, assinatura, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1));
        }
    }
}
