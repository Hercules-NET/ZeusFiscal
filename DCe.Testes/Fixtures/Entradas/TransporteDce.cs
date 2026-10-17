using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

internal class TransporteDce
{
    /// <summary>Modalidade do transporte, conforme domínio de modTrans no XSD.</summary>
    public string Modalidade { get; set; } = string.Empty;

    public string CnpjTransportador { get; set; } = string.Empty;
}
