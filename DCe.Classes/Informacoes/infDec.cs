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
[XmlRoot("infDec", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infDec
{
    private string _xObs1 = "É contribuinte de ICMS qualquer pessoa física ou jurídica, que realize, com habitualidade ou em volume que caracterize intuito comercial, operações de circulação de mercadoria ou prestações de serviços de transportes interestadual e intermunicipal e de comunicação, ainda que as operações e prestações de iniciem no exterior (Lei Complementar nº 87/96, Art. 4º)";
    [XmlElement("xObs1", Order = 0)]
    public string xObs1
    {
        get => _xObs1;
        set { _xObs1 = value; xObs1Specified = true; }
    }

    private string _xObs2 = "Constitui crime contra a ordem tributária suprimir ou reduzir tributo, ou contribuição social e qualquer acessório: quando negar ou deixar de fornecer, quando obrigatório, nota fiscal ou documento equivalente, relativa a venda de mercadoria ou prestação de serviço, efetivamente realizada ou fornece-la em desacordo com a legislação. Sob pena de reclusão de 2 (dois) e 5 (cinco) anos, e multa (Lei 8.137/90, Art 1ª, V)";
    [XmlElement("xObs2", Order = 1)]
    public string xObs2
    {
        get => _xObs2;
        set { _xObs2 = value; xObs2Specified = true; }
    }

    [XmlIgnore]
    public bool xObs1Specified { get; set; }

    [XmlIgnore]
    public bool xObs2Specified { get; set; }

}
