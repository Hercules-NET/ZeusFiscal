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
[XmlRoot("transp", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class DCeTransp
{
    [XmlElement("modTrans", Order = 0)]
    public ModalidadeTransporteDCe modTrans
    {
        get => _modTrans;
        set { _modTrans = value; modTransSpecified = true; }
    }

    private ModalidadeTransporteDCe _modTrans;

    [XmlIgnore]
    public bool modTransSpecified { get; set; }

    [XmlElement("CNPJTransp", Order = 1)]
    public string CNPJTransp { get; set; } = string.Empty;

}
