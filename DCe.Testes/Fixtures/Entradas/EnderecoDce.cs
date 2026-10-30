using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

internal class EnderecoDce
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public int CodigoMunicipio { get; set; }
    public string Municipio { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public int CodigoPais { get; set; } = 1058;
    public string Pais { get; set; } = "BRASIL";
    public string? Telefone { get; set; }
    public string? Email { get; set; }
}
