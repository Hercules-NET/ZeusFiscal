using System.Xml.Serialization;

namespace NFe.Classes.Servicos.Evento
{
    /// <summary>
    /// Informações de quantidade de estoque influenciadas pelos eventos 112130 e 211124.
    /// </summary>
    public class gControleEstoque
    {
        private decimal _qPerecimento;
        private decimal? _vIBS;
        private decimal? _vCBS;

        /// <summary>
        /// Quantidade objeto de roubo, perda, furto ou perecimento.
        /// </summary>
        [XmlElement("qPerecimento")]
        public decimal qPerecimento
        {
            get { return _qPerecimento.Arredondar(4); }
            set { _qPerecimento = value.Arredondar(4); }
        }

        /// <summary>
        /// Unidade relativa ao campo qPerecimento.
        /// </summary>
        [XmlElement("uPerecimento")]
        public string uPerecimento { get; set; }

        /// <summary>
        /// Valor do crédito IBS referente às aquisições a ser estornado.
        /// <para>Obrigatório no evento 112130 e não informado no evento 211124.</para>
        /// </summary>
        [XmlElement("vIBS")]
        public decimal? vIBS
        {
            get { return _vIBS.GetValueOrDefault(); }
            set { _vIBS = value?.Arredondar(2); }
        }

        /// <summary>
        /// Valor do crédito CBS referente às aquisições a ser estornado.
        /// <para>Obrigatório no evento 112130 e não informado no evento 211124.</para>
        /// </summary>
        [XmlElement("vCBS")]
        public decimal? vCBS
        {
            get { return _vCBS.GetValueOrDefault(); }
            set { _vCBS = value?.Arredondar(2); }
        }

        public bool ShouldSerializevIBS()
        {
            return _vIBS.HasValue;
        }

        public bool ShouldSerializevCBS()
        {
            return _vCBS.HasValue;
        }
    }
}
