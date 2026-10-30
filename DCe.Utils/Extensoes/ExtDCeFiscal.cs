using System;
using System.Globalization;
using System.Linq;
using DCe.Classes.Flags;
using DCe.Classes.Servicos.Evento;
using DCe.Utils.Ibge;
using DCeDocumento = DCe.Classes.Informacoes.DCe;

namespace DCe.Utils.Extensoes;

/// <summary>Coerência fiscal e preparação da chave a partir dos campos do modelo XML.</summary>
public static class ExtDCeFiscal
{
    /// <summary>Atualiza Id, DV e QR Code usando o offset de dhEmi e o CNPJ de quem assina.</summary>
    public static DCeDocumento Preparar(this DCeDocumento documento, string cnpjSignatario)
    {
        documento.ValidarDadosLocais();
        var inf = documento.infDCe;
        // Textos normativos obrigatórios do leiaute.
        if (!inf.infDecSpecified)
        {
            inf.infDec = new Classes.Informacoes.infDec
            {
                xObs1Specified = true,
                xObs2Specified = true
            };
        }
        var ide = inf.ide;
        var chave = DceChaveAcesso.Montar((int)ide.cUF, ide.dhEmi.UtcDateTime, cnpjSignatario,
            ide.serie, ide.nDC, (int)ide.tpEmis, (int)ide.tpEmit, ide.nSiteAutoriz,
            int.Parse(ide.cDC, CultureInfo.InvariantCulture), ide.dhEmi.Offset);
        documento.infDCe.Id = "DCe" + chave;
        ide.cDV = chave[43] - '0';
        const string url = "https://www.fazenda.pr.gov.br/dce/qrcode";
        documento.infDCeSupl = new Classes.Informacoes.infDCeSupl
        {
            qrCodDCe = url + "?chDCe=" + chave + "&tpAmb=" + ((int)ide.tpAmb).ToString(CultureInfo.InvariantCulture),
            urlChave = url
        };
        return documento;
    }

    public static eventoDCe Preparar(this eventoDCe evento)
    {
        evento.ValidarDadosLocais();
        var inf = evento.infEvento;
        inf.Id = "ID" + ((int)inf.tpEvento).ToString(CultureInfo.InvariantCulture) + inf.chDCe +
            inf.nSeqEvento.ToString("D3", CultureInfo.InvariantCulture);
        return evento;
    }

    public static void ValidarDadosLocais(this DCeDocumento documento)
    {
        if (documento == null) throw new ArgumentNullException(nameof(documento));
        var inf = documento.infDCe;
        var origem = inf.emit.enderEmit;
        var destino = inf.dest.enderDest;
        var codigoUf = UnidadesFederativas.CodigoDe(origem.UF.ToString());
        if (codigoUf == null)
            throw new ArgumentException("UF do emitente inválida: \"" + origem.UF + "\". O emitente da DC-e é sempre nacional — informe uma das 27 siglas de unidade federativa brasileira.", nameof(documento));
        if ((int)inf.ide.cUF != codigoUf)
            throw new ArgumentException($"cUF {(int)inf.ide.cUF} não corresponde à UF do emitente ({origem.UF} = {codigoUf}). A SEFAZ rejeita com cStat 220.", nameof(documento));
        ValidarMunicipio(origem.cMun, origem.UF.ToString(), "emitente", 273, false);
        ValidarMunicipio(destino.cMun, destino.UF.ToString(), "destinatário", 275, true);
        ValidarIdentificacao(inf.emit.CNPJ, inf.emit.CPF, inf.emit.idOutros, "emitente/destinatário");
        ValidarIdentificacao(inf.dest.CNPJ, inf.dest.CPF, inf.dest.idOutros, "emitente/destinatário");
    }

    private static void ValidarMunicipio(int municipio, string uf, string papel, int cStat, bool permiteExterior)
    {
        if (permiteExterior && uf == "EX")
        {
            if (municipio != 9999999)
                throw new ArgumentException($"Município do {papel} no exterior (UF \"EX\") deve ser 9999999, e não {municipio}. A SEFAZ rejeita com cStat 509.");
            return;
        }
        var codigo = UnidadesFederativas.CodigoDe(uf);
        if (codigo == null) throw new ArgumentException($"UF do {papel} inválida: \"{uf}\".");
        var prefixo = municipio / 100000;
        if (prefixo != codigo)
            throw new ArgumentException($"Município {municipio} do {papel} não pertence a {uf} — as duas primeiras posições do código IBGE são {prefixo:D2}, e {uf} é {codigo}. A SEFAZ rejeita com cStat {cStat}.");
    }

    public static void ValidarDadosLocais(this eventoDCe evento)
    {
        if (evento == null) throw new ArgumentNullException(nameof(evento));
        var inf = evento.infEvento;
        var chave = inf.chDCe ?? string.Empty;
        if (chave.Length != 44 || !chave.All(char.IsDigit))
            throw new ArgumentException($"Chave de acesso da DC-e inválida: esperados 44 dígitos, veio \"{chave}\".", nameof(evento));
        var uf = int.Parse(chave.Substring(0, 2), CultureInfo.InvariantCulture);
        if ((int)inf.cOrgao != uf)
            throw new ArgumentException($"cOrgao {(int)inf.cOrgao} não corresponde ao cUF da chave ({uf}). O cOrgao do evento é a UF da própria DC-e, e não a do autorizador.", nameof(evento));
        ValidarIdentificacao(inf.CNPJUsEmit, inf.CPFUsEmit, inf.IdOutrosUsEmit, "usuário emitente do evento");
    }

    private static void ValidarIdentificacao(string? cnpj, string? cpf, string? outros, string descricao)
    {
        var quantidade = new[] { cnpj, cpf, outros }.Count(v => !string.IsNullOrWhiteSpace(v));
        if (quantidade != 1)
            throw new InvalidOperationException($"A identificação de {descricao} exige exatamente um entre CNPJ, CPF e idOutros (informados: {quantidade}).");
    }
}
