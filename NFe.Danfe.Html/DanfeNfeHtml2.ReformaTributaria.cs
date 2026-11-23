using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using NFe.Danfe.Html.CrossCutting;
using NFe.Danfe.Html.Dominio;

namespace NFe.Danfe.Html
{
    /// <summary>
    ///     Leiaute do DANFE NF-e da NT 2026.010 (Reforma Tributária), usado quando DanfeNFe.Leiaute = ReformaTributaria
    /// </summary>
    public partial class DanfeNfeHtml2
    {
        #region Dimensões do leiaute (mm)

        //medidas aferidas no Chromium (impressão A4, mesmas opções do PDF via Playwright); cada página fica abaixo de 285 mm.
        //o item tem altura fixa (4 linhas: descrição, informações adicionais, NCM/cClassTrib e tributos), o que torna a paginação previsível
        private const decimal AlturaLinhaProdutoRtc = 10.25m;
        private const decimal AlturaCabecalhoTabelaProdutosRtc = 3.5m;

        //quadro de produtos na primeira página com todos os quadros facultativos impressos
        private const decimal AlturaQuadroProdutosPrimeiraPaginaRtc = 41m;
        private const decimal AlturaQuadroProdutosDemaisPaginasRtc = 185m;
        private const decimal AlturaDadosAdicionaisRtc = 24m;

        //espaço devolvido ao quadro de produtos quando um quadro facultativo não é impresso
        private static readonly Dictionary<string, decimal> AlturaSecoesFacultativasRtc = new Dictionary<string, decimal>
        {
            { "fatura", 8.12m },
            { "fcp", 6.44m },
            { "icms_mono", 6.44m },
            { "ibscbs_mono", 6.44m },
            { "issqn", 9.88m },
            { "transporte", 25.28m }
        };

        #endregion

        private static readonly CultureInfo CulturaBrasil = CultureInfo.GetCultureInfo("pt-BR");

        private bool LeiauteReformaTributaria => _danfe.Leiaute == LeiauteDanfe.ReformaTributaria;

        private string MontarDanfeReformaTributaria(DanfeNFe danfeNfe)
        {
            var produtos = danfeNfe.Produtos.ToList();
            var secoes = ObterSecoesFacultativasRtc(danfeNfe);

            var alturaQuadroPrimeiraPagina = AlturaQuadroProdutosPrimeiraPaginaRtc +
                                             secoes.Where(s => !s.Value).Sum(s => AlturaSecoesFacultativasRtc[s.Key]);
            var itensPrimeiraPagina = ObterCapacidadeQuadroProdutosRtc(alturaQuadroPrimeiraPagina);
            var itensDemaisPaginas = ObterCapacidadeQuadroProdutosRtc(AlturaQuadroProdutosDemaisPaginasRtc);

            var restantes = Math.Max(0, produtos.Count - itensPrimeiraPagina);
            var totalPaginas = 1 + (int)Math.Ceiling((double)restantes / itensDemaisPaginas);

            var html = MontarPaginaInicialRtc(danfeNfe, produtos.Take(itensPrimeiraPagina), secoes, alturaQuadroPrimeiraPagina,
                    totalPaginas.ToString());
            html = MontarMarcaDagua(danfeNfe, html);

            var skip = itensPrimeiraPagina;
            for (var pagina = 2; pagina <= totalPaginas; pagina++)
            {
                var proximaPagina = MontarPaginaSubsequenteRtc(danfeNfe, produtos.Skip(skip).Take(itensDemaisPaginas),
                        pagina.ToString(), totalPaginas.ToString());
                proximaPagina = MontarMarcaDagua(danfeNfe, proximaPagina);
                html = AdicionarProximaPagina(html, proximaPagina);
                skip += itensDemaisPaginas;
            }

            return FinalizarProcesso(html);
        }

        private static int ObterCapacidadeQuadroProdutosRtc(decimal alturaQuadro)
        {
            return Math.Max(1, (int)Math.Floor((alturaQuadro - AlturaCabecalhoTabelaProdutosRtc) / AlturaLinhaProdutoRtc));
        }

