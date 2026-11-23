using System;
using System.Xml.Serialization;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("infFisco", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infFisco
{
    [XmlElement("cMsg", Order = 0)]
    public int cMsg
    {
        get => _cMsg;
        set { _cMsg = value; cMsgSpecified = true; }
    }

    [XmlIgnore]
    public bool cMsgSpecified { get; set; }

    private int _cMsg;

    [XmlElement("xMsg", Order = 1)]
    public string xMsg
    {
        get => _xMsg;
        set { _xMsg = value; xMsgSpecified = true; }
    }

    [XmlIgnore]
    public bool xMsgSpecified { get; set; }

    private string _xMsg = string.Empty;
}
