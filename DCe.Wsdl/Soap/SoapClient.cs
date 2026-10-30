using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DCe.Wsdl.Soap;

/// <summary>SOAP 1.2 sobre HTTPS com certificado cliente e validação TLS da plataforma.</summary>
public sealed class SoapClient : ISoapClient
{
    private readonly Func<X509Certificate2, HttpMessageHandler> _criarHandler;

    public SoapClient() : this(CriarHandler) { }

    internal SoapClient(Func<X509Certificate2, HttpMessageHandler> criarHandler)
        => _criarHandler = criarHandler ?? throw new ArgumentNullException(nameof(criarHandler));

    internal static HttpClientHandler CriarHandler(X509Certificate2 certificado)
    {
        if (certificado == null) throw new ArgumentNullException(nameof(certificado));
        var handler = new HttpClientHandler { ClientCertificateOptions = ClientCertificateOption.Manual };
#if NET462
        System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
#else
        handler.SslProtocols = SslProtocols.Tls12;
#endif
        handler.ClientCertificates.Add(certificado);
        return handler;
    }

    public async Task<string> SendAsync(string endpoint, string soapAction, string soapEnvelope,
        X509Certificate2 certificado, int timeOut)
    {
        if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentException("O endpoint não pode ser vazio.", nameof(endpoint));
        if (string.IsNullOrWhiteSpace(soapAction)) throw new ArgumentException("A ação SOAP não pode ser vazia.", nameof(soapAction));
        if (string.IsNullOrWhiteSpace(soapEnvelope)) throw new ArgumentException("O envelope SOAP não pode ser vazio.", nameof(soapEnvelope));
        if (certificado == null) throw new ArgumentNullException(nameof(certificado));
        if (timeOut <= 0) throw new ArgumentOutOfRangeException(nameof(timeOut), "O timeout deve ser positivo.");

        using (var client = new HttpClient(_criarHandler(certificado)))
        using (var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml"))
        {
            client.Timeout = TimeSpan.FromMilliseconds(timeOut);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/soap+xml"));
            content.Headers.ContentType!.Parameters.Add(new NameValueHeaderValue("action", "\"" + soapAction + "\""));
            using (var response = await client.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                var resposta = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Erro HTTP {(int)response.StatusCode} ({response.StatusCode}) ao comunicar com SEFAZ. Endpoint: {endpoint}. Resposta: {resposta}");
                return resposta;
            }
        }
    }
}
