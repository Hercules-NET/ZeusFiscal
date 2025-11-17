using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class IBSCBS
    {
        // UB13
        [XmlElement(Order = 1)]
        public CSTIBSCBS CST { get; set; }

        // UB14
        [XmlElement(Order = 2)]
        public cClassTrib cClassTrib { get; set; }

        // UB14a
        [XmlElement(Order = 3)]
        public string indDoacao { get; set; } //nullable

        // UB15
        [XmlElement(Order = 4)]
        public gIBSCBS gIBSCBS { get; set; }

        // UB84
        [XmlElement(Order = 5)]
        public gIBSCBSMono gIBSCBSMono { get; set; }

        // UB106
        [XmlElement(Order = 6)]
        public gTransfCred gTransfCred { get; set; }

        // UB112
        [XmlElement(Order = 7)]
        public gAjusteCompet gAjusteCompet { get; set; }

        // UB116
        [XmlElement(Order = 8)]
        public gEstornoCred gEstornoCred { get; set; }

        // UB120
        [XmlElement(Order = 9)]
        public gCredPresOper gCredPresOper { get; set; }

        // UB131
        [XmlElement(Order = 10)]
        public gCredPresIBSZFM gCredPresIBSZFM { get; set; }

        public bool ShouldSerializeindDoacao()
        {
            return indDoacao != null;
        }
    }
}