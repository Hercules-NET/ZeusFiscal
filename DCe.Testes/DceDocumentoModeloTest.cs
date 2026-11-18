using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Xml;
using DCe.Classes.Servicos.Evento;
using DCe.Utils.Extensoes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Testes;

[TestClass]
public class DceDocumentoModeloTest
{
    private static string Fixture(bool evento) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", evento ? "Cancelamento.xml" : "Declaracao.xml"));

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Modelo_LeEReproduzFixtureLiteralmente(bool evento)
    {
        var xml = Fixture(evento);
        var saida = evento ? new eventoDCe().CarregarDeXmlString(xml).ObterXmlString() : new DCeDocumento().CarregarDeXmlString(xml).ObterXmlString();
        Assert.AreEqual(xml, saida);
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ModeloAssinado_IdaVoltaPreservaXmlSchemaECriptografia(bool evento)
    {
        using var certificado = DceServiceTest.Autoassinado();
        var xml = evento
            ? new eventoDCe().CarregarDeXmlString(Fixture(true)).Assina(certificado).ObterXmlString()
            : new DCeDocumento().CarregarDeXmlString(Fixture(false)).Assina(certificado).ObterXmlString();
        string saida;
        if (evento)
        {
            var modelo = new eventoDCe().CarregarDeXmlString(xml);
            modelo.Valida();
            saida = modelo.ObterXmlString();
        }
        else
        {
            var modelo = new DCeDocumento().CarregarDeXmlString(xml);
            modelo.Valida();
            saida = modelo.ObterXmlString();
        }
        Assert.AreEqual(xml, saida);
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(saida);
        var assinatura = new SignedXml(doc);
        assinatura.LoadXml((XmlElement)doc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0]!);
        Assert.IsTrue(assinatura.CheckSignature(certificado, true));
    }

    [DataTestMethod]
    [DataRow("pt-BR", -3)]
    [DataRow("en-US", -4)]
    [DataRow("ar-SA", -5)]
    public void PrepararModelo_ChaveUsaOffsetDaEmissaoECnpjDoSignatario(string cultura, int offset)
    {
        var anterior = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultura);
            var dce = new DCeDocumento().CarregarDeXmlString(Fixture(false));
            dce.infDCe.ide.dhEmi = new DateTimeOffset(2026, 9, 1, 1, 0, 0, TimeSpan.Zero).ToOffset(TimeSpan.FromHours(offset));
            dce.Preparar("99999999000191");
            Assert.AreEqual("2608", dce.infDCe.Id.Substring(5, 4));
            Assert.AreEqual("99999999000191", dce.infDCe.Id.Substring(9, 14));
            var volta = new DCeDocumento().CarregarDeXmlString(dce.ObterXmlString());
            Assert.AreEqual(TimeSpan.FromHours(offset), volta.infDCe.ide.dhEmi.Offset);
            Assert.AreEqual(dce.ObterXmlString(), volta.ObterXmlString());
        }
        finally { CultureInfo.CurrentCulture = anterior; }
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Modelo_ArquivoIdaVoltaPreservaConteudo(bool evento)
    {
        var caminho = Path.Combine(Path.GetTempPath(), "dce-modelo-" + Guid.NewGuid().ToString("N") + ".xml");
        try
        {
            if (evento)
            {
                var modelo = new eventoDCe().CarregarDeXmlString(Fixture(true));
                modelo.SalvarXmlEmDisco(caminho);
                Assert.AreEqual(Fixture(true), new eventoDCe().CarregarDeArquivoXml(caminho).ObterXmlString());
            }
            else
            {
                var modelo = new DCeDocumento().CarregarDeXmlString(Fixture(false));
                modelo.SalvarXmlEmDisco(caminho);
                Assert.AreEqual(Fixture(false), new DCeDocumento().CarregarDeArquivoXml(caminho).ObterXmlString());
            }
        }
        finally { File.Delete(caminho); }
    }
}
