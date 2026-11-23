using System.Collections.Generic;
using System.Linq;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;

namespace NFe.Danfe.Html.Testes.Fixtures
{
    /// <summary>
    /// NF-e modelo 55 com os grupos da Reforma Tributária (IBS/CBS/IS) usados pelo DANFE da NT 2026.010
    /// </summary>
    public static partial class NFeFixture
    {
        /// <param name="quantidadeProdutos">Quantidade de itens</param>
        /// <param name="comQuadrosFacultativos">
        ///     true preenche fatura, FCP, ICMS monofásico, IBS/CBS monofásico e transporte; false omite todos
        /// </param>
        public static Classes.NFe CriarNfeReformaTributaria(int quantidadeProdutos, bool comQuadrosFacultativos = true)
        {
            var nfe = CriarNfe(quantidadeProdutos);
            var infNFe = nfe.infNFe;

            foreach (var det in infNFe.det)
            {
                var item = det.nItem;
                det.imposto.IBSCBS = new IBSCBS
                {
                    CST = CSTIBSCBS.cst000,
                    cClassTrib = "000001",
                    gIBSCBS = new gIBSCBS
                    {
                        vBC = 100m,
                        gIBSUF = new gIBSUF { pIBSUF = 0.1m, vIBSUF = 0.1m },
                        gIBSMun = new gIBSMun { pIBSMun = 0m, vIBSMun = 0m },
                        vIBS = 0.1m,
                        gCBS = new gCBS { pCBS = 0.9m, vCBS = 0.9m }
                    }
                };

                //item 2: redução de alíquota (gRed) - o DANFE deve imprimir a alíquota efetiva
                if (item == 2)
                {
                    var g = det.imposto.IBSCBS.gIBSCBS;
                    det.imposto.IBSCBS.CST = CSTIBSCBS.cst200;
                    det.imposto.IBSCBS.cClassTrib = "200003";
                    g.gIBSUF = new gIBSUF { pIBSUF = 0.1m, gRed = new gRed { pRedAliq = 60m, pAliqEfet = 0.04m }, vIBSUF = 0.04m };
                    g.gIBSMun = new gIBSMun { pIBSMun = 0m, gRed = new gRed { pRedAliq = 60m, pAliqEfet = 0m }, vIBSMun = 0m };
                    g.gCBS = new gCBS { pCBS = 0.9m, gRed = new gRed { pRedAliq = 60m, pAliqEfet = 0.36m }, vCBS = 0.36m };
                    g.vIBS = 0.04m;
                }

                //item 3: Imposto Seletivo
                if (item == 3)
                {
                    det.imposto.IS = new IS
                    {
                        CSTIS = "000",
                        cClassTribIS = "000001",
                        vBCIS = 100m,
                        pIS = 0.25m,
                        vIS = 0.25m
                    };
                }
            }

            var dets = infNFe.det;
            infNFe.total.IBSCBSTot = new IBSCBSTot
            {
                vBCIBSCBS = dets.Sum(d => d.imposto.IBSCBS.gIBSCBS.vBC),
                gIBS = new gIBSTotal
                {
                    gIBSUF = new gIBSUFTotal { vIBSUF = dets.Sum(d => d.imposto.IBSCBS.gIBSCBS.gIBSUF.vIBSUF) },
                    gIBSMun = new gIBSMunTotal { vIBSMun = 0m },
                    vIBS = dets.Sum(d => d.imposto.IBSCBS.gIBSCBS.gIBSUF.vIBSUF)
                },
                gCBS = new gCBSTotal { vCBS = dets.Sum(d => d.imposto.IBSCBS.gIBSCBS.gCBS.vCBS) }
            };
            infNFe.total.ISTot = new ISTot { vIS = dets.Where(d => d.imposto.IS != null).Sum(d => d.imposto.IS.vIS) };

            if (comQuadrosFacultativos)
            {
                infNFe.total.ICMSTot.vFCP = 2m;
                infNFe.total.ICMSTot.vFCPST = 0m;
                infNFe.total.ICMSTot.qBCMono = 1000m;
                infNFe.total.ICMSTot.vICMSMono = 1470m;
                infNFe.total.IBSCBSTot.gMono = new gMonoTotal
                {
                    vIBSMono = 12.3m,
                    vCBSMono = 110.7m,
                    vIBSMonoReten = 0m,
                    vCBSMonoReten = 0m
                };
            }
            else
            {
                infNFe.cobr = null;
                infNFe.transp = new transp { modFrete = ModalidadeFrete.mfSemFrete };
            }

            return nfe;
        }
    }
}
