namespace NFe.Danfe.Base
{
    /// <summary>
    /// Modelo do DANFE NF-e a ser impresso:
    /// Padrao - usa o NFeRetrato.frx; RTC - usa o NFeRetratoRtc.frx (NT 2026.010 - DANFE RTC)
    /// </summary>
    public enum NfeModelo
    {
        /// <summary>Layout padrão (NFeRetrato.frx)</summary>
        Padrao = 0,

        /// <summary>Layout da Reforma Tributária do Consumo (NFeRetratoRtc.frx)</summary>
        RTC = 1
    }

    public enum NfceDetalheVendaNormal
    {
        NaoImprimir = 0,
        UmaLinha = 1,
        DuasLinhas = 2,
        Completo = 3
    }

    public enum NfceDetalheVendaContigencia
    {
        UmaLinha = 1,
        DuasLinhas = 2,
        Completo = 3
    }

    public enum NfceModoImpressao
    {
        //Imprime o conteúdo em múltiplas páginas
        MultiplasPaginas = 0,

        //Imprime o conteúdo em uma única página, mesmo que o tamanho da página exceda o tamanho pré-definido (A4)
        UnicaPagina = 1
    }

    /// <summary>
    /// Layout de impressão do DANFE:
    /// Abaixo - QRCode abaixo dos dados do cliente; Lateral - QRCode ao lado dos dados do cliente (usa menos papel)
    /// </summary>
    public enum NfceLayoutQrCode
    {
        Abaixo = 0,
        Lateral = 1
    }

    public enum NfeSimplificadoTipo2DetalheVendaNormal
    {
        NaoImprimir = 0,
        UmaLinha = 1,
        DuasLinhas = 2,
        Completo = 3
    }

    public enum NfeSimplificadoTipo2DetalheVendaContigencia
    {
        UmaLinha = 1,
        DuasLinhas = 2,
        Completo = 3
    }

    public enum NfeSimplificadoTipo2ModoImpressao
    {
        /// <summary>Imprime o conteúdo em múltiplas páginas</summary>
        MultiplasPaginas = 0,

        /// <summary>Imprime o conteúdo em uma única página, mesmo que o tamanho da página exceda o tamanho pré-definido</summary>
        UnicaPagina = 1
    }

    /// <summary>
    /// Layout de impressão do DANFE NF-e Simplificado Tipo 2:
    /// Abaixo - QRCode abaixo dos dados; Lateral - QRCode ao lado dos dados (usa menos papel)
    /// </summary>
    public enum NfeSimplificadoTipo2LayoutQrCode
    {
        Abaixo = 0,
        Lateral = 1
    }
}