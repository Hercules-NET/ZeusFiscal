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
[XmlRoot("det", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeDet
{
    private int _nItem = default;
    [XmlAttribute("nItem")]
    public int nItem
    {
        get => _nItem;
        set { _nItem = value; nItemSpecified = true; }
    }

    [XmlElement("prod", Order = 0)]
    public DCeProd prod { get; set; } = new DCeProd();

    [XmlElement("infAdProd", Order = 1)]
    public string? infAdProd { get; set; }

    [XmlIgnore]
    public bool nItemSpecified { get; set; }

}
