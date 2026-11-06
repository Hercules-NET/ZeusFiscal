using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TipoEmitenteDCe do leiaute DC-e.</summary>
public enum TipoEmitenteDCe
{
    [XmlEnum("0")]
    Fisco = 0,

    [XmlEnum("1")]
    Marketplace = 1,

    [XmlEnum("2")]
    Proprio = 2,

    [XmlEnum("3")]
    Transportadora = 3,

    [XmlEnum("4")]
    ECT = 4,

}
