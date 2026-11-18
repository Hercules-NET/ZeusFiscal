using System;
using System.Globalization;
using System.Linq;
using DCe.Classes.Flags;
using DCe.Classes.Informacoes;
using DCe.Testes.Fixtures.Entradas;
using DCe.Classes.Servicos.Evento;
using DCe.Utils;
using DCe.Utils.Abstractions;
using DCe.Utils.Extensoes;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Testes.Fixtures;

/// <summary>Fábrica dos dados de entrada dos testes, sem serialização própria.</summary>
internal sealed class FabricaModeloDCe
{
    private readonly IClock _clock;
    private readonly IRandomSource _random;
    public FabricaModeloDCe() : this(new SystemClock(), new SecureRandomSource()) { }
    public FabricaModeloDCe(IClock clock, IRandomSource randomSource)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _random = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }
    private static string Digitos(string? s) => new((s ?? string.Empty).Where(char.IsDigit).ToArray());
    private static string? Documento(string? s) => string.IsNullOrWhiteSpace(s) ? null : Digitos(s);
    private static string? Texto(string? s) => string.IsNullOrWhiteSpace(s) ? null : s;
    private static TUf Uf(string s) => (TUf)Enum.Parse(typeof(TUf), s.Trim().ToUpperInvariant());
    private DateTimeOffset Instante(TimeSpan offset) => new DateTimeOffset(DateTime.SpecifyKind(_clock.UtcNow, DateTimeKind.Utc)).ToOffset(offset);

    public DCeDocumento Criar(DeclaracaoConteudo dc)
    {
        var e = dc.Emitente.Endereco;
        var d = dc.Destinatario.Endereco;
        var documento = new DCeDocumento
        {
            infDCe = new infDCe
            {
                ide = new DCeIde
                {
                    cUF = dc.CodigoUf, cDC = _random.ProximoCodigoDocumento().ToString("D6", CultureInfo.InvariantCulture),
                    serie = dc.Serie, nDC = dc.Numero, dhEmi = Instante(dc.OffsetHorario),
                    tpEmis = (TipoEmissaoDCe)dc.TipoEmissao, tpEmit = (TipoEmitenteDCe)dc.TipoEmitente,
                    nSiteAutoriz = dc.SiteAutorizador, tpAmb = dc.Ambiente, verProc = dc.VersaoAplicativo
                },
                emit = new DCeEmit
                {
                    CNPJ = Documento(dc.Emitente.Cnpj), CPF = Documento(dc.Emitente.Cpf), idOutros = Texto(dc.Emitente.IdOutros)?.Trim(), xNome = dc.Emitente.Nome,
                    enderEmit = new enderEmit
                    {
                        xLgr = e.Logradouro, nro = e.Numero, xCpl = Texto(e.Complemento), xBairro = e.Bairro,
                        cMun = e.CodigoMunicipio, xMun = e.Municipio, UF = Uf(e.Uf), CEP = Digitos(e.Cep).PadLeft(8, '0'),
                        cPais = (PaisEmitenteDCe)e.CodigoPais, xPais = (NomePaisEmitenteDCe)Enum.Parse(typeof(NomePaisEmitenteDCe),e.Pais), fone = Documento(e.Telefone)
                    }
                },
                Transportadora = dc.TipoEmitente == 3 ? new Transportadora { CNPJ = Digitos(dc.Signatario.Cnpj), xNome = dc.Signatario.Nome } : null,
                dest = new DCeDest
                {
                    CNPJ = Documento(dc.Destinatario.Cnpj), CPF = Documento(dc.Destinatario.Cpf), idOutros = Texto(dc.Destinatario.IdOutros)?.Trim(), xNome = dc.Destinatario.Nome,
                    enderDest = new enderDest
                    {
                        xLgr = d.Logradouro, nro = d.Numero, xCpl = Texto(d.Complemento), xBairro = d.Bairro,
                        cMun = d.CodigoMunicipio, xMun = d.Municipio, UF = Uf(d.Uf), CEP = Digitos(d.Cep).PadLeft(8,'0'),
                        cPais = d.CodigoPais, xPais = d.Pais, fone = Documento(d.Telefone), email = Texto(d.Email)
                    }
                },
                det = dc.Itens.Select((i,n) => new DCeDet { nItem = n+1, prod = new DCeProd { xProd=i.Descricao, NCM=i.Ncm, qCom=i.Quantidade, vUnCom=i.ValorUnitario, vProd=i.ValorTotal }, infAdProd=Texto(i.InformacoesAdicionais) }).ToList(),
                total = new DCeTotal { vDC = dc.Itens.Sum(i => i.ValorTotal) },
                transp = new DCeTransp { modTrans = (ModalidadeTransporteDCe)int.Parse(dc.Transporte.Modalidade, CultureInfo.InvariantCulture), CNPJTransp = Digitos(dc.Transporte.CnpjTransportador) },
                infAdic = string.IsNullOrWhiteSpace(dc.InformacoesComplementares) ? null : new infAdic { infCpl=dc.InformacoesComplementares }
            },
            infSolicDCe = new infSolicDCe { xSolic = dc.Solicitacao ?? string.Empty }
        };
        return documento.Preparar(dc.Signatario.Cnpj);
    }

    public eventoDCe Criar(CancelamentoDce p) => new eventoDCe
    {
        infEvento = new infEvento
        {
            cOrgao = (TCOrgaoIBGE)(int)p.CodigoOrgao, tpAmb=p.Ambiente, tpEmit=(TipoEmitenteDCe)p.TipoEmitente,
            CNPJAutor=Digitos(p.CnpjAutor), CNPJUsEmit=Documento(p.CnpjUsuarioEmitente), CPFUsEmit=Documento(p.CpfUsuarioEmitente),
            IdOutrosUsEmit=Texto(p.IdOutrosUsuarioEmitente)?.Trim(), chDCe=p.ChaveAcesso, dhEvento=Instante(p.OffsetHorario), nSeqEvento=p.SequenciaEvento,
            detEvento = new detEvento { evCancDCe = new evCancDCe { nProt=p.NumeroProtocolo, xJust=p.Justificativa } }
        }
    }.Preparar();
}
