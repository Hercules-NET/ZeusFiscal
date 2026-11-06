using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

/// <summary>
/// Quem assina a DC-e. No XSD é um choice entre Fisco, Marketplace, Transportadora e ECT —
/// bloco separado do emit, que é o usuário emitente. Esta fixture usa Transportadora.
/// </summary>
internal class SignatarioDce
{
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
