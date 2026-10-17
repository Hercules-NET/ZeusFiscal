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
[XmlRoot("obsECT", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class obsECT
{
    [XmlAttribute("xCampo")]
    public string xCampo { get; set; } = string.Empty;

    [XmlElement("xTexto", Order = 0)]
    public string xTexto { get; set; } = string.Empty;

}
