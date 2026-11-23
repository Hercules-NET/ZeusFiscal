using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB86 - Grupo de informações da Tributação Monofásica Padrão do IBS Ad Rem (tag gMonoPadrao)
    /// </summary>
    public class gMonoPadraoIBSAdRem
    {
        private decimal _qBcMono;
        private decimal _adRemIbs;
        private decimal _vIbsMono;

        // UB86a
        [XmlElement(Order = 1)]
        public decimal qBCMono
        {
            get => _qBcMono.Arredondar(4);
            set => _qBcMono = value.Arredondar(4);
        }

        // UB86b
        [XmlElement(Order = 2)]
        public decimal adRemIBS
        {
            get => _adRemIbs.Arredondar(4);
            set => _adRemIbs = value.Arredondar(4);
        }

        // UB86c
        [XmlElement(Order = 3)]
        public decimal vIBSMono
        {
            get => _vIbsMono.Arredondar(2);
            set => _vIbsMono = value.Arredondar(2);
        }
    }
}
