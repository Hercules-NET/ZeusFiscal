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
[XmlRoot("autXML", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class autXML
{
    [XmlElement("CNPJ", Order = 0)]
    public string? CNPJ { get; set; }

    [XmlElement("CPF", Order = 1)]
    public string? CPF { get; set; }

}
