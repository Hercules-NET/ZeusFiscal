using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gEstornoCred
    {
        private decimal _vIBSEstCred;
        private decimal _vCBSEstCred;

        // UB117
        [XmlElement(Order = 1)]
        public decimal vIBSEstCred
        {
            get => _vIBSEstCred.Arredondar(2);
            set => _vIBSEstCred = value.Arredondar(2);
        }

        // UB118
        [XmlElement(Order = 2)]
        public decimal vCBSEstCred
        {
            get => _vCBSEstCred.Arredondar(2);
            set => _vCBSEstCred = value.Arredondar(2);
        }
    }
}