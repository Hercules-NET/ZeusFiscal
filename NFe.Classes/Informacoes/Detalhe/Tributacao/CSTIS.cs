using System.ComponentModel;
using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public enum CSTIS
    {
        [Description("Tributação integral")]
        [XmlEnum("000")]
        Is000 = 000,

        [Description("Tributação com alíquotas uniformes")]
        [XmlEnum("010")]
        Is010 = 010,

        [Description("Tributação com alíquotas uniformes reduzidas")]
        [XmlEnum("011")]
        Is011 = 011,

        [Description("Alíquota reduzida")]
        [XmlEnum("200")]
        Is200 = 200,

        [Description("Alíquota fixa")]
        [XmlEnum("220")]
        Is220 = 220,

        [Description("Alíquota fixa rateada")]
        [XmlEnum("221")]
        Is221 = 221,

        [Description("Redução de Base de Cálculo")]
        [XmlEnum("222")]
        Is222 = 222,

        [Description("Isenção")]
        [XmlEnum("400")]
        Is400 = 400,

        [Description("Imunidade e não incidência")]
        [XmlEnum("410")]
        Is410 = 410,

        [Description("Diferimento")]
        [XmlEnum("510")]
        Is510 = 510,

        [Description("Diferimento com redução de alíquota")]
        [XmlEnum("515")]
        Is515 = 515,

        [Description("Suspensão")]
        [XmlEnum("550")]
        Is550 = 550,

        [Description("Tributação Monofásica")]
        [XmlEnum("620")]
        Is620 = 620,

        [Description("Transferência de crédito")]
        [XmlEnum("800")]
        Is800 = 800,

        [Description("Ajuste de IBS na ZFM")]
        [XmlEnum("810")]
        Is810 = 810,

        [Description("Ajustes")]
        [XmlEnum("811")]
        Is811 = 811,

        [Description("Tributação em declaração de regime específico")]
        [XmlEnum("820")]
        Is820 = 820,

        [Description("Exclusão da Base de Cálculo")]
        [XmlEnum("830")]
        Is830 = 830,
    }
}