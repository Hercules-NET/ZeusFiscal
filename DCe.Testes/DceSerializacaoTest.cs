using DCe.Utils.Extensoes;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using DCe.Utils.Abstractions;
using DCe.Utils;
using DCe.Testes.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceSerializacaoTest
{
    private static readonly XNamespace Dce = "http://www.portalfiscal.inf.br/dce";
    private static readonly XNamespace Ds = "http://www.w3.org/2000/09/xmldsig#";

    private sealed class ClockFixo : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class RandomFixo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }

    private static string Construir() =>
        new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(DceFixture.Padrao()).ObterXmlString();

    [TestMethod]
    public void ConstruirDce_DeclaraEncodingUtf8()
    {
        Assert.IsTrue(Construir().StartsWith("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.Ordinal));
    }

    /// <summary>O manual proíbe prefixo de namespace, e exige uma única declaração na raiz.</summary>
    [TestMethod]
    public void ConstruirDce_UsaNamespaceSemPrefixo()
    {
        var raiz = XDocument.Parse(Construir()).Root!;

        Assert.AreEqual("DCe", raiz.Name.LocalName);
        Assert.AreEqual(Dce.NamespaceName, raiz.Name.NamespaceName);
        Assert.IsTrue(Construir().IndexOf("xmlns:", StringComparison.Ordinal) < 0);
    }

    /// <summary>Id é o literal "DCe" seguido dos 44 dígitos da chave — é a tag que será assinada.</summary>
    [TestMethod]
    public void ConstruirDce_MontaOIdComOLiteralEAChave()
    {
        var infDCe = XDocument.Parse(Construir()).Root!.Element(Dce + "infDCe")!;
        var id = infDCe.Attribute("Id")!.Value;

        Assert.IsTrue(id.StartsWith("DCe", StringComparison.Ordinal));
        Assert.AreEqual(47, id.Length);
        Assert.IsTrue(id.Substring(3).All(char.IsDigit));
    }

    /// <summary>O cDC injetado tem que aparecer na chave e no ide — se não aparecer, algo gerou outro.</summary>
    [TestMethod]
    public void ConstruirDce_UsaOCodigoAleatorioInjetado()
    {
        var ide = XDocument.Parse(Construir()).Root!.Element(Dce + "infDCe")!.Element(Dce + "ide")!;

        Assert.AreEqual("123456", ide.Element(Dce + "cDC")!.Value);
        Assert.AreEqual("99", ide.Element(Dce + "mod")!.Value);
    }

    /// <summary>
    /// Nesta fixture, o usuário emitente é não contribuinte e o signatário é informado
    /// no grupo Transportadora, separado do grupo emit.
    /// </summary>
    [TestMethod]
    public void ConstruirDce_SeparaUsuarioEmitenteDeQuemAssina()
    {
        var inf = XDocument.Parse(Construir()).Root!.Element(Dce + "infDCe")!;

        Assert.AreEqual("USUARIO NAO CONTRIBUINTE", inf.Element(Dce + "emit")!.Element(Dce + "xNome")!.Value);
        Assert.AreEqual("12345678909", inf.Element(Dce + "emit")!.Element(Dce + "CPF")!.Value);

        var transportadora = inf.Element(Dce + "Transportadora")!;
        Assert.AreEqual("99999999000191", transportadora.Element(Dce + "CNPJ")!.Value);
        Assert.AreEqual("TRANSPORTADORA EXEMPLO LTDA", transportadora.Element(Dce + "xNome")!.Value);
    }

    /// <summary>Otimização exigida pelo manual: sem tag vazia, sem espaço nem quebra entre tags.</summary>
    [TestMethod]
    public void ConstruirDce_NaoEmiteTagVaziaNemFormatacao()
    {
        var xml = Construir();

        Assert.IsTrue(xml.Replace("</", "@@").IndexOf("></", StringComparison.Ordinal) < 0);
        Assert.IsTrue(xml.IndexOf("\n  <", StringComparison.Ordinal) < 0);
        Assert.IsTrue(xml.IndexOf("> <", StringComparison.Ordinal) < 0);
    }

    [TestMethod]
    public void ConstruirDce_ValidaContraOSchemaOficial()
    {
        var xdoc = XDocument.Parse(Construir());
        var id = xdoc.Root!.Element(Dce + "infDCe")!.Attribute("Id")!.Value;
        xdoc.Root!.Add(ConstruirAssinaturaPlaceholder(id));

        var doc = new XmlDocument { Schemas = DceSchemaLoader.Carregar("dce", "dce_v1.00.xsd") };
        doc.LoadXml(xdoc.ToString(SaveOptions.DisableFormatting));

        var erros = new List<string>();
        doc.Validate((_, e) => erros.Add(e.Message));

        Assert.IsTrue(erros.Count == 0, string.Join(" | ", erros));
    }

    /// <summary>
    /// Nó ds:Signature estruturalmente válido para o XSD (SignedInfo/Reference/Transforms
    /// conforme o manual — enveloped-signature + c14n, SHA-1 fixo pelo próprio
    /// xmldsig-core-schema_v1.01.xsd) com DigestValue/SignatureValue/X509Certificate
    /// placeholder. Existe só para fechar a validação de schema neste teste; não tem
    /// significado criptográfico e não faz parte do XML devolvido por Construir.
    /// </summary>
    private static XElement ConstruirAssinaturaPlaceholder(string idAssinado) =>
        new(Ds + "Signature",
            new XElement(Ds + "SignedInfo",
                new XElement(Ds + "CanonicalizationMethod",
                    new XAttribute("Algorithm", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")),
                new XElement(Ds + "SignatureMethod",
                    new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#rsa-sha1")),
                new XElement(Ds + "Reference",
                    new XAttribute("URI", "#" + idAssinado),
                    new XElement(Ds + "Transforms",
                        new XElement(Ds + "Transform",
                            new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#enveloped-signature")),
                        new XElement(Ds + "Transform",
                            new XAttribute("Algorithm", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"))),
                    new XElement(Ds + "DigestMethod",
                        new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#sha1")),
                    new XElement(Ds + "DigestValue", "AAAA"))),
            new XElement(Ds + "SignatureValue", "AAAA"),
            new XElement(Ds + "KeyInfo",
                new XElement(Ds + "X509Data",
                    new XElement(Ds + "X509Certificate", "AAAA"))));

    [TestMethod]
    public void ConstruirDce_EhDeterministico()
    {
        Assert.AreEqual(Construir(), Construir());
    }

    /// <summary>
    /// A SEFAZ compara o LITERAL de dhEmi contra o relógio local dela (cStat 703, regra
    /// B07-10) — mandar em "+00:00" faz um horário local aparecer 3h no futuro mesmo
    /// representando o mesmo instante. O default de <see cref="DCe.Testes.Fixtures.Entradas.DeclaracaoConteudo.OffsetHorario"/>
    /// é -3h; o instante fixo do ClockFixo é 2026-08-22T12:00:00Z.
    /// </summary>
    [TestMethod]
    public void ConstruirDce_EmiteDhEmiNoOffsetLocalPadrao()
    {
        var ide = XDocument.Parse(Construir()).Root!.Element(Dce + "infDCe")!.Element(Dce + "ide")!;

        Assert.AreEqual("2026-08-22T09:00:00-03:00", ide.Element(Dce + "dhEmi")!.Value);
    }

    /// <summary>Offset customizado (ex.: AM/RO/RR/AC, -04:00) precisa ser respeitado.</summary>
    [TestMethod]
    public void ConstruirDce_RespeitaOffsetCustomizado()
    {
        var dc = DceFixture.Padrao();
        dc.OffsetHorario = TimeSpan.FromHours(-4);

        var xml = new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(dc).ObterXmlString();
        var ide = XDocument.Parse(xml).Root!.Element(Dce + "infDCe")!.Element(Dce + "ide")!;

        Assert.AreEqual("2026-08-22T08:00:00-04:00", ide.Element(Dce + "dhEmi")!.Value);
    }

    [DataTestMethod]
    [DataRow("xObs1")]
    [DataRow("xObs2")]
    public void ConstruirDce_TextoFixoDeInfDec_BateComADocumentacaoDoXsd(string elemento)
    {
        var infDec = XDocument.Parse(Construir()).Root!.Element(Dce + "infDCe")!.Element(Dce + "infDec")!;
        var textoNoXml = infDec.Element(Dce + elemento)!.Value;

        var esperado = TextoFixoNoXsd(elemento);

        Assert.AreEqual(esperado, textoNoXml);
    }

    private static string TextoFixoNoXsd(string elemento)
    {
        using var stream = typeof(global::DCe.Classes.Informacoes.DCe).Assembly
            .GetManifestResourceStream("DCe.Classes.Schemas.dce.dceTiposBasico_v1.00.xsd")!;
        using var reader = new StreamReader(stream);
        var xsd = reader.ReadToEnd();

        var blocoElemento = Regex.Match(
            xsd, $@"<xs:element name=""{elemento}"">.*?</xs:element>", RegexOptions.Singleline);
        Assert.IsTrue(blocoElemento.Success, $"bloco do elemento {elemento} não encontrado no XSD");

        var textoFixo = Regex.Match(blocoElemento.Value, "Deverá conter o texto fixo “(.*?)”", RegexOptions.Singleline);
        Assert.IsTrue(textoFixo.Success, $"texto fixo não encontrado na documentação de {elemento}");

        return textoFixo.Groups[1].Value;
    }

    /// <summary>
    /// vUnCom/vProd são TDec_1302_1308: o schema aceita de 2 a 8 decimais.
    /// A serialização preserva a precisão informada, com o mínimo de duas casas decimais.
    /// </summary>
    [DataTestMethod]
    [DataRow("117000.12345678", "117000.12345678")]
    [DataRow("0.12345678", "0.12345678")]
    [DataRow("100", "100.00")]              // minimo do padrao: 2 casas, nunca "100" nem "100.0"
    [DataRow("100.5", "100.50")]
    [DataRow("100.123", "100.123")]
    public void ConstruirDce_PreservaAPrecisaoDosValoresDeItem(string entrada, string esperado)
    {
        var dc = DceFixture.Padrao();
        var valor = decimal.Parse(entrada, System.Globalization.CultureInfo.InvariantCulture);
        dc.Itens[0].ValorUnitario = valor;
        dc.Itens[0].ValorTotal = valor;

        var xml = new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(dc).ObterXmlString();
        var prod = XDocument.Parse(xml).Root!
            .Element(Dce + "infDCe")!.Element(Dce + "det")!.Element(Dce + "prod")!;

        Assert.AreEqual(esperado, prod.Element(Dce + "vUnCom")!.Value);
        Assert.AreEqual(esperado, prod.Element(Dce + "vProd")!.Value);
    }

    /// <summary>Prova de que a precisao ampliada continua casando com o padrao do XSD.</summary>
    [TestMethod]
    public void ConstruirDce_ComOitoCasasDecimais_ContinuaValidoContraOSchema()
    {
        var dc = DceFixture.Padrao();
        dc.Itens[0].ValorUnitario = 117000.12345678m;
        dc.Itens[0].ValorTotal = 117000.12345678m;

        var xml = new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(dc).ObterXmlString();
        var padrao = new System.Text.RegularExpressions.Regex(@"^[0-9]{1,13}(\.[0-9]{2,8})?$");
        var prod = XDocument.Parse(xml).Root!
            .Element(Dce + "infDCe")!.Element(Dce + "det")!.Element(Dce + "prod")!;

        Assert.IsTrue(padrao.IsMatch(prod.Element(Dce + "vUnCom")!.Value));
        Assert.IsTrue(padrao.IsMatch(prod.Element(Dce + "vProd")!.Value));
    }
}
