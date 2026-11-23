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
[XmlRoot("prod", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeProd
{
    [XmlElement("xProd", Order = 0)]
    public string xProd { get; set; } = string.Empty;

    [XmlElement("NCM", Order = 1)]
    public string NCM { get; set; } = string.Empty;

    private decimal _qCom;
    private string? _qComXml;

    [XmlIgnore]
    public decimal qCom
    {
        get => _qCom;
        set { _qCom = value; _qComXml = null; qComProxySpecified = true; }
    }

    [XmlElement("qCom", Order = 2)]
    public string qComProxy
    {
        get => _qComXml ?? _qCom.ToString("F4", CultureInfo.InvariantCulture);
        set { _qCom = decimal.Parse(value, CultureInfo.InvariantCulture); _qComXml = value; qComProxySpecified = true; }
    }

    private decimal _vUnCom;
    private string? _vUnComXml;

    [XmlIgnore]
    public decimal vUnCom
    {
        get => _vUnCom;
        set { _vUnCom = value; _vUnComXml = null; vUnComProxySpecified = true; }
    }

    [XmlElement("vUnCom", Order = 3)]
    public string vUnComProxy
    {
        get => _vUnComXml ?? Math.Round(_vUnCom, 8, MidpointRounding.AwayFromZero).ToString("0.00########", CultureInfo.InvariantCulture);
        set { _vUnCom = decimal.Parse(value, CultureInfo.InvariantCulture); _vUnComXml = value; vUnComProxySpecified = true; }
    }

    private decimal _vProd;
    private string? _vProdXml;

    [XmlIgnore]
    public decimal vProd
    {
        get => _vProd;
        set { _vProd = value; _vProdXml = null; vProdProxySpecified = true; }
    }

    [XmlElement("vProd", Order = 4)]
    public string vProdProxy
    {
        get => _vProdXml ?? Math.Round(_vProd, 8, MidpointRounding.AwayFromZero).ToString("0.00########", CultureInfo.InvariantCulture);
        set { _vProd = decimal.Parse(value, CultureInfo.InvariantCulture); _vProdXml = value; vProdProxySpecified = true; }
    }

    [XmlIgnore]
    public bool qComProxySpecified { get; set; }

    [XmlIgnore]
    public bool vUnComProxySpecified { get; set; }

    [XmlIgnore]
    public bool vProdProxySpecified { get; set; }

}
