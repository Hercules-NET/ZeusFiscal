using System;
using System.Xml.Serialization;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("protDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class protDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlElement("infProt", Order = 0)]
    public infProt? infProt
    {
        get => _infProt;
        set { _infProt = value; infProtSpecified = true; }
    }

    [XmlIgnore]
    public bool infProtSpecified { get; set; }

    private infProt? _infProt;

    [XmlElement("infFisco", Order = 1)]
    public infFisco? infFisco { get; set; }

    [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 2)]
    public Signature? Signature { get; set; }
}
