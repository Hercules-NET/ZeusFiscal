using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Total
{
    public class gEstornoCredTotal
    {
        private decimal _vIBSEstCred;
        private decimal _vCBSEstCred;

        /// <summary>
        /// W59f - Valor total do IBS estornado
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal vIBSEstCred
        {
            get => _vIBSEstCred.Arredondar(2);
            set => _vIBSEstCred = value.Arredondar(2);
        }

        /// <summary>
        /// W59g - Valor total da CBS estornada
        /// </summary>
        [XmlElement(Order = 2)]
        public decimal vCBSEstCred
        {
            get => _vCBSEstCred.Arredondar(2);
            set => _vCBSEstCred = value.Arredondar(2);
        }
    }
}
