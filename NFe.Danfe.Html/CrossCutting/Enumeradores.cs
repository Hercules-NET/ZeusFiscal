namespace NFe.Danfe.Html.CrossCutting
{
   
        public enum Status
        {
            Autorizada,
            Cancelada,
            NaoAuotorizada,
            Negada,
            Recusada,
            Contingencia,
            EsperandoAutorizacao
        }

        /// <summary>
        ///     Leiaute de impressão do DANFE NF-e
        /// </summary>
        public enum LeiauteDanfe
        {
            /// <summary>
            ///     Leiaute anterior à Reforma Tributária, sem os quadros de IBS, CBS e IS
            /// </summary>
            Atual,

            /// <summary>
            ///     Leiaute da NT 2026.010 (Reforma Tributária), com o quadro "Total do IBS/CBS/IS", o CRT do emitente
            ///     e as bases, alíquotas e valores de IBS, CBS e IS por item. Obrigatório a partir de 01/12/2026
            /// </summary>
            ReformaTributaria
        }

}
