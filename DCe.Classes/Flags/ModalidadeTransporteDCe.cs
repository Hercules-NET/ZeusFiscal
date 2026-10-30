using System.Xml.Serialization;

namespace DCe.Classes.Flags;

/// <summary>Domínio XML ModalidadeTransporteDCe do leiaute DC-e.</summary>
public enum ModalidadeTransporteDCe
{
    [XmlEnum("0")]
    Correios = 0,

    [XmlEnum("1")]
    Proprio = 1,

    [XmlEnum("2")]
    Transportadora = 2,

}
