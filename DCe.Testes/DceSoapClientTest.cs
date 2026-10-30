using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using DCe.Wsdl.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceSoapClientTest
{
    private const string Endpoint = "https://homologacao.dce.fazenda.pr.gov.br/dce/DCeStatusServico";
    private const string Action = "http://www.portalfiscal.inf.br/dce/wsdl/DCeStatusServico/dceStatusServico";
    private const string Envelope = "<Envelope xmlns='http://www.w3.org/2003/05/soap-envelope'><Body/></Envelope>";

    private sealed class HandlerEspiao : HttpMessageHandler
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
        public Exception? Excecao { get; set; }
        public Uri? Uri { get; private set; }
        public HttpMethod? Metodo { get; private set; }
        public MediaTypeHeaderValue? ContentType { get; private set; }
        public string? Conteudo { get; private set; }
        public string? Accept { get; private set; }
        public bool TemSoapAction { get; private set; }
        public bool Liberado { get; private set; }
        public bool AguardarCancelamento { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Excecao != null) throw Excecao;
            if (AguardarCancelamento) await Task.Delay(Timeout.Infinite, cancellationToken);
            Uri = request.RequestUri;
            Metodo = request.Method;
            ContentType = request.Content!.Headers.ContentType;
            Conteudo = await request.Content.ReadAsStringAsync();
            Accept = request.Headers.Accept.Single().MediaType;
            TemSoapAction = request.Headers.Contains("SOAPAction");
            return new HttpResponseMessage(Status) { Content = new StringContent("<retorno>SEFAZ</retorno>") };
        }

        protected override void Dispose(bool disposing)
        {
            Liberado = true;
            base.Dispose(disposing);
        }
    }

    [TestMethod]
    public async Task TransporteReal_EnviaPostUtf8Soap12ComActionNoContentType()
    {
        using var cert = DceServiceTest.Autoassinado();
        var handler = new HandlerEspiao();
        X509Certificate2? recebido = null;
        var client = new SoapClient(c => { recebido = c; return handler; });
        var resposta = await client.SendAsync(Endpoint, Action, Envelope, cert, 12345);
        Assert.AreSame(cert, recebido);
        Assert.AreEqual(Endpoint, handler.Uri!.AbsoluteUri);
        Assert.AreEqual(HttpMethod.Post, handler.Metodo);
        Assert.AreEqual(Envelope, handler.Conteudo);
        Assert.AreEqual("application/soap+xml", handler.ContentType!.MediaType);
        Assert.AreEqual("utf-8", handler.ContentType.CharSet);
        Assert.AreEqual("\"" + Action + "\"", handler.ContentType.Parameters.Single(p => p.Name == "action").Value);
        Assert.AreEqual("application/soap+xml", handler.Accept);
        Assert.IsFalse(handler.TemSoapAction);
        Assert.AreEqual("<retorno>SEFAZ</retorno>", resposta);
        Assert.IsTrue(handler.Liberado);
        Assert.IsTrue(cert.HasPrivateKey, "O transporte não deve liberar o certificado do chamador.");
    }

    [TestMethod]
    public void HandlerReal_ConfiguraCertificadoMtlsTls12SemIgnorarValidacaoDoServidor()
    {
        using var cert = DceServiceTest.Autoassinado();
        using var handler = SoapClient.CriarHandler(cert);
        Assert.AreEqual(ClientCertificateOption.Manual, handler.ClientCertificateOptions);
        Assert.AreEqual(1, handler.ClientCertificates.Count);
        Assert.AreSame(cert, handler.ClientCertificates[0]);
        Assert.AreEqual(SslProtocols.Tls12, handler.SslProtocols);
        Assert.IsNull(handler.ServerCertificateCustomValidationCallback);
    }

    [TestMethod]
    public async Task ErroHttp_PreservaStatusEndpointEResposta()
    {
        using var cert = DceServiceTest.Autoassinado();
        var handler = new HandlerEspiao { Status = HttpStatusCode.InternalServerError };
        var client = new SoapClient(_ => handler);
        var erro = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => client.SendAsync(Endpoint, Action, Envelope, cert, 1000));
        StringAssert.Contains(erro.Message, "500");
        StringAssert.Contains(erro.Message, Endpoint);
        StringAssert.Contains(erro.Message, "<retorno>SEFAZ</retorno>");
        Assert.IsTrue(handler.Liberado);
    }

    [TestMethod]
    public async Task TimeoutConfigurado_CancelaRequisicaoELiberaHandler()
    {
        using var cert = DceServiceTest.Autoassinado();
        var handler = new HandlerEspiao { AguardarCancelamento = true };
        var client = new SoapClient(_ => handler);
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => client.SendAsync(Endpoint, Action, Envelope, cert, 100));
        Assert.IsTrue(handler.Liberado);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ErroDeTransporte_PropagaExcecaoELiberaRecursos(bool timeout)
    {
        using var cert = DceServiceTest.Autoassinado();
        var handler = new HandlerEspiao { Excecao = timeout ? new TaskCanceledException("Tempo excedido") : new HttpRequestException("Falha de conexão") };
        var client = new SoapClient(_ => handler);
        if (timeout)
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => client.SendAsync(Endpoint, Action, Envelope, cert, 1000));
        else
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => client.SendAsync(Endpoint, Action, Envelope, cert, 1000));
        Assert.IsTrue(handler.Liberado);
    }
}
