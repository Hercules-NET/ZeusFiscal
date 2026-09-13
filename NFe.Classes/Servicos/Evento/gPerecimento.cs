using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações por item da Nota de Fornecimento (evento 112130) ou da Nota de Aquisição (evento 211124).
    /// </summary>
    public class gPerecimento
    {
        private decimal _vIBS;
        private decimal _vCBS;
        /// <summary>
        /// Corresponde ao atributo nItem do elemento det da NF-e.
        /// </summary>
        [XmlAttribute("nItem")]
        public int nItem { get; set; }

        /// <summary>
        /// Valor do IBS na Nota de Fornecimento/Aquisição correspondente à quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("vIBS")]
        public decimal vIBS 
        { 
            get { return _vIBS.Arredondar(2); } 
            set { _vIBS = value.Arredondar(2); } 
        }

        /// <summary>
        /// Valor da CBS na Nota de Fornecimento/Aquisição correspondente à quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("vCBS")]
        public decimal vCBS
        {
            get { return _vCBS.Arredondar(2); }
            set { _vCBS = value.Arredondar(2); }
        }

        /// <summary>
        /// Informações de quantidade de estoque influenciadas pelo evento.
        /// </summary>
        [XmlElement("gControleEstoque")]
        public gControleEstoque gControleEstoque { get; set; }
    }
}