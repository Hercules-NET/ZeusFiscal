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
[XmlRoot("total", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeTotal
{
    private decimal _vDC;
    private string? _vDCXml;

    [XmlIgnore]
    public decimal vDC
    {
        get => _vDC;
        set { _vDC = value; _vDCXml = null; vDCProxySpecified = true; }
    }

    [XmlElement("vDC", Order = 0)]
    public string vDCProxy
    {
        get => _vDCXml ?? _vDC.ToString("F2", CultureInfo.InvariantCulture);
        set { _vDC = decimal.Parse(value, CultureInfo.InvariantCulture); _vDCXml = value; vDCProxySpecified = true; }
    }

    [XmlIgnore]
    public bool vDCProxySpecified { get; set; }

}
