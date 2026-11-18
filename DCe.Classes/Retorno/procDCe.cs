using System;
using System.Globalization;
using System.Xml.Serialization;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("dceProc", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class procDCe
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

    [XmlElement("DCe", Order = 0)]
    public global::DCe.Classes.Informacoes.DCe? DCe
    {
        get => _DCe;
        set { _DCe = value; DCeSpecified = true; }
    }

    [XmlIgnore]
    public bool DCeSpecified { get; set; }

    private global::DCe.Classes.Informacoes.DCe? _DCe;

    [XmlElement("protDCe", Order = 1)]
    public protDCe? protDCe
    {
        get => _protDCe;
        set { _protDCe = value; protDCeSpecified = true; }
    }

    [XmlIgnore]
    public bool protDCeSpecified { get; set; }

    private protDCe? _protDCe;
}
