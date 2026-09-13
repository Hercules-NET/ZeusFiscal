using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações por item da Nota de Pagamento antecipado para o evento 112140.
    /// </summary>
    public class gItemNaoFornecido
    {
        private decimal _vIBS;
        private decimal _vCBS;

        /// <summary>
        /// P24 - Corresponde ao atributo nItem do elemento det do documento referenciado.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// P25 - Valor do IBS na nota de débito de pagamento antecipado correspondente à quantidade que não foi fornecida.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS
        {
            get { return _vIBS.Arredondar(2); }
            set { _vIBS = value.Arredondar(2); }
        }

        /// <summary>
        /// P26 - Valor da CBS na nota de débito de pagamento antecipado correspondente à quantidade que não foi fornecida.
        /// </summary>
        [XmlElement("vCBS")]
        public decimal vCBS
        {
            get { return _vCBS.Arredondar(2); }
            set { _vCBS = value.Arredondar(2); }
        }

        /// <summary>
        /// P27 - Informações de quantidade de estoque influenciadas pelo evento.
        /// </summary>
        [XmlElement("gControleEstoque")]
        public gControleEstoqueItemNaoFornecido gControleEstoque { get; set; }
    }
}
