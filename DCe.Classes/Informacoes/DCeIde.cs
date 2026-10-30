using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using DCe.Classes.Flags;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Informacoes;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("ide", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeIde
{
    [XmlElement("cUF", Order = 0)]
    public Estado cUF { get; set; }

    [XmlElement("cDC", Order = 1)]
    public string cDC { get; set; } = string.Empty;

    [XmlElement("mod", Order = 2)]
    public ModeloDCe mod { get; set; } = ModeloDCe.DeclaracaoConteudo;

    private int _serie = default;
    [XmlElement("serie", Order = 3)]
    public int serie
    {
        get => _serie;
        set { _serie = value; serieSpecified = true; }
    }

    private int _nDC = default;
    [XmlElement("nDC", Order = 4)]
    public int nDC
    {
        get => _nDC;
        set { _nDC = value; nDCSpecified = true; }
    }

    private DateTimeOffset _dhEmi;
    private string? _dhEmiXml;

    [XmlIgnore]
    public DateTimeOffset dhEmi
    {
        get => _dhEmi;
        set { _dhEmi = value; _dhEmiXml = null; dhEmiProxySpecified = true; }
    }

    [XmlElement("dhEmi", Order = 5)]
    public string dhEmiProxy
    {
        get => _dhEmiXml ?? _dhEmi.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        set { _dhEmi = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None); _dhEmiXml = value; dhEmiProxySpecified = true; }
    }

    private TipoEmissaoDCe _tpEmis = TipoEmissaoDCe.Normal;
    [XmlElement("tpEmis", Order = 6)]
    public TipoEmissaoDCe tpEmis
    {
        get => _tpEmis;
        set { _tpEmis = value; tpEmisSpecified = true; }
    }

    [XmlElement("tpEmit", Order = 7)]
    public TipoEmitenteDCe tpEmit { get; set; }

    [XmlElement("nSiteAutoriz", Order = 8)]
    public int nSiteAutoriz { get; set; }

    [XmlElement("cDV", Order = 9)]
    public int cDV { get; set; }

    private TipoAmbiente _tpAmb = default;
    [XmlElement("tpAmb", Order = 10)]
    public TipoAmbiente tpAmb
    {
        get => _tpAmb;
        set { _tpAmb = value; tpAmbSpecified = true; }
    }

    [XmlElement("verProc", Order = 11)]
    public string verProc { get; set; } = string.Empty;

    [XmlIgnore]
    public bool serieSpecified { get; set; }

    [XmlIgnore]
    public bool nDCSpecified { get; set; }

    [XmlIgnore]
    public bool tpEmisSpecified { get; set; }

    [XmlIgnore]
    public bool tpAmbSpecified { get; set; }

    [XmlIgnore]
    public bool dhEmiProxySpecified { get; set; }

}
