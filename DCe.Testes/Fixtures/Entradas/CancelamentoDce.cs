using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
namespace DCe.Testes.Fixtures.Entradas;

/// <summary>
/// Evento de cancelamento da DC-e, tpEvento 110111 — o único evento que a DC-e tem.
/// </summary>
internal class CancelamentoDce
{
    public TipoAmbiente Ambiente { get; set; } = TipoAmbiente.Homologacao;
    public string ChaveAcesso { get; set; } = string.Empty;
    public Estado CodigoOrgao { get; set; }

    /// <summary>Mesmo domínio de tpEmit da emissão: 3 = Transportadora.</summary>
    public int TipoEmitente { get; set; } = 3;

    /// <summary>CNPJ de quem assina o evento — tem que ser o mesmo emissor da DC-e.</summary>
    public string CnpjAutor { get; set; } = string.Empty;

    /// <summary>Identificação do usuário emitente da DC-e original: um destes três.</summary>
    public string? CnpjUsuarioEmitente { get; set; }
    public string? CpfUsuarioEmitente { get; set; }
    public string? IdOutrosUsuarioEmitente { get; set; }

    public string NumeroProtocolo { get; set; } = string.Empty;
    public string Justificativa { get; set; } = string.Empty;

    /// <summary>1 no primeiro cancelamento. Até 3 dígitos.</summary>
    public int SequenciaEvento { get; set; } = 1;

    /// <summary>
    /// Offset local do emitente usado para formatar <c>dhEvento</c>. Mesma razão de
    /// <see cref="DeclaracaoConteudo.OffsetHorario"/>: a SEFAZ compara o literal contra o
    /// relógio local dela (cStat 703, regra B07-10). Default -3h; ajustar para AM/RO/RR/AC.
    /// </summary>
    public TimeSpan OffsetHorario { get; set; } = TimeSpan.FromHours(-3);
}
