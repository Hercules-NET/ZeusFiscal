using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB90 - Grupo de informações da Tributação Monofásica Ad Valorem do IBS (NT 2025.002 v1.50)
    /// </summary>
    public class gIBSMonoAdValorem
    {
        // UB91
        [XmlElement(Order = 1)]
        public gMonoPadraoIBSAdValorem gMonoPadrao { get; set; }

        // UB92
        [XmlElement(Order = 2)]
        public gMonoRetenIBSAdValorem gMonoReten { get; set; }

        // UB93
        [XmlElement(Order = 3)]
        public gMonoRetIBS gMonoRet { get; set; }

        // UB94
        [XmlElement(Order = 4)]
        public gpBioDiferencaIBS gpBioDiferenca { get; set; }
    }
}
