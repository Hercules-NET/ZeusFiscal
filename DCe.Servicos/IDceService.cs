using System.Threading.Tasks;
using DCe.Classes.Servicos.Evento;
using DCe.Classes.Servicos.Consulta;
using DCe.Classes.Servicos.StatusServico;
using DCeDocumento = DCe.Classes.Informacoes.DCe;
using DCe.Classes.Retorno;
using DFe.Classes.Entidades;

namespace DCe.Servicos;

public interface IDceService
{
    Task<retDCe> AutorizarAsync(DCeDocumento declaracao);
    Task<retConsSitDCe> ConsultarAsync(string chaveAcesso);
    Task<retConsSitDCe> ConsultarAsync(consSitDCe consulta);
    Task<retConsStatServDCe> StatusServicoAsync(Estado codigoUf);
    Task<retConsStatServDCe> StatusServicoAsync(consStatServDCe consulta);
    Task<retEventoDCe> CancelarAsync(eventoDCe pedido);
}
