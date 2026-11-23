using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DCe.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

[TestClass]
public class DceChaveAcessoTest
{
    [DataTestMethod]
    [DataRow("0")]
    [DataRow("6")]
    public void CalcularDv_RestoZeroOuUm_DevolveZero(string ultimoDigito)
    {
        var chave = new string('0', 42) + ultimoDigito;
        Assert.AreEqual(0, DceChaveAcesso.CalcularDv(chave));
    }

    [TestMethod]
    public void Montar_CulturaComOutroCalendario_MantemAammGregoriano()
    {
        var anterior = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("ar-SA");
            var chave = DceChaveAcesso.Montar(26, new DateTime(2026, 9, 1, 1, 0, 0, DateTimeKind.Utc),
                "99999999000191", 1, 42, 1, 3, 0, 123456, TimeSpan.FromHours(-3));
            Assert.AreEqual("2608", chave.Substring(2, 4));
        }
        finally { System.Globalization.CultureInfo.CurrentCulture = anterior; }
    }

    /// <summary>Chave sintética sem DV, com CNPJ de teste e soma ponderada 644.</summary>
    private const string ChaveSintetica = "1101019999999900019199001000000001130000029";

    /// <summary>
    /// Aplica o módulo 11 do item 2.2.4.1 do Manual DC-e — Visão Geral a uma chave sintética.
    /// A soma 644 deixa resto 6 e, portanto, DV 5.
    /// </summary>
    [TestMethod]
    public void CalcularDv_ChaveSintetica_DevolveDvEsperado()
    {
        Assert.AreEqual(5, DceChaveAcesso.CalcularDv(ChaveSintetica));
    }

    /// <summary>
    /// Confere a soma ponderada separadamente do DV para isolar as etapas do cálculo.
    /// A chave sintética tem soma conhecida de 644.
    /// </summary>
    [TestMethod]
    public void SomatoriaDasPonderacoes_ChaveSintetica_DevolveSomaEsperada()
    {
        Assert.AreEqual(644, DceChaveAcesso.SomarPonderacoes(ChaveSintetica));
    }

    /// <summary>
    /// Módulo 11: quando o resto da divisão é 0 ou 1, o DV é 0 — não 11 nem 10.
    /// É o caso de borda que quebra implementações ingênuas.
    /// </summary>
    [DataTestMethod]
    [DataRow("1101019999999900019199001000000001130000029", 5)]
    public void CalcularDv_NuncaDevolveValorForaDeZeroANove(string chave43, int esperado)
    {
        var dv = DceChaveAcesso.CalcularDv(chave43);

        Assert.IsTrue(dv >= 0 && dv <= 9);
        Assert.AreEqual(esperado, dv);
    }

    [TestMethod]
    public void Montar_ProduzChaveDe44DigitosComOsCamposNaOrdemDaTabela()
    {
        // CNPJ sintético dos fixtures: base 999999990001, DVs 9 e 1 por módulo 11.
        const string cnpjFicticio = "99999999000191";
        var pesosPrimeiroDv = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var pesosSegundoDv = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var somaPrimeiroDv = pesosPrimeiroDv.Select((peso, i) => peso * (cnpjFicticio[i] - '0')).Sum();
        var somaSegundoDv = pesosSegundoDv.Select((peso, i) => peso * (cnpjFicticio[i] - '0')).Sum();
        Assert.AreEqual(398, somaPrimeiroDv);
        Assert.AreEqual(417, somaSegundoDv);
        Assert.AreEqual(cnpjFicticio[12] - '0', 11 - somaPrimeiroDv % 11);
        Assert.AreEqual(cnpjFicticio[13] - '0', 11 - somaSegundoDv % 11);

        var chave = DceChaveAcesso.Montar(
            cUF: 26,
            dhEmi: new DateTime(2026, 8, 22, 10, 0, 0, DateTimeKind.Utc),
            cnpjSignatario: cnpjFicticio,
            serie: 1,
            nDC: 42,
            tpEmis: 1,
            tpEmit: 3,
            nSiteAutoriz: 0,
            cDC: 123456,
            offsetHorario: TimeSpan.FromHours(-3));

        Assert.AreEqual(44, chave.Length);
        Assert.IsTrue(chave.All(char.IsDigit));

        Assert.AreEqual("26", chave.Substring(0, 2));      // cUF
        Assert.AreEqual("2608", chave.Substring(2, 4));    // AAMM
        Assert.AreEqual("99999999000191", chave.Substring(6, 14));
        Assert.AreEqual("99", chave.Substring(20, 2));     // mod — sempre 99
        Assert.AreEqual("001", chave.Substring(22, 3));    // serie
        Assert.AreEqual("000000042", chave.Substring(25, 9)); // nDC
        Assert.AreEqual("1", chave.Substring(34, 1));      // tpEmis
        Assert.AreEqual("3", chave.Substring(35, 1));      // tpEmit — Transportadora
        Assert.AreEqual("0", chave.Substring(36, 1));      // nSiteAutoriz
        Assert.AreEqual("123456", chave.Substring(37, 6)); // cDC
        Assert.AreEqual(DceChaveAcesso.CalcularDv(chave.Substring(0, 43)).ToString(), chave.Substring(43, 1));
    }

    /// <summary>
    /// O AAMM da chave sai do mesmo instante local que vai no dhEmi — não do UTC. Sem isto,
    /// toda emissão entre 00:00 e 03:00 UTC do dia 1º produziria uma chave do mês novo num
    /// documento datado do mês anterior, e a SEFAZ rejeitaria com cStat 502 (regra A03-10:
    /// "Chave de Acesso do campo Id difere da concatenação dos campos correspondentes").
    /// Três horas por mês em que nada emitiria — e o relógio fixo ao meio-dia dos demais
    /// testes nunca chega perto da virada.
    /// </summary>
    [TestMethod]
    public void Montar_DerivaAammDoInstanteLocal_NaoDoUtc()
    {
        // 01/09/2026 01:00 UTC = 31/08/2026 22:00 em -03:00 — o documento é de AGOSTO
        var viradaDeMes = new DateTime(2026, 9, 1, 1, 0, 0, DateTimeKind.Utc);

        var chave = DceChaveAcesso.Montar(
            cUF: 26, dhEmi: viradaDeMes, cnpjSignatario: "99999999000191", serie: 1, nDC: 42,
            tpEmis: 1, tpEmit: 3, nSiteAutoriz: 0, cDC: 123456,
            offsetHorario: TimeSpan.FromHours(-3));

        Assert.AreEqual("2608", chave.Substring(2, 4));
    }

    /// <summary>Fora da virada, local e UTC coincidem — a correção não pode mexer no caso comum.</summary>
    [TestMethod]
    public void Montar_ForaDaViradaDeMes_MantemOAamm()
    {
        var chave = DceChaveAcesso.Montar(
            cUF: 26, dhEmi: new DateTime(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc),
            cnpjSignatario: "99999999000191", serie: 1, nDC: 42,
            tpEmis: 1, tpEmit: 3, nSiteAutoriz: 0, cDC: 123456,
            offsetHorario: TimeSpan.FromHours(-3));

        Assert.AreEqual("2608", chave.Substring(2, 4));
    }
}
