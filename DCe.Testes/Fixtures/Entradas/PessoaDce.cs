using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

internal class PessoaDce
{
    public string? Cnpj { get; set; }
    public string? Cpf { get; set; }

    /// <summary>Identificação de estrangeiro, quando não há CNPJ nem CPF.</summary>
    public string? IdOutros { get; set; }

    public string Nome { get; set; } = string.Empty;
    public EnderecoDce Endereco { get; set; } = new();
}
