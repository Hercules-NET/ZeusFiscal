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
[XmlRoot("infEvento", Namespace = "http://www.portalfiscal.inf.br/dce")]
public class infEvento
{
    [XmlAttribute("Id")]
    public string Id { get; set; } = string.Empty;

    [XmlElement("cOrgao", Order = 0)]
    public TCOrgaoIBGE cOrgao { get; set; }

    [XmlElement("tpAmb", Order = 1)]
    public TipoAmbiente tpAmb { get; set; }

    [XmlElement("tpEmit", Order = 2)]
    public TipoEmitenteDCe tpEmit { get; set; }

    [XmlElement("CNPJAutor", Order = 3)]
    public string CNPJAutor { get; set; } = string.Empty;

    [XmlElement("CNPJUsEmit", Order = 4)]
    public string? CNPJUsEmit { get; set; }

    [XmlElement("CPFUsEmit", Order = 5)]
    public string? CPFUsEmit { get; set; }

    [XmlElement("IdOutrosUsEmit", Order = 6)]
    public string? IdOutrosUsEmit { get; set; }

    [XmlElement("chDCe", Order = 7)]
    public string chDCe { get; set; } = string.Empty;

    private DateTimeOffset _dhEvento;
    private string? _dhEventoXml;

    [XmlIgnore]
    public DateTimeOffset dhEvento
    {
        get => _dhEvento;
        set { _dhEvento = value; _dhEventoXml = null; }
    }

    [XmlElement("dhEvento", Order = 8)]
    public string dhEventoProxy
    {
        get => _dhEventoXml ?? _dhEvento.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        set { _dhEvento = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None); _dhEventoXml = value; }
    }

    [XmlElement("tpEvento", Order = 9)]
    public TipoEventoDCe tpEvento { get; set; } = TipoEventoDCe.Cancelamento;

    [XmlElement("nSeqEvento", Order = 10)]
    public int nSeqEvento { get; set; } = 1;

    [XmlElement("detEvento", Order = 11)]
    public detEvento detEvento { get; set; } = new detEvento();

}
