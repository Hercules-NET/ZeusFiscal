using System.Xml.Serialization;
using DFe.Classes;

namespace CTe.Classes.Informacoes.Impostos.ReformaTributaria
{
    /// <summary>
    /// Grupo de Valores Totais da Tributação Monofásica
    /// Reforma Tributária - NT 2025.001
    /// </summary>
    public class gMono
    {
        private decimal _vIbsMono;
        private decimal _vCbsMono;
        private decimal _vIbsMonoReten;
        private decimal _vCbsMonoReten;
        private decimal _vIbsMonoRet;
        private decimal _vCbsMonoRet;

        /// <summary>
        /// Valor total do IBS da tributação monofásica
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal vIBSMono
        {
            get => _vIbsMono;
            set => _vIbsMono = value.Arredondar(2);
        }

        /// <summary>
        /// Valor total da CBS da tributação monofásica
        /// </summary>
        [XmlElement(Order = 2)]
        public decimal vCBSMono
        {
            get => _vCbsMono;
            set => _vCbsMono = value.Arredondar(2);
        }

        /// <summary>
        /// Valor total do IBS retido anteriormente por tributação monofásica
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal vIBSMonoReten
        {
            get => _vIbsMonoReten;
            set => _vIbsMonoReten = value.Arredondar(2);
        }

        /// <summary>
        /// Valor total da CBS retida anteriormente por tributação monofásica
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vCBSMonoReten
        {
            get => _vCbsMonoReten;
            set => _vCbsMonoReten = value.Arredondar(2);
        }

        /// <summary>
        /// Valor total do IBS a recolher por tributação monofásica
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal vIBSMonoRet
        {
            get => _vIbsMonoRet;
            set => _vIbsMonoRet = value.Arredondar(2);
        }

        /// <summary>
        /// Valor total da CBS a recolher por tributação monofásica
        /// </summary>
        [XmlElement(Order = 6)]
        public decimal vCBSMonoRet
        {
            get => _vCbsMonoRet;
            set => _vCbsMonoRet = value.Arredondar(2);
        }
    }
}
