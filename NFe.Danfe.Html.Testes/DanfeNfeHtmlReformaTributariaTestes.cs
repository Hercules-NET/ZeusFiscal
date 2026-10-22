using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NFe.Danfe.Html.CrossCutting;
using NFe.Danfe.Html.Dominio;
using NFe.Danfe.Html.Testes.Fixtures;
using Xunit;

namespace NFe.Danfe.Html.Testes
{
    /// <summary>
    /// Leiaute do DANFE NF-e da NT 2026.010 (Reforma Tributária)
    /// </summary>
    public class DanfeNfeHtmlReformaTributariaTestes
    {
        private static async Task<string> GerarHtmlAsync(int quantidadeProdutos, bool comQuadrosFacultativos = true, Issqn issqn = null)
        {
            var nfe = NFeFixture.CriarNfeReformaTributaria(quantidadeProdutos, comQuadrosFacultativos);
            var danfe = new DanfeNFe(nfe, Status.Autorizada, "135261000001234 - 21/10/2026 10:31:00", "Hercules.NET",
                    LeiauteDanfe.ReformaTributaria, issqn);
            return (await new DanfeNfeHtml2(danfe).ObterDocHtmlAsync()).Html;
        }

        private static int ContarPaginas(string html)
        {
            return Regex.Matches(html, "class=\"page nfeArea rtc\"").Count;
        }

        [Fact]
        public async Task GerarHtml_ImprimeOsQuadrosDaNt2026010()
        {
            // Act
            var html = await GerarHtmlAsync(3);

            // Assert
            Assert.Contains("Total do IBS / CBS / IS", html);
            Assert.Contains("CÓDIGO DO REGIME TRIBUTÁRIO", html);
            Assert.Contains("3 - REGIME NORMAL", html);
            Assert.Contains("TIPO DE REGIME DE APURAÇÃO DO IBS E DA CBS", html);
            Assert.Contains("BASES DE CÁLCULO", html);
            Assert.Contains("ALÍQUOTAS", html);
            Assert.Contains("VALOR DOS TRIBUTOS", html);
            Assert.Contains("[NCM 27101921] [cClassTrib 000001]", html);
            Assert.Contains("CST 000", html);
        }

        [Fact]
        public async Task GerarHtml_PreencheOsTotaisDeIbsCbsIsEMonofasia()
        {
            // Act
            var html = await GerarHtmlAsync(3);

            // Assert
            Assert.Contains("VALOR DA CBS</span>\n                            <span class=\"info\">2,16</span>", html);
            Assert.Contains("VALOR DO IBS UF</span>\n                            <span class=\"info\">0,24</span>", html);
            Assert.Contains("VALOR DO IMPOSTO SELETIVO</span>\n                            <span class=\"info\">0,25</span>", html);
            Assert.Contains("VALOR DO IBS MONOFÁSICO</span>\n                            <span class=\"info\">12,30</span>", html);
            Assert.Contains("VALOR DA CBS MONOFÁSICA</span>\n                            <span class=\"info\">110,70</span>", html);
            Assert.Contains("BC DO ICMS MONOFÁSICO</span>\n                            <span class=\"info\">1.000,00</span>", html);
        }

        [Fact]
        public async Task GerarHtml_ItemComGRed_ImprimeAAliquotaEfetiva()
        {
            // Act
            var html = await GerarHtmlAsync(3);
            var item2 = Regex.Match(html, "<tr><td><div class=\"descricao\"[^>]*><strong>00002</strong>.*?</tr></table></td></tr>").Value;

            // Assert
            Assert.Contains("[cClassTrib 200003]", item2);
            Assert.Contains("<td class=\"rot\">IBS UF</td><td class=\"val\">0,04%</td>", item2);
            Assert.Contains("<td class=\"rot\">CBS</td><td class=\"val\">0,36%</td>", item2);
            Assert.DoesNotContain("0,10%", item2);
            Assert.DoesNotContain("0,90%", item2);
        }

        [Fact]
        public async Task GerarHtml_ItemSemIsOuIpi_DeixaOsCamposEmBranco()
        {
            // Act
            var html = await GerarHtmlAsync(3);
            var item1 = Regex.Match(html, "<tr><td><div class=\"descricao\"[^>]*><strong>00001</strong>.*?</tr></table></td></tr>").Value;
            var item3 = Regex.Match(html, "<tr><td><div class=\"descricao\"[^>]*><strong>00003</strong>.*?</tr></table></td></tr>").Value;

            // Assert
            Assert.Contains("<td class=\"rot\">IS</td><td class=\"val\"></td>", item1);
            Assert.Contains("<td class=\"rot\">IPI</td><td class=\"val\"></td>", item1);
            Assert.Contains("<td class=\"rot\">IS</td><td class=\"val\">0,25%</td>", item3);
            Assert.Contains("<td class=\"rot\">IPI</td><td class=\"val\">5,00</td>", item3);
        }

