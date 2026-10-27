using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB95a - Grupo de informações da Tributação Monofásica Ad Rem da CBS (NT 2025.002 v1.50)
    /// </summary>
    public class gCBSMonoAdRem
    {
        // UB96
        [XmlElement(Order = 1)]
        public gMonoPadraoCBSAdRem gMonoPadrao { get; set; }

        // UB97
        [XmlElement(Order = 2)]
        public gMonoRetenCBSAdRem gMonoReten { get; set; }

        // UB98
        [XmlElement(Order = 3)]
        public gMonoRetCBS gMonoRet { get; set; }

        // UB99
        [XmlElement(Order = 4)]
        public gpBioDiferencaCBS gpBioDiferenca { get; set; }
    }
}