        /// <summary>
        ///     Quadros de exibição facultativa (NT 2026.010, item 4.4): só são impressos quando a informação existe no XML
        /// </summary>
        private static Dictionary<string, bool> ObterSecoesFacultativasRtc(DanfeNFe danfeNfe)
        {
            var imposto = danfeNfe.ImpostoReformaTributaria;
            return new Dictionary<string, bool>
            {
                { "fatura", danfeNfe.Faturas != null && danfeNfe.Faturas.Any() },
                { "fcp", new[] { imposto.vFCP, imposto.vFCPST, imposto.vICMSUFDest, imposto.vFCPUFDest }.Any(v => v > 0) },
                { "icms_mono", new[] { imposto.qBCMono, imposto.vICMSMono, imposto.qBCMonoReten, imposto.vICMSMonoReten }.Any(v => v.HasValue) },
                { "ibscbs_mono", imposto.PossuiMonofasia },
                { "issqn", danfeNfe.ISsqn != null },
                { "transporte", danfeNfe.Transportadora != null || (danfeNfe.Volumes != null && danfeNfe.Volumes.Any()) }
            };
        }

        private string MontarPaginaInicialRtc(DanfeNFe danfeNfe, IEnumerable<Produto> produtos, Dictionary<string, bool> secoes,
                decimal alturaQuadroProdutos, string qtdPaginasTotal)
        {
            var html = Resource.danfe_model_55_nfe_rtc_page_one;
            foreach (var secao in secoes)
                html = AplicarSecao(html, secao.Key, secao.Value);

            html = Substituir(html, new Dictionary<string, string>
            {
                { "[cabecalho_rtc]", MontarCabecalhoRtc(danfeNfe, "1", qtdPaginasTotal) },
                { "[produtos_rtc]", MontarQuadroProdutosRtc(produtos, alturaQuadroProdutos) },
                { "[altura_dados_adicionais]", FormatarMilimetros(AlturaDadosAdicionaisRtc) },
                { "[altura_linha_produto]", FormatarMilimetros(AlturaLinhaProdutoRtc - 0.85m) }
            });

            html = MontarCanhotoRecibo(danfeNfe, html);
            html = Substituir(html, new Dictionary<string, string> { { "[ds_company_issuer_name]", danfeNfe.Emitente.Nome } });
            html = MontarFaturasRtc(danfeNfe, html);
            html = MontarTotaisRtc(danfeNfe, html);
            html = MontarTransportadoraRtc(danfeNfe, html);
            html = MontarInformacoesAdicionais(danfeNfe, html);
            html = MontarCreditos(danfeNfe, html);
            return html;
        }

        private string MontarPaginaSubsequenteRtc(DanfeNFe danfeNfe, IEnumerable<Produto> produtos, string paginaAtual,
                string qtdPaginasTotal)
        {
            var html = Substituir(Resource.danfe_model_55_nfe_rtc_page_next, new Dictionary<string, string>
            {
                { "[cabecalho_rtc]", MontarCabecalhoRtc(danfeNfe, paginaAtual, qtdPaginasTotal) },
                { "[produtos_rtc]", MontarQuadroProdutosRtc(produtos, AlturaQuadroProdutosDemaisPaginasRtc) }
            });
            return MontarCreditos(danfeNfe, html);
        }

        private string MontarCabecalhoRtc(DanfeNFe danfeNfe, string paginaAtual, string qtdPaginasTotal)
        {
            var html = MontarCabecalho(danfeNfe, Resource.danfe_model_55_nfe_rtc_cabecalho, paginaAtual, qtdPaginasTotal);
            html = MontarDestinatarioEmitente(danfeNfe, html);
            return Substituir(html, new Dictionary<string, string> { { "[ds_company_crt]", danfeNfe.Emitente.Crt } });
        }

        private string MontarQuadroProdutosRtc(IEnumerable<Produto> produtos, decimal alturaQuadro)
        {
            var linhas = new StringBuilder();
            foreach (var produto in produtos)
                linhas.Append(MontarLinhaTabelaProdutoRtc(produto));

            return Substituir(Resource.danfe_model_55_nfe_rtc_produtos, new Dictionary<string, string>
            {
                { "[altura_quadro_produtos]", FormatarMilimetros(alturaQuadro) },
                { "[itens_products]", linhas.ToString() }
            });
        }

