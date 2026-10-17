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
[XmlRoot("emit", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeEmit
{
    [XmlElement("CNPJ", Order = 0)]
    public string? CNPJ { get; set; }

    [XmlElement("CPF", Order = 1)]
    public string? CPF { get; set; }

    [XmlElement("idOutros", Order = 2)]
    public string? idOutros { get; set; }

    [XmlElement("xNome", Order = 3)]
    public string xNome { get; set; } = string.Empty;

    [XmlElement("enderEmit", Order = 4)]
    public enderEmit enderEmit { get; set; } = new enderEmit();

}
