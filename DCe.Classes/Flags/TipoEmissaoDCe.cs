using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TipoEmissaoDCe do leiaute DC-e.</summary>
public enum TipoEmissaoDCe
{
    [XmlEnum("1")]
    Normal = 1,

    [XmlEnum("2")]
    Contingencia = 2,

}
