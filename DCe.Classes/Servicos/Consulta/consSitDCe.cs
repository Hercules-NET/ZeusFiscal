using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using DCe.Classes.Flags;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Servicos.Consulta;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("consSitDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class consSitDCe
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlElement("tpAmb", Order = 0)]
    public TipoAmbiente tpAmb { get; set; }

    [XmlElement("xServ", Order = 1)]
    public ServicoConsultaDCe xServ { get; set; } = ServicoConsultaDCe.Consultar;

    [XmlElement("chDCe", Order = 2)]
    public string chDCe { get; set; } = string.Empty;

}
