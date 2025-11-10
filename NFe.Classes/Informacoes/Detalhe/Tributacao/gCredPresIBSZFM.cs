using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gCredPresIBSZFM
    {
        private decimal? _vCredPresIbsZfm;
        
        [XmlElement(Order = 1)]
        public string competApur { get; set; }

        // UB133
        [XmlElement(Order = 2)]
        public tpCredPresIBSZFM tpCredPresIBSZFM { get; set; }

        // UB134
        [XmlElement(Order = 3)]
        public decimal? vCredPresIBSZFM
        {
            get => _vCredPresIbsZfm.Arredondar(2);
            set => _vCredPresIbsZfm = value.Arredondar(2);
        }

        public bool ShouldSerializevCredPresIBSZFM()
        {
            return vCredPresIBSZFM.HasValue;
        }
    }
}