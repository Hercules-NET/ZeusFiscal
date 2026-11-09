using System.Globalization;
using System.Xml;
using DCe.Classes.Informacoes;
using DCe.Classes.Retorno;
using DCe.Utils.Extensoes;

namespace DCe.Dace.QuestPdf;

/// <summary>Dados de uma pessoa exatamente como aparecem no XML autorizado.</summary>
public sealed record DacePessoa(string Documento, string Nome, string Endereco, string CidadeUf);

/// <summary>Um bem ou mercadoria declarado em <c>det/prod</c>.</summary>
public sealed record DaceItem(int Numero, string Descricao, decimal Quantidade, decimal ValorUnitario, decimal ValorTotal);

/// <summary>
/// Modelo independente do desenho da DACE. A DACE deve ser criada de um <c>procDCe</c> para que
/// todo conteúdo impresso tenha origem no documento fiscal autorizado.
/// </summary>
public sealed record DaceViewModel(
    string ChaveAcesso,
    int Numero,
    int Serie,
    DateTimeOffset DataEmissao,
    string ModalidadeTransporte,
    int TipoEmissao,
    string Ambiente,
    string? Protocolo,
    DacePessoa Remetente,
    DacePessoa Destinatario,
    IReadOnlyList<DaceItem> Itens,
    decimal ValorTotal,
    string? InformacoesComplementares,
    string ObservacaoLegal1,
    string ObservacaoLegal2,
    string? UrlQrCode)
{
    public string? ParteAdicionalTipo { get; init; }
    public DacePessoa? ParteAdicional { get; init; }
    public string? InformacoesAdicionaisFisco { get; init; }
    public string ChaveFormatada => string.Join(" ", Enumerable.Range(0, 11).Select(i => ChaveAcesso.Substring(i * 4, 4)));
    public bool EmHomologacao => Ambiente == "2";
    public string CampoProtocolo => TipoEmissao == 2 ? "EMITIDA EM CONTINGÊNCIA" : Protocolo ?? string.Empty;
    public DaceViewModel SemItens() => this with { Itens = Array.Empty<DaceItem>() };
}

/// <summary>Converte o XML processado da DC-e no modelo que a DACE pode imprimir.</summary>
public static class DaceXmlParser
{
    public static DaceViewModel Parse(string procDCeXml)
    {
        if (string.IsNullOrWhiteSpace(procDCeXml))
            throw new ArgumentException("XML do procDCe vazio.", nameof(procDCeXml));

        // A verificação da raiz mantém o contrato da fachada; os dados fiscais são
        // desserializados exclusivamente pelo modelo XML compartilhado da DC-e.
        using (var reader = XmlReader.Create(new StringReader(procDCeXml)))
        {
            reader.MoveToContent();
            if (reader.LocalName != "dceProc" || reader.NamespaceURI != "http://www.portalfiscal.inf.br/dce")
                throw new InvalidOperationException("Esperado dceProc no namespace da DC-e.");
        }

        return Parse(new procDCe().CarregarDeXmlString(procDCeXml));
    }

    public static DaceViewModel Parse(procDCe processado)
    {
        if (processado is null) throw new ArgumentNullException(nameof(processado));
        var dce = processado.DCe;
        var infDce = dce?.infDCe;
        if (dce is null || !dce.infDCeSpecified || infDce is null)
            throw new InvalidOperationException("Nó infDCe não encontrado no procDCe.");
        if (!infDce.ideSpecified || infDce.ide is null)
            throw new InvalidOperationException("Nó ide não encontrado no procDCe.");
        var ide = infDce.ide;
        var protocolo = processado.protDCe?.infProt;
        var chave = protocolo?.chDCe;
        if (string.IsNullOrWhiteSpace(chave))
        {
            var id = infDce.Id ?? string.Empty;
            chave = id.StartsWith("DCe", StringComparison.OrdinalIgnoreCase) ? id.Substring(3) : id;
        }
        if (chave.Length != 44 || !chave.All(c => c >= '0' && c <= '9'))
            throw new InvalidOperationException("Chave de acesso ausente ou inválida no procDCe.");

        var ambiente = protocolo is not null && protocolo.tpAmbSpecified
            ? ((int)protocolo.tpAmb).ToString(CultureInfo.InvariantCulture)
            : ide.tpAmbSpecified ? ((int)ide.tpAmb).ToString(CultureInfo.InvariantCulture)
            : throw new InvalidOperationException("Ambiente ausente no procDCe.");
        var qrCode = dce.infDCeSupl?.qrCodDCe;
        var parte = ParseParteAdicional(infDce);

        return new DaceViewModel(
            chave,
            Obrigatorio(ide.nDC, ide.nDCSpecified, "Inteiro"),
            Obrigatorio(ide.serie, ide.serieSpecified, "Inteiro"),
            Obrigatorio(ide.dhEmi, ide.dhEmiProxySpecified, "Data"),
            Modalidade(!infDce.transpSpecified || infDce.transp?.modTransSpecified != true ? null : ((int)infDce.transp.modTrans).ToString(CultureInfo.InvariantCulture)),
            Obrigatorio((int)ide.tpEmis, ide.tpEmisSpecified, "Inteiro"),
            ambiente,
            protocolo?.nProt,
            infDce.emitSpecified ? ParsePessoa(infDce.emit) : PessoaVazia(),
            infDce.destSpecified ? ParsePessoa(infDce.dest) : PessoaVazia(),
            infDce.det.Select(ParseItem).ToList(),
            Obrigatorio(infDce.total.vDC, infDce.total.vDCProxySpecified, "Decimal"),
            infDce.infAdic?.infCpl,
            infDce.infDecSpecified && infDce.infDec?.xObs1Specified == true ? infDce.infDec.xObs1 : string.Empty,
            infDce.infDecSpecified && infDce.infDec?.xObs2Specified == true ? infDce.infDec.xObs2 : string.Empty,
            string.IsNullOrWhiteSpace(qrCode) ? null : qrCode)
        {
            ParteAdicionalTipo = parte.Tipo,
            ParteAdicional = parte.Pessoa,
            InformacoesAdicionaisFisco = infDce.infAdic?.infAdFisco
        };
    }

