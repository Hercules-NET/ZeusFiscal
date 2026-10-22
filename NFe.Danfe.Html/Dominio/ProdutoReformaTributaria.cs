namespace NFe.Danfe.Html.Dominio
{
    /// <summary>
    ///     Tributação do item exibida somente no leiaute da NT 2026.010 (Reforma Tributária)
    /// </summary>
    public class ProdutoReformaTributaria
    {
        #region Propriedades

        /// <summary>
        ///     Origem + CST (ou CSOSN) do ICMS, como impresso no DANFE (ex.: "000")
        /// </summary>
        public string CstIcms { get; set; }

        /// <summary>
        ///     UB14 - Classificação Tributária do IBS/CBS
        /// </summary>
        public string cClassTrib { get; set; }

        /// <summary>
        ///     UB16 - Base de cálculo do IBS/CBS
        /// </summary>
        public decimal? vBCIBSCBS { get; set; }

        /// <summary>
        ///     UB18 (pIBSUF) ou UB28 (pAliqEfet) quando houver gRed
        /// </summary>
        public decimal? pIBSUF { get; set; }

        /// <summary>
        ///     UB35 - Valor do IBS UF
        /// </summary>
        public decimal? vIBSUF { get; set; }

        /// <summary>
        ///     UB37 (pIBSMun) ou UB47 (pAliqEfet) quando houver gRed
        /// </summary>
        public decimal? pIBSMun { get; set; }

        /// <summary>
        ///     UB54 - Valor do IBS Município
        /// </summary>
        public decimal? vIBSMun { get; set; }

        /// <summary>
        ///     UB56 (pCBS) ou UB66 (pAliqEfet) quando houver gRed
        /// </summary>
        public decimal? pCBS { get; set; }

        /// <summary>
        ///     UB67 - Valor da CBS
        /// </summary>
        public decimal? vCBS { get; set; }

        /// <summary>
        ///     UB05 - Base de cálculo do IS
        /// </summary>
        public decimal? vBCIS { get; set; }

        /// <summary>
        ///     UB06 - Alíquota do IS
        /// </summary>
        public decimal? pIS { get; set; }

        /// <summary>
        ///     UB11 - Valor do IS
        /// </summary>
        public decimal? vIS { get; set; }

        /// <summary>
        ///     O10 - Base de cálculo do IPI
        /// </summary>
        public decimal? vBCIPI { get; set; }

        #endregion
    }
}
