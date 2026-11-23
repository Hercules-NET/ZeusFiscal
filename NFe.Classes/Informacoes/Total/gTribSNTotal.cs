using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Total
{
    public class gTribSNTotal
    {
        private decimal _vIBSSN;
        private decimal _vCBSSN;

        // W59i
        [XmlElement(Order = 1)]
        public decimal vIBSSN
        {
            get => _vIBSSN;
            set => _vIBSSN = value.Arredondar(2);
        }

        // W59j
        [XmlElement(Order = 2)]
        public decimal vCBSSN
        {
            get => _vCBSSN;
            set => _vCBSSN = value.Arredondar(2);
        }
    }
}