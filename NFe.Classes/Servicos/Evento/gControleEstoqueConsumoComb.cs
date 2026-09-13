using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade por item influenciadas pelo evento 211140.
    /// </summary>
    public class gControleEstoqueConsumoComb
    {
        private decimal _qComb;

        /// <summary>
        /// P28 - Informar a quantidade de consumo do item.
        /// </summary>
        [XmlElement("qComb")]
        public decimal qComb
        {
            get { return _qComb.Arredondar(4); }
            set { _qComb = value.Arredondar(4); }
        }

        /// <summary>
        /// P29 - Informar a unidade relativa ao campo qComb.
        /// </summary>
        [XmlElement("uComb")]
        public string uComb { get; set; }
    }
}
