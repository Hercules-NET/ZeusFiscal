using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade de estoque influenciadas pelo evento 211130.
    /// </summary>
    public class gControleEstoqueImobilizacao
    {
        private decimal _qImobilizado;

        /// <summary>
        /// P28 - Informar a quantidade do item a ser imobilizado.
        /// </summary>
        [XmlElement("qImobilizado")]
        public decimal qImobilizado
        {
            get { return _qImobilizado.Arredondar(4); }
            set { _qImobilizado = value.Arredondar(4); }
        }

        /// <summary>
        /// P29 - Informar a unidade relativa ao campo qImobilizado.
        /// </summary>
        [XmlElement("uImobilizado")]
        public string uImobilizado { get; set; }
    }
}
