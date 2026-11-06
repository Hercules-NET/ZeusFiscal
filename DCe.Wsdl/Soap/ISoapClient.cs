using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DCe.Wsdl.Soap;

/// <summary>Transporte substituível; não constrói nem interpreta mensagens fiscais.</summary>
public interface ISoapClient
{
    Task<string> SendAsync(string endpoint, string soapAction, string soapEnvelope,
        X509Certificate2 certificado, int timeOut);
}
