using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB100 - Grupo de informações da Tributação Monofásica Ad Valorem da CBS (NT 2025.002 v1.50)
    /// </summary>
    public class gCBSMonoAdValorem
    {
        // UB101
        [XmlElement(Order = 1)]
        public gMonoPadraoCBSAdValorem gMonoPadrao { get; set; }

        // UB102
        [XmlElement(Order = 2)]
        public gMonoRetenCBSAdValorem gMonoReten { get; set; }

        // UB103
        [XmlElement(Order = 3)]
        public gMonoRetCBS gMonoRet { get; set; }

        // UB104
        [XmlElement(Order = 4)]
        public gpBioDiferencaCBS gpBioDiferenca { get; set; }
    }
}
