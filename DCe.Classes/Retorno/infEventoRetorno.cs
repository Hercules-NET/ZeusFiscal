using System;
using System.Globalization;
using System.Xml.Serialization;
using DCe.Classes.Flags;
using DFe.Classes.Flags;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("infEventoRetorno", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infEventoRetorno
{
    [XmlAttribute("Id")]
    public string? Id { get; set; }

    [XmlElement("tpAmb", Order = 0)]
    public TipoAmbiente tpAmb
    {
        get => _tpAmb;
        set { _tpAmb = value; tpAmbSpecified = true; }
    }

    [XmlIgnore]
    public bool tpAmbSpecified { get; set; }

    private TipoAmbiente _tpAmb;

    [XmlElement("verAplic", Order = 1)]
    public string verAplic
    {
        get => _verAplic;
        set { _verAplic = value; verAplicSpecified = true; }
    }

    [XmlIgnore]
    public bool verAplicSpecified { get; set; }

    private string _verAplic = string.Empty;

    [XmlElement("cOrgao", Order = 2)]
    public TCOrgaoIBGE cOrgao
    {
        get => _cOrgao;
        set { _cOrgao = value; cOrgaoSpecified = true; }
    }

    [XmlIgnore]
    public bool cOrgaoSpecified { get; set; }

    private TCOrgaoIBGE _cOrgao;

    [XmlElement("cStat", Order = 3)]
    public int cStat
    {
        get => _cStat;
        set { _cStat = value; cStatSpecified = true; }
    }

    [XmlIgnore]
    public bool cStatSpecified { get; set; }

    private int _cStat;

    [XmlElement("xMotivo", Order = 4)]
    public string xMotivo
    {
        get => _xMotivo;
        set { _xMotivo = value; xMotivoSpecified = true; }
    }

    [XmlIgnore]
    public bool xMotivoSpecified { get; set; }

    private string _xMotivo = string.Empty;

    [XmlElement("chDCe", Order = 5)]
    public string? chDCe { get; set; }

    [XmlElement("tpEvento", Order = 6)]
    public string? tpEvento { get; set; }

    [XmlElement("xEvento", Order = 7)]
    public string? xEvento { get; set; }

    [XmlElement("nSeqEvento", Order = 8)]
    public string? nSeqEvento { get; set; }

    private DateTimeOffset? _dhRegEvento;
    private string? _dhRegEventoXml;

    [XmlIgnore]
    public DateTimeOffset? dhRegEvento
    {
        get => _dhRegEvento;
        set { _dhRegEvento = value; _dhRegEventoXml = null; }
    }

    [XmlElement("dhRegEvento", Order = 9)]
    public string? dhRegEventoProxy
    {
        get => _dhRegEventoXml ?? _dhRegEvento?.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        set
        {
            _dhRegEvento = value == null ? (DateTimeOffset?)null : DateTimeOffset.Parse(value!, CultureInfo.InvariantCulture, DateTimeStyles.None);
            _dhRegEventoXml = value;
        }
    }

    [XmlElement("nProt", Order = 10)]
    public string? nProt { get; set; }
}
