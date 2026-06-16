using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Total
{
    public class IBSCBSTot
    {
        private decimal _vBCIBSCBS;

        /// <summary>
        /// W35 - Valor total da BC do IBS e da CBS
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal vBCIBSCBS
        {
            get => _vBCIBSCBS;
            set => _vBCIBSCBS = value.Arredondar(2);
        }

        /// <summary>
        /// W36 - Grupo total do IBS
        /// </summary>
        [XmlElement(Order = 2)]
        public gIBSTotal gIBS { get; set; }

        /// <summary>
        /// W50 - Grupo total da CBS
        /// </summary>
        [XmlElement(Order = 3)]
        public gCBSTotal gCBS { get; set; }

        /// <summary>
        /// W57 - Grupo total da Monofasia
        /// </summary>
        [XmlElement(Order = 4)]
        public gMonoTotal gMono { get; set; }

        /// <summary>
        /// W59e - Grupo total do Estorno de Crédito (nt 1.30)
        /// </summary>
        [XmlElement(Order = 5)]
        public gEstornoCredTotal gEstornoCred { get; set; }
    }
}