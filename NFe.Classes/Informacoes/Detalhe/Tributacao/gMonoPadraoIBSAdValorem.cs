using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB91 - Grupo de informações da Tributação Monofásica Padrão do IBS Ad Valorem (tag gMonoPadrao)
    /// </summary>
    public class gMonoPadraoIBSAdValorem
    {
        private decimal _vBcMono;
        private decimal _pAliqMonoUf;
        private decimal _vIbsMonoUf;
        private decimal _pAliqMonoMun;
        private decimal _vIbsMonoMun;
        private decimal _vIbsMono;

        // UB91a
        [XmlElement(Order = 1)]
        public decimal vBCMono
        {
            get => _vBcMono.Arredondar(2);
            set => _vBcMono = value.Arredondar(2);
        }

        // UB91b
        [XmlElement(Order = 2)]
        public decimal pAliqMonoUF
        {
            get => _pAliqMonoUf.Arredondar(4);
            set => _pAliqMonoUf = value.Arredondar(4);
        }

        // UB91c
        [XmlElement(Order = 3)]
        public decimal vIBSMonoUF
        {
            get => _vIbsMonoUf.Arredondar(2);
            set => _vIbsMonoUf = value.Arredondar(2);
        }

        // UB91d
        [XmlElement(Order = 4)]
        public decimal pAliqMonoMun
        {
            get => _pAliqMonoMun.Arredondar(4);
            set => _pAliqMonoMun = value.Arredondar(4);
        }

        // UB91e
        [XmlElement(Order = 5)]
        public decimal vIBSMonoMun
        {
            get => _vIbsMonoMun.Arredondar(2);
            set => _vIbsMonoMun = value.Arredondar(2);
        }

        /// <summary>
        /// UB91f - Valor do IBS monofásico (vIBSMonoUF + vIBSMonoMun)
        /// </summary>
        [XmlElement(Order = 6)]
        public decimal vIBSMono
        {
            get => _vIbsMono.Arredondar(2);
            set => _vIbsMono = value.Arredondar(2);
        }
    }
}
