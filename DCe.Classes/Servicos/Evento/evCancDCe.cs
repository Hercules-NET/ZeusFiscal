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
[XmlRoot("evCancDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class evCancDCe
{
    [XmlElement("descEvento", Order = 0)]
    public DescricaoEventoDCe descEvento { get; set; } = DescricaoEventoDCe.Cancelamento;

    [XmlElement("nProt", Order = 1)]
    public string nProt { get; set; } = string.Empty;

    [XmlElement("xJust", Order = 2)]
    public string xJust { get; set; } = string.Empty;

}
