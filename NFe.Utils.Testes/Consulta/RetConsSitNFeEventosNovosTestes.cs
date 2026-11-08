using DFe.Utils;
using NFe.Classes.Servicos.Consulta;
using NFe.Classes.Servicos.Tipos;
using NFe.Utils.Consulta;
using NFe.Utils.Evento;
using Xunit;

namespace NFe.Utils.Testes.Consulta
{
    /// <summary>
    /// Eventos com XSD em NFe.AppTeste/Schemas que não existiam no enum <see cref="NFeTipoEvento"/>
    /// (211120 - Destinação de item para consumo pessoal, 112150 - Atualização da Data de Previsão de Entrega
    /// e 110150 - Ator interessado na NF-e)
    /// </summary>
    public class RetConsSitNFeEventosNovosTestes
    {
        [Theory]
        [InlineData("211120", "Destinação de item para consumo pessoal", NFeTipoEvento.TeNfeDestinacaoConsumoPessoalNFe)]
        [InlineData("112150", "Atualização da Data de Previsão de Entrega", NFeTipoEvento.TeNfeAtualizacaoDataPrevisaoEntregaNFe)]
        [InlineData("110150", "Ator interessado na NF-e", NFeTipoEvento.TeNfeAtorInteressadoNFe)]
        public void CarregarDeXmlString_ComEventoNovo_DeserializaTipoEvento(string tpEvento, string descEvento, NFeTipoEvento esperado)
        {
            // Arrange
            var xml = RetConsSitNFeEventosCTeTestes.MontarRetConsSitNFe(tpEvento, descEvento);

            // Act
            var retorno = new retConsSitNFe().CarregarDeXmlString(xml);

            // Assert
            var procEvento = Assert.Single(retorno.procEventoNFe);
            Assert.Equal(esperado, procEvento.evento.infEvento.tpEvento);
            Assert.Equal(esperado, procEvento.retEvento.infEvento.tpEvento);
            Assert.Equal(descEvento, procEvento.retEvento.infEvento.xEvento);
        }

        [Theory]
        [InlineData(NFeTipoEvento.TeNfeDestinacaoConsumoPessoalNFe, "Destinação de item para consumo pessoal")]
        [InlineData(NFeTipoEvento.TeNfeAtualizacaoDataPrevisaoEntregaNFe, "Atualização da Data de Previsão de Entrega")]
        [InlineData(NFeTipoEvento.TeNfeAtorInteressadoNFe, "Ator interessado na NF-e")]
        public void RemoverAcentosPreservandoDescEvento_ComEventoNovo_MantemDescricaoAceitaPeloXsd(NFeTipoEvento tipoEvento, string descEventoXsd)
        {
            // Arrange
            var xml = "<detEvento versao=\"1.00\"><descEvento>" + tipoEvento.Descricao() + "</descEvento></detEvento>";

            // Act
            var resultado = xml.RemoverAcentosPreservandoDescEvento();

            // Assert
            Assert.Equal("<detEvento versao=\"1.00\"><descEvento>" + descEventoXsd + "</descEvento></detEvento>", resultado);
        }
    }
}
