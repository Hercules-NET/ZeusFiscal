using System.Xml.Serialization;
using DFe.Classes;

namespace CTe.Classes.Informacoes.Impostos.ReformaTributaria
{
    /// <summary>
    /// Grupo de Tributação Monofásica Padrão
    /// Reforma Tributária - NT 2025.001
    /// </summary>
    public class gMonoPadrao
    {
        private decimal _qBcMono;
        private decimal _adRemIbs;
        private decimal _adRemCbs;
        private decimal _vIbsMono;
        private decimal _vCbsMono;

        /// <summary>
        /// Quantidade tributada sujeita à tributação monofásica
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal qBCMono
        {
            get => _qBcMono.Arredondar(4);
            set => _qBcMono = value.Arredondar(4);
        }

        /// <summary>
        /// Alíquota ad rem do IBS
        /// </summary>
        [XmlElement(Order = 2)]
        public decimal adRemIBS
        {
            get => _adRemIbs.Arredondar(4);
            set => _adRemIbs = value.Arredondar(4);
        }

        /// <summary>
        /// Alíquota ad rem da CBS
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal adRemCBS
        {
            get => _adRemCbs.Arredondar(4);
            set => _adRemCbs = value.Arredondar(4);
        }

        /// <summary>
        /// Valor do IBS da tributação monofásica
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vIBSMono
        {
            get => _vIbsMono.Arredondar(2);
            set => _vIbsMono = value.Arredondar(2);
        }

        /// <summary>
        /// Valor da CBS da tributação monofásica
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal vCBSMono
        {
            get => _vCbsMono.Arredondar(2);
            set => _vCbsMono = value.Arredondar(2);
        }
    }
}
