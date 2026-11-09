using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using DCe.Classes.Flags;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Servicos.StatusServico;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("consStatServDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class consStatServDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlElement("tpAmb", Order = 0)]
    public TipoAmbiente tpAmb { get; set; }

    [XmlElement("cUF", Order = 1)]
    public Estado cUF { get; set; }

    [XmlElement("xServ", Order = 2)]
    public ServicoStatusDCe xServ { get; set; } = ServicoStatusDCe.Status;

}
