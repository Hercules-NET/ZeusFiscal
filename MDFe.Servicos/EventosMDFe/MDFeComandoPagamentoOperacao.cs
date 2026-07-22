using MDFe.Classes.Informacoes;
using MDFe.Classes.Informacoes.Evento.CorpoEvento;
using System.Collections.Generic;

namespace MDFe.Servicos.EventosMDFe
{
    /// <summary>
    ///     Dados para emitir o evento de pagamento de operação de transporte.
    /// </summary>
    public class MDFeComandoPagamentoOperacao : MDFeComandoEvento
    {        
        public string Protocolo { get; set; }
        public infViagens InfViagens { get; set; }
        public List<infPag> Pagamentos { get; set; }
    }
}