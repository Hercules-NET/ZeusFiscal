using System.Xml.Serialization;
using DFe.Classes;

namespace CTe.Classes.Informacoes.Impostos.ReformaTributaria
{
    /// <summary>
    /// Grupo de Tributação Monofásica com Alíquota Diferenciada
    /// Reforma Tributária - NT 2025.001
    /// </summary>
    public class gMonoDif
    {
        private decimal _pDifIbs;
        private decimal _vIbsMonoDif;
        private decimal _pDifCbs;
        private decimal _vCbsMonoDif;

        /// <summary>
        /// Percentual de diferimento do IBS
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal pDifIBS
        {
            get => _pDifIbs.Arredondar(4);
            set => _pDifIbs = value.Arredondar(4);
        }

        /// <summary>
        /// Valor do IBS diferido
        /// </summary>
        [XmlElement(Order = 2)]
        public decimal vIBSMonoDif
        {
            get => _vIbsMonoDif.Arredondar(2);
            set => _vIbsMonoDif = value.Arredondar(2);
        }

        /// <summary>
        /// Percentual de diferimento da CBS
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal pDifCBS
        {
            get => _pDifCbs.Arredondar(4);
            set => _pDifCbs = value.Arredondar(4);
        }

        /// <summary>
        /// Valor da CBS diferida
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vCBSMonoDif
        {
            get => _vCbsMonoDif.Arredondar(2);
            set => _vCbsMonoDif = value.Arredondar(2);
        }
    }
}
