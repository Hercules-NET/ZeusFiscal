using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML TipoEventoDCe do leiaute DC-e.</summary>
public enum TipoEventoDCe
{
    [XmlEnum("110111")]
    Cancelamento = 110111,

}