        /// <summary>
        ///     Linha do item no quadro "Dados dos produtos/serviços" (NT 2026.010, item 4.3)
        ///     <para>
        ///         Alíquotas de IBS UF, IBS Município e CBS: a efetiva (pAliqEfet) quando houver gRed; caso contrário, a
        ///         vigente. Campos de grupos ausentes no XML ficam em branco
        ///     </para>
        /// </summary>
        private string MontarLinhaTabelaProdutoRtc(Produto produto)
        {
            var rtc = produto.ReformaTributaria ?? new ProdutoReformaTributaria();
            var temInfAdic = !string.IsNullOrWhiteSpace(produto.InfAdic);

            var descricao = new StringBuilder();
            descricao.Append($"<div class=\"descricao\" style=\"-webkit-line-clamp: {(temInfAdic ? 2 : 3)}\">");
            descricao.Append($"<strong>{Html(produto.Codigo)}</strong> {Html(produto.Descricao)}</div>");
            if (temInfAdic)
                descricao.Append($"<span class=\"linha\">{Html(produto.InfAdic)}</span>");
            var cClassTrib = string.IsNullOrEmpty(rtc.cClassTrib) ? string.Empty : $" [cClassTrib {Html(rtc.cClassTrib)}]";
            descricao.Append($"<span class=\"linha\">[NCM {Html(produto.Ncm)}]{cClassTrib}</span>");

            var bases = MontarTabelaTributosRtc(
                    ("ICMS", FormatarValorRtc(produto.BaseCalculo)),
                    ("IBS / CBS", FormatarValorRtc(rtc.vBCIBSCBS)),
                    ("IS", FormatarValorRtc(rtc.vBCIS)),
                    ("IPI", FormatarValorRtc(rtc.vBCIPI)));

            var aliquotas = MontarTabelaTributosParesRtc(
                    ("ICMS", FormatarAliquotaRtc(produto.PIcms), "CBS", FormatarAliquotaRtc(rtc.pCBS)),
                    ("IBS UF", FormatarAliquotaRtc(rtc.pIBSUF), "IPI", FormatarAliquotaRtc(produto.PIpi)),
                    ("IBS MUN", FormatarAliquotaRtc(rtc.pIBSMun), "IS", FormatarAliquotaRtc(rtc.pIS)));

            var valores = MontarTabelaTributosParesRtc(
                    ("ICMS", FormatarValorRtc(produto.VIcms), "CBS", FormatarValorRtc(rtc.vCBS)),
                    ("IBS UF", FormatarValorRtc(rtc.vIBSUF), "IPI", FormatarValorRtc(produto.ValorIpi)),
                    ("IBS MUN", FormatarValorRtc(rtc.vIBSMun), "IS", FormatarValorRtc(rtc.vIS)));

            var sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append($"<td>{descricao}</td>");
            sb.Append($"<td><span class=\"linha\">CST {Html(rtc.CstIcms ?? produto.Origem)}</span><span class=\"linha\">CFOP {Html(produto.Cfop)}</span></td>");
            sb.Append($"<td class=\"num\"><span class=\"linha\">{FormatarQuantidadeRtc(produto.Quantidade)}</span><span class=\"linha\">[{Html(produto.Unidade)}]</span></td>");
            sb.Append($"<td class=\"num\">{FormatarValorUnitarioRtc(produto.ValorUnitario)}</td>");
            sb.Append($"<td class=\"num\">{FormatarValorRtc(produto.ValorTotal)}</td>");
            sb.Append($"<td>{bases}</td>");
            sb.Append($"<td>{aliquotas}</td>");
            sb.Append($"<td>{valores}</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        private static string MontarTabelaTributosRtc(params (string rotulo, string valor)[] linhas)
        {
            var sb = new StringBuilder("<table class=\"tributos\">");
            foreach (var (rotulo, valor) in linhas)
                sb.Append($"<tr><td class=\"rot\">{rotulo}</td><td class=\"val\">{valor}</td></tr>");
            sb.Append("</table>");
            return sb.ToString();
        }

        private static string MontarTabelaTributosParesRtc(params (string rotulo1, string valor1, string rotulo2, string valor2)[] linhas)
        {
            var sb = new StringBuilder("<table class=\"tributos\"><colgroup><col style=\"width: 30%\" /><col /><col class=\"sep\" /><col style=\"width: 15%\" /><col /></colgroup>");
            foreach (var (rotulo1, valor1, rotulo2, valor2) in linhas)
                sb.Append($"<tr><td class=\"rot\">{rotulo1}</td><td class=\"val\">{valor1}</td><td class=\"sep\">/</td><td class=\"rot\">{rotulo2}</td><td class=\"val\">{valor2}</td></tr>");
            sb.Append("</table>");
            return sb.ToString();
        }

        private string MontarFaturasRtc(DanfeNFe danfeNfe, string nfeHtml)
        {
            var faturas = (danfeNfe.Faturas ?? new List<Fatura>())
                    .Select(f => $"<strong>Nº {Html(f.NumFatura)}</strong> &nbsp;Venc. {f.DataVencimento:dd/MM/yyyy} &nbsp;Valor {FormatarValorRtc(f.Valor)}");
            return Substituir(nfeHtml, new Dictionary<string, string> { { "[duplicates]", string.Join(" &nbsp;|&nbsp; ", faturas) } });
        }

        private string MontarTotaisRtc(DanfeNFe danfeNfe, string nfeHtml)
        {
            var imposto = danfeNfe.Imposto;
            var rtc = danfeNfe.ImpostoReformaTributaria;
            var issqn = danfeNfe.ISsqn;
            return Substituir(nfeHtml, new Dictionary<string, string>
            {
                { "[vl_total_prod]", FormatarValorRtc(imposto.vProd) },
                { "[vl_shipping]", FormatarValorRtc(imposto.vFrete) },
                { "[vl_insurance]", FormatarValorRtc(imposto.vSeg) },
                { "[vl_discount]", FormatarValorRtc(imposto.vDesc) },
                { "[vl_other_expense]", FormatarValorRtc(imposto.vOutro) },
                { "[vl_total]", FormatarValorRtc(imposto.vNF) },
                { "[tot_bc_icms]", FormatarValorRtc(imposto.vBCIMS) },
                { "[tot_icms]", FormatarValorRtc(imposto.vICMS) },
                { "[tot_bc_icms_st]", FormatarValorRtc(imposto.vBCICMSST ?? 0) },
                { "[tot_icms_st]", FormatarValorRtc(imposto.vICMSST ?? 0) },
                { "[tot_total_ipi_tax]", FormatarValorRtc(imposto.vIpi) },
                { "[tot_fcp]", FormatarValorRtc(rtc.vFCP) },
                { "[tot_fcp_st]", FormatarValorRtc(rtc.vFCPST) },
                { "[tot_icms_uf_dest]", FormatarValorRtc(rtc.vICMSUFDest) },
                { "[tot_fcp_uf_dest]", FormatarValorRtc(rtc.vFCPUFDest) },
                { "[tot_qbc_mono]", FormatarQuantidadeRtc(rtc.qBCMono) },
                { "[tot_icms_mono]", FormatarValorRtc(rtc.vICMSMono) },
                { "[tot_qbc_mono_reten]", FormatarQuantidadeRtc(rtc.qBCMonoReten) },
                { "[tot_icms_mono_reten]", FormatarValorRtc(rtc.vICMSMonoReten) },
                { "[tot_cbs]", FormatarValorRtc(rtc.vCBS) },
                { "[tot_ibs_uf]", FormatarValorRtc(rtc.vIBSUF) },
                { "[tot_ibs_mun]", FormatarValorRtc(rtc.vIBSMun) },
                { "[tot_is]", FormatarValorRtc(rtc.vIS) },
                { "[tot_ibs_mono]", FormatarValorRtc(rtc.vIBSMono) },
                { "[tot_cbs_mono]", FormatarValorRtc(rtc.vCBSMono) },
                { "[tot_ibs_mono_reten]", FormatarValorRtc(rtc.vIBSMonoReten) },
                { "[tot_cbs_mono_reten]", FormatarValorRtc(rtc.vCBSMonoReten) },
                { "[ds_company_im]", Html(issqn?.Im) },
                { "[vl_total_serv]", FormatarValorRtc(issqn?.ValorTotServ) },
                { "[tot_bc_issqn]", FormatarValorRtc(issqn?.BaseCalculo) },
                { "[tot_issqn]", FormatarValorRtc(issqn?.ValorIssqn) }
            });
        }

        private string MontarTransportadoraRtc(DanfeNFe danfeNfe, string nfeHtml)
        {
            var transportadora = danfeNfe.Transportadora;
            var volume = danfeNfe.Volumes?.FirstOrDefault();
            return Substituir(nfeHtml, new Dictionary<string, string>
            {
                { "[ds_transport_carrier_name]", Html(transportadora?.Nome) },
                { "[ds_transport_shipping_type]", Html(danfeNfe.ModalidadeFreteDescricao) },
                { "[ds_transport_rntc]", Html(transportadora?.CodAntt) },
                { "[ds_transport_vehicle_plate]", Html(transportadora?.Placa) },
                { "[ds_transport_vehicle_uf]", string.IsNullOrEmpty(transportadora?.Placa) ? string.Empty : Html(transportadora?.Endereco?.Uf) },
                { "[nl_transport_cnpj_cpf]", CrossCutting.Utils.FormatarCnpjCpf(transportadora?.CnpjCpf) },
                { "[ds_transport_address]", Html($"{transportadora?.Endereco?.NomeEnd} {transportadora?.Endereco?.Num}".Trim()) },
                { "[ds_transport_city]", Html(transportadora?.Endereco?.Cidade) },
                { "[ds_transport_uf]", Html(transportadora?.Endereco?.Uf) },
                { "[ds_transport_ie]", Html(transportadora?.Ie) },
                { "[nu_transport_amount_transported_volumes]", volume?.Quantidade?.ToString("#,##0", CulturaBrasil) },
                { "[ds_transport_type_volumes_transported]", Html(volume?.Especie) },
                { "[ds_transport_mark_volumes_transported]", Html(volume?.Marca) },
                { "[ds_transport_number_volumes_transported]", Html(volume?.NumeroVolume) },
                { "[vl_transport_gross_weight]", volume == null ? string.Empty : $"{volume.PesoBruto.ToString("#,##0.000", CulturaBrasil)} kg" },
                { "[vl_transport_net_weight]", volume == null ? string.Empty : $"{volume.PesoLiq.ToString("#,##0.000", CulturaBrasil)} kg" }
            });
        }

        /// <summary>
        ///     Mantém ou remove um trecho do template delimitado por &lt;!--[secao:nome]--&gt; e &lt;!--[/secao:nome]--&gt;
        /// </summary>
        private static string AplicarSecao(string html, string secao, bool exibir)
        {
            var inicio = $"<!--[secao:{secao}]-->";
            var fim = $"<!--[/secao:{secao}]-->";
            var posInicio = html.IndexOf(inicio, StringComparison.Ordinal);
            var posFim = html.IndexOf(fim, StringComparison.Ordinal);
            if (posInicio < 0 || posFim < posInicio) return html;

            return exibir
                ? html.Remove(posFim, fim.Length).Remove(posInicio, inicio.Length)
                : html.Remove(posInicio, posFim + fim.Length - posInicio);
        }

        private static string Html(string texto)
        {
            return string.IsNullOrEmpty(texto) ? string.Empty : WebUtility.HtmlEncode(texto);
        }

        private static string FormatarValorRtc(decimal valor)
        {
            return valor.ToString("N2", CulturaBrasil);
        }

        private static string FormatarValorRtc(decimal? valor)
        {
            return valor.HasValue ? FormatarValorRtc(valor.Value) : string.Empty;
        }

        private static string FormatarValorUnitarioRtc(decimal valor)
        {
            return valor.ToString("#,##0.00########", CulturaBrasil);
        }

        private static string FormatarQuantidadeRtc(decimal? valor)
        {
            return valor?.ToString("#,##0.00##", CulturaBrasil) ?? string.Empty;
        }

        private static string FormatarAliquotaRtc(decimal? valor)
        {
            return valor.HasValue ? $"{valor.Value.ToString("0.00##", CulturaBrasil)}%" : string.Empty;
        }

        private static string FormatarMilimetros(decimal valor)
        {
            return valor.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
