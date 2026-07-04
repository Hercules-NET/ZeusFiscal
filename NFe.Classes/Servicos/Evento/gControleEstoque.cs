using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade de estoque influenciadas pelo evento 112130.
    /// </summary>
    public class gControleEstoque
    {
        /// <summary>
        /// Quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("qPerecimento")]
        public decimal qPerecimento { get; set; }

        /// <summary>
        /// Unidade relativa ao campo qPerecimento.
        /// </summary>
        [XmlElement("uPerecimento")]
        public string uPerecimento { get; set; }

        /// <summary>
        /// Valor do crédito IBS referente às aquisições a ser estornado.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS { get; set; }

        /// <summary>
        /// Valor do crédito CBS referente às aquisições a ser estornado.
        /// </summary>
        [XmlElement("vCBS")]
        public decimal vCBS { get; set; }
    }
}