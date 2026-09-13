using System.ComponentModel;
using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    ///     Informar "1 - Empresa Emitente" para este evento.
    ///     Nota:
    ///     1 - Empresa Emitente;
    ///     2 - Empresa Destinatária;
    ///     3 - Empresa;
    ///     5 - Fisco;
    ///     6 - RFB;
    ///     8 - Empresa Sucessora;
    ///     9 - Outros Órgãos.
    /// </summary>
    public enum TipoAutor
    {
        /// <summary>
        /// 1 - Empresa Emitente
        /// </summary>
        [Description("Empresa Emitente")]
        [XmlEnum("1")]
        taEmpresaEmitente = 1,

        /// <summary>
        /// 2 - Empresa Destinatária
        /// </summary>
        [Description("Empresa Destinatária")]
        [XmlEnum("2")]
        taEmpresaDestinataria = 2,

        /// <summary>
        /// 3 - Empresa
        /// </summary>
        [Description("Empresa")]
        [XmlEnum("3")]
        taEmpresa = 3,

        /// <summary>
        /// 5 - Fisco
        /// </summary>
        [Description("Fisco")]
        [XmlEnum("5")]
        taFisco = 5,

        /// <summary>
        /// 6 - RFB
        /// </summary>
        [Description("RFB")]
        [XmlEnum("6")]
        taRFB = 6,

        /// <summary>
        /// 8 - Empresa Sucessora
        /// </summary>
        [Description("Empresa Sucessora")]
        [XmlEnum("8")]
        taEmpresaSucessora = 8,

        /// <summary>
        /// 9 - Outros Órgãos
        /// </summary>
        [Description("Outros Órgãos")]
        [XmlEnum("9")]
        taOutrosOrgaos = 9
    }

    /// <summary>
    ///     Motivo de Insucesso.
    ///     Nota:
    ///     1 - Recebedor não encontrado;
    ///     2 - Recusa do recebedor;
    ///     3 - Endereço inexistente;
    ///     4 - Outros (exige informar justificativa);
    /// </summary>
    public enum MotivoInsucesso
    {
        /// <summary>
        /// 1 - Recebedor não encontrado 
        /// </summary>
        [Description("Recebedor não encontrado")]
        [XmlEnum("1")]
        RecebedorNaoEncontrado = 1,

        /// <summary>
        /// 2 - Recusa do recebedor
        /// </summary>
        [Description("Recusa do recebedor")]
        [XmlEnum("2")]
        RecusaRecebedor = 2,

        /// <summary>
        /// 3 - Endereço inexistente
        /// </summary>
        [Description("Endereço inexistente")]
        [XmlEnum("3")]
        EnderecoInexistente = 3,

        /// <summary>
        /// 4 - Outros
        /// </summary>
        [Description("Outros")]
        [XmlEnum("4")]
        Outros = 4
    }

    /// <summary>
    ///     Indicador de efetiva quitação do pagamento integral da operação referente à NF-e referenciada.
    ///     Nota:
    ///     1 - Quitado;
    /// </summary>
    public enum IndicadorQuitacao
    {
        /// <summary>
        /// 1 - Quitado
        /// </summary>
        [Description("Quitado")]
        [XmlEnum("1")]
        Quitado = 1
    }

    /// <summary>
    ///     Indicador de aceitação.
    ///     Nota:
    ///     0 - Não Aceite;
    ///     1 - Aceite;
    /// </summary>
    public enum IndicadorAceitacao
    {
        /// <summary>
        /// 0 - Não Aceite
        /// </summary>
        [Description("Não Aceite")]
        [XmlEnum("0")]
        NaoAceite = 0,

        /// <summary>
        /// 1 - Aceite
        /// </summary>
        [Description("Aceite")]
        [XmlEnum("1")]
        Aceite = 1
    }

    /// <summary>
    ///     Indicador de deferimento do fisco sobre o pedido de transferência de crédito.
    ///     Nota:
    ///     0 - Não Aceite;
    ///     1 - Aceite;
    /// </summary>
    public enum IndicadorDeferimento
    {
        /// <summary>
        /// 0 - Não Aceite
        /// </summary>
        [Description("Não Aceite")]
        [XmlEnum("0")]
        NaoAceite = 0,

        /// <summary>
        /// 1 - Aceite
        /// </summary>
        [Description("Aceite")]
        [XmlEnum("1")]
        Aceite = 1
    }

    /// <summary>
    ///     Motivo da manifestação do fisco sobre o pedido de transferência de crédito.
    ///     Nota:
    ///     1 - Falta de manifestação de todas as sucessoras;
    ///     2 - Outros;
    /// </summary>
    public enum MotivoDeferimento
    {
        /// <summary>
        /// 1 - Falta de manifestação de todas as sucessoras
        /// </summary>
        [Description("Falta de manifestação de todas as sucessoras")]
        [XmlEnum("1")]
        FaltaManifestacaoSucessoras = 1,

        /// <summary>
        /// 2 - Outros
        /// </summary>
        [Description("Outros")]
        [XmlEnum("2")]
        Outros = 2
    }
}
