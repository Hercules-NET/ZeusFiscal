using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações por item da NF-e de importação para o evento 112120.
    /// </summary>
    public class gConsumo
    {
        private decimal _vIBS;
        private decimal _vCBS;

        /// <summary>
        /// P24 - Corresponde ao atributo nItem do elemento det da NF-e de importação.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// P25 - Valor do IBS correspondente à quantidade que não atendeu aos requisitos para a conversão em isenção.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS
        {
            get { return _vIBS.Arredondar(2); }
            set { _vIBS = value.Arredondar(2); }
        }

        /// <summary>
        /// P26 - Valor da CBS correspondente à quantidade que não atendeu aos requisitos para a conversão em isenção.
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
        public gControleEstoqueConsumo gControleEstoque { get; set; }
    }
}
