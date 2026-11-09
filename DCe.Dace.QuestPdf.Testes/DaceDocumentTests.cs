using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using DCe.Classes.Retorno;
using DCe.Testes.Fixtures;
using DCe.Utils;
using DCe.Utils.Abstractions;
using DCe.Utils.Extensoes;
using DCe.Utils.Validacao;
using DFe.Classes.Flags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using UglyToad.PdfPig;

namespace DCe.Dace.QuestPdf.Testes;

[TestClass]
public class DaceDocumentTests
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/dce";
    private static readonly XNamespace Ds = "http://www.w3.org/2000/09/xmldsig#";
    private static readonly Dictionary<(int, int), string> Documentos = new();

    [ClassInitialize]
    public static void Preparar(TestContext _)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        foreach (var ambiente in new[] { 1, 2 })
        foreach (var emissao in new[] { 1, 2 })
            Documentos[(ambiente, emissao)] = CriarProcDce(ambiente, emissao);
    }

    [TestMethod]
    public void Parse_ExtraiConteudoFiscalSemCalcularValores()
    {
        var xml = Documentos[(2, 1)];
        var dados = DaceXmlParser.Parse(xml);
        var doc = XDocument.Parse(xml);
        Assert.AreEqual(doc.Descendants(Ns + "chDCe").Single().Value, dados.ChaveAcesso);
        Assert.AreEqual(1, dados.Numero);
        Assert.AreEqual(1, dados.Serie);
        Assert.AreEqual(new DateTimeOffset(2026, 8, 22, 9, 0, 0, TimeSpan.FromHours(-3)), dados.DataEmissao);
        Assert.AreEqual("PETROLINA/PE", dados.Remetente.CidadeUf);
        StringAssert.Contains(dados.Remetente.Endereco, "56300-000");
        Assert.AreEqual("RECIFE/PE", dados.Destinatario.CidadeUf);
        Assert.AreEqual("Transportadora", dados.ParteAdicionalTipo);
        Assert.AreEqual("99999999000191", dados.ParteAdicional!.Documento);
        Assert.AreEqual("TRANSPORTADORA EXEMPLO LTDA", dados.ParteAdicional.Nome);
        Assert.AreEqual(1, dados.Itens.Count);
        Assert.AreEqual(210342m, dados.ValorTotal);
        Assert.AreEqual(1m, dados.Itens[0].Quantidade);
        Assert.AreEqual(210342m, dados.Itens[0].ValorUnitario);
        Assert.AreEqual("INFORMAÇÕES DO DECLARANTE", dados.InformacoesComplementares);
        Assert.AreEqual("INFORMAÇÕES DO FISCO", dados.InformacoesAdicionaisFisco);
        Assert.AreEqual(doc.Descendants(Ns + "xObs1").Single().Value, dados.ObservacaoLegal1);
        Assert.AreEqual(doc.Descendants(Ns + "xObs2").Single().Value, dados.ObservacaoLegal2);
        Assert.AreEqual(doc.Descendants(Ns + "qrCodDCe").Single().Value, dados.UrlQrCode);
        Assert.AreEqual(11, dados.ChaveFormatada.Split(' ').Length);
        Assert.IsTrue(dados.ChaveFormatada.Split(' ').All(grupo => grupo.Length == 4));
        Assert.AreEqual(dados.ChaveAcesso, dados.ChaveFormatada.Replace(" ", ""));
        // Mesmo com itens divergentes, o total vem exclusivamente do XML.
        doc.Descendants(Ns + "vDC").Single().Value = "99.00";
        Assert.AreEqual(99m, DaceXmlParser.Parse(doc.ToString()).ValorTotal);
    }

    [DataTestMethod]
    [DataRow(DaceModelo.Completa, 1, 1)]
    [DataRow(DaceModelo.Resumida, 1, 1)]
    [DataRow(DaceModelo.Completa, 2, 1)]
    [DataRow(DaceModelo.Resumida, 2, 1)]
    [DataRow(DaceModelo.Completa, 1, 2)]
    [DataRow(DaceModelo.Resumida, 1, 2)]
    [DataRow(DaceModelo.Completa, 2, 2)]
    [DataRow(DaceModelo.Resumida, 2, 2)]
    public void GerarPdf_ConteudoFiscalAmbienteEContingencia(DaceModelo modelo, int ambiente, int emissao)
    {
        var xml = Documentos[(ambiente, emissao)];
        var dados = DaceXmlParser.Parse(xml);
        var bytes = new DaceDocument(xml, modelo).GeneratePdf();
        using var pdf = PdfDocument.Open(bytes);
        var pagina = pdf.GetPage(1);
        var texto = pagina.Text;
        Assert.AreEqual(1, pdf.NumberOfPages);
        Assert.AreEqual(595.28, (double)pagina.Width, 1);
        Assert.AreEqual(841.89, (double)pagina.Height, 1);
        Assert.AreEqual(ambiente == 2, texto.Contains("AMBIENTE DE HOMOLOGAÇÃO"));
        Assert.AreEqual(emissao == 2, texto.Contains("EMITIDA EM CONTINGÊNCIA"));
        Assert.AreEqual(emissao == 1, texto.Contains("1262600000000001"));
        Assert.AreEqual(modelo == DaceModelo.Completa, texto.Contains(dados.Itens[0].Descricao));
        foreach (var valor in new[] { dados.Remetente.Nome, dados.Remetente.Endereco,
                     dados.Destinatario.Endereco, dados.ParteAdicional!.Nome,
                     dados.InformacoesComplementares!, dados.InformacoesAdicionaisFisco!,
                     dados.ObservacaoLegal1, dados.ObservacaoLegal2, dados.ChaveFormatada })
            StringAssert.Contains(SemEspacos(texto), SemEspacos(valor));
        Assert.AreEqual(2, pagina.NumberOfImages); // QR do XML e CODE-128 da chave.
        // Exportação opcional das próprias fixtures verificadas, fora do repositório.
        var pasta = Environment.GetEnvironmentVariable("DACE_REFERENCE_OUTPUT");
        if (!string.IsNullOrWhiteSpace(pasta))
        {
            Directory.CreateDirectory(pasta);
            var nome = $"dace-{modelo.ToString().ToLowerInvariant()}-{(ambiente == 1 ? "producao" : "homologacao")}{(emissao == 2 ? "-contingencia" : "")}";
            File.WriteAllBytes(Path.Combine(pasta, nome + ".pdf"), bytes);
            File.WriteAllText(Path.Combine(pasta, nome + ".xml"), xml);
        }
    }

    [DataTestMethod]
    [DataRow(DaceModelo.Completa)]
    [DataRow(DaceModelo.Resumida)]
    public void SemQrNoXml_NaoInventaUrlNemImagem(DaceModelo modelo)
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        doc.Descendants(Ns + "infDCeSupl").Remove();
        var dados = DaceXmlParser.Parse(doc.ToString());
        Assert.IsNull(dados.UrlQrCode);
        using var pdf = PdfDocument.Open(new DaceDocument(dados, modelo).GeneratePdf());
        Assert.AreEqual(1, pdf.GetPage(1).NumberOfImages);
    }

    [DataTestMethod]
    [DataRow("Fisco", "xOrgao")]
    [DataRow("Marketplace", "xNome")]
    [DataRow("Transportadora", "xNome")]
    [DataRow("ECT", "xNome")]
    public void Parse_IdentificaTodasAsPartesAdicionais(string tipo, string nome)
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        var parte = doc.Descendants(Ns + "Transportadora").Single();
        parte.Name = Ns + tipo;
        parte.Element(Ns + "xNome")!.Name = Ns + nome;
        if (tipo == "Fisco") parte.Add(new XElement(Ns + "UF", "PE"));
        if (tipo == "Marketplace") parte.Add(new XElement(Ns + "Site", "https://marketplace.exemplo.test"));
        Validador.Valida(doc.ToString(), "dce", "procDCe_v1.00.xsd");
        var dados = DaceXmlParser.Parse(doc.ToString());
        Assert.AreEqual(tipo, dados.ParteAdicionalTipo);
        Assert.AreEqual("TRANSPORTADORA EXEMPLO LTDA", dados.ParteAdicional!.Nome);
    }

    [TestMethod]
    public void Parse_ExigeDocumentoProcessadoENamespaceCorreto()
    {
        var dce = XDocument.Parse(Documentos[(2, 1)]).Root!.Element(Ns + "DCe")!;
        Assert.ThrowsException<InvalidOperationException>(() => DaceXmlParser.Parse(dce.ToString()));
        Assert.ThrowsException<InvalidOperationException>(() => DaceXmlParser.Parse("<dceProc />"));
        Assert.ThrowsException<ArgumentException>(() => DaceXmlParser.Parse(" "));
    }

    [TestMethod]
    public void Parse_QrPreservadoLiteralmenteEChaveDoIdQuandoSemProtocolo()
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        var chave = doc.Descendants(Ns + "chDCe").Single().Value;
        doc.Root!.Element(Ns + "protDCe")!.Remove();
        const string url = "https://consulta.exemplo.test/dce?parametro=original&tpAmb=2";
        doc.Descendants(Ns + "qrCodDCe").Single().Value = url;
        var dados = DaceXmlParser.Parse(doc.ToString());
        Assert.AreEqual(url, dados.UrlQrCode);
        Assert.AreEqual(chave, dados.ChaveAcesso);
        Assert.IsNull(dados.Protocolo);
    }

    [DataTestMethod]
    [DataRow("dhEmi")]
    [DataRow("nDC")]
    [DataRow("vDC")]
    public void Parse_CampoObrigatorioAusenteNaoProduzValorInventado(string campo)
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        doc.Descendants(Ns + campo).Single().Remove();
        Assert.ThrowsException<InvalidOperationException>(() => DaceXmlParser.Parse(doc.ToString()));
    }

    private static string SemEspacos(string valor) => new(valor.Where(c => !char.IsWhiteSpace(c)).ToArray());

    [TestMethod]
    public void Parse_ModeloProcessadoPreservaDadosDaFachadaXml()
    {
        var xml = Documentos[(2, 1)];
        var processado = new procDCe().CarregarDeXmlString(xml);
        var porXml = DaceXmlParser.Parse(xml);
        var porModelo = DaceXmlParser.Parse(processado);
        Assert.AreEqual(porXml.ChaveAcesso, porModelo.ChaveAcesso);
        Assert.AreEqual(porXml.DataEmissao, porModelo.DataEmissao);
        Assert.AreEqual(porXml.Remetente, porModelo.Remetente);
        Assert.AreEqual(porXml.Destinatario, porModelo.Destinatario);
        Assert.AreEqual(porXml.ValorTotal, porModelo.ValorTotal);
        Assert.AreEqual(porXml.Protocolo, porModelo.Protocolo);
        Assert.AreEqual(porXml.UrlQrCode, porModelo.UrlQrCode);
        CollectionAssert.AreEqual(porXml.Itens.ToArray(), porModelo.Itens.ToArray());
        Assert.ThrowsException<ArgumentNullException>(() => DaceXmlParser.Parse((procDCe)null!));
    }

    [DataTestMethod]
    [DataRow("infDec")]
    [DataRow("xObs1")]
    [DataRow("xObs2")]
    [DataRow("transp")]
    [DataRow("modTrans")]
    public void Parse_CampoAusenteNaoImprimePadraoDoModelo(string campo)
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        doc.Descendants(Ns + campo).Single().Remove();
        var dados = DaceXmlParser.Parse(doc.ToString());
        if (campo is "infDec" or "xObs1") Assert.AreEqual(string.Empty, dados.ObservacaoLegal1);
        if (campo is "infDec" or "xObs2") Assert.AreEqual(string.Empty, dados.ObservacaoLegal2);
        if (campo is "transp" or "modTrans") Assert.AreEqual(string.Empty, dados.ModalidadeTransporte);
    }

    [DataTestMethod]
    [DataRow("emit", "enderEmit")]
    [DataRow("dest", "enderDest")]
    public void Parse_UfAusenteNaoImprimePrimeiraUfDoEnum(string pessoa, string endereco)
    {
        var doc = XDocument.Parse(Documentos[(2, 1)]);
        doc.Descendants(Ns + pessoa).Single().Element(Ns + endereco)!.Element(Ns + "UF")!.Remove();
        var dados = DaceXmlParser.Parse(doc.ToString());
        var impressa = pessoa == "emit" ? dados.Remetente : dados.Destinatario;
        Assert.IsTrue(impressa.CidadeUf.EndsWith("/", StringComparison.Ordinal));
        Assert.IsFalse(impressa.CidadeUf.EndsWith("/AC", StringComparison.Ordinal));
    }

    private static string CriarProcDce(int ambiente, int emissao)
    {
        var declaracao = DceFixture.Padrao();
        declaracao.Ambiente = (TipoAmbiente)ambiente;
        declaracao.TipoEmissao = emissao;
        declaracao.VersaoAplicativo = "ZeusFiscal 1.0";
        declaracao.InformacoesComplementares = "INFORMAÇÕES DO DECLARANTE";
        var dce = XElement.Parse(new FabricaModeloDCe(new Relogio(), new Codigo()).Criar(declaracao).ObterXmlString());
        dce.Descendants(Ns + "infAdic").Single().AddFirst(new XElement(Ns + "infAdFisco", "INFORMAÇÕES DO FISCO"));
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=Teste DACE", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificado = pedido.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        dce = XElement.Parse(DceAssinatura.Assinar(dce.ToString(SaveOptions.DisableFormatting), certificado));
        var chave = dce.Element(Ns + "infDCe")!.Attribute("Id")!.Value[3..];
        var protocolo = new XElement(Ns + "protDCe", new XAttribute("versao", "1.00"),
            new XElement(Ns + "infProt",
                new XElement(Ns + "tpAmb", ambiente), new XElement(Ns + "verAplic", "ZeusFiscal 1.0"),
                new XElement(Ns + "chDCe", chave), new XElement(Ns + "dhRecbto", "2026-08-22T09:00:01-03:00"),
                new XElement(Ns + "nProt", "1262600000000001"),
                new XElement(Ns + "digVal", dce.Descendants(Ds + "DigestValue").Single().Value),
                new XElement(Ns + "cStat", "100"), new XElement(Ns + "xMotivo", "Autorizado o uso da DC-e")));
        var xml = new XElement(Ns + "dceProc", new XAttribute("versao", "1.00"), dce, protocolo).ToString(SaveOptions.DisableFormatting);
        Validador.Valida(xml, "dce", "procDCe_v1.00.xsd");
        return xml;
    }

    private sealed class Relogio : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class Codigo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }
}
