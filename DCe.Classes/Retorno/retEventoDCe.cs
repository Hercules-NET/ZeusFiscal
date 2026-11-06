using System;
using System.Globalization;
using System.Xml.Serialization;
using DFe.Classes.Assinatura;

namespace DCe.Classes.Retorno;

[Serializable]
[XmlType(Namespace = "http://www.portalfiscal.inf.br/dce")]
[XmlRoot("retEventoDCe", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class retEventoDCe : RetornoBase
{
    [XmlAttribute("versao")]
    public string versao { get; set; } = "1.00";

    [XmlElement("infEvento", Order = 0)]
    public infEventoRetorno? infEvento
    {
        get => _infEvento;
        set { _infEvento = value; infEventoSpecified = true; }
    }

    [XmlIgnore]
    public bool infEventoSpecified { get; set; }

    private infEventoRetorno? _infEvento;

    [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 1)]
    public Signature? Signature { get; set; }

    [XmlIgnore]
    public string Ambiente => infEvento == null ? string.Empty : ((int)infEvento.tpAmb).ToString(CultureInfo.InvariantCulture);

    [XmlIgnore]
    public string VersaoAplicativo => infEvento?.verAplic ?? string.Empty;

    [XmlIgnore]
    public string CodigoOrgao => infEvento?.cOrgaoSpecified == true ? ((int)infEvento.cOrgao).ToString(CultureInfo.InvariantCulture) : string.Empty;

    [XmlIgnore]
    public string CodigoStatus => infEvento?.cStat.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

    [XmlIgnore]
    public string Motivo => infEvento?.xMotivo ?? string.Empty;

    [XmlIgnore]
    public string? ChaveAcesso => infEvento?.chDCe;

    [XmlIgnore]
    public string? TipoEvento => infEvento?.tpEvento;

    [XmlIgnore]
    public string? DescricaoEvento => infEvento?.xEvento;

    [XmlIgnore]
    public string? SequenciaEvento => infEvento?.nSeqEvento;

    [XmlIgnore]
    public DateTimeOffset? DataRegistro => infEvento?.dhRegEvento;

    [XmlIgnore]
    public string? NumeroProtocolo => infEvento?.nProt;

    [XmlIgnore]
    public bool Registrado => infEvento?.cStat == 135;
}
