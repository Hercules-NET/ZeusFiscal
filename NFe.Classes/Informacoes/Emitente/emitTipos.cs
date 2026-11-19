using System.ComponentModel;
using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Emitente
{
    /// <summary>
    ///     <para>1 – Simples Nacional;</para>
    ///     <para>2 – Simples Nacional – excesso de sublimite de receita bruta;</para>
    ///     <para>3 – Regime Normal. (v2.0).</para>
    ///     <para>4 – Simples Nacional MEI;</para>
    /// </summary>
    public enum CRT
    {
        /// <summary>
        /// 1 – Simples Nacional
        /// </summary>
        [Description("Simples Nacional")]
        [XmlEnum("1")]
        SimplesNacional = 1,

        /// <summary>
        /// 2 – Simples Nacional – excesso de sublimite de receita bruta
        /// </summary>
        [Description("Simples Nacional – excesso de sublimite de receita bruta")]
        [XmlEnum("2")]
        SimplesNacionalExcessoSublimite = 2,

        /// <summary>
        /// 3 – Regime Normal
        /// </summary>
        [Description("Regime Normal")]
        [XmlEnum("3")]
        RegimeNormal = 3,

        /// <summary>
        /// 4 – Simples Nacional MEI
        /// </summary>
        [Description("Simples Nacional MEI")]
        [XmlEnum("4")]
        SimplesNacionalMei = 4
    }

    /// <summary>
    ///     Tipo de regime do IBS/CBS para contribuintes do Simples Nacional ou com opção pendente.
    ///     <para>1 = Contribuinte do Simples Nacional (CRT=1 ou 2) com opção pelo regime regular do IBS e da CBS;</para>
    ///     <para>2 = Contribuinte do Regime Normal (CRT=3) com opção pendente pelo CRT="1-Simples Nacional" sem regime regular do IBS e da CBS;</para>
    ///     <para>3 = Contribuinte do Regime Normal (CRT=3) com opção pendente pelo CRT="2-Simples Nacional, excesso sublimite de receita bruta" sem regime regular do IBS e da CBS;</para>
    ///     <para>4 = Contribuinte do Regime Normal (CRT=3) com opção pendente pelo Simples Nacional (CRT=1 ou 2), com regime regular do IBS e da CBS.</para>
    /// </summary>
    public enum tpRegimeIBSCBS
    {
        /// <summary>
        /// 1 - Contribuinte do Simples Nacional (CRT=1 ou 2) com opção pelo regime regular do IBS e da CBS
        /// </summary>
        [Description("Simples Nacional com opção pelo regime regular do IBS e da CBS")]
        [XmlEnum("1")]
        SimplesNacionalComRegimeRegular = 1,

        /// <summary>
        /// 2 - Contribuinte do Regime Normal (CRT=3) com opção pendente pelo CRT=1 (Simples Nacional) sem regime regular do IBS e da CBS
        /// </summary>
        [Description("Regime Normal com opção pendente pelo CRT 1 sem regime regular do IBS e da CBS")]
        [XmlEnum("2")]
        RegimeNormalPendenteCRT1SemRegimeRegular = 2,

        /// <summary>
        /// 3 - Contribuinte do Regime Normal (CRT=3) com opção pendente pelo CRT=2 (Simples Nacional, excesso sublimite) sem regime regular do IBS e da CBS
        /// </summary>
        [Description("Regime Normal com opção pendente pelo CRT 2 sem regime regular do IBS e da CBS")]
        [XmlEnum("3")]
        RegimeNormalPendenteCRT2SemRegimeRegular = 3,

        /// <summary>
        /// 4 - Contribuinte do Regime Normal (CRT=3) com opção pendente pelo Simples Nacional (CRT=1 ou 2), com regime regular do IBS e da CBS
        /// </summary>
        [Description("Regime Normal com opção pendente pelo Simples Nacional com regime regular do IBS e da CBS")]
        [XmlEnum("4")]
        RegimeNormalPendenteSimplesComRegimeRegular = 4
    }
}