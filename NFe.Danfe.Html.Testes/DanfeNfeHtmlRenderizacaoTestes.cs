using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NFe.Danfe.Html.CrossCutting;
using NFe.Danfe.Html.Dominio;
using NFe.Danfe.Html.Testes.Fixtures;
using Xunit;

namespace NFe.Danfe.Html.Testes
{
    /// <summary>
    /// Renderiza o DANFE no Chromium (impressão A4, mesmas opções usadas para gerar o PDF via Playwright) e confere que
    /// cada página lógica cabe em uma folha. Salva os PDFs em artifacts/danfe-html para conferência visual.
    /// <para>
    /// Só executa com RUN_PLAYWRIGHT_DANFE_TESTS=1. O Chromium vem do "playwright install chromium" ou do caminho em
    /// PLAYWRIGHT_CHROMIUM_EXECUTABLE.
    /// </para>
    /// </summary>
    public class DanfeNfeHtmlRenderizacaoTestes
    {
        //A4 tem 297 mm; a folga cobre a variação de renderização entre plataformas
        private const double AlturaMaximaPaginaMm = 290;

        private static bool Habilitado =>
            string.Equals(Environment.GetEnvironmentVariable("RUN_PLAYWRIGHT_DANFE_TESTS"), "1", StringComparison.Ordinal);

        [Theory]
        [InlineData(LeiauteDanfe.Atual, 3, true)]
        [InlineData(LeiauteDanfe.Atual, 40, true)]
        [InlineData(LeiauteDanfe.ReformaTributaria, 3, true)]
        [InlineData(LeiauteDanfe.ReformaTributaria, 40, true)]
        [InlineData(LeiauteDanfe.ReformaTributaria, 9, false)]
        [InlineData(LeiauteDanfe.ReformaTributaria, 27, false)]
        public async Task Renderizar_CadaPaginaCabeEmUmaFolhaA4(LeiauteDanfe leiaute, int quantidadeProdutos, bool comQuadrosFacultativos)
        {
            if (!Habilitado) return;

            // Arrange
            var nfe = NFeFixture.CriarNfeReformaTributaria(quantidadeProdutos, comQuadrosFacultativos);
            var issqn = comQuadrosFacultativos ? new Issqn { Im = "123456", ValorTotServ = 50m, BaseCalculo = 50m, ValorIssqn = 2.5m } : null;
            var danfe = new DanfeNFe(nfe, Status.Autorizada, "135261000001234 - 21/10/2026 10:31:00", "Hercules.NET", leiaute, issqn);
            var html = (await new DanfeNfeHtml2(danfe).ObterDocHtmlAsync()).Html;

            using var playwright = await Playwright.CreateAsync();
            var executavel = Environment.GetEnvironmentVariable("PLAYWRIGHT_CHROMIUM_EXECUTABLE");
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                ExecutablePath = string.IsNullOrWhiteSpace(executavel) ? null : executavel
            });
            var page = await browser.NewPageAsync();
            await page.EmulateMediaAsync(new PageEmulateMediaOptions { Media = Media.Print });

            // Act
            await page.SetContentAsync(html, new PageSetContentOptions { WaitUntil = WaitUntilState.Load });
            var alturas = JsonSerializer.Deserialize<double[]>(await page.EvaluateAsync<string>(
                    "() => JSON.stringify([...document.querySelectorAll('.boxFields')].map(b => b.getBoundingClientRect().height * 25.4 / 96))"));

            var pasta = Path.Combine(AppContext.BaseDirectory, "artifacts", "danfe-html");
            Directory.CreateDirectory(pasta);
            var nome = $"danfe-{leiaute}-{quantidadeProdutos}-itens{(comQuadrosFacultativos ? "" : "-sem-facultativos")}";
            await File.WriteAllTextAsync(Path.Combine(pasta, nome + ".html"), html);
            await page.PdfAsync(new PagePdfOptions
            {
                Path = Path.Combine(pasta, nome + ".pdf"),
                Format = "A4",
                PrintBackground = true,
                PreferCSSPageSize = true
            });

            // Assert
            Assert.NotEmpty(alturas);
            Assert.All(alturas, altura => Assert.True(altura <= AlturaMaximaPaginaMm,
                    $"Página com {altura:0.0} mm não cabe na folha A4 ({string.Join("; ", alturas.Select(a => a.ToString("0.0")))})"));
        }
    }
}
