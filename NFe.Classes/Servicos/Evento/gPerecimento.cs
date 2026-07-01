using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações por item da Nota de Fornecimento para o evento 112130.
    /// </summary>
    public class gPerecimento
    {
        /// <summary>
        /// Corresponde ao atributo nItem do elemento det da NF-e.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// Valor do IBS na Nota de Fornecimento correspondente à quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS { get; set; }

        /// <summary>
        /// Valor da CBS na Nota de Fornecimento correspondente à quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("vCBS")]
        public decimal vCBS { get; set; }

        /// <summary>
        /// Informações de quantidade de estoque influenciadas pelo evento.
        /// </summary>
        [XmlElement("gControleEstoque")]
        public gControleEstoque gControleEstoque { get; set; }
    }
}