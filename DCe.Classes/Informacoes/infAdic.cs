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
[XmlRoot("infAdic", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infAdic
{
    [XmlElement("infAdFisco", Order = 0)]
    public string? infAdFisco { get; set; }

    [XmlElement("infCpl", Order = 1)]
    public string? infCpl { get; set; }

    [XmlElement("infAdMarketplace", Order = 2)]
    public string? infAdMarketplace { get; set; }

    [XmlElement("infAdECT", Order = 3)]
    public string? infAdECT { get; set; }

    [XmlElement("obsEmit", Order = 4)]
    public List<obsEmit> obsEmit { get; set; } = new List<obsEmit>();

    [XmlElement("obsFisco", Order = 5)]
    public List<obsFisco> obsFisco { get; set; } = new List<obsFisco>();

    [XmlElement("obsMarketplace", Order = 6)]
    public List<obsMarketplace> obsMarketplace { get; set; } = new List<obsMarketplace>();

    [XmlElement("obsECT", Order = 7)]
    public List<obsECT> obsECT { get; set; } = new List<obsECT>();

}
