using System;
using System.Linq;
using System.Globalization;
namespace DCe.Utils;

/// <summary>
/// Chave de acesso da DC-e: 44 dígitos, conforme Tabela 2-1 do Manual — Visão Geral.
/// cUF(2) AAMM(4) CNPJ(14) mod(2) serie(3) nDC(9) tpEmis(1) tpEmit(1) nSiteAutoriz(1)
/// cDC(6) cDV(1).
/// </summary>
public static class DceChaveAcesso
{
    /// <summary>
    /// DV por módulo 11, pesos 2..9 ciclando da direita para a esquerda.
    /// Resto 0 ou 1 → DV 0 (regra explícita do manual).
    /// </summary>
    public static int CalcularDv(string chave43)
    {
        var resto = SomarPonderacoes(chave43) % 11;
        return resto is 0 or 1 ? 0 : 11 - resto;
    }

    /// <summary>
    /// Somatória dos dígitos ponderados, conforme item 2.2.4.1 do Manual — Visão Geral.
    /// Exposta separadamente para permitir a conferência do cálculo do DV.
    /// </summary>
    public static int SomarPonderacoes(string chave43)
    {
        if (chave43 is null || chave43.Length != 43 || !chave43.All(char.IsDigit))
            throw new ArgumentException("A chave sem DV precisa ter exatamente 43 dígitos.", nameof(chave43));

        var soma = 0;
        var peso = 2;

        for (var i = chave43.Length - 1; i >= 0; i--)
        {
            soma += (chave43[i] - '0') * peso;
            peso = peso == 9 ? 2 : peso + 1;
        }

        return soma;
    }

    public const int ModeloDce = 99;

    /// <param name="offsetHorario">
    /// Fuso do emitente. O <c>AAMM</c> da chave tem de sair do <b>mesmo instante local</b> que
    /// vai no <c>dhEmi</c> do XML, não do UTC: a regra A03-10 do Anexo I rejeita com
    /// <c>cStat 502</c> quando a chave "difere da concatenação dos campos correspondentes".
    /// Derivar de UTC produz uma chave de agosto num documento datado de julho sempre que a
    /// emissão cai entre 00:00 e 03:00 UTC do dia 1º, para o offset -03:00.
    /// </param>
    public static string Montar(
        int cUF, DateTime dhEmi, string cnpjSignatario, int serie, int nDC,
        int tpEmis, int tpEmit, int nSiteAutoriz, int cDC, TimeSpan offsetHorario)
    {
        var cnpj = new string((cnpjSignatario ?? string.Empty).Where(char.IsDigit).ToArray());
        if (cnpj.Length != 14)
            throw new ArgumentException("CNPJ do signatário precisa ter 14 dígitos.", nameof(cnpjSignatario));

        var utc = dhEmi.Kind == DateTimeKind.Utc ? dhEmi : dhEmi.ToUniversalTime();
        var local = utc + offsetHorario;

        var sem = string.Concat(
            cUF.ToString("D2", CultureInfo.InvariantCulture),
            local.ToString("yyMM", CultureInfo.InvariantCulture),
            cnpj,
            ModeloDce.ToString("D2", CultureInfo.InvariantCulture),
            serie.ToString("D3", CultureInfo.InvariantCulture),
            nDC.ToString("D9", CultureInfo.InvariantCulture),
            tpEmis.ToString("D1", CultureInfo.InvariantCulture),
            tpEmit.ToString("D1", CultureInfo.InvariantCulture),
            nSiteAutoriz.ToString("D1", CultureInfo.InvariantCulture),
            cDC.ToString("D6", CultureInfo.InvariantCulture));

        return sem + CalcularDv(sem);
    }
}
