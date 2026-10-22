using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB84 - Grupo de Informações do IBS e CBS em operações com imposto monofásico (CST 620)
    /// <para>
    /// Leiaute da NT 2025.002 v1.50 (PL_010f): o IBS vai em gIBSMonoAdRem ou gIBSMonoAdValorem
    /// e a CBS em gCBSMonoAdRem ou gCBSMonoAdValorem.
    /// </para>
    /// <para>
    /// gMonoPadrao, gMonoReten, gMonoRet e gMonoDif são do leiaute anterior, que deixa de ser aceito em 03/11/2026.
    /// Ficam somente para a leitura de documentos já autorizados e não devem ser preenchidos em novas emissões.
    /// </para>
    /// </summary>
    public class gIBSCBSMono
    {
        private decimal _vTotIbsMonoItem;
        private decimal _vTotCbsMonoItem;

        // UB84a (leiaute anterior à NT 2025.002 v1.50)
        [XmlElement(Order = 1)]
        public gMonoPadrao gMonoPadrao { get; set; }

        // UB90 (leiaute anterior à NT 2025.002 v1.50)
        [XmlElement(Order = 2)]
        public gMonoReten gMonoReten { get; set; }

        // UB94 (leiaute anterior à NT 2025.002 v1.50)
        [XmlElement(Order = 3)]
        public gMonoRet gMonoRet { get; set; }

        // UB99 (leiaute anterior à NT 2025.002 v1.50)
        [XmlElement(Order = 4)]
        public gMonoDif gMonoDif { get; set; }

        /// <summary>
        /// UB85a - Tributação Monofásica Ad Rem do IBS. Informar este ou gIBSMonoAdValorem
        /// </summary>
        [XmlElement(Order = 5)]
        public gIBSMonoAdRem gIBSMonoAdRem { get; set; }

        /// <summary>
        /// UB90 - Tributação Monofásica Ad Valorem do IBS. Informar este ou gIBSMonoAdRem
        /// </summary>
        [XmlElement(Order = 6)]
        public gIBSMonoAdValorem gIBSMonoAdValorem { get; set; }

        /// <summary>
        /// UB95a - Tributação Monofásica Ad Rem da CBS. Informar este ou gCBSMonoAdValorem
        /// </summary>
        [XmlElement(Order = 7)]
        public gCBSMonoAdRem gCBSMonoAdRem { get; set; }

        /// <summary>
        /// UB100 - Tributação Monofásica Ad Valorem da CBS. Informar este ou gCBSMonoAdRem
        /// </summary>
        [XmlElement(Order = 8)]
        public gCBSMonoAdValorem gCBSMonoAdValorem { get; set; }

        // UB105a
        [XmlElement(Order = 9)]
        public decimal vTotIBSMonoItem
        {
            get => _vTotIbsMonoItem.Arredondar(2);
            set => _vTotIbsMonoItem = value.Arredondar(2);
        }

        // UB105b
        [XmlElement(Order = 10)]
        public decimal vTotCBSMonoItem
        {
            get => _vTotCbsMonoItem.Arredondar(2);
            set => _vTotCbsMonoItem = value.Arredondar(2);
        }
    }
}