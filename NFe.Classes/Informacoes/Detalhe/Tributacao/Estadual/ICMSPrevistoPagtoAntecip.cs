using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual
{
    public class ICMSPrevistoPagtoAntecip : ICMSBasico
    {
        private decimal _vICMSPrevisto;

        [XmlElement(Order = 1)]
        public decimal VICMSPrevisto
        {
            get { return _vICMSPrevisto.Arredondar(2); }
            set { _vICMSPrevisto = value.Arredondar(2); }
        }
    }
}
