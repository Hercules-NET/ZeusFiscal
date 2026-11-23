using System.Security.Cryptography.X509Certificates;
using DFe.Classes.Flags;
using DFe.Utils;

namespace DCe.Servicos;

/// <summary>Configuração por instância dos serviços nacionais da DC-e.</summary>
public sealed class DCeConfiguracao
{
    public TipoAmbiente TipoAmbiente { get; set; } = TipoAmbiente.Homologacao;
    public ConfiguracaoCertificado ConfiguracaoCertificado { get; set; } = new ConfiguracaoCertificado();

    /// <summary>Certificado já carregado, opcional. Seu ciclo de vida pertence ao chamador.</summary>
    public X509Certificate2? Certificado { get; set; }

    /// <summary>Tempo limite de comunicação em milissegundos, como nas demais famílias Zeus.</summary>
    public int TimeOut { get; set; } = 60000;
    /// <summary>Pasta dos XSDs, contendo dce/, consulta/ e evento/. Nulo usa os recursos embutidos.</summary>
    public string? CaminhoSchemas { get; set; }

    public bool IsSalvarXml { get; set; }
    public string CaminhoSalvarXml { get; set; } = string.Empty;
}
