using System;
using System.Net;

namespace DFe.Wsdl.Common
{
    public static class ConfiguracaoServicoWSDL
    {
        public static bool ValidarCertificadoDoServidorNetCore { get; set; }
        private static Func<IRequestSefaz> _requestSefazFactory;

        public static void SetRequestSefazFactory(Func<IRequestSefaz> factory)
        {
            _requestSefazFactory = factory;
        }

        public static IRequestSefaz GetRequestSefaz()
        {
            return _requestSefazFactory();
        }

        static ConfiguracaoServicoWSDL()
        {
            ValidarCertificadoDoServidorNetCore = true;

            //a partir de .net 9 utilizar o HttpClient
            //pois o WebRequest, HttpWebRequest, ServicePoint, and WebClient foi DESCONTINUADO
            //Ver https://github.com/Hercules-NET/ZeusFiscal/issues/59
            //Verificado em execução, e não na compilação: o build net8.0 também roda no .NET 9+ (no .NET Framework a versão é 4.x)
            if (Environment.Version.Major >= 9)
                SetRequestSefazFactory(() => new RequestSefazHttpClientHandler());
            else
                SetRequestSefazFactory(() => new RequestSefazDefault());
        }

        /// <summary>
        /// TLS 1.1/1.2 no ServicePointManager, como os construtores dos serviços de CT-e e MDF-e sempre fizeram
        /// </summary>
        internal static void AplicarProtocoloSegurancaLegado()
        {
#pragma warning disable SYSLIB0014 // ServicePointManager é obsoleto, mas segue lido por HttpWebRequest e SmtpClient: mantido como antes
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014
        }
    }
}