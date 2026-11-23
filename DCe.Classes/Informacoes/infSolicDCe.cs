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
[XmlRoot("infSolicDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infSolicDCe
{
    [XmlElement("xSolic", Order = 0)]
    public string xSolic { get; set; } = string.Empty;

}
