using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gIBSSN
    {
        private decimal _pIBSSN;
        private decimal _vIBSSN;

        // UB115c
        [XmlElement(Order = 1)]
        public decimal pIBSSN
        {
            get => _pIBSSN.Arredondar(4);
            set => _pIBSSN = value.Arredondar(4);
        }

        // UB115d
        [XmlElement(Order = 2)]
        public decimal vIBSSN
        {
            get => _vIBSSN.Arredondar(2);
            set => _vIBSSN = value.Arredondar(2);
        }
    }
}