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
[XmlRoot("detEvento", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class detEvento
{
    [XmlAttribute("versaoEvento")]
    public string versaoEvento { get; set; } = "1.00";

    [XmlElement("evCancDCe", Order = 0)]
    public evCancDCe evCancDCe { get; set; } = new evCancDCe();

}
