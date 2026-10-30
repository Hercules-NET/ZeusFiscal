using DCe.Testes.Fixtures;
using System;
using System.Threading.Tasks;
using DCe.Testes.Fixtures.Entradas;
using DCe.Classes.Retorno;
using DCe.Classes.Servicos.Evento;
using DCe.Servicos;
using DCe.Utils;
using DCe.Utils.Abstractions;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Testes;

// Converte as entradas das fixtures para os modelos fiscais usados pelos serviços.
internal static class EntradasModelo
{
    private sealed class Relogio : IClock { public DateTime UtcNow => new(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc); }
    private sealed class Codigo : IRandomSource { public int ProximoCodigoDocumento() => 123456; }
    private static FabricaModeloDCe Ponte() => new(new Relogio(), new Codigo());
    public static Task<retDCe> AutorizarAsync(this DceService s, DeclaracaoConteudo d) =>
        s.AutorizarAsync(d == null ? null! : Ponte().Criar(d));
    public static Task<retEventoDCe> CancelarAsync(this DceService s, CancelamentoDce d) =>
        s.CancelarAsync(d == null ? null! : Ponte().Criar(d));
}
