using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TFinDCe do leiaute DC-e.</summary>
public enum TFinDCe
{
    [XmlEnum("0")]
    Valor0 = 0,

    [XmlEnum("3")]
    Valor3 = 3,

    [XmlEnum("4")]
    Valor4 = 4,

}
