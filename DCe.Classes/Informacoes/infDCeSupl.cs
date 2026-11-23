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
[XmlRoot("infDCeSupl", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infDCeSupl
{
    [XmlElement("qrCodDCe", Order = 0)]
    public string qrCodDCe { get; set; } = string.Empty;

    [XmlElement("urlChave", Order = 1)]
    public string urlChave { get; set; } = string.Empty;

}
