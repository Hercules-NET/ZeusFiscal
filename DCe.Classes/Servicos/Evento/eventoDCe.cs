using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using DCe.Classes.Flags;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Servicos.Evento;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("eventoDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class eventoDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlElement("infEvento", Order = 0)]
    public infEvento infEvento { get; set; } = new infEvento();

    [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 1)]
    public Signature? Signature { get; set; }

}
