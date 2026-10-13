using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DFe.Utils.Assinatura
{
    internal static class MetodosNativos
    {
        #region Públicos

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CryptAcquireContext(
            ref IntPtr hProv,
            string containerName,
            string providerName,
            int providerType,
            CryptContextFlags flags
            );

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CryptSetProvParam(
            IntPtr hProv,
            CryptParameter dwParam,
            [In] byte[] pbData,
            uint dwFlags);

        public static void Executar(Func<bool> action)
        {
            if (!action())
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        #endregion

        #region Internos

        internal enum CryptContextFlags
        {
            None = 0,
            Silent = 0x40
        }

        internal enum CertificateProperty
        {
            None = 0,
            CryptoProviderHandle = 0x1,
            KeyProviderInfo = 0x2
        }

        /// <summary>
        /// CRYPT_KEY_PROV_INFO
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CryptKeyProvInfo
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string ContainerName;
            [MarshalAs(UnmanagedType.LPWStr)] public string ProviderName;
            public int ProviderType;
            public int Flags;
            public int ProvParamCount;
            public IntPtr ProvParams;
            public int KeySpec;

            /// <summary>
            /// ProviderType 0: chave em provedor CNG (KSP), comum nos tokens A3 mais novos, em vez do CSP legado
            /// </summary>
            internal bool EhCng
            {
                get { return ProviderType == 0; }
            }
        }

        internal enum CryptParameter
        {
            None = 0,
            KeyExchangePin = 0x20
        }

        /// <summary>
        /// NTE_BAD_PROV_TYPE ("Tipo de provedor inválido especificado"): PrivateKey do .NET Framework com chave em provedor CNG
        /// </summary>
        internal const int NteBadProvType = unchecked((int)0x80090014);

        /// <summary>
        /// NCRYPT_PIN_PROPERTY: PIN do cartão/token para chaves CNG
        /// </summary>
        internal const string NcryptPinProperty = "SmartCardPin";

        /// <summary>
        /// NCRYPT_IMPL_TYPE_PROPERTY e flags NCRYPT_IMPL_HARDWARE_FLAG | NCRYPT_IMPL_REMOVABLE_FLAG
        /// </summary>
        internal const string NcryptImplTypeProperty = "Impl Type";
        internal const int NcryptImplHardwareRemovivel = 0x1 | 0x8;

        [DllImport("CRYPT32.DLL", SetLastError = true)]
        internal static extern bool CertSetCertificateContextProperty(
            IntPtr pCertContext,
            CertificateProperty propertyId,
            uint dwFlags,
            IntPtr pvData
            );

        [DllImport("CRYPT32.DLL", SetLastError = true)]
        internal static extern bool CertGetCertificateContextProperty(
            IntPtr pCertContext,
            CertificateProperty propertyId,
            IntPtr pvData,
            ref int pcbData
            );

        /// <summary>
        /// Lê o CERT_KEY_PROV_INFO_PROP_ID do certificado (provedor e contêiner da chave privada) sem abrir a chave
        /// </summary>
        internal static CryptKeyProvInfo ObterInfoProvedorChave(X509Certificate2 certificado)
        {
            var tamanho = 0;
            if (!CertGetCertificateContextProperty(certificado.Handle, CertificateProperty.KeyProviderInfo, IntPtr.Zero, ref tamanho))
                throw new CryptographicException("Não foi possível obter o provedor da chave privada do certificado digital.",
                    new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));

            var buffer = Marshal.AllocHGlobal(tamanho);
            try
            {
                Executar(() => CertGetCertificateContextProperty(certificado.Handle, CertificateProperty.KeyProviderInfo, buffer, ref tamanho));
                return Marshal.PtrToStructure<CryptKeyProvInfo>(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Guarda de plataforma: com o atributo, o analisador CA1416 reconhece o código Windows-only protegido por ela
        /// </summary>
#if NET6_0_OR_GREATER
        [System.Runtime.Versioning.SupportedOSPlatformGuard("windows")]
#endif
        internal static bool EhWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        #endregion
    }
}