    private static (string? Tipo, DacePessoa? Pessoa) ParseParteAdicional(infDCe documento)
    {
        if (documento.Fisco is { } fisco)
            return ("Fisco", Parte(fisco.CNPJ, fisco.xOrgao));
        if (documento.Marketplace is { } marketplace)
            return ("Marketplace", Parte(marketplace.CNPJ, marketplace.xNome));
        if (documento.Transportadora is { } transportadora)
            return ("Transportadora", Parte(transportadora.CNPJ, transportadora.xNome));
        if (documento.ECT is { } ect)
            return ("ECT", Parte(ect.CNPJ, ect.xNome));
        return (null, null);
    }

    private static DacePessoa Parte(string documento, string nome) => new(documento, nome, string.Empty, string.Empty);
    private static DacePessoa PessoaVazia() => Parte(string.Empty, string.Empty);

    private static DaceItem ParseItem(DCeDet item) => new(
        Obrigatorio(item.nItem, item.nItemSpecified, "Inteiro"),
        item.prod.xProd,
        Obrigatorio(item.prod.qCom, item.prod.qComProxySpecified, "Decimal"),
        Obrigatorio(item.prod.vUnCom, item.prod.vUnComProxySpecified, "Decimal"),
        Obrigatorio(item.prod.vProd, item.prod.vProdProxySpecified, "Decimal"));

    private static DacePessoa ParsePessoa(DCeEmit? pessoa)
    {
        if (pessoa is null) return PessoaVazia();
        var endereco = pessoa.enderEmit;
        return Pessoa(pessoa.CNPJ ?? pessoa.CPF ?? pessoa.idOutros ?? string.Empty, pessoa.xNome,
            endereco?.xLgr, endereco?.nro, endereco?.xCpl, endereco?.xBairro, endereco?.CEP,
            endereco?.xMun, endereco?.UFSpecified == true ? endereco.UF.ToString() : null);
    }

    private static DacePessoa ParsePessoa(DCeDest? pessoa)
    {
        if (pessoa is null) return PessoaVazia();
        var endereco = pessoa.enderDest;
        return Pessoa(pessoa.CNPJ ?? pessoa.CPF ?? pessoa.idOutros ?? string.Empty, pessoa.xNome,
            endereco?.xLgr, endereco?.nro, endereco?.xCpl, endereco?.xBairro, endereco?.CEP,
            endereco?.xMun, endereco?.UFSpecified == true ? endereco.UF.ToString() : null);
    }

    private static DacePessoa Pessoa(string documento, string nome, string? logradouro, string? numero,
        string? complemento, string? bairro, string? cep, string? cidade, string? uf)
    {
        var endereco = (logradouro ?? string.Empty) + ", " + (numero ?? string.Empty) +
            (string.IsNullOrWhiteSpace(complemento) ? string.Empty : " - " + complemento) +
            (string.IsNullOrWhiteSpace(bairro) ? string.Empty : " - " + bairro) +
            (cep is { Length: > 0 } ? " - " + (cep.Length == 8 ? cep.Insert(5, "-") : cep) : string.Empty);
        return new DacePessoa(documento, nome, endereco, (cidade ?? string.Empty) + "/" + (uf ?? string.Empty));
    }

    private static T Obrigatorio<T>(T valor, bool presente, string tipo) => presente ? valor :
        throw new InvalidOperationException(tipo == "Data" ? "Data obrigatória ausente ou inválida no procDCe." :
            tipo + " obrigatório ausente ou inválido no procDCe.");

    private static string Modalidade(string? codigo) => codigo switch
    {
        "0" => "0 - TRANSPORTE PELOS CORREIOS",
        "1" => "1 - TRANSPORTE POR CONTA PRÓPRIA",
        "2" => "2 - TRANSPORTE POR EMPRESA TRANSPORTADORA",
        _ => codigo ?? string.Empty
    };
}