        [Fact]
        public async Task GerarHtml_SemInformacaoNoXml_OmiteOsQuadrosFacultativos()
        {
            // Act
            var html = await GerarHtmlAsync(3, comQuadrosFacultativos: false);

            // Assert
            Assert.DoesNotContain("Fatura / Duplicatas", html);
            Assert.DoesNotContain("VALOR DO FCP RETIDO POR ST", html);
            Assert.DoesNotContain("BC DO ICMS MONOFÁSICO", html);
            Assert.DoesNotContain("VALOR DO IBS MONOFÁSICO", html);
            Assert.DoesNotContain("Cálculo do ISSQN", html);
            Assert.DoesNotContain("Transportador / volumes transportados", html);
            Assert.Contains("Total do IBS / CBS / IS", html);
        }

        [Fact]
        public async Task GerarHtml_ComInformacaoNoXml_ImprimeOsQuadrosFacultativos()
        {
            // Act
            var html = await GerarHtmlAsync(3, issqn: new Issqn { Im = "123456", ValorTotServ = 50m, BaseCalculo = 50m, ValorIssqn = 2.5m });

            // Assert
            Assert.Contains("Fatura / Duplicatas", html);
            Assert.Contains("VALOR DO FCP RETIDO POR ST", html);
            Assert.Contains("BC DO ICMS MONOFÁSICO", html);
            Assert.Contains("VALOR DO IBS MONOFÁSICO", html);
            Assert.Contains("Cálculo do ISSQN", html);
            Assert.Contains("Transportador / volumes transportados", html);
            Assert.Contains("0 - Remetente", html);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(40)]
        public async Task GerarHtml_NaoDeixaMarcadoresDoTemplate(int quantidadeProdutos)
        {
            // Act
            var html = await GerarHtmlAsync(quantidadeProdutos);

            // Assert
            Assert.DoesNotContain("<!--[secao", html);
            Assert.DoesNotContain("<!--[/secao", html);
            var marcadores = Regex.Matches(html, @"\[[a-z_]+\]").Cast<Match>().Select(m => m.Value).Distinct().ToList();
            Assert.Empty(marcadores);
        }

        [Theory]
        //todos os quadros facultativos: 3 itens na primeira folha e 17 nas demais
        [InlineData(1, true, 1)]
        [InlineData(3, true, 1)]
        [InlineData(4, true, 2)]
        [InlineData(20, true, 2)]
        [InlineData(21, true, 3)]
        [InlineData(40, true, 4)]
        //sem quadros facultativos: 9 itens na primeira folha e 17 nas demais
        [InlineData(9, false, 1)]
        [InlineData(10, false, 2)]
        [InlineData(26, false, 2)]
        [InlineData(27, false, 3)]
        public async Task GerarHtml_PaginaPelaCapacidadeDeCadaFolha(int quantidadeProdutos, bool comQuadrosFacultativos, int paginasEsperadas)
        {
            // Arrange
            var issqn = comQuadrosFacultativos ? new Issqn { Im = "123456", ValorTotServ = 50m, BaseCalculo = 50m, ValorIssqn = 2.5m } : null;

            // Act
            var html = await GerarHtmlAsync(quantidadeProdutos, comQuadrosFacultativos, issqn);

            // Assert
            Assert.Equal(paginasEsperadas, ContarPaginas(html));
            Assert.Contains($"<span>{paginasEsperadas}</span>", html);
            for (var i = 1; i <= quantidadeProdutos; i++)
                Assert.Contains($"<strong>{i:00000}</strong>", html);
        }

        [Fact]
        public async Task GerarHtml_EscapaTextoDoItem()
        {
            // Arrange
            var nfe = NFeFixture.CriarNfeReformaTributaria(1);
            nfe.infNFe.det[0].prod.xProd = "Oleo <15W40> & aditivo";
            var danfe = new DanfeNFe(nfe, Status.Autorizada, "1", "", LeiauteDanfe.ReformaTributaria);

            // Act
            var html = (await new DanfeNfeHtml2(danfe).ObterDocHtmlAsync()).Html;

            // Assert
            Assert.Contains("Oleo &lt;15W40&gt; &amp; aditivo", html);
        }
    }
}
