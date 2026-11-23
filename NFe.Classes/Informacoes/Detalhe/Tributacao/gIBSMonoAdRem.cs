using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB85a - Grupo de informações da Tributação Monofásica Ad Rem do IBS (NT 2025.002 v1.50)
    /// </summary>
    public class gIBSMonoAdRem
    {
        // UB86
        [XmlElement(Order = 1)]
        public gMonoPadraoIBSAdRem gMonoPadrao { get; set; }

        // UB87
        [XmlElement(Order = 2)]
        public gMonoRetenIBSAdRem gMonoReten { get; set; }

        // UB88
        [XmlElement(Order = 3)]
        public gMonoRetIBS gMonoRet { get; set; }

        // UB89
        [XmlElement(Order = 4)]
        public gpBioDiferencaIBS gpBioDiferenca { get; set; }
    }
}
