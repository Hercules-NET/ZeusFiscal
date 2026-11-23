using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TCRT do leiaute DC-e.</summary>
public enum TCRT
{
    [XmlEnum("1")]
    Valor1 = 1,

    [XmlEnum("2")]
    Valor2 = 2,

    [XmlEnum("3")]
    Valor3 = 3,

}
