using System;
using System.Linq;
using System.Collections.Generic;
using DFe.Classes.Entidades;

namespace DCe.Utils.Ibge;

/// <summary>Consulta os códigos IBGE do enum comum do Zeus, excluindo autorizadores e exterior.</summary>
public static class UnidadesFederativas
{
    public static int? CodigoDe(string? sigla)
    {
        if (string.IsNullOrWhiteSpace(sigla)) return null;
        var nome = sigla!.Trim();
        return TodasAsSiglas().Any(s => string.Equals(s, nome, StringComparison.OrdinalIgnoreCase))
            ? (int?)(Estado)Enum.Parse(typeof(Estado), nome, true) : null;
    }

    public static string? SiglaDe(int codigo) =>
        codigo >= 11 && codigo <= 53 && Enum.IsDefined(typeof(Estado), codigo)
            ? ((Estado)codigo).ToString() : null;

    public static IReadOnlyCollection<string> TodasAsSiglas() =>
        Enum.GetValues(typeof(Estado)).Cast<Estado>()
            .Where(uf => (int)uf >= 11 && (int)uf <= 53).Select(uf => uf.ToString()).ToList();
}
