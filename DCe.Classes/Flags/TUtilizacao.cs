using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TUtilizacao do leiaute DC-e.</summary>
public enum TUtilizacao
{
    [XmlEnum("1")]
    Valor1 = 1,

    [XmlEnum("2")]
    Valor2 = 2,

    [XmlEnum("3")]
    Valor3 = 3,

    [XmlEnum("4")]
    Valor4 = 4,

    [XmlEnum("5")]
    Valor5 = 5,

    [XmlEnum("6")]
    Valor6 = 6,

    [XmlEnum("7")]
    Valor7 = 7,

}
