using NFe.Classes.Servicos.Consulta;
using NFe.Classes.Servicos.Tipos;
using NFe.Utils.Consulta;
using Xunit;

namespace NFe.Utils.Testes.Consulta
{
    /// <summary>
    /// Eventos registrados pelo Ambiente Nacional na NF-e quando um MDF-e que a referencia é autorizado/cancelado
    /// (610610 - MDF-e Autorizado e 610611 - MDF-e Cancelado)
    /// </summary>
    public class RetConsSitNFeEventosMDFeTestes
    {
        [Theory]
        [InlineData("610610", "MDF-e Autorizado", NFeTipoEvento.TeNfeMDFeAutorizado)]
        [InlineData("610611", "MDF-e Cancelado", NFeTipoEvento.TeNfeMDFeCancelado)]
        public void CarregarDeXmlString_ComEventoDeMDFeVinculado_DeserializaTipoEvento(string tpEvento, string descEvento, NFeTipoEvento esperado)
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
    }
}