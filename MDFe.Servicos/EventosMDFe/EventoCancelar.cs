using MDFe.Classes.Informacoes.Evento.Flags;
using MDFe.Classes.Retorno.MDFeEvento;
using MDFe.Servicos.Factory;
using MDFe.Utils.Configuracoes;
using MDFeEletronico = MDFe.Classes.Informacoes.MDFe;

namespace MDFe.Servicos.EventosMDFe
{
    public class EventoCancelar
    {
        public MDFeRetEventoMDFe MDFeEventoCancelar(MDFeEletronico mdfe, byte sequenciaEvento, string protocolo, string justificativa, MDFeConfiguracao cfgMdfe = null)
        {
            var config = cfgMdfe ?? MDFeConfiguracao.Instancia;
            var cancelamento = ClassesFactory.CriaEvCancMDFe(protocolo, justificativa);

            var retorno = new ServicoController().Executar(mdfe, sequenciaEvento, cancelamento, MDFeTipoEvento.Cancelamento, config);

            return retorno;
        }

        public MDFeRetEventoMDFe MDFeEventoCancelar(MDFeComandoCancelamento comando, MDFeConfiguracao cfgMdfe = null)
        {
            var config = cfgMdfe ?? MDFeConfiguracao.Instancia;
            var evento = ClassesFactory.CriaEvCancMDFe(comando.Protocolo, comando.Justificativa);
            return new ServicoController().Executar(comando, evento, MDFeTipoEvento.Cancelamento, config);
        }
    }
}