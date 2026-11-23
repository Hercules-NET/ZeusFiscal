using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB101 - Grupo de informações da Tributação Monofásica Padrão da CBS Ad Valorem (tag gMonoPadrao)
    /// </summary>
    public class gMonoPadraoCBSAdValorem
    {
        private decimal _vBcMono;
        private decimal _pAliqMonoCbs;
        private decimal _vCbsMono;

        // UB101a
        [XmlElement(Order = 1)]
        public decimal vBCMono
        {
            get => _vBcMono.Arredondar(2);
            set => _vBcMono = value.Arredondar(2);
        }

        // UB101b
        [XmlElement(Order = 2)]
        public decimal pAliqMonoCBS
        {
            get => _pAliqMonoCbs.Arredondar(4);
            set => _pAliqMonoCbs = value.Arredondar(4);
        }

        // UB101c
        [XmlElement(Order = 3)]
        public decimal vCBSMono
        {
            get => _vCbsMono.Arredondar(2);
            set => _vCbsMono = value.Arredondar(2);
        }
    }
}
