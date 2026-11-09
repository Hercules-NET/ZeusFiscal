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
[XmlRoot("DCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCe
{
    private infDCe _infDCe = new infDCe();
    [XmlElement("infDCe", Order = 0)]
    public infDCe infDCe
    {
        get => _infDCe;
        set { _infDCe = value; infDCeSpecified = true; }
    }

    [XmlElement("infSolicDCe", Order = 1)]
    public infSolicDCe infSolicDCe { get; set; } = new infSolicDCe();

    [XmlElement("infDCeSupl", Order = 2)]
    public infDCeSupl? infDCeSupl { get; set; }

    [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 3)]
    public Signature? Signature { get; set; }

    [XmlIgnore]
    public bool infDCeSpecified { get; set; }

}
