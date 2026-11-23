using System.Collections.Generic;
using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Pagamento
{
    /// <summary>
    ///     Grupo de Informações da Vinculação com a Transação de Pagamento (NT 2026.006)
    /// </summary>
    public class gPgtoVinc
    {
        /// <summary>
        ///     Dados do pagamento vinculado
        ///     <para>Ocorrência: 1-99</para>
        /// </summary>
        [XmlElement("gPgto")]
        public List<gPgto> gPgto { get; set; }
    }

    /// <summary>
    ///     Dados do pagamento vinculado ao DF-e (NT 2026.006)
    /// </summary>
    public class gPgto
    {
        /// <summary>
        ///     Número sequencial do pagamento (1 a 990)
        /// </summary>
        [XmlAttribute]
        public string nPag { get; set; }

        /// <summary>
        ///     Identificador da transação financeira, de acordo com a transação de pagamento (2 a 35 caracteres)
        /// </summary>
        [XmlAttribute]
        public string idTransacao { get; set; }

        /// <summary>
        ///     Código do meio de pagamento, 2 dígitos (ver IT DFe 2026.001)
        /// </summary>
        public string tpMeioPgto { get; set; }

        /// <summary>
        ///     CNPJ do recebedor do pagamento. Informar zeros não significativos
        /// </summary>
        public string CNPJReceb { get; set; }

        /// <summary>
        ///     CNPJ base da instituição financeira ou de pagamento. Informar zeros não significativos
        /// </summary>
        public string CNPJBasePSP { get; set; }
    }
}