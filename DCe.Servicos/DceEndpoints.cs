using System;
using DFe.Classes.Flags;

namespace DCe.Servicos;

/// <summary>Autorizador nacional SEFAZ-PR, sem seleção por UF.</summary>
public static class DceEndpoints
{
    public static string Base(TipoAmbiente ambiente)
    {
        switch (ambiente)
        {
            case TipoAmbiente.Producao: return "https://dce.fazenda.pr.gov.br/dce";
            case TipoAmbiente.Homologacao: return "https://homologacao.dce.fazenda.pr.gov.br/dce";
            default: throw new ArgumentOutOfRangeException(nameof(ambiente), "Ambiente da DC-e inválido.");
        }
    }

    public static string Autorizacao(TipoAmbiente ambiente) => Base(ambiente) + "/DCeAutorizacao";
    public static string Consulta(TipoAmbiente ambiente) => Base(ambiente) + "/DCeConsulta";
    public static string RecepcaoEvento(TipoAmbiente ambiente) => Base(ambiente) + "/DCeRecepcaoEvento";
    public static string StatusServico(TipoAmbiente ambiente) => Base(ambiente) + "/DCeStatusServico";
}
