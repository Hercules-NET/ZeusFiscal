using DCe.Utils.Extensoes;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System.Xml;
using DCe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceLeituraModeloTest
{
    private const string Ns = "http://www.portalfiscal.inf.br/dce";
    private const string Chave = "26260899999999000191990010000000011300123455";

    /// <summary>
    /// Valida o fixture contra o schema oficial ANTES de entregá-lo ao parser. Fixture que
    /// o XSD recusaria testaria o parser contra algo que a SEFAZ nunca mandaria.
    /// </summary>
    private static string Valido(string xml, string pasta, string arquivo)
    {
        var doc = new XmlDocument { Schemas = DceSchemaLoader.Carregar(pasta, arquivo) };
        doc.LoadXml(xml);

        var erros = new List<string>();
        doc.Validate((_, e) => erros.Add(e.Message));
        Assert.IsTrue(erros.Count == 0, $"fixture inválido contra {arquivo}: {string.Join(" | ", erros)}");

        return xml;
    }

    private static string RetDceAutorizado() => Valido($"""
        <retDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic>
        <cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo><cUF>26</cUF>
        <dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto>
        <protDCe versao="1.00"><infProt><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic>
        <chDCe>{Chave}</chDCe><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto>
        <nProt>1262600000000001</nProt><digVal>SGVsbG8=</digVal>
        <cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo></infProt></protDCe></retDCe>
        """.Replace("\r\n", "").Replace("\n", ""), "dce", "retDCe_v1.00.xsd");

    [TestMethod]
    public void LerAutorizacao_ExtraiStatusEProtocolo()
    {
        var r = new global::DCe.Classes.Retorno.retDCe().CarregarDeXmlString(RetDceAutorizado());

        Assert.AreEqual("100", r.CodigoStatus);
        Assert.IsTrue(r.Autorizada);
        Assert.AreEqual("Autorizado o uso da DCe", r.Motivo);
        Assert.AreEqual("2", r.Ambiente);
        Assert.IsNotNull(r.Protocolo);
        Assert.AreEqual(Chave, r.Protocolo!.ChaveAcesso);
        Assert.AreEqual("1262600000000001", r.Protocolo.NumeroProtocolo);
    }

    /// <summary>
    /// Rejeição não traz protDCe. O parser tem que devolver o motivo e Protocolo nulo, sem
    /// lançar — é o caminho mais comum em homologação.
    /// </summary>
    [TestMethod]
    public void LerAutorizacao_TrataRejeicaoSemProtocolo()
    {
        var xml = Valido($"""
            <retDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic>
            <cStat>203</cStat><xMotivo>Rejeicao: Emissor nao habilitado para emissao de DCe</xMotivo>
            <cUF>26</cUF><dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto></retDCe>
            """.Replace("\r\n", "").Replace("\n", ""), "dce", "retDCe_v1.00.xsd");

        var r = new global::DCe.Classes.Retorno.retDCe().CarregarDeXmlString(xml);

        Assert.AreEqual("203", r.CodigoStatus);
        Assert.IsFalse(r.Autorizada);
        Assert.IsNull(r.Protocolo);
        Assert.IsTrue(r.Motivo.IndexOf("nao habilitado", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    [TestMethod]
    public void LerStatusServico_ExtraiOsCamposOpcionais()
    {
        var xml = Valido($"""
            <retConsStatServDCe xmlns="{Ns}" versao="1.00"><tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic>
            <cStat>107</cStat><xMotivo>Servico em Operacao</xMotivo><cUF>26</cUF>
            <dhRecbto>2026-08-22T12:00:00-03:00</dhRecbto><tMed>1</tMed></retConsStatServDCe>
            """.Replace("\r\n", "").Replace("\n", ""), "consulta", "retConsStatServDCe_v1.00.xsd");

        var r = new global::DCe.Classes.Retorno.retConsStatServDCe().CarregarDeXmlString(xml);

        Assert.IsTrue(r.EmOperacao);
        Assert.AreEqual(1, r.TempoMedioResposta);
        Assert.IsNull(r.PrevisaoRetorno);
    }

    [TestMethod]
    public void LerEvento_ExtraiORegistroDoCancelamento()
    {
        var xml = Valido($"""
            <retEventoDCe xmlns="{Ns}" versao="1.00"><infEvento><tpAmb>2</tpAmb>
            <verAplic>PR-1.0</verAplic><cOrgao>26</cOrgao><cStat>135</cStat>
            <xMotivo>Evento registrado e vinculado a DCe</xMotivo><chDCe>{Chave}</chDCe>
            <tpEvento>110111</tpEvento><xEvento>Cancelamento</xEvento><nSeqEvento>1</nSeqEvento>
            <dhRegEvento>2026-08-22T13:00:00-03:00</dhRegEvento>
            <nProt>1262600000000002</nProt></infEvento></retEventoDCe>
            """.Replace("\r\n", "").Replace("\n", ""), "evento", "retEventoDCe_v1.00.xsd");

        var r = new global::DCe.Classes.Retorno.retEventoDCe().CarregarDeXmlString(xml);

        Assert.IsTrue(r.Registrado);
        Assert.AreEqual("110111", r.TipoEvento);
        Assert.AreEqual("1262600000000002", r.NumeroProtocolo);
        Assert.AreEqual(Chave, r.ChaveAcesso);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("<naoEhResposta/>")]
    public void LerAutorizacao_LancaComEntradaQueNaoEhResposta(string entrada)
    {
        if (entrada.Length == 0)
            Assert.ThrowsException<ArgumentException>(() => new global::DCe.Classes.Retorno.retDCe().CarregarDeXmlString(entrada));
        else
            Assert.ThrowsException<InvalidOperationException>(() => new global::DCe.Classes.Retorno.retDCe().CarregarDeXmlString(entrada));
    }
}
