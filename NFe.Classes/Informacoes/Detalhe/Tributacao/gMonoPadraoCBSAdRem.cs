using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB96 - Grupo de informações da Tributação Monofásica Padrão da CBS Ad Rem (tag gMonoPadrao)
    /// </summary>
    public class gMonoPadraoCBSAdRem
    {
        private decimal _qBcMono;
        private decimal _adRemCbs;
        private decimal _vCbsMono;

        // UB96a
        [XmlElement(Order = 1)]
        public decimal qBCMono
        {
            get => _qBcMono.Arredondar(4);
            set => _qBcMono = value.Arredondar(4);
        }

        // UB96b
        [XmlElement(Order = 2)]
        public decimal adRemCBS
        {
            get => _adRemCbs.Arredondar(4);
            set => _adRemCbs = value.Arredondar(4);
        }

        // UB96c
        [XmlElement(Order = 3)]
        public decimal vCBSMono
        {
            get => _vCbsMono.Arredondar(2);
            set => _vCbsMono = value.Arredondar(2);
        }
    }
}
