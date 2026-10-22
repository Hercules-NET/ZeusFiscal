using System;
using System.Collections.Generic;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Cobranca;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Observacoes;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;

namespace NFe.Danfe.Html.Testes.Fixtures
{
    /// <summary>
    /// NF-e modelo 55 com os campos do leiaute anterior à Reforma Tributária (sem IBS/CBS/IS)
    /// </summary>
    public static partial class NFeFixture
    {
        public const string Chave = "35261012345678000199550010000012341000012340";

        public static Classes.NFe CriarNfe(int quantidadeProdutos)
        {
            var det = new List<det>();
            for (var i = 1; i <= quantidadeProdutos; i++)
                det.Add(CriarItem(i));

            var vProd = quantidadeProdutos * 100m;
            var vIcms = quantidadeProdutos * 18m;
            var vIpi = Math.Ceiling(quantidadeProdutos / 3m) * 5m;

            return new Classes.NFe
            {
                infNFe = new infNFe
                {
                    versao = "4.00",
                    Id = "NFe" + Chave,
                    ide = new ide
                    {
                        cUF = Estado.SP,
                        cNF = "00001234",
                        natOp = "VENDA DE MERCADORIA ADQUIRIDA OU RECEBIDA DE TERCEIROS",
                        mod = ModeloDocumento.NFe,
                        serie = 1,
                        nNF = 1234,
                        dhEmi = new DateTimeOffset(2026, 10, 21, 10, 30, 0, TimeSpan.FromHours(-3)),
                        dhSaiEnt = new DateTimeOffset(2026, 10, 21, 11, 0, 0, TimeSpan.FromHours(-3)),
                        tpNF = TipoNFe.tnSaida,
                        idDest = DestinoOperacao.doInterna,
                        cMunFG = 3550308,
                        tpImp = TipoImpressao.tiRetrato,
                        tpEmis = TipoEmissao.teNormal,
                        cDV = 0,
                        tpAmb = TipoAmbiente.Producao,
                        finNFe = FinalidadeNFe.fnNormal,
                        indFinal = ConsumidorFinal.cfNao,
                        indPres = PresencaComprador.pcPresencial,
                        procEmi = ProcessoEmissao.peAplicativoContribuinte,
                        verProc = "DanfeHtmlTestes"
                    },
                    emit = new emit
                    {
                        CNPJ = "12345678000199",
                        xNome = "POSTO EXEMPLO COMERCIO DE COMBUSTIVEIS LTDA",
                        xFant = "POSTO EXEMPLO",
                        IE = "110042490114",
                        IEST = "",
                        CRT = CRT.RegimeNormal,
                        enderEmit = new enderEmit
                        {
                            xLgr = "Avenida das Bombas",
                            nro = "1500",
                            xBairro = "Centro",
                            cMun = 3550308,
                            xMun = "Sao Paulo",
                            UF = Estado.SP,
                            CEP = "01001000",
                            fone = 1133334444
                        }
                    },
                    dest = new dest(VersaoServico.Versao400)
                    {
                        CNPJ = "98765432000188",
                        xNome = "Transportadora Cliente S/A",
                        IE = "110042490115",
                        indIEDest = indIEDest.ContribuinteICMS,
                        enderDest = new enderDest
                        {
                            xLgr = "Rodovia Exemplo km 10",
                            nro = "S/N",
                            xBairro = "Distrito Industrial",
                            cMun = 3550308,
                            xMun = "Sao Paulo",
                            UF = "SP",
                            CEP = "01002000",
                            fone = 1144445555
                        }
                    },
                    det = det,
                    total = new total
                    {
                        ICMSTot = new ICMSTot
                        {
                            vBC = vProd,
                            vICMS = vIcms,
                            vBCST = 0m,
                            vST = 0m,
                            vProd = vProd,
                            vFrete = 15m,
                            vSeg = 0m,
                            vDesc = 2.5m,
                            vOutro = 0m,
                            vII = 0m,
                            vIPI = vIpi,
                            vPIS = 1.65m * quantidadeProdutos,
                            vCOFINS = 7.6m * quantidadeProdutos,
                            vNF = vProd + 15m - 2.5m + vIpi,
                            vTotTrib = 0m
                        }
                    },
                    transp = new transp
                    {
                        modFrete = ModalidadeFrete.mfContaEmitenteOumfContaRemetente,
                        transporta = new transporta
                        {
                            CNPJ = "22222222000122",
                            xNome = "Transportadora Exemplo",
                            IE = "333333333",
                            xEnder = "Rod. Exemplo, km 10",
                            xMun = "Campinas",
                            UF = "SP"
                        },
                        vol = new List<vol>
                        {
                            new vol { qVol = 1, esp = "Volume", marca = "", nVol = "001", pesoB = 12.5m, pesoL = 11.9m }
                        }
                    },
                    cobr = new cobr
                    {
                        dup = new List<dup>
                        {
                            new dup { nDup = "001", dVenc = new DateTime(2026, 11, 20), vDup = 60m },
                            new dup { nDup = "002", dVenc = new DateTime(2026, 12, 20), vDup = 52.5m }
                        }
                    },
                    infAdic = new infAdic
                    {
                        infCpl = "Documento emitido para teste do leiaute do DANFE HTML."
                    }
                }
            };
        }

        private static det CriarItem(int nItem)
        {
            //itens alternam descrição curta/longa e com/sem IPI para exercitar a paginação
            var descricao = nItem % 4 == 0
                ? $"Produto teste {nItem:000} com uma descricao bem mais longa que o normal para ocupar mais de uma linha no quadro de produtos do DANFE"
                : $"Produto teste {nItem:000}";

            var item = new det
            {
                nItem = nItem,
                prod = new prod
                {
                    cProd = nItem.ToString("00000"),
                    cEAN = "SEM GTIN",
                    xProd = descricao,
                    NCM = "27101921",
                    CFOP = 5102,
                    uCom = "UN",
                    qCom = 1m,
                    vUnCom = 100m,
                    vProd = 100m,
                    cEANTrib = "SEM GTIN",
                    uTrib = "UN",
                    qTrib = 1m,
                    vUnTrib = 100m,
                    indTot = IndicadorTotal.ValorDoItemCompoeTotalNF
                },
                imposto = new imposto
                {
                    ICMS = new ICMS
                    {
                        TipoICMS = new ICMS00
                        {
                            orig = OrigemMercadoria.OmNacional,
                            CST = Csticms.Cst00,
                            modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                            vBC = 100m,
                            pICMS = 18m,
                            vICMS = 18m
                        }
                    }
                },
                infAdProd = nItem % 3 == 1 ? $"Lote L{nItem:0000}" : null
            };

            if (nItem % 3 == 0)
            {
                item.imposto.IPI = new IPI
                {
                    cEnq = 999,
                    TipoIPI = new IPITrib { CST = CSTIPI.ipi50, vBC = 100m, pIPI = 5m, vIPI = 5m }
                };
            }

            return item;
        }
    }
}
