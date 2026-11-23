using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TProcEmi do leiaute DC-e.</summary>
public enum TProcEmi
{
    [XmlEnum("0")]
    Valor0 = 0,

    [XmlEnum("3")]
    Valor3 = 3,

}
