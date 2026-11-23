using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NFe.Danfe.Html.CrossCutting;
using NFe.Danfe.Html.Dominio;
using NFe.Danfe.Html.Testes.Fixtures;
using Xunit;

namespace NFe.Danfe.Html.Testes
{
    /// <summary>
    /// Garante que o leiaute atual continua idêntico ao gerado antes da NT 2026.010.
    /// Os arquivos em Fixtures/Golden foram gerados a partir do master anterior a esta alteração, com a cultura pt-BR.
    /// </summary>
    public class DanfeNfeHtmlLeiauteAtualTestes
    {
        private const string Protocolo = "135261000001234 - 21/10/2026 10:31:00";
        private const string Creditos = "Hercules.NET";

        public DanfeNfeHtmlLeiauteAtualTestes()
        {
            //a formatação do leiaute atual depende da cultura corrente
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        }

        private static string LerGolden(int quantidadeProdutos)
        {
            return File.ReadAllText(Path.Combine("Fixtures", "Golden", $"danfe-leiaute-atual-{quantidadeProdutos}-itens.html"));
        }

        private static async Task<string> GerarHtmlAsync(DanfeNFe danfe)
        {
            return (await new DanfeNfeHtml2(danfe).ObterDocHtmlAsync()).Html;
        }

        [Theory]
        [InlineData(3)]
        [InlineData(40)]
        public async Task ConstrutorAnterior_GeraHtmlIdenticoAoLeiauteAtual(int quantidadeProdutos)
        {
            // Arrange
            var danfe = new DanfeNFe(NFeFixture.CriarNfe(quantidadeProdutos), Status.Autorizada, Protocolo, Creditos);

            // Act
            var html = await GerarHtmlAsync(danfe);

            // Assert
            Assert.Equal(LeiauteDanfe.Atual, danfe.Leiaute);
            Assert.Equal(LerGolden(quantidadeProdutos), html);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(40)]
        public async Task LeiauteAtualExplicito_GeraHtmlIdenticoAoLeiauteAtual(int quantidadeProdutos)
        {
            // Arrange
            var danfe = new DanfeNFe(NFeFixture.CriarNfe(quantidadeProdutos), Status.Autorizada, Protocolo, Creditos, LeiauteDanfe.Atual);

            // Act
            var html = await GerarHtmlAsync(danfe);

            // Assert
            Assert.Equal(LerGolden(quantidadeProdutos), html);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(40)]
        public async Task LeiauteAtual_ComXmlDaReformaTributaria_IgnoraOsGruposNovos(int quantidadeProdutos)
        {
            // Arrange
            var nfe = NFeFixture.CriarNfeReformaTributaria(quantidadeProdutos, comQuadrosFacultativos: true);
            var danfe = new DanfeNFe(nfe, Status.Autorizada, Protocolo, Creditos);

            // Act
            var html = await GerarHtmlAsync(danfe);

            // Assert
            Assert.Equal(LerGolden(quantidadeProdutos), html);
        }
    }
}
