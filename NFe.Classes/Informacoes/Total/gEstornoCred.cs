using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Total
{
    public class gEstornoCred
    {
        // UB59f 
        [XmlElement(Order = 1)]
        public decimal vIBSEstCred { get; set; }

        // UB59g 
        [XmlElement(Order = 2)]
        public decimal vCBSEstCred { get; set; }
    }
}