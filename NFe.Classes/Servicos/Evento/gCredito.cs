using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de crédito por item para o evento 211150.
    /// </summary>
    public class gCredito
    {
        private decimal _vCredIBS;
        private decimal _vCredCBS;

        /// <summary>
        /// P24 - Corresponde ao atributo nItem do elemento det do documento referenciado.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// P25 - Valor da solicitação de crédito a ser apropriado de IBS.
        /// </summary>
        [XmlElement("vCredIBS")]
        public decimal vCredIBS
        {
            get { return _vCredIBS.Arredondar(2); }
            set { _vCredIBS = value.Arredondar(2); }
        }

        /// <summary>
        /// P26 - Valor da solicitação de crédito a ser apropriado de CBS.
        /// </summary>
        [XmlElement("vCredCBS")]
        public decimal vCredCBS
        {
            get { return _vCredCBS.Arredondar(2); }
            set { _vCredCBS = value.Arredondar(2); }
        }
    }
}
