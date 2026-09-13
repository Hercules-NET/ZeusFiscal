using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de crédito presumido por item para o evento 211110.
    /// </summary>
    public class gCredPresOper
    {
        private decimal _vBCCredPres;

        /// <summary>
        /// P24 - Corresponde ao atributo nItem do elemento det do documento referenciado.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// P25 - Valor da base de cálculo do item.
        /// </summary>
        [XmlElement("vBCCredPres")]
        public decimal vBCCredPres
        {
            get { return _vBCCredPres.Arredondar(2); }
            set { _vBCCredPres = value.Arredondar(2); }
        }

        /// <summary>
        /// P25a - Código de Classificação do Crédito presumido, conforme tabela cCredPres (Anexo IV).
        /// </summary>
        [XmlElement("cCredPres")]
        public string cCredPres { get; set; }

        /// <summary>
        /// P26 - Grupo de Informações do Crédito Presumido do IBS.
        /// </summary>
        [XmlElement("gIBSCredPres")]
        public gCredPresTributo gIBSCredPres { get; set; }

        /// <summary>
        /// P30 - Grupo de Informações do Crédito Presumido da CBS.
        /// </summary>
        [XmlElement("gCBSCredPres")]
        public gCredPresTributo gCBSCredPres { get; set; }
    }
}
