using CTe.Classes.Informacoes.Impostos.IBSCBS;
using CTe.Classes.Informacoes.Tipos;
using System.Xml.Serialization;

namespace CTe.Classes.Informacoes.Impostos.Tributacao
{
    public class IBSCBS
    {
        [XmlElement(Order = 1)]
        public CSTIBSCBS CST { get; set; }

        [XmlElement(Order = 2)]
        public cClassTrib cClassTrib { get; set; }

        [XmlElement(Order = 3)]
        public short? indDoacao { get; set; }

        [XmlElement(Order = 4)]
        public gIBSCBS gIBSCBS { get; set; }

        //define se o campo indDoacao deve ser serializado
        public bool ShouldSerializeindDoacao()
        {
            return indDoacao.HasValue;
        }
    }
}
