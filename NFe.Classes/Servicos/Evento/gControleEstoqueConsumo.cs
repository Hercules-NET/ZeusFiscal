using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade de estoque influenciadas pelo evento 112120.
    /// </summary>
    public class gControleEstoqueConsumo
    {
        private decimal _qtde;

        /// <summary>
        /// P28 - Informar a quantidade que não atendeu os requisitos para a conversão em isenção.
        /// </summary>
        [XmlElement("qtde")]
        public decimal qtde
        {
            get { return _qtde.Arredondar(4); }
            set { _qtde = value.Arredondar(4); }
        }

        /// <summary>
        /// P29 - Informar a unidade relativa ao campo gConsumo.
        /// </summary>
        [XmlElement("unidade")]
        public string unidade { get; set; }
    }
}
