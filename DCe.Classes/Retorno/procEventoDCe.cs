using System;
using System.Globalization;
using System.Xml.Serialization;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("procEventoDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class procEventoDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlAttribute("ipTransmissor")]
    public string? ipTransmissor { get; set; }

    [XmlAttribute("nPortaCon")]
    public string? nPortaCon { get; set; }

    private DateTimeOffset? _dhConexao;
    private string? _dhConexaoXml;

    [XmlIgnore]
    public DateTimeOffset? dhConexao
    {
        get => _dhConexao;
        set { _dhConexao = value; _dhConexaoXml = null; }
    }

    [XmlAttribute("dhConexao")]
    public string? dhConexaoProxy
    {
        get => _dhConexaoXml ?? _dhConexao?.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        set
        {
            _dhConexao = value == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(value!, CultureInfo.InvariantCulture, DateTimeStyles.None);
            _dhConexaoXml = value;
        }
    }

    [XmlElement("eventoDCe", Order = 0)]
    public global::DCe.Classes.Servicos.Evento.eventoDCe? eventoDCe
    {
        get => _eventoDCe;
        set { _eventoDCe = value; eventoDCeSpecified = true; }
    }

    [XmlIgnore]
    public bool eventoDCeSpecified { get; set; }

    private global::DCe.Classes.Servicos.Evento.eventoDCe? _eventoDCe;

    [XmlElement("retEventoDCe", Order = 1)]
    public retEventoDCe? retEventoDCe
    {
        get => _retEventoDCe;
        set { _retEventoDCe = value; retEventoDCeSpecified = true; }
    }

    [XmlIgnore]
    public bool retEventoDCeSpecified { get; set; }

    private retEventoDCe? _retEventoDCe;
}
