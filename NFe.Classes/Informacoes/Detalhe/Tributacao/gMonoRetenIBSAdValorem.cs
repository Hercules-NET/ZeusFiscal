using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB92 - Grupo de informações da Tributação Monofásica Sujeita à Retenção do IBS Ad Valorem (tag gMonoReten)
    /// </summary>
    public class gMonoRetenIBSAdValorem
    {
        private decimal _vBcMonoReten;
        private decimal _pAliqMonoReten;
        private decimal _vIbsMonoReten;

        // UB92a
        [XmlElement(Order = 1)]
        public decimal vBCMonoReten
        {
            get => _vBcMonoReten.Arredondar(2);
            set => _vBcMonoReten = value.Arredondar(2);
        }

        // UB92b
        [XmlElement(Order = 2)]
        public decimal pAliqMonoReten
        {
            get => _pAliqMonoReten.Arredondar(4);
            set => _pAliqMonoReten = value.Arredondar(4);
        }

        // UB92c
        [XmlElement(Order = 3)]
        public decimal vIBSMonoReten
        {
            get => _vIbsMonoReten.Arredondar(2);
            set => _vIbsMonoReten = value.Arredondar(2);
        }
    }
}
