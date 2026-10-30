using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using DCe.Classes.Retorno;
using DCe.Classes.Servicos.Evento;
using DCe.Utils;
using DCe.Utils.Extensoes;
using DCe.Utils.Validacao;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceRetornosModeloTest
{
    private const string Namespace = "http://www.portalfiscal.inf.br/dce";
    private const string Chave = "26260899999999000191990010000000011300123455";
    private const string Data = "2026-08-31T23:59:59-04:00";
    private static readonly XNamespace Ns = Namespace;

    private static string Cabecalho(int status) =>
        $"<tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cStat>{status}</cStat>" +
        $"<xMotivo>Resposta do autorizador</xMotivo><cUF>26</cUF><dhRecbto>{Data}</dhRecbto>";

    private static string Protocolo(bool opcionais = true) =>
        "<protDCe versao=\"1.00\"><infProt Id=\"ID1262600000000001\">" +
        $"<tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><chDCe>{Chave}</chDCe><dhRecbto>{Data}</dhRecbto>" +
        (opcionais ? "<nProt>1262600000000001</nProt><digVal>SGVsbG8=</digVal>" : "") +
        "<cStat>100</cStat><xMotivo>Autorizado o uso da DCe</xMotivo></infProt>" +
        (opcionais ? "<infFisco><cMsg>123</cMsg><xMsg>Mensagem do Fisco</xMsg></infFisco>" : "") + "</protDCe>";

    private static string RetEvento(bool opcionais = true) =>
        "<retEventoDCe versao=\"1.00\"><infEvento Id=\"ID1262600000000002\">" +
        "<tpAmb>2</tpAmb><verAplic>PR-1.0</verAplic><cOrgao>26</cOrgao><cStat>135</cStat>" +
        "<xMotivo>Evento registrado e vinculado a DCe</xMotivo>" +
        (opcionais ? $"<chDCe>{Chave}</chDCe><tpEvento>110111</tpEvento><xEvento>Cancelamento</xEvento>" +
            $"<nSeqEvento>001</nSeqEvento><dhRegEvento>{Data}</dhRegEvento><nProt>1262600000000002</nProt>" : "") +
        "</infEvento></retEventoDCe>";

    private static string Raiz(string nome, string conteudo, string atributos = "") =>
        $"<{nome} xmlns=\"{Namespace}\" versao=\"1.00\"{atributos}>{conteudo}</{nome}>";

    private static XElement ConteudoXml(string xml)
    {
        var raiz = XElement.Parse(xml);
        // Declarações de namespace e ordem dos atributos não alteram o conteúdo fiscal.
        foreach (var elemento in raiz.DescendantsAndSelf())
        {
            var atributos = elemento.Attributes().Where(a => !a.IsNamespaceDeclaration)
                .OrderBy(a => a.Name.ToString(), StringComparer.Ordinal).ToList();
            elemento.RemoveAttributes();
            elemento.Add(atributos);
        }
        return raiz;
    }

    private static T IdaEVolta<T>(T modelo, string xml, string pasta, string schema) where T : class
    {
        Validador.Valida(xml, pasta, schema);
        var primeiro = modelo.CarregarDeXmlString(xml);
        var serializado = primeiro.ObterXmlString();
        Validador.Valida(serializado, pasta, schema);
        Assert.IsTrue(XNode.DeepEquals(ConteudoXml(xml), ConteudoXml(serializado)), "Nenhum campo do XML original pode ser perdido.");
        var relido = modelo.CarregarDeXmlString(serializado);
        Assert.AreEqual(serializado, relido.ObterXmlString(), "O XML do modelo deve ser idêntico após leitura e nova escrita.");
        return relido;
    }

    [TestMethod]
    public void Autorizacao_ProtocoloCompletoPreservaCamposEOffset()
    {
        var retorno = IdaEVolta(new retDCe(), Raiz("retDCe", Cabecalho(100) + Protocolo()), "dce", "retDCe_v1.00.xsd");
        Assert.AreEqual(TipoAmbiente.Homologacao, retorno.tpAmb);
        Assert.AreEqual(Estado.PE, retorno.cUF);
        Assert.AreEqual(100, retorno.cStat);
        Assert.AreEqual(Data, retorno.dhRecbtoProxy);
        Assert.AreEqual(TimeSpan.FromHours(-4), retorno.dhRecbto.Offset);
        Assert.AreEqual("ID1262600000000001", retorno.protDCe!.infProt!.Id);
        Assert.AreEqual(Chave, retorno.protDCe.infProt.chDCe);
        Assert.AreEqual("1262600000000001", retorno.protDCe.infProt.nProt);
        Assert.AreEqual("SGVsbG8=", retorno.protDCe.infProt.digVal);
        Assert.AreEqual(Data, retorno.protDCe.infProt.dhRecbtoProxy);
        Assert.AreEqual(123, retorno.protDCe.infFisco!.cMsg);
        Assert.AreEqual("Mensagem do Fisco", retorno.protDCe.infFisco.xMsg);
    }

    [TestMethod]
    public void Rejeicao_OmiteProtocoloAusente()
    {
        var retorno = IdaEVolta(new retDCe(), Raiz("retDCe", Cabecalho(203)), "dce", "retDCe_v1.00.xsd");
        Assert.AreEqual(203, retorno.cStat);
        Assert.IsNull(retorno.protDCe);
        Assert.IsFalse(retorno.Autorizada);
        Assert.IsFalse(XDocument.Parse(retorno.ObterXmlString()).Descendants(Ns + "protDCe").Any());
    }

    [TestMethod]
    public void Protocolo_OmiteCamposOpcionaisAusentes()
    {
        var retorno = IdaEVolta(new retDCe(), Raiz("retDCe", Cabecalho(100) + Protocolo(false)), "dce", "retDCe_v1.00.xsd");
        var protocolo = retorno.protDCe!;
        Assert.IsNull(protocolo.infProt!.nProt);
        Assert.IsNull(protocolo.infProt.digVal);
        Assert.IsNull(protocolo.infFisco);
        Assert.IsNull(protocolo.Signature);
        var nomes = XDocument.Parse(protocolo.ObterXmlString()).Descendants().Select(e => e.Name.LocalName).ToList();
        foreach (var nome in new[] { "nProt", "digVal", "infFisco", "Signature" }) Assert.IsFalse(nomes.Contains(nome));
    }

    [TestMethod]
    public void StatusServico_PreservaOpcionaisEOffsetDeRetorno()
    {
        const string previsao = "2026-09-01T01:30:00+05:00";
        var retorno = IdaEVolta(new retConsStatServDCe(), Raiz("retConsStatServDCe", Cabecalho(108) +
            $"<tMed>0</tMed><dhRetorno>{previsao}</dhRetorno><xObs>Manutencao programada</xObs>"), "consulta", "retConsStatServDCe_v1.00.xsd");
        Assert.AreEqual(0, retorno.tMed);
        Assert.AreEqual(previsao, retorno.dhRetornoProxy);
        Assert.AreEqual(TimeSpan.FromHours(5), retorno.dhRetorno!.Value.Offset);
        Assert.AreEqual("Manutencao programada", retorno.xObs);
        retorno.dhRetorno = new DateTimeOffset(2026, 9, 1, 2, 0, 0, TimeSpan.FromHours(-3));
        Assert.AreEqual("2026-09-01T02:00:00-03:00", retorno.dhRetornoProxy);
    }

    [TestMethod]
    public void StatusServico_OmiteOpcionaisAusentes()
    {
        var retorno = IdaEVolta(new retConsStatServDCe(), Raiz("retConsStatServDCe", Cabecalho(107)), "consulta", "retConsStatServDCe_v1.00.xsd");
        Assert.IsNull(retorno.tMed);
        Assert.IsNull(retorno.dhRetorno);
        Assert.IsNull(retorno.xObs);
        var nomes = XDocument.Parse(retorno.ObterXmlString()).Descendants().Select(e => e.Name.LocalName).ToList();
        foreach (var nome in new[] { "tMed", "dhRetorno", "xObs" }) Assert.IsFalse(nomes.Contains(nome));
    }

    [TestMethod]
    public void Evento_PreservaTodosCamposEOpcionalAusente()
    {
        var xml = XElement.Parse(RetEvento());
        xml.Name = Ns + "retEventoDCe";
        foreach (var e in xml.Descendants()) e.Name = Ns + e.Name.LocalName;
        var retorno = IdaEVolta(new retEventoDCe(), xml.ToString(), "evento", "retEventoDCe_v1.00.xsd");
        Assert.AreEqual("ID1262600000000002", retorno.infEvento!.Id);
        Assert.AreEqual(26, (int)retorno.infEvento.cOrgao);
        Assert.AreEqual(135, retorno.infEvento.cStat);
        Assert.AreEqual(Chave, retorno.infEvento.chDCe);
        Assert.AreEqual("110111", retorno.infEvento.tpEvento);
        Assert.AreEqual("Cancelamento", retorno.infEvento.xEvento);
        Assert.AreEqual("001", retorno.infEvento.nSeqEvento);
        Assert.AreEqual(Data, retorno.infEvento.dhRegEventoProxy);
        Assert.AreEqual(TimeSpan.FromHours(-4), retorno.infEvento.dhRegEvento!.Value.Offset);
        Assert.AreEqual("1262600000000002", retorno.infEvento.nProt);
        var minimo = RetEvento(false).Replace("<retEventoDCe ", $"<retEventoDCe xmlns=\"{Namespace}\" ");
        var ausente = IdaEVolta(new retEventoDCe(), minimo, "evento", "retEventoDCe_v1.00.xsd");
        Assert.IsNull(ausente.infEvento!.chDCe);
        Assert.IsNull(ausente.infEvento.tpEvento);
        Assert.IsNull(ausente.infEvento.xEvento);
        Assert.IsNull(ausente.infEvento.nSeqEvento);
        Assert.IsNull(ausente.infEvento.dhRegEvento);
        Assert.IsNull(ausente.infEvento.nProt);
        Assert.IsFalse(XDocument.Parse(ausente.ObterXmlString()).Descendants(Ns + "dhRegEvento").Any());
    }

    [TestMethod]
    public void Consulta_PreservaChaveProtocoloEAusenciaDeEventos()
    {
        var retorno = IdaEVolta(new retConsSitDCe(), Raiz("retConsSitDCe", Cabecalho(100) +
            $"<chDCe>{Chave}</chDCe>" + Protocolo(false)), "consulta", "retConsSitDCe_v1.00.xsd");
        Assert.AreEqual(Chave, retorno.chDCe);
        Assert.AreEqual(Chave, retorno.protDCe!.infProt!.chDCe);
        Assert.AreEqual(0, retorno.procEventoDCe.Count);
        Assert.IsFalse(XDocument.Parse(retorno.ObterXmlString()).Descendants(Ns + "procEventoDCe").Any());
    }

    [TestMethod]
    public void Processados_PreservamRaizesModelosAssinaturasEAtributosDeConexao()
    {
        using var rsa = RSA.Create(2048);
        var pedido = new CertificateRequest("CN=Teste retornos DCe", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificado = pedido.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1));
        var documento = XElement.Parse(DceAssinatura.Assinar(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Declaracao.xml")), certificado));
        var modeloEvento = new eventoDCe().CarregarDeXmlString(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cancelamento.xml")));
        Validador.Valida(modeloEvento.infEvento.detEvento.evCancDCe.ObterXmlString(), "evento", "evCancDCe_v1.00.xsd");
        var evento = XElement.Parse(modeloEvento.Assina(certificado).ObterXmlString());
        var atributos = $" ipTransmissor=\"192.0.2.1\" nPortaCon=\"0\" dhConexao=\"{Data}\"";
        var processado = IdaEVolta(new procDCe(), Raiz("dceProc", documento + Protocolo(), atributos), "dce", "procDCe_v1.00.xsd");
        Assert.IsNotNull(processado.DCe!.Signature);
        Assert.AreEqual("192.0.2.1", processado.ipTransmissor);
        Assert.AreEqual("0", processado.nPortaCon);
        Assert.AreEqual(Data, processado.dhConexaoProxy);
        Assert.AreEqual(TimeSpan.FromHours(-4), processado.dhConexao!.Value.Offset);
        Assert.AreEqual(documento.Element(Ns + "infDCe")!.Attribute("Id")!.Value, processado.DCe.infDCe.Id);
        Assert.AreEqual("dceProc", XDocument.Parse(processado.ObterXmlString()).Root!.Name.LocalName);
        var eventoProcessado = IdaEVolta(new procEventoDCe(), Raiz("procEventoDCe", evento + RetEvento(), atributos), "evento", "procEventoDCe_v1.00.xsd");
        Assert.IsNotNull(eventoProcessado.eventoDCe!.Signature);
        Assert.AreEqual("1262600000000002", eventoProcessado.retEventoDCe!.infEvento!.nProt);
        Assert.AreEqual(Data, eventoProcessado.dhConexaoProxy);
        Assert.AreEqual("procEventoDCe", XDocument.Parse(eventoProcessado.ObterXmlString()).Root!.Name.LocalName);
        processado.protDCe!.Signature = processado.DCe.Signature;
        eventoProcessado.retEventoDCe.Signature = eventoProcessado.eventoDCe.Signature;
        Validador.Valida(processado.ObterXmlString(), "dce", "procDCe_v1.00.xsd");
        Validador.Valida(eventoProcessado.ObterXmlString(), "evento", "procEventoDCe_v1.00.xsd");
        var protocoloRelido = new procDCe().CarregarDeXmlString(processado.ObterXmlString()).protDCe!;
        var retornoEventoRelido = new procEventoDCe().CarregarDeXmlString(eventoProcessado.ObterXmlString()).retEventoDCe!;
        Assert.AreEqual(processado.DCe.Signature!.SignatureValue, protocoloRelido.Signature!.SignatureValue);
        Assert.AreEqual(eventoProcessado.eventoDCe.Signature!.SignatureValue, retornoEventoRelido.Signature!.SignatureValue);
        var consulta = new retConsSitDCe().CarregarDeXmlString(Raiz("retConsSitDCe", Cabecalho(100) + $"<chDCe>{Chave}</chDCe>" + Protocolo(false)));
        consulta.procEventoDCe.AddRange(new[] { eventoProcessado, eventoProcessado });
        // O XSD da consulta usa xs:any singular, apesar de cada procEventoDCe ter dois filhos.
        // A coleção é conferida por ida/volta; o processado completo foi validado acima.
        var consultaRelida = new retConsSitDCe().CarregarDeXmlString(consulta.ObterXmlString());
        Assert.AreEqual(2, consultaRelida.procEventoDCe.Count);
        Assert.AreEqual(eventoProcessado.eventoDCe.infEvento.Id, consultaRelida.procEventoDCe[1].eventoDCe!.infEvento.Id);
        Assert.AreEqual("1262600000000002", consultaRelida.procEventoDCe[0].retEventoDCe!.infEvento!.nProt);
        processado.dhConexao = null;
        processado.ipTransmissor = null;
        processado.nPortaCon = null;
        var raiz = XDocument.Parse(processado.ObterXmlString()).Root!;
        foreach (var nome in new[] { "dhConexao", "ipTransmissor", "nPortaCon" }) Assert.IsNull(raiz.Attribute(nome));
    }
}
