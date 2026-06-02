using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gAjusteCompet
    {
        private decimal _vIBS;
        private decimal _vCBS;

        //UB113
        [XmlElement(Order = 1)]
        public string competApur { get; set; }

        //UB114
        [XmlElement(Order = 2)]
        public decimal vIBS
        {
            get => _vIBS.Arredondar(2);
            set => _vIBS = value.Arredondar(2);
        }

        //UB115
        [XmlElement(Order = 3)]
        public decimal vCBS
        {
            get => _vCBS.Arredondar(2);
            set => _vCBS = value.Arredondar(2);
        }
    }
}