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
[XmlRoot("enderDest", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class enderDest
{
    [XmlElement("xLgr", Order = 0)]
    public string xLgr { get; set; } = string.Empty;

    [XmlElement("nro", Order = 1)]
    public string nro { get; set; } = string.Empty;

    [XmlElement("xCpl", Order = 2)]
    public string? xCpl { get; set; }

    [XmlElement("xBairro", Order = 3)]
    public string xBairro { get; set; } = string.Empty;

    [XmlElement("cMun", Order = 4)]
    public int cMun { get; set; }

    [XmlElement("xMun", Order = 5)]
    public string xMun { get; set; } = string.Empty;

    [XmlElement("UF", Order = 6)]
    public TUf UF
    {
        get => _UF;
        set { _UF = value; UFSpecified = true; }
    }

    private TUf _UF;

    [XmlIgnore]
    public bool UFSpecified { get; set; }

    [XmlElement("CEP", Order = 7)]
    public string CEP { get; set; } = string.Empty;

    [XmlElement("cPais", Order = 8)]
    public int cPais { get; set; } = 1058;

    [XmlElement("xPais", Order = 9)]
    public string xPais { get; set; } = "BRASIL";

    [XmlElement("fone", Order = 10)]
    public string? fone { get; set; }

    [XmlElement("email", Order = 11)]
    public string? email { get; set; }

}
