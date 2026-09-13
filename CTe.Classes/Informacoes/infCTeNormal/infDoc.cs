using System.Collections.Generic;
using System.Xml.Serialization;
using CTe.Classes.Informacoes.infCTeNormal.infDocumentos;

namespace CTe.Classes.Informacoes.infCTeNormal
{
    public class infDoc
    {
        [XmlElement(ElementName = "infNF")]
        public List<infNF> infNF { get; set; }

        [XmlElement(ElementName = "infNFe")]
        public List<infNFe> infNFe { get; set; }

        [XmlElement(ElementName = "infOutros")]
        public List<infOutros> infOutros { get; set; }

        /// <summary>
        ///     Informações das DCe (modelo 99) transportadas pelo CT-e.
        ///     <para>Grupo criado pela NT 2025.001, disponível somente no leiaute 4.00.</para>
        /// </summary>
        [XmlElement(ElementName = "infDCe")]
        public List<infDCe> infDCe { get; set; }

        public string nCont { get; set; }

        public string dPrev { get; set; }
    }
}