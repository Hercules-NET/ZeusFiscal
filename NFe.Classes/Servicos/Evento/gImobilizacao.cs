using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de itens integrados ao ativo imobilizado para o evento 211130.
    /// </summary>
    public class gImobilizacao
    {
        private decimal _vIBS;
        private decimal _vCBS;

        /// <summary>
        /// P24 - Corresponde ao atributo nItem do elemento det do documento referenciado.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// P25 - Valor do IBS relativo à imobilização.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS
        {
            get { return _vIBS.Arredondar(2); }
            set { _vIBS = value.Arredondar(2); }
        }

        /// <summary>
        /// P26 - Valor da CBS relativo à imobilização.
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
        public gControleEstoqueImobilizacao gControleEstoque { get; set; }
    }
}
