using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gCBSSN
    {
        private decimal _pCBSSN;
        private decimal _vCBSSN;

        // UB115f
        [XmlElement(Order = 1)]
        public decimal pCBSSN
        {
            get => _pCBSSN.Arredondar(4);
            set => _pCBSSN = value.Arredondar(4);
        }

        // UB115g
        [XmlElement(Order = 2)]
        public decimal vCBSSN
        {
            get => _vCBSSN.Arredondar(2);
            set => _vCBSSN = value.Arredondar(2);
        }
    }
}