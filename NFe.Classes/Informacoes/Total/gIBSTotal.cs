using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Total
{
    public class gIBSTotal
    {
        private decimal _vIBS;
        private decimal _vCredPres;
        private decimal _vCredPresCondSus;

        /// <summary>
        /// W37 - Grupo total do IBS da UF
        /// </summary>
        [XmlElement(Order = 1)]
        public gIBSUFTotal gIBSUF { get; set; }

        /// <summary>
        /// W42 - Grupo total do IBS do Município
        /// </summary>
        [XmlElement(Order = 2)]
        public gIBSMunTotal gIBSMun { get; set; }

        /// <summary>
        /// W47 - Valor total do IBS
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal vIBS
        {
            get => _vIBS;
            set => _vIBS = value.Arredondar(2);
        }

        /// <summary>
        /// W48 - Valor total do crédito presumido
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vCredPres
        {
            get => _vCredPres;
            set => _vCredPres = value.Arredondar(2);
        }

        /// <summary>
        /// W49 - Valor total do crédito presumido em condição suspensiva
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal vCredPresCondSus
        {
            get => _vCredPresCondSus;
            set => _vCredPresCondSus = value.Arredondar(2);
        }
    }
}