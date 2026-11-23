namespace NFe.Danfe.Html.Dominio
{
    /// <summary>
    ///     Totais da nota exibidos somente no leiaute da NT 2026.010 (Reforma Tributária)
    /// </summary>
    public class ImpostoReformaTributaria
    {
        #region Propriedades

        /// <summary>
        ///     W04h - Valor do FCP
        /// </summary>
        public decimal? vFCP { get; set; }

        /// <summary>
        ///     W06a - Valor do FCP retido por ST
        /// </summary>
        public decimal? vFCPST { get; set; }

        /// <summary>
        ///     W04e - Valor do ICMS (DIFAL) na UF de destino
        /// </summary>
        public decimal? vICMSUFDest { get; set; }

        /// <summary>
        ///     W04c - Valor do FCP na UF de destino
        /// </summary>
        public decimal? vFCPUFDest { get; set; }

        /// <summary>
        ///     W06b.1 - BC (quantidade) do ICMS monofásico
        /// </summary>
        public decimal? qBCMono { get; set; }

        /// <summary>
        ///     W06c - Valor do ICMS monofásico
        /// </summary>
        public decimal? vICMSMono { get; set; }

        /// <summary>
        ///     W06c.1 - BC (quantidade) do ICMS monofásico por retenção
        /// </summary>
        public decimal? qBCMonoReten { get; set; }

        /// <summary>
        ///     W06d - Valor do ICMS monofásico por retenção
        /// </summary>
        public decimal? vICMSMonoReten { get; set; }

        /// <summary>
        ///     W56 - Valor da CBS
        /// </summary>
        public decimal vCBS { get; set; }

        /// <summary>
        ///     W41 - Valor do IBS UF
        /// </summary>
        public decimal vIBSUF { get; set; }

        /// <summary>
        ///     W46 - Valor do IBS Município
        /// </summary>
        public decimal vIBSMun { get; set; }

        /// <summary>
        ///     W33 - Valor do Imposto Seletivo
        /// </summary>
        public decimal vIS { get; set; }

        /// <summary>
        ///     Indica se o grupo de monofasia (gMono - W57) foi informado
        /// </summary>
        public bool PossuiMonofasia { get; set; }

        /// <summary>
        ///     W58 - Valor do IBS monofásico
        /// </summary>
        public decimal vIBSMono { get; set; }

        /// <summary>
        ///     W59 - Valor da CBS monofásica
        /// </summary>
        public decimal vCBSMono { get; set; }

        /// <summary>
        ///     W59a - Valor do IBS monofásico por retenção
        /// </summary>
        public decimal vIBSMonoReten { get; set; }

        /// <summary>
        ///     W59b - Valor da CBS monofásica por retenção
        /// </summary>
        public decimal vCBSMonoReten { get; set; }

        #endregion
    }
}
