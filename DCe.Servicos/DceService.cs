using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DCe.Classes.Servicos.Evento;
using DCe.Classes.Servicos.Consulta;
using DCe.Classes.Servicos.StatusServico;
using DCe.Utils.Extensoes;
using DCeDocumento = DCe.Classes.Informacoes.DCe;
using DCe.Classes.Retorno;
using DCe.Wsdl.Soap;
using DCe.Utils;
using DCe.Utils.Validacao;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Utils.Assinatura;

namespace DCe.Servicos;

/// <summary>Autorização, consulta, status e evento 110111 no autorizador nacional.</summary>
public sealed class DceService : IDceService
{
    private readonly DCeConfiguracao _configuracao;
    private readonly ISoapClient _soapClient;

    public DceService(DCeConfiguracao configuracao, ISoapClient? soapClient = null)
    {
        _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
        _soapClient = soapClient ?? new SoapClient();
    }

    public Task<retDCe> AutorizarAsync(DCeDocumento declaracao)
    {
        if (declaracao == null) throw new ArgumentNullException(nameof(declaracao));
        ValidarAmbiente(declaracao.infDCe.ide.tpAmb);
        declaracao.ValidarDadosLocais();
        var xml = declaracao.ObterXmlString();
        return ComCertificadoAsync(async certificado =>
        {
            var assinado = new DCeDocumento().CarregarDeXmlString(xml).Assina(certificado);
            assinado.Valida(_configuracao.CaminhoSchemas);
            return await EnviarAsync<retDCe>(assinado.ObterXmlString(), "DCeAutorizacao", "dceAutorizacao",
                DceEndpoints.Autorizacao(declaracao.infDCe.ide.tpAmb), certificado).ConfigureAwait(false);
        });
    }

    public Task<retConsSitDCe> ConsultarAsync(string chaveAcesso)
    {
        if (string.IsNullOrWhiteSpace(chaveAcesso)) throw new ArgumentException("A chave de acesso não pode ser vazia.", nameof(chaveAcesso));
        return ConsultarAsync(new consSitDCe { tpAmb = _configuracao.TipoAmbiente, chDCe = chaveAcesso });
    }

    public Task<retConsSitDCe> ConsultarAsync(consSitDCe consulta)
    {
        if (consulta == null) throw new ArgumentNullException(nameof(consulta));
        ValidarAmbiente(consulta.tpAmb);
        var endpoint = DceEndpoints.Consulta(consulta.tpAmb);
        var xml = consulta.ObterXmlString();
        Validador.Valida(xml, "consulta", "consSitDCe_v1.00.xsd", _configuracao.CaminhoSchemas);
        return ComCertificadoAsync(async certificado => await EnviarAsync<retConsSitDCe>(xml,
            "DCeConsulta", "dceConsulta", endpoint, certificado).ConfigureAwait(false));
    }

    public Task<retConsStatServDCe> StatusServicoAsync(Estado codigoUf)
    {
        return StatusServicoAsync(new consStatServDCe { tpAmb = _configuracao.TipoAmbiente, cUF = codigoUf });
    }

    public Task<retConsStatServDCe> StatusServicoAsync(consStatServDCe consulta)
    {
        if (consulta == null) throw new ArgumentNullException(nameof(consulta));
        ValidarAmbiente(consulta.tpAmb);
        var endpoint = DceEndpoints.StatusServico(consulta.tpAmb);
        var xml = consulta.ObterXmlString();
        Validador.Valida(xml, "consulta", "consStatServDCe_v1.00.xsd", _configuracao.CaminhoSchemas);
        return ComCertificadoAsync(async certificado => await EnviarAsync<retConsStatServDCe>(xml,
            "DCeStatusServico", "dceStatusServico", endpoint, certificado).ConfigureAwait(false));
    }

    public Task<retEventoDCe> CancelarAsync(eventoDCe pedido)
    {
        if (pedido == null) throw new ArgumentNullException(nameof(pedido));
        ValidarAmbiente(pedido.infEvento.tpAmb);
        pedido.ValidarDadosLocais();
        var xml = pedido.ObterXmlString();
        // O schema geral usa processContents="skip" no detalhe; validar também o evento 110111.
        var detalhe = pedido.infEvento.detEvento.evCancDCe;
        Validador.Valida(detalhe.ObterXmlString(), "evento", "evCancDCe_v1.00.xsd", _configuracao.CaminhoSchemas);
        return ComCertificadoAsync(async certificado =>
        {
            var assinado = new eventoDCe().CarregarDeXmlString(xml).Assina(certificado);
            assinado.Valida(_configuracao.CaminhoSchemas);
            return await EnviarAsync<retEventoDCe>(assinado.ObterXmlString(), "DCeRecepcaoEvento", "dceRecepcaoEvento",
                DceEndpoints.RecepcaoEvento(pedido.infEvento.tpAmb), certificado).ConfigureAwait(false);
        });
    }

    private void ValidarAmbiente(TipoAmbiente ambiente)
    {
        DceEndpoints.Base(ambiente);
        if (ambiente != _configuracao.TipoAmbiente)
            throw new ArgumentException("O ambiente do documento difere da configuração do serviço.", nameof(ambiente));
    }

    private async Task<T> ComCertificadoAsync<T>(Func<X509Certificate2, Task<T>> executar)
    {
        if (_configuracao.TimeOut <= 0) throw new ArgumentOutOfRangeException(nameof(_configuracao.TimeOut), "O timeout deve ser positivo.");
        var certificado = _configuracao.Certificado ?? CertificadoDigital.ObterCertificado(_configuracao.ConfiguracaoCertificado);
        var liberar = _configuracao.Certificado == null && !_configuracao.ConfiguracaoCertificado.ManterDadosEmCache;
        try { return await executar(certificado).ConfigureAwait(false); }
        finally { if (liberar) certificado.Dispose(); }
    }

    private async Task<T> EnviarAsync<T>(string xml, string servico, string operacao, string endpoint, X509Certificate2 certificado)
        where T : RetornoBase, new()
    {
        var nsServico = SoapEnvelope.NamespaceWsdl + servico;
        var envelope = SoapEnvelope.Criar(xml, servico);
        var identificador = Guid.NewGuid().ToString("N");
        SalvarXml(xml, identificador + "-" + servico + "-env.xml");
        var resposta = await _soapClient.SendAsync(endpoint, nsServico + "/" + operacao,
            envelope, certificado, _configuracao.TimeOut).ConfigureAwait(false);
        var retorno = SoapEnvelope.ExtrairXmlFiscal(resposta);
        SalvarXml(retorno, identificador + "-" + servico + "-ret.xml");
        var tipado = new T().CarregarDeXmlString(SoapEnvelope.Desembrulhar(resposta));
        tipado.EnvioStr = SoapEnvelope.XmlFiscal(xml);
        tipado.RetornoStr = retorno;
        tipado.RetornoCompletoStr = resposta;
        return tipado;
    }

    private void SalvarXml(string xml, string arquivo)
    {
        if (!_configuracao.IsSalvarXml) return;
        if (string.IsNullOrWhiteSpace(_configuracao.CaminhoSalvarXml))
            throw new InvalidOperationException("Informe CaminhoSalvarXml quando IsSalvarXml estiver habilitado.");
        Directory.CreateDirectory(_configuracao.CaminhoSalvarXml);
        File.WriteAllText(Path.Combine(_configuracao.CaminhoSalvarXml, arquivo), xml, new UTF8Encoding(false));
    }

}
