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
[XmlRoot("infDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlAttribute("Id")]
    public string Id { get; set; } = string.Empty;

    private DCeIde _ide = new DCeIde();
    [XmlElement("ide", Order = 0)]
    public DCeIde ide
    {
        get => _ide;
        set { _ide = value; ideSpecified = true; }
    }

    private DCeEmit _emit = new DCeEmit();
    [XmlElement("emit", Order = 1)]
    public DCeEmit emit
    {
        get => _emit;
        set { _emit = value; emitSpecified = true; }
    }

    [XmlElement("Fisco", Order = 2)]
    public Fisco? Fisco { get; set; }

    [XmlElement("Marketplace", Order = 3)]
    public Marketplace? Marketplace { get; set; }

    [XmlElement("Transportadora", Order = 4)]
    public Transportadora? Transportadora { get; set; }

    [XmlElement("ECT", Order = 5)]
    public ECT? ECT { get; set; }

    private DCeDest _dest = new DCeDest();
    [XmlElement("dest", Order = 6)]
    public DCeDest dest
    {
        get => _dest;
        set { _dest = value; destSpecified = true; }
    }

    [XmlElement("autXML", Order = 7)]
    public List<autXML> autXML { get; set; } = new List<autXML>();

    [XmlElement("det", Order = 8)]
    public List<DCeDet> det { get; set; } = new List<DCeDet>();

    [XmlElement("total", Order = 9)]
    public DCeTotal total { get; set; } = new DCeTotal();

    private DCeTransp _transp = new DCeTransp();
    [XmlElement("transp", Order = 10)]
    public DCeTransp transp
    {
        get => _transp;
        set { _transp = value; transpSpecified = true; }
    }

    [XmlElement("infAdic", Order = 11)]
    public infAdic? infAdic { get; set; }

    private infDec _infDec = new infDec();
    [XmlElement("infDec", Order = 12)]
    public infDec infDec
    {
        get => _infDec;
        set { _infDec = value; infDecSpecified = true; }
    }

    [XmlIgnore]
    public bool ideSpecified { get; set; }

    [XmlIgnore]
    public bool emitSpecified { get; set; }

    [XmlIgnore]
    public bool destSpecified { get; set; }

    [XmlIgnore]
    public bool infDecSpecified { get; set; }

    [XmlIgnore]
    public bool transpSpecified { get; set; }

}
