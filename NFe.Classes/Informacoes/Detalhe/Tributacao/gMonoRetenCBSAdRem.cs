using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB97 - Grupo de informações da Tributação Monofásica Sujeita à Retenção da CBS Ad Rem (tag gMonoReten)
    /// </summary>
    public class gMonoRetenCBSAdRem
    {
        private decimal _qBcMonoReten;
        private decimal _adRemCbsReten;
        private decimal _vCbsMonoReten;

        // UB97a
        [XmlElement(Order = 1)]
        public decimal qBCMonoReten
        {
            get => _qBcMonoReten.Arredondar(4);
            set => _qBcMonoReten = value.Arredondar(4);
        }

        // UB97b
        [XmlElement(Order = 2)]
        public decimal adRemCBSReten
        {
            get => _adRemCbsReten.Arredondar(4);
            set => _adRemCbsReten = value.Arredondar(4);
        }

        // UB97c
        [XmlElement(Order = 3)]
        public decimal vCBSMonoReten
        {
            get => _vCbsMonoReten.Arredondar(2);
            set => _vCbsMonoReten = value.Arredondar(2);
        }
    }
}
