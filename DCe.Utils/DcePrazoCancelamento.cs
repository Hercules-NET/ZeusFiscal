using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace DCe.Utils;

/// <summary>
/// Prazo de cancelamento da DC-e: 24 horas da autorização, conforme a Cláusula 11ª do
/// Ajuste SINIEF 05/2021. Lógica pura, sem relógio próprio — quem chama passa o "agora".
/// </summary>
public static class DcePrazoCancelamento
{
    public static readonly TimeSpan Janela = TimeSpan.FromHours(24);

    /// <summary>
    /// Verdadeiro enquanto a janela não fechou.
    ///
    /// <para>Decorrido negativo conta como dentro do prazo para tolerar deriva entre relógios.
    /// <c>autorizadaEm</c> corresponde ao <c>dhRecbto</c> do protocolo da SEFAZ, enquanto
    /// <c>agora</c> é informado pelo chamador. Exigir decorrido não negativo poderia impedir
    /// o cancelamento de uma DC-e recém-autorizada quando o relógio local estiver atrasado.</para>
    ///
    /// <para>A SEFAZ é a autoridade sobre o prazo e rejeita cancelamentos fora da janela com
    /// <c>cStat 501</c>.</para>
    /// </summary>
    public static bool DentroDoPrazo(DateTimeOffset autorizadaEm, DateTimeOffset agora)
        => agora - autorizadaEm < Janela;
}
