using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB87 - Grupo de informações da Tributação Monofásica Sujeita à Retenção do IBS Ad Rem (tag gMonoReten)
    /// </summary>
    public class gMonoRetenIBSAdRem
    {
        private decimal _qBcMonoReten;
        private decimal _adRemIbsReten;
        private decimal _vIbsMonoReten;

        // UB87a
        [XmlElement(Order = 1)]
        public decimal qBCMonoReten
        {
            get => _qBcMonoReten.Arredondar(4);
            set => _qBcMonoReten = value.Arredondar(4);
        }

        // UB87b
        [XmlElement(Order = 2)]
        public decimal adRemIBSReten
        {
            get => _adRemIbsReten.Arredondar(4);
            set => _adRemIbsReten = value.Arredondar(4);
        }

        // UB87c
        [XmlElement(Order = 3)]
        public decimal vIBSMonoReten
        {
            get => _vIbsMonoReten.Arredondar(2);
            set => _vIbsMonoReten = value.Arredondar(2);
        }
    }
}
