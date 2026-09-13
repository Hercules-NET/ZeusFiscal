using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Grupo de informações do crédito presumido do IBS (gIBSCredPres) ou da CBS (gCBSCredPres)
    /// para o evento 211110.
    /// </summary>
    public class gCredPresTributo
    {
        private decimal _pCredPres;
        private decimal _vCredPres;

        /// <summary>
        /// Percentual do Crédito Presumido.
        /// </summary>
        [XmlElement("pCredPres")]
        public decimal pCredPres
        {
            get { return _pCredPres.Arredondar(4); }
            set { _pCredPres = value.Arredondar(4); }
        }

        /// <summary>
        /// Valor do Crédito Presumido.
        /// </summary>
        [XmlElement("vCredPres")]
        public decimal vCredPres
        {
            get { return _vCredPres.Arredondar(2); }
            set { _vCredPres = value.Arredondar(2); }
        }
    }
}
