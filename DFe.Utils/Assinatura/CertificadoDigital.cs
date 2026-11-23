using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
            return ObterChavePrivadaAssinatura(certificado).SignData(value, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
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
        /// Chave privada usada nas assinaturas (XML e PKCS#1) e no PIN do A3: sempre a mesma instância para o mesmo certificado,
        /// que não deve ser descartada. O handle fica aberto enquanto o certificado existir e o PIN definido nele vale para todas
        /// as assinaturas, sem depender do cache de PIN do provedor.
        /// <para>É o <see cref="X509Certificate2.PrivateKey"/>; quando ele não abre a chave CNG no .NET Framework ("Tipo de provedor
        /// inválido", PR #126, ou null com chave efêmera), o GetRSAPrivateKey, guardado junto do certificado.</para>
        /// </summary>
        internal static RSA ObterChavePrivadaAssinatura(X509Certificate2 certificado)
        {
            RSA chave;
#pragma warning disable SYSLIB0028 // PrivateKey é obsoleto no .NET 6+, mas é ele que guarda a chave no certificado
            try
            {
                chave = certificado.PrivateKey as RSA;
            }
            catch (CryptographicException ex) when (ex.HResult == MetodosNativos.NteBadProvType)
            {
                chave = null;
            }
#pragma warning restore SYSLIB0028

            return chave ?? ChavesPrivadasCng.GetValue(certificado, c => c.GetRSAPrivateKey());
        }

        #endregion

        #region Privados

        private static readonly Dictionary<string, X509Certificate2> CacheCertificado = new Dictionary<string, X509Certificate2>();

        /// <summary>
        /// Chaves CNG que o PrivateKey não abre no .NET Framework, guardadas enquanto o certificado existir
        /// </summary>
        private static readonly ConditionalWeakTable<X509Certificate2, RSA> ChavesPrivadasCng = new ConditionalWeakTable<X509Certificate2, RSA>();

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
            // .NET Framework: contêiner vindo do PrivateKey, como antes; chave CNG (RSACng) fica ProviderType 0 => EhCng
            var chaveCsp = ObterChavePrivadaAssinatura(certificado) as RSACryptoServiceProvider;
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
                // o PIN fica no handle da chave usada nas assinaturas
                var chave = (RSACng)ObterChavePrivadaAssinatura(certificado);
                chave.Key.SetProperty(new CngProperty(MetodosNativos.NcryptPinProperty,
                    Encoding.Unicode.GetBytes(pin + '\0'), CngPropertyOptions.None));

                // valida o PIN já no carregamento, como o CSP: o TLS abre a chave por conta própria e depende do PIN já verificado
                chave.SignData(new byte[] { 0 }, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
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
