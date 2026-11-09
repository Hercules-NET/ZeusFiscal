using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

internal class ItemDce
{
    public string Descricao { get; set; } = string.Empty;
    public string Ncm { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string? InformacoesAdicionais { get; set; }
}
