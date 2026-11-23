using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB102 - Grupo de informações da Tributação Monofásica Sujeita à Retenção da CBS Ad Valorem (tag gMonoReten)
    /// </summary>
    public class gMonoRetenCBSAdValorem
    {
        private decimal _vBcMonoReten;
        private decimal _pAliqMonoReten;
        private decimal _vCbsMonoReten;

        // UB102a
        [XmlElement(Order = 1)]
        public decimal vBCMonoReten
        {
            get => _vBcMonoReten.Arredondar(2);
            set => _vBcMonoReten = value.Arredondar(2);
        }

        // UB102b
        [XmlElement(Order = 2)]
        public decimal pAliqMonoReten
        {
            get => _pAliqMonoReten.Arredondar(4);
            set => _pAliqMonoReten = value.Arredondar(4);
        }

        // UB102c
        [XmlElement(Order = 3)]
        public decimal vCBSMonoReten
        {
            get => _vCbsMonoReten.Arredondar(2);
            set => _vCbsMonoReten = value.Arredondar(2);
        }
    }
}
