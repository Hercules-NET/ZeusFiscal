using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade de estoque influenciadas pelo evento 112140.
    /// </summary>
    public class gControleEstoqueItemNaoFornecido
    {
        private decimal _qNaoFornecida;

        /// <summary>
        /// P28 - Informar a quantidade que não foi fornecida e teve o imposto antecipado.
        /// </summary>
        [XmlElement("qNaoFornecida")]
        public decimal qNaoFornecida
        {
            get { return _qNaoFornecida.Arredondar(4); }
            set { _qNaoFornecida = value.Arredondar(4); }
        }

        /// <summary>
        /// P29 - Informar a unidade relativa ao campo qNaoFornecida.
        /// </summary>
        [XmlElement("uNaoFornecida")]
        public string uNaoFornecida { get; set; }
    }
}
