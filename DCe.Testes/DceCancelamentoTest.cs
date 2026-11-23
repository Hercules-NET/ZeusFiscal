using DCe.Testes.Fixtures;
using DCe.Utils.Extensoes;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System.Xml;
using System.Xml.Linq;
using DCe.Utils.Abstractions;
using DCe.Testes.Fixtures.Entradas;
using DCe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceCancelamentoTest
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/dce";
    private const string Chave = "26260899999999000191990010000000011300123455";

    private sealed class ClockFixo : IClock
    {
        public DateTime UtcNow => new(2026, 8, 22, 15, 0, 0, DateTimeKind.Utc);
    }

    private sealed class RandomFixo : IRandomSource
    {
        public int ProximoCodigoDocumento() => 123456;
    }

    private static CancelamentoDce Pedido() => new()
    {
        Ambiente = TipoAmbiente.Homologacao,
        ChaveAcesso = Chave,
        CodigoOrgao = (Estado)26,
        TipoEmitente = 3,
        CnpjAutor = "99999999000191",
        CpfUsuarioEmitente = "12345678909",
        NumeroProtocolo = "126260000000001",
        Justificativa = "TRANSPORTE CANCELADO A PEDIDO DO DECLARANTE",
        SequenciaEvento = 1
    };

    private static string Construir() =>
        new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(Pedido()).ObterXmlString();

    /// <summary>
    /// A regra de formação do Id está no XSD: "ID" + tpEvento(6) + chave(44) + nSeqEvento(3
    /// com zeros à esquerda) = ID + 53 dígitos. É a tag que será assinada; errar aqui invalida
    /// a assinatura inteira.
    /// </summary>
    [TestMethod]
    public void ConstruirEventoCancelamento_MontaOIdComARegraDoXsd()
    {
        var id = XDocument.Parse(Construir()).Root!
            .Element(Ns + "infEvento")!.Attribute("Id")!.Value;

        Assert.AreEqual("ID" + "110111" + Chave + "001", id);
        Assert.AreEqual(55, id.Length);
    }

    [TestMethod]
    public void ConstruirEventoCancelamento_UsaOTipoDeEvento110111()
    {
        var inf = XDocument.Parse(Construir()).Root!.Element(Ns + "infEvento")!;

        Assert.AreEqual("110111", inf.Element(Ns + "tpEvento")!.Value);
        Assert.AreEqual("1", inf.Element(Ns + "nSeqEvento")!.Value);
    }

    [TestMethod]
    public void ConstruirEventoCancelamento_LevaProtocoloEJustificativa()
    {
        var ev = XDocument.Parse(Construir()).Root!
            .Element(Ns + "infEvento")!.Element(Ns + "detEvento")!.Element(Ns + "evCancDCe")!;

        Assert.AreEqual("126260000000001", ev.Element(Ns + "nProt")!.Value);
        Assert.AreEqual("TRANSPORTE CANCELADO A PEDIDO DO DECLARANTE", ev.Element(Ns + "xJust")!.Value);
    }

    /// <summary>
    /// O usuário emitente é um choice entre CNPJ, CPF e IdOutros, igual à emissão. Emitir
    /// dois, ou um vazio, reprova no schema.
    /// </summary>
    [TestMethod]
    public void ConstruirEventoCancelamento_EmiteUmSoIdentificadorDoUsuarioEmitente()
    {
        var inf = XDocument.Parse(Construir()).Root!.Element(Ns + "infEvento")!;
        var presentes = new[] { "CNPJUsEmit", "CPFUsEmit", "IdOutrosUsEmit" }
            .Count(n => inf.Element(Ns + n) is not null);

        Assert.AreEqual(1, presentes);
        Assert.AreEqual("12345678909", inf.Element(Ns + "CPFUsEmit")!.Value);
    }

    [TestMethod]
    public void ConstruirEventoCancelamento_NaoEmiteAssinatura()
    {
        XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";

        Assert.AreEqual(0, XDocument.Parse(Construir()).Descendants(ds + "Signature").Count());
    }

    [TestMethod]
    public void ConstruirEventoCancelamento_DeclaraEncodingUtf8()
    {
        Assert.IsTrue(Construir().StartsWith("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.Ordinal));
    }

    /// <summary>
    /// O schema exige ds:Signature. O placeholder verifica apenas a estrutura XML;
    /// a validade criptográfica é coberta pelos testes de assinatura.
    /// </summary>
    [TestMethod]
    public void ConstruirEventoCancelamento_ValidaContraOSchemaOficialDepoisDeAssinado()
    {
        XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
        var raiz = XDocument.Parse(Construir());
        var id = raiz.Root!.Element(Ns + "infEvento")!.Attribute("Id")!.Value;
        raiz.Root.Add(AssinaturaPlaceholder(ds, id));

        var doc = new XmlDocument { Schemas = DceSchemaLoader.Carregar("evento", "eventoDCe_v1.00.xsd") };
        doc.LoadXml(raiz.ToString(SaveOptions.DisableFormatting));

        var erros = new List<string>();
        doc.Validate((_, e) => erros.Add(e.Message));

        Assert.IsTrue(erros.Count == 0, string.Join(" | ", erros));
    }

    private static XElement AssinaturaPlaceholder(XNamespace ds, string idAssinado) =>
        new(ds + "Signature",
            new XElement(ds + "SignedInfo",
                new XElement(ds + "CanonicalizationMethod",
                    new XAttribute("Algorithm", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315")),
                new XElement(ds + "SignatureMethod",
                    new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#rsa-sha1")),
                new XElement(ds + "Reference",
                    new XAttribute("URI", "#" + idAssinado),
                    new XElement(ds + "Transforms",
                        new XElement(ds + "Transform",
                            new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#enveloped-signature")),
                        new XElement(ds + "Transform",
                            new XAttribute("Algorithm", "http://www.w3.org/TR/2001/REC-xml-c14n-20010315"))),
                    new XElement(ds + "DigestMethod",
                        new XAttribute("Algorithm", "http://www.w3.org/2000/09/xmldsig#sha1")),
                    new XElement(ds + "DigestValue", "AAAA"))),
            new XElement(ds + "SignatureValue", "AAAA"),
            new XElement(ds + "KeyInfo",
                new XElement(ds + "X509Data",
                    new XElement(ds + "X509Certificate", "AAAA"))));

    /// <summary>
    /// Assim como dhEmi (regra B07-10, cStat 703), dhEvento precisa sair no offset local
    /// do emitente, não em "+00:00". Instante fixo do ClockFixo é 2026-08-22T15:00:00Z, offset
    /// default -3h.
    /// </summary>
    [TestMethod]
    public void ConstruirEventoCancelamento_EmiteDhEventoNoOffsetLocalPadrao()
    {
        var inf = XDocument.Parse(Construir()).Root!.Element(Ns + "infEvento")!;

        Assert.AreEqual("2026-08-22T12:00:00-03:00", inf.Element(Ns + "dhEvento")!.Value);
    }

    [TestMethod]
    public void ConstruirEventoCancelamento_RespeitaOffsetCustomizado()
    {
        var pedido = Pedido();
        pedido.OffsetHorario = TimeSpan.FromHours(-4);

        var xml = new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(pedido).ObterXmlString();
        var inf = XDocument.Parse(xml).Root!.Element(Ns + "infEvento")!;

        Assert.AreEqual("2026-08-22T11:00:00-04:00", inf.Element(Ns + "dhEvento")!.Value);
    }

    // ---- prazo ----

    /// <summary>
    /// A Cláusula 11ª do Ajuste SINIEF 05/2021 prevê 24 horas da autorização.
    /// A verificação local tolera deriva entre o relógio do chamador e o dhRecbto da SEFAZ:
    /// decorrido negativo não impede o cancelamento de um documento recém-autorizado.
    /// A SEFAZ verifica o prazo e pode rejeitar o evento com cStat 501.
    /// </summary>
    [DataTestMethod]
    [DataRow(-1)]      // relógio do chamador 1h atrás do da SEFAZ
    [DataRow(-24)]
    public void DentroDoPrazo_ComDerivaDeRelogioParaTras_ContinuaLiberado(int horas)
    {
        var autorizada = new DateTimeOffset(2026, 8, 22, 12, 0, 0, TimeSpan.Zero);

        Assert.IsTrue(DcePrazoCancelamento.DentroDoPrazo(autorizada, autorizada.AddHours(horas)));
    }

    [DataTestMethod]
    [DataRow(0, true)]
    [DataRow(23, true)]
    [DataRow(24, false)]
    [DataRow(25, false)]
    public void DentroDoPrazo_AplicaAJanelaDe24Horas(int horasDepois, bool esperado)
    {
        var autorizada = new DateTimeOffset(2026, 8, 22, 12, 0, 0, TimeSpan.Zero);

        Assert.AreEqual(esperado,
            DcePrazoCancelamento.DentroDoPrazo(autorizada, autorizada.AddHours(horasDepois)));
    }

    /// <summary>
    /// O cOrgao do evento é o cUF da própria chave, não o do autorizador (41).
    /// Confirmado contra a SEFAZ em homologação: cOrgao corresponde aos dois primeiros
    /// dígitos da chave e pode ser conferido localmente.
    /// </summary>
    [TestMethod]
    public void CodigoOrgao_DivergindoDoCufDaChave_Recusa()
    {
        var pedido = Pedido();
        pedido.CodigoOrgao = (Estado)41;                 // PR, o autorizador — chave é de PE (26)

        var erro = Assert.ThrowsException<ArgumentException>(
            () => new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(pedido).ObterXmlString());

        Assert.IsTrue(erro.Message.IndexOf("41", StringComparison.Ordinal) >= 0);
        Assert.IsTrue(erro.Message.IndexOf("26", StringComparison.Ordinal) >= 0);
    }

    [TestMethod]
    public void ChaveAcesso_ForaDoFormato_Recusa()
    {
        var pedido = Pedido();
        pedido.ChaveAcesso = "2626089999999900019199001";

        Assert.ThrowsException<ArgumentException>(
            () => new FabricaModeloDCe(new ClockFixo(), new RandomFixo()).Criar(pedido).ObterXmlString());
    }
}
