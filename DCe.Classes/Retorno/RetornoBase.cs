using System;
using System.Xml.Serialization;

namespace DCe.Classes.Retorno;

/// <summary>XMLs da comunicação, disponíveis junto ao retorno fiscal tipado.</summary>
[Serializable]
public abstract class RetornoBase
{
    /// <summary>Mensagem fiscal exatamente como inserida no Body, sem declaração XML.</summary>
    [XmlIgnore]
    public string EnvioStr { get; set; } = string.Empty;

    /// <summary>Mensagem fiscal original recebida, sem reserialização.</summary>
    [XmlIgnore]
    public string RetornoStr { get; set; } = string.Empty;

    /// <summary>Resposta integral recebida do transporte.</summary>
    [XmlIgnore]
    public string RetornoCompletoStr { get; set; } = string.Empty;
}
