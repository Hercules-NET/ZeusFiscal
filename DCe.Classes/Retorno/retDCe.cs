using System;
using System.Globalization;
using System.Xml.Serialization;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("retDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class retDCe : RetornoBase
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

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

    [XmlElement("cStat", Order = 2)]
    public int cStat
    {
        get => _cStat;
        set { _cStat = value; cStatSpecified = true; }
    }

    [XmlIgnore]
    public bool cStatSpecified { get; set; }

    private int _cStat;

    [XmlElement("xMotivo", Order = 3)]
    public string xMotivo
    {
        get => _xMotivo;
        set { _xMotivo = value; xMotivoSpecified = true; }
    }

    [XmlIgnore]
    public bool xMotivoSpecified { get; set; }

    private string _xMotivo = string.Empty;

    [XmlElement("cUF", Order = 4)]
    public Estado cUF
    {
        get => _cUF;
        set { _cUF = value; cUFSpecified = true; }
    }

    [XmlIgnore]
    public bool cUFSpecified { get; set; }

    private Estado _cUF;

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

    [XmlElement("dhRecbto", Order = 5)]
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

    [XmlElement("protDCe", Order = 6)]
    public protDCe? protDCe { get; set; }

    [XmlIgnore]
    public string Ambiente => tpAmbSpecified ? ((int)tpAmb).ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public string VersaoAplicativo => verAplic;

    [XmlIgnore]
    public string CodigoStatus => cStatSpecified ? cStat.ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public string Motivo => xMotivo;

    [XmlIgnore]
    public string CodigoUf => cUFSpecified ? ((int)cUF).ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public DateTimeOffset? DataRecebimento => dhRecbtoProxySpecified ? dhRecbto : (DateTimeOffset?)null;

    [XmlIgnore]
    public infProt? Protocolo => protDCe?.infProt;

    [XmlIgnore]
    public bool Autorizada => cStat == 100;
}
