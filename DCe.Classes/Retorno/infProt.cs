using System;
using System.Globalization;
using System.Xml.Serialization;
using DFe.Classes.Flags;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("infProt", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infProt
{
    [XmlAttribute("Id")]
    public string? Id { get; set; }

    [XmlElement("tpAmb", Order = 0)]
    public TipoAmbiente tpAmb
    {
        get => _tpAmb;
        set { _tpAmb = value; tpAmbSpecified = true; }
    }

    private TipoAmbiente _tpAmb;

    [XmlIgnore]
    public bool tpAmbSpecified { get; set; }

    [XmlElement("verAplic", Order = 1)]
    public string verAplic
    {
        get => _verAplic;
        set { _verAplic = value; verAplicSpecified = true; }
    }

    [XmlIgnore]
    public bool verAplicSpecified { get; set; }

    private string _verAplic = string.Empty;

    [XmlElement("chDCe", Order = 2)]
    public string chDCe
    {
        get => _chDCe;
        set { _chDCe = value; chDCeSpecified = true; }
    }

    [XmlIgnore]
    public bool chDCeSpecified { get; set; }

    private string _chDCe = string.Empty;

    [XmlIgnore]
    public bool dhRecbtoProxySpecified { get; set; }

    private DateTimeOffset _dhRecbto;
    private string? _dhRecbtoXml;

    [XmlIgnore]
    public DateTimeOffset dhRecbto
    {
        get => _dhRecbto;
        set { _dhRecbto = value; _dhRecbtoXml = null; dhRecbtoProxySpecified = true; }
    }

    [XmlElement("dhRecbto", Order = 3)]
    public string dhRecbtoProxy
    {
        get => _dhRecbtoXml ?? _dhRecbto.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        set
        {
            _dhRecbto = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None);
            _dhRecbtoXml = value;
            dhRecbtoProxySpecified = true;
        }
    }

    [XmlElement("nProt", Order = 4)]
    public string? nProt { get; set; }

    [XmlElement("digVal", Order = 5)]
    public string? digVal { get; set; }

    [XmlElement("cStat", Order = 6)]
    public int cStat
    {
        get => _cStat;
        set { _cStat = value; cStatSpecified = true; }
    }

    [XmlIgnore]
    public bool cStatSpecified { get; set; }

    private int _cStat;

    [XmlElement("xMotivo", Order = 7)]
    public string xMotivo
    {
        get => _xMotivo;
        set { _xMotivo = value; xMotivoSpecified = true; }
    }

    [XmlIgnore]
    public bool xMotivoSpecified { get; set; }

    private string _xMotivo = string.Empty;

    [XmlIgnore]
    public string Ambiente => tpAmbSpecified ? ((int)tpAmb).ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public string VersaoAplicativo => verAplic;

    [XmlIgnore]
    public string ChaveAcesso => chDCe;

    [XmlIgnore]
    public DateTimeOffset? DataRecebimento => dhRecbtoProxySpecified ? dhRecbto : (DateTimeOffset?)null;

    [XmlIgnore]
    public string? NumeroProtocolo => nProt;

    [XmlIgnore]
    public string? DigestValue => digVal;

    [XmlIgnore]
    public string CodigoStatus => cStatSpecified ? cStat.ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public string Motivo => xMotivo;
}
