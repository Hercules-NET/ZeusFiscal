using System.ComponentModel;
using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public enum TipocCredPres
    {
        [Description("Sem Crédito Presumido")]
        [XmlEnum("00")]
        SemCreditoPresumido = 0,

        [Description("Crédito presumido da aquisição de bens e serviços de produtor rural não contribuinte.")]
        [XmlEnum("01")]
        AquisicaoProdutorRuralNaoContribuinte = 1,

        [Description("Crédito presumido da aquisição de serviço de transporte de TAC pessoa física não contribuinte.")]
        [XmlEnum("02")]
        TransporteTacPessoaFisicaNaoContribuinte = 2,

        [Description("Crédito presumido da aquisição de resíduos e desperdícios destinados à reciclagem.")]
        [XmlEnum("03")]
        AquisicaoResiduosReciclagem = 3,

        [Description("Crédito presumido da aquisição de bens móveis usados de pessoa física não contribuinte para revenda.")]
        [XmlEnum("04")]
        AquisicaoBensMoveisUsadosPfParaRevenda = 4,

        [Description("Crédito presumido no regime automotivo – vendas a consumidor final (com incidência de CBS).")]
        [XmlEnum("05")]
        RegimeAutomotivoVendaConsumidorFinal = 5,

        [Description("Crédito presumido no regime automotivo – vendas para contribuinte (com incidência de CBS).")]
        [XmlEnum("06")]
        RegimeAutomotivoVendaContribuinte = 6,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços tributados pelo IBS.")]
        [XmlEnum("07")]
        AquisicaoContribuinteTributadaIbs = 7,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços com alíquota zero.")]
        [XmlEnum("08")]
        AquisicaoContribuinteAliquotaZero = 8,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços isentos.")]
        [XmlEnum("09")]
        AquisicaoContribuinteIsenta = 9,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços tributados pela CBS.")]
        [XmlEnum("10")]
        AquisicaoContribuinteTributadaCbs = 10,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços com incidência de IBS.")]
        [XmlEnum("11")]
        AquisicaoContribuinteComIncidenciaIbs = 11,

        [Description("Crédito presumido na aquisição por contribuinte de bens e serviços com alíquota zero (IBS).")]
        [XmlEnum("12")]
        AquisicaoContribuinteAliquotaZeroIbs = 12,

        [Description("Crédito presumido na aquisição pela indústria de insumos específicos, conforme legislação.")]
        [XmlEnum("13")]
        AquisicaoIndustriaInsumosEspecificos = 13,
    }
}