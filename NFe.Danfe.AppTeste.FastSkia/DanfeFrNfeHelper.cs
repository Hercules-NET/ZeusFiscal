using DFe.Classes.Flags;
using NFe.Classes;
using NFe.Danfe.Base.NFe;
using NFe.Danfe.Fast.Skia.NFe;
using NFe.Utils.NFe;

namespace NFe.Danfe.AppTeste.FastSkia
{
    public static class DanfeFrNfeHelper
    {
        public static DanfeFrNfe GeraClasseDanfeFrNFe(string xml)
        {
            try
            {
                nfeProc proc = null;
                try
                {
                    proc = new nfeProc().CarregarDeXmlString(xml);
                }
                catch //Carregar NFe ainda não transmitida à sefaz, como uma pré-visualização.
                {
                    proc = new nfeProc() { NFe = new Classes.NFe().CarregarDeXmlString(xml), protNFe = new Classes.Protocolo.protNFe() };
                }

                if (proc.NFe.infNFe.ide.mod != ModeloDocumento.NFe)
                {
                    throw new Exception("O XML informado não é um NFe!");
                }

                

                var caminhoFrx = @"C:\Caminho\Para\Danfe.frx";


                DanfeFrNfe danfe = new DanfeFrNfe(proc: proc, configuracaoDanfeNfe: new ConfiguracaoDanfeNfe()
                {
                    Logomarca = null,
                    DuasLinhas = false,
                    DocumentoCancelado = false,
                    QuebrarLinhasObservacao = false,
                    ExibirResumoCanhoto = false,
                    ExibeCampoFatura = false,
                    ImprimirISSQN = false,
                    ImprimirDescPorc = false,
                    ImprimirTotalLiquido = false,
                    ExibirTotalTributos = false,
                    ExibeRetencoes = false
                },
                desenvolvedor: "NOME DA SOFTWARE HOUSE",
                arquivoRelatorio: @"C:\Users\danil\source\repos\ZeusFiscal\NFe.Danfe.Base\NFe\NFeRetrato.frx");

                return danfe;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
