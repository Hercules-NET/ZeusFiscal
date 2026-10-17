using DCe.Utils.Extensoes;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System.Xml.Linq;
using DCe.Utils.Abstractions;
using DCe.Utils.Ibge;
using DCe.Testes.Fixtures.Entradas;
using DCe.Utils;
using DCe.Testes.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

/// <summary>
/// Verifica localmente divergências entre UF e município que a SEFAZ rejeita,
/// identificando os campos incoerentes antes da transmissão.
///
/// Confirmado contra a SEFAZ em homologação: <c>cUF</c> diferente da UF do emitente gera
/// <c>cStat 220</c> ("Codigo da UF da DC-e difere da UF do emitente"). O <c>cUF</c> da chave
/// corresponde à UF do emitente; a UF do destinatário e a do autorizador são independentes.
/// </summary>
[TestClass]
public class DceCoerenciaUfTest
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/dce";

    private sealed class ClockFixo : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc);
    }

    private sealed class RandomFixo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }

    private static string Construir(Action<DeclaracaoConteudo> ajustar)
    {
        var dc = DceFixture.Padrao();
        ajustar(dc);
        return new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(dc).ObterXmlString();
    }

    [TestMethod]
    public void CodigoUf_DivergindoDaUfDoEmitente_Recusa()
    {
        // cUF = 29 (BA) com emitente em PE: divergência correspondente ao cStat 220.
        var erro = Assert.ThrowsException<ArgumentException>(() => Construir(dc => dc.CodigoUf = (Estado)29));

        Assert.IsTrue(erro.Message.IndexOf("cUF", StringComparison.Ordinal) >= 0);
        Assert.IsTrue(erro.Message.IndexOf("29", StringComparison.Ordinal) >= 0);
        Assert.IsTrue(erro.Message.IndexOf("PE", StringComparison.Ordinal) >= 0);
        Assert.IsTrue(erro.Message.IndexOf("220", StringComparison.Ordinal) >= 0);
    }

    [TestMethod]
    public void CodigoUf_CoerenteComOEmitente_Constroi()
    {
        // A fixture já é coerente (cUF 26, emitente em PE): a guarda não pode barrar o caminho feliz
        var xml = Construir(_ => { });

        Assert.IsTrue(xml.IndexOf("<cUF>26</cUF>", StringComparison.Ordinal) >= 0);
    }

    /// <summary>Emitente na BA com cUF 29 também passa — a guarda compara, não fixa uma UF.</summary>
    [TestMethod]
    public void CodigoUf_CoerenteComOutraUf_Constroi()
    {
        var xml = Construir(dc =>
        {
            dc.CodigoUf = (Estado)29;
            dc.Emitente.Endereco.Uf = "BA";
            dc.Emitente.Endereco.CodigoMunicipio = 2918407;   // Juazeiro/BA
            dc.Emitente.Endereco.Municipio = "JUAZEIRO";
        });

        Assert.IsTrue(xml.IndexOf("<cUF>29</cUF>", StringComparison.Ordinal) >= 0);
    }

    [TestMethod]
    public void UfDoEmitente_Desconhecida_Recusa()
    {
        var erro = Assert.ThrowsException<ArgumentException>(() => Construir(dc => dc.Emitente.Endereco.Uf = "XX"));

        Assert.IsTrue(erro.Message.IndexOf("XX", StringComparison.Ordinal) >= 0);
    }

    /// <summary>Regra C09-20 do Anexo I — rejeição 273.</summary>
    [TestMethod]
    public void MunicipioDoEmitente_DeOutraUf_Recusa()
    {
        var erro = Assert.ThrowsException<ArgumentException>(
            () => Construir(dc => dc.Emitente.Endereco.CodigoMunicipio = 2918407));   // Juazeiro/BA num emitente PE

        Assert.IsTrue(erro.Message.IndexOf("2918407", StringComparison.Ordinal) >= 0);
        Assert.IsTrue(erro.Message.IndexOf("273", StringComparison.Ordinal) >= 0);
    }

    /// <summary>Regra E10-20 do Anexo I — rejeição 275.</summary>
    [TestMethod]
    public void MunicipioDoDestinatario_DeOutraUf_Recusa()
    {
        var erro = Assert.ThrowsException<ArgumentException>(
            () => Construir(dc => dc.Destinatario.Endereco.CodigoMunicipio = 2918407));   // Juazeiro/BA num dest PE

        Assert.IsTrue(erro.Message.IndexOf("275", StringComparison.Ordinal) >= 0);
    }

    /// <summary>
    /// Destinatário no exterior é a exceção que a própria E10-20 abre: UF "EX" e município
    /// 9999999. A guarda não pode confundir isso com incoerência.
    /// </summary>
    [TestMethod]
    public void DestinatarioNoExterior_NaoEhBarrado()
    {
        var xml = Construir(dc =>
        {
            dc.Destinatario.Endereco.Uf = "EX";
            dc.Destinatario.Endereco.CodigoMunicipio = 9999999;
            dc.Destinatario.Endereco.CodigoPais = 2496;          // BACEN — Uruguai
            dc.Destinatario.Endereco.Pais = "URUGUAI";
        });

        var ender = XDocument.Parse(xml).Root!
            .Element(Ns + "infDCe")!.Element(Ns + "dest")!.Element(Ns + "enderDest")!;

        Assert.AreEqual("EX", ender.Element(Ns + "UF")!.Value);
        Assert.AreEqual("9999999", ender.Element(Ns + "cMun")!.Value);
    }

    /// <summary>
    /// O tipo <c>TUf</c> de <c>tiposGeralDCe_v1.00.xsd</c> enumera <c>EX</c> junto das
    /// 27 siglas. O endereço <c>enderDest</c> usa <c>TEndeDest</c>, cujo campo <c>UF</c>
    /// tem tipo <c>TUf</c>. O teste confere essa permissão no schema embutido.
    /// </summary>
    [TestMethod]
    public void SchemaAdmiteUfExterior()
    {
        using var schema = typeof(global::DCe.Classes.Informacoes.DCe).Assembly
            .GetManifestResourceStream("DCe.Classes.Schemas.dce.tiposGeralDCe_v1.00.xsd")!;
        var texto = new StreamReader(schema).ReadToEnd();
        var tUf = texto.Substring(texto.IndexOf("name=\"TUf\"", StringComparison.Ordinal));
        tUf = tUf.Substring(0, tUf.IndexOf("</xs:simpleType>", StringComparison.Ordinal));

        Assert.IsTrue(tUf.IndexOf("<xs:enumeration value=\"EX\"/>", StringComparison.Ordinal) >= 0);
    }

    /// <summary>
    /// O emitente é sempre nacional — a DC-e acoberta transporte dentro do país. "EX" no
    /// emitente é erro, não exceção.
    /// </summary>
    [TestMethod]
    public void EmitenteNoExterior_Recusa()
    {
        Assert.ThrowsException<ArgumentException>(() => Construir(dc => dc.Emitente.Endereco.Uf = "EX"));
    }

    [DataTestMethod]
    [DataRow("PE", 26)]
    [DataRow("BA", 29)]
    [DataRow("pe", 26)]
    [DataRow("SP", 35)]
    [DataRow("DF", 53)]
    public void TabelaDeUf_MapeiaSiglaParaCodigoIbge(string sigla, int esperado)
    {
        Assert.AreEqual(esperado, UnidadesFederativas.CodigoDe(sigla));
    }

    [TestMethod]
    public void TabelaDeUf_TemAsVinteESeteUnidades()
    {
        Assert.AreEqual(27, Enumerable.Range(11, 43).Count(c => UnidadesFederativas.SiglaDe(c) is not null));
    }

    [TestMethod]
    public void TabelaDeUf_SiglaDesconhecida_DevolveNulo()
    {
        Assert.IsNull(UnidadesFederativas.CodigoDe("XX"));
        Assert.IsNull(UnidadesFederativas.CodigoDe(null));
        Assert.IsNull(UnidadesFederativas.SiglaDe(99));
    }
}
