using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gCredPresOper
    {
        private decimal _vBCCredPres;

        //UB121
        [XmlElement(Order = 1)]
        public decimal vBCCredPres
        {
            get => _vBCCredPres.Arredondar(2);
            set => _vBCCredPres = value.Arredondar(2);
        }

        //UB122
        [XmlElement(Order = 2)]
        public TipocCredPres cCredPres { get; set; }

        // UB123
        [XmlElement(Order = 3)]
        public gIBSCredPres gIBSCredPres { get; set; }

        // UB127
        [XmlElement(Order = 4)]
        public gIBSCredPres gCBSCredPres { get; set; }
    }
}