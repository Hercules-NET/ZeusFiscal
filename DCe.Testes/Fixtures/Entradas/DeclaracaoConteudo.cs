using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System;
using System.Collections.Generic;
namespace DCe.Testes.Fixtures.Entradas;

/// <summary>
/// Uma DC-e. O manual permite no máximo UMA por chamada ao DCeAutorizacao — não há lote.
/// </summary>
internal class DeclaracaoConteudo
{
    /// <summary>"1" = Produção, "2" = Homologação.</summary>
    public TipoAmbiente Ambiente { get; set; } = TipoAmbiente.Homologacao;

    /// <summary>Código IBGE da UF do emitente (2 dígitos).</summary>
    public Estado CodigoUf { get; set; }

    public int Serie { get; set; } = 1;
    public int Numero { get; set; }

    /// <summary>1 = normal, 2 = contingência off-line. A NT corrigiu de 9 para 2.</summary>
    public int TipoEmissao { get; set; } = 1;

    /// <summary>0 Fisco · 1 Marketplace · 2 Emissor próprio · 3 Transportadora · 4 Correios.</summary>
    public int TipoEmitente { get; set; } = 3;

    public int SiteAutorizador { get; set; }

    public string VersaoAplicativo { get; set; } = string.Empty;

    /// <summary>O usuário emitente — o não contribuinte dono dos bens.</summary>
    public PessoaDce Emitente { get; set; } = new();

    /// <summary>Quem assina. Para a modalidade Transportadora, é a transportadora.</summary>
    public SignatarioDce Signatario { get; set; } = new();

    public PessoaDce Destinatario { get; set; } = new();

    public List<ItemDce> Itens { get; set; } = new();

    public TransporteDce Transporte { get; set; } = new();

    /// <summary>Texto do grupo infSolicDCe.</summary>
    public string Solicitacao { get; set; } = string.Empty;

    public string? InformacoesComplementares { get; set; }

    /// <summary>
    /// Offset local do emitente usado para formatar <c>dhEmi</c>. Existe porque a SEFAZ compara
    /// o LITERAL de <c>dhEmi</c> contra o relógio local dela, e não o instante UTC equivalente —
    /// mandar em <c>+00:00</c> (ainda que representando o mesmo instante que a hora local) faz o
    /// literal parecer 3h no futuro e a SEFAZ rejeita com cStat 703 ("Data-Hora de Emissao
    /// posterior ao horario de recebimento", regra B07-10 do Anexo I, tolerância de 5 minutos).
    /// O XSD (<c>TDateTimeUTC</c>) aceita offsets de -00:00 a -11:00/+12:00;
    /// a validade estrutural não substitui a verificação fiscal do horário.
    /// Default -3h cobre a maior parte do país, inclusive PE e PR (autorizador), mas
    /// AM/RO/RR/AC são -04:00/-05:00: setar explicitamente para emitentes desses estados.
    /// </summary>
    public TimeSpan OffsetHorario { get; set; } = TimeSpan.FromHours(-3);
}
