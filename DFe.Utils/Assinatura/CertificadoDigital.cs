using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DFe.Utils.Assinatura
{
    public static class CertificadoDigital
    {
        #region Públicos

        /// <summary>
        /// Cria e devolve um objeto <see cref="X509Store"/>
        /// </summary>
        /// <param name="openFlags"></param>
        /// <returns></returns>
        public static X509Store ObterX509Store(OpenFlags openFlags, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            X509Store store = new X509Store(StoreName.My, storeLocation);
            store.Open(openFlags);
            return store;
        }

        /// <summary>
        /// Obtém o certificado digital conforme <see cref="ConfiguracaoCertificado.TipoCertificado"/>
        /// <para>Com <see cref="ConfiguracaoCertificado.ManterDadosEmCache"/> a instância é reutilizada; sem cache,
        /// libere os recursos após o uso com <see cref="X509Certificate2.Reset()"/></para>
        /// </summary>
        public static X509Certificate2 ObterCertificado(ConfiguracaoCertificado configuracaoCertificado)
        {
            if (!configuracaoCertificado.ManterDadosEmCache)
                return ObterDadosCertificado(configuracaoCertificado);

            if (!string.IsNullOrEmpty(configuracaoCertificado.CacheId) && CacheCertificado.ContainsKey(configuracaoCertificado.CacheId))
                return CacheCertificado[configuracaoCertificado.CacheId];

            var certificado = ObterDadosCertificado(configuracaoCertificado);

            var keyCertificado = string.IsNullOrEmpty(configuracaoCertificado.CacheId)
                ? certificado.SerialNumber
                : configuracaoCertificado.CacheId;

            configuracaoCertificado.CacheId = keyCertificado;

            CacheCertificado.Add(keyCertificado, certificado);

            return CacheCertificado[keyCertificado];
        }

        /// <summary>
        /// Obtém a assinatura do certificado digital no formato PKCS#1, baseado em um array de bytes passado como Argumento [value].
        /// </summary>
        public static byte[] ObterAssinaturaPkcs1(ConfiguracaoCertificado configuracaoCertificado, byte[] value)
        {
            return ObterAssinaturaPkcs1(ObterCertificado(configuracaoCertificado), value);
        }

        /// <summary>
        /// Obtém a assinatura RSA PKCS#1 v1.5 com SHA-1 (usada nos QR Codes em contingência) dos bytes informados em [value].
        /// </summary>
        public static byte[] ObterAssinaturaPkcs1(X509Certificate2 certificado, byte[] value)
        {
            using (RSA rsa = certificado.ObterChavePrivadaRsa())
                return rsa.SignData(value, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        public static void ClearCache()
        {
            CacheCertificado.Clear();
        }

        #endregion

        #region Internos

        /// <summary>
        /// Carrega um PFX. No .NET 9+ os construtores são obsoletos (SYSLIB0057); nos demais o construtor é mantido,
        /// preservando o comportamento legado (e, no .NET Framework, é mais rápido que o X509CertificateLoader do Microsoft.Bcl.Cryptography)
        /// </summary>
        internal static X509Certificate2 CarregarPkcs12(byte[] conteudo, string senha, X509KeyStorageFlags keyStorageFlags)
        {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12(conteudo, senha, keyStorageFlags, LimitesPkcs12);
#else
            return new X509Certificate2(conteudo, senha, keyStorageFlags);
#endif
        }

        /// <inheritdoc cref="CarregarPkcs12"/>
        internal static X509Certificate2 CarregarPkcs12DeArquivo(string arquivo, string senha, X509KeyStorageFlags keyStorageFlags)
        {
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12FromFile(arquivo, senha, keyStorageFlags, LimitesPkcs12);
#else
            return new X509Certificate2(arquivo, senha, keyStorageFlags);
#endif
        }

        /// <summary>
        /// .NET Framework: a chave pelo <see cref="X509Certificate2.PrivateKey"/>, exatamente como antes (CAPI; a instância fica guardada
        /// no certificado e não deve ser descartada; os demais erros continuam sendo lançados como antes).
        /// <para>Retorna null — e o chamador usa GetRSAPrivateKey — quando a chave está em provedor CNG (o PrivateKey falha com
        /// "Tipo de provedor inválido", PR #126), quando não há chave privada RSA e fora do .NET Framework.</para>
        /// </summary>
        internal static RSACryptoServiceProvider ObterChavePrivadaLegada(X509Certificate2 certificado)
        {
#if NETFRAMEWORK
            try
            {
                return certificado.PrivateKey as RSACryptoServiceProvider;
            }
            catch (CryptographicException ex) when (ex.HResult == MetodosNativos.NteBadProvType)
            {
                return null;
            }
#else
            return null;
#endif
        }

        #endregion

        #region Privados

        private static readonly Dictionary<string, X509Certificate2> CacheCertificado = new Dictionary<string, X509Certificate2>();

#if NET9_0_OR_GREATER
        /// <summary>
        /// Mantém provedor, nome da chave e alias gravados no PFX, como o construtor fazia
        /// </summary>
        private static readonly Pkcs12LoaderLimits LimitesPkcs12 = new Pkcs12LoaderLimits
        {
            PreserveStorageProvider = true,
            PreserveKeyName = true,
            PreserveCertificateAlias = true
        };
#endif

        /// <summary>
        /// Obtém um certificado a partir do arquivo e da senha passados nos parâmetros
        /// </summary>
        /// <param name="arquivo">Arquivo do certificado digital</param>
        /// <param name="senha">Senha do certificado digital</param>
        /// <returns></returns>
        private static X509Certificate2 ObterDeArquivo(string arquivo, string senha, X509KeyStorageFlags keyStorageFlag)
        {
            if (!File.Exists(arquivo))
            {
                throw new Exception(string.Format("Certificado digital {0} não encontrado!", arquivo));
            }

            return CarregarPkcs12DeArquivo(arquivo, senha, keyStorageFlag);
        }


        /// <summary>
        /// Obtém um certificado a partir do array de bytes e da senha passados nos parâmetros
        /// </summary>
        /// <param name="arrayBytes">Array de bytes do certificado digital</param>
        /// <param name="senha">Senha do certificado digital</param>
        /// <returns></returns>
        private static X509Certificate2 ObterDoArrayBytes(byte[] arrayBytes, string senha, X509KeyStorageFlags keyStorageFlag)
        {
            try
            {
                return CarregarPkcs12(arrayBytes, senha, keyStorageFlag);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possivel converter o stream para o certificado.", ex);
            }
        }

        /// <summary>
        /// Obtém um objeto <see cref="X509Certificate2"/> pelo serial passado no parÂmetro
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2 ObterDoRepositorio(string serial, OpenFlags opcoesDeAbertura, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            if (string.IsNullOrEmpty(serial))
                throw new ArgumentException("O número de série do certificado digital não foi informado!");
            X509Certificate2 certificado = null;
            var store = ObterX509Store(opcoesDeAbertura, storeLocation);
            try
            {
                foreach (var item in store.Certificates)
                {
                    if (item.SerialNumber != null && item.SerialNumber.ToUpper().Equals(serial.ToUpper(), StringComparison.InvariantCultureIgnoreCase))
                        certificado = item;
                }

                if (certificado == null)
                    throw new Exception(string.Format("Certificado digital nº {0} não encontrado!", serial.ToUpper()));
            }
            finally
            {
                store.Close();
            }

            return certificado;
        }

        /// <summary>
        /// Obtém um objeto <see cref="X509Certificate2"/> pelo serial passado no parâmetro e com opção de definir o PIN
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        private static X509Certificate2 ObterDoRepositorioPassandoPin(string serial, string senha = null, StoreLocation storeLocation = StoreLocation.CurrentUser)
        {
            var certificado = ObterDoRepositorio(serial, OpenFlags.ReadOnly, storeLocation);
            if (string.IsNullOrEmpty(senha)) return certificado;
            certificado.DefinirPinParaChavePrivada(senha);
            return certificado;
        }

        /// <summary>
        /// Define o PIN para chave privada de um objeto <see cref="X509Certificate2"/> passado no parâmetro
        /// </summary>
        private static void DefinirPinParaChavePrivada(this X509Certificate2 certificado, string pin)
        {
            if (!MetodosNativos.EhWindows())
                throw new NotSupportedException("Metodo DefinirPinParaChavePrivada com suporte apenas no Windows atualmente!");

#if NETFRAMEWORK
            // .NET Framework: contêiner vindo do PrivateKey, como antes; sem chave legada (CNG) fica ProviderType 0 => EhCng
            var chaveCsp = ObterChavePrivadaLegada(certificado);
            var infoChave = chaveCsp == null
                ? new MetodosNativos.CryptKeyProvInfo()
                : new MetodosNativos.CryptKeyProvInfo
                {
                    ContainerName = chaveCsp.CspKeyContainerInfo.KeyContainerName,
                    ProviderName = chaveCsp.CspKeyContainerInfo.ProviderName,
                    ProviderType = chaveCsp.CspKeyContainerInfo.ProviderType
                };
#else
            // O provedor registrado no certificado define o caminho, sem abrir a chave: pelo tipo do objeto .NET,
            // chaves de CSPs da Microsoft (ex.: tokens com minidriver) também chegariam como RSACng
            var infoChave = MetodosNativos.ObterInfoProvedorChave(certificado);
#endif

            if (infoChave.EhCng)
            {
                using (var chave = (RSACng)certificado.ObterChavePrivadaRsa())
                {
                    chave.Key.SetProperty(new CngProperty(MetodosNativos.NcryptPinProperty,
                        Encoding.Unicode.GetBytes(pin + '\0'), CngPropertyOptions.None));

                    // O PIN fica associado a este handle: uma operação com a chave o valida enquanto ele está aberto.
                    // As assinaturas seguintes abrem outro handle e dependem do cache de PIN do provedor.
                    chave.SignData(new byte[] { 0 }, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
                return;
            }

            var providerHandle = IntPtr.Zero;
            var pinBuffer = Encoding.ASCII.GetBytes(pin);

            MetodosNativos.Executar(() => MetodosNativos.CryptAcquireContext(ref providerHandle,
                infoChave.ContainerName,
                infoChave.ProviderName,
                infoChave.ProviderType,
                MetodosNativos.CryptContextFlags.Silent));
            MetodosNativos.Executar(() => MetodosNativos.CryptSetProvParam(providerHandle,
                MetodosNativos.CryptParameter.KeyExchangePin,
                pinBuffer, 0));
            MetodosNativos.Executar(() => MetodosNativos.CertSetCertificateContextProperty(
                certificado.Handle,
                MetodosNativos.CertificateProperty.CryptoProviderHandle,
                0, providerHandle));
        }

        /// <summary>
        /// Busca o certificado de acordo com o <see cref="ConfiguracaoCertificado.TipoCertificado"/>
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2 ObterDadosCertificado(ConfiguracaoCertificado configuracaoCertificado)
        {
            switch (configuracaoCertificado.TipoCertificado)
            {
                case TipoCertificado.A1Repositorio:
                    return ObterDoRepositorio(configuracaoCertificado.Serial, OpenFlags.MaxAllowed, configuracaoCertificado.StoreLocation);
                case TipoCertificado.A1ByteArray:
                    return ObterDoArrayBytes(configuracaoCertificado.ArrayBytesArquivo, configuracaoCertificado.Senha, configuracaoCertificado.KeyStorageFlags);
                case TipoCertificado.A1Arquivo:
                    return ObterDeArquivo(configuracaoCertificado.Arquivo, configuracaoCertificado.Senha, configuracaoCertificado.KeyStorageFlags);
                case TipoCertificado.A3:
                    return ObterDoRepositorioPassandoPin(configuracaoCertificado.Serial, configuracaoCertificado.Senha, configuracaoCertificado.StoreLocation);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
