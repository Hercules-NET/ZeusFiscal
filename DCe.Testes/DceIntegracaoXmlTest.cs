using DCe.Utils.Extensoes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;
using DCe.Testes.Fixtures.Entradas;
using DCe.Testes.Fixtures;
using DCe.Utils;
using DCe.Utils.Abstractions;
using DCe.Utils.Validacao;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceIntegracaoXmlTest
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/dce";

    private sealed class RelogioVirada : IClock
    {
        public DateTime UtcNow => new(2026, 9, 1, 1, 0, 0, DateTimeKind.Utc);
    }

    private sealed class CodigoFixo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }

    private sealed class RelogioFixo(int hora) : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, hora, 0, 0, DateTimeKind.Utc);
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void XmlGerado_CorrespondeIntegralmenteAFixture(bool evento)
    {
        var builder = new FabricaModeloDCe(new RelogioFixo(evento ? 15 : 12), new CodigoFixo());
        var xml = evento ? builder.Criar(new CancelamentoDce
        {
            ChaveAcesso = "26260899999999000191990010000000011300123455",
            CodigoOrgao = DFe.Classes.Entidades.Estado.PE,
            CnpjAutor = "99999999000191",
            CpfUsuarioEmitente = "12345678909",
            NumeroProtocolo = "1262600000000001",
            Justificativa = "TRANSPORTE CANCELADO A PEDIDO DO DECLARANTE"
        }).ObterXmlString() : builder.Criar(DceFixture.Padrao()).ObterXmlString();
        var referencia = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", evento ? "Cancelamento.xml" : "Declaracao.xml"));
        Assert.AreEqual(referencia, xml);
    }

    [DataTestMethod]
    [DataRow(-3, "2026-08-31T22:00:00-03:00")]
    [DataRow(-4, "2026-08-31T21:00:00-04:00")]
    [DataRow(-5, "2026-08-31T20:00:00-05:00")]
    public void ChaveEDhEmi_UsamOMesmoMesLocal(int horas, string dataEsperada)
    {
        var dc = DceFixture.Padrao();
        dc.OffsetHorario = TimeSpan.FromHours(horas);
        var inf = XDocument.Parse(new FabricaModeloDCe(new RelogioVirada(), new CodigoFixo())
            .Criar(dc).ObterXmlString()).Root!.Element(Ns + "infDCe")!;
        Assert.AreEqual("2608", inf.Attribute("Id")!.Value.Substring(5, 4));
        Assert.AreEqual(dataEsperada, inf.Element(Ns + "ide")!.Element(Ns + "dhEmi")!.Value);
    }

#if NET8_0_OR_GREATER
    // CertificateRequest permite testar com chave efêmera sem distribuir certificados.
    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void AssinaturaZeus_ValidaCriptografiaESchema(bool evento)
    {
        var builder = new FabricaModeloDCe(new RelogioVirada(), new CodigoFixo());
        var dc = DceFixture.Padrao();
        var xml = builder.Criar(dc).ObterXmlString();
        if (evento)
        {
            var chave = XDocument.Parse(xml).Root!.Element(Ns + "infDCe")!.Attribute("Id")!.Value.Substring(3);
            xml = builder.Criar(new CancelamentoDce
            {
                ChaveAcesso = chave,
                CodigoOrgao = dc.CodigoUf,
                CnpjAutor = dc.Signatario.Cnpj,
                CpfUsuarioEmitente = dc.Emitente.Cpf,
                NumeroProtocolo = "1262600000000001",
                Justificativa = "TRANSPORTE CANCELADO A PEDIDO DO DECLARANTE"
            }).ObterXmlString();
        }

        using var chaveRsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=Teste DCe", chaveRsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificado = pedido.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        var assinado = DceAssinatura.Assinar(xml, certificado);
        Validador.Valida(assinado, evento ? "evento" : "dce", evento ? "eventoDCe_v1.00.xsd" : "dce_v1.00.xsd");

        var documento = new XmlDocument { PreserveWhitespace = true };
        documento.LoadXml(assinado);
        var assinatura = new SignedXml(documento);
        assinatura.LoadXml((XmlElement)documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!);
        Assert.AreEqual(SignedXml.XmlDsigRSASHA1Url, assinatura.SignedInfo!.SignatureMethod);
        Assert.AreEqual(SignedXml.XmlDsigC14NTransformUrl, assinatura.SignedInfo.CanonicalizationMethod);
        Assert.IsTrue(assinatura.CheckSignature(certificado, true));
        Assert.ThrowsException<ArgumentException>(() => DceAssinatura.Assinar(assinado, certificado));

        // Alterar um campo assinado deve invalidar a verificação criptográfica.
        documento.GetElementsByTagName(evento ? "xJust" : "xNome", Ns.NamespaceName)[0]!.InnerText = "ALTERADO";
        var adulterada = new SignedXml(documento);
        adulterada.LoadXml((XmlElement)documento.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!);
        Assert.IsFalse(adulterada.CheckSignature(certificado, true));
    }
#endif

    [TestMethod]
    public void LerConsulta_ExtraiProtocoloEOffset()
    {
        const string chave = "26260899999999000191990010000000011300123455";
        var xml = $"""
            <retConsSitDCe xmlns="http://www.portalfiscal.inf.br/dce" versao="1.00">
            <tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cStat>100</cStat>
            <xMotivo>Autorizado o uso da DCe</xMotivo><cUF>26</cUF>
            <dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto><chDCe>{chave}</chDCe>
            <protDCe versao="1.00"><infProt><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic>
            <chDCe>{chave}</chDCe><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto>
            <nProt>1262600000000001</nProt><digVal>SGVsbG8=</digVal>
            <cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo></infProt></protDCe>
            </retConsSitDCe>
            """;
        Validador.Valida(xml, "consulta", "retConsSitDCe_v1.00.xsd");
        var retorno = new global::DCe.Classes.Retorno.retConsSitDCe().CarregarDeXmlString(xml);
        Assert.AreEqual(chave, retorno.ChaveAcesso);
        Assert.AreEqual("1262600000000001", retorno.Protocolo!.NumeroProtocolo);
        Assert.AreEqual(TimeSpan.FromHours(-3), retorno.Protocolo.DataRecebimento!.Value.Offset);
        Assert.AreEqual("100", retorno.CodigoStatus);
    }

    [TestMethod]
    public void DestinatarioExterior_ComMunicipioNacional_RecusaCom509()
    {
        var dc = DceFixture.Padrao();
        dc.Destinatario.Endereco.Uf = "EX";
        var erro = Assert.ThrowsException<ArgumentException>(() => new FabricaModeloDCe().Criar(dc).ObterXmlString());
        StringAssert.Contains(erro.Message, "509");
    }

    [TestMethod]
    public void Validador_RecusaXmlInvalidoERecursosExternos()
    {
        Assert.ThrowsException<System.Xml.Schema.XmlSchemaValidationException>(() =>
            Validador.Valida("<DCe xmlns='http://www.portalfiscal.inf.br/dce'><naoExiste/></DCe>", "dce", "dce_v1.00.xsd"));
        Assert.ThrowsException<XmlException>(() => Validador.Valida(
            "<!DOCTYPE DCe [<!ENTITY externo SYSTEM 'file:///nao-existe'>]><DCe>&externo;</DCe>", "dce", "dce_v1.00.xsd"));
    }
}
