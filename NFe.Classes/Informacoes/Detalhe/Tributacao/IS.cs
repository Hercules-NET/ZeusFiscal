using System.Xml.Serialization;
using System;
using System.ComponentModel;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class IS
    {
        private decimal _vBcIs;
        private decimal _pIs;
        private decimal? _adRemIS;
        private decimal _qTrib;
        private decimal _vIs;
        private string _cClassTribIS;

        // UB02
        [XmlElement(Order = 1)]
        public string CSTIS { get; set; }

        // UB03
        [XmlElement(Order = 2)]
        public string cClassTribIS
        {
            get => _cClassTribIS;
            set => _cClassTribIS = value;
        }

        // UB05
        [XmlElement(Order = 3)]
        public decimal vBCIS
        {
            get => _vBcIs.Arredondar(2);
            set => _vBcIs = value.Arredondar(2);
        }

        // UB06
        [XmlElement(Order = 4)]
        public decimal pIS
        {
            get => _pIs.Arredondar(4);
            set => _pIs = value.Arredondar(4);
        }

        /// <summary>
        /// UB07 - Alíquota do Imposto Seletivo (por valor), conforme NT 2025.002 v1.40.
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal? adRemIS
        {
            get => _adRemIS.Arredondar(4);
            set => _adRemIS = value.Arredondar(4);
        }
        public bool ShouldSerializeadRemIS() => adRemIS.HasValue;

        /// <summary>
        /// Nome anterior da alíquota específica, mantido para compatibilidade de código.
        /// </summary>
        [Obsolete("Utilize adRemIS (NT 2025.002 v1.40).")]
        [XmlIgnore]
        public decimal? pISEspec
        {
            get => adRemIS;
            set => adRemIS = value;
        }
        public bool ShouldSerializepISEspec() => false;

        /// <summary>
        /// Lê a tag de XMLs antigos; a emissão utiliza exclusivamente adRemIS.
        /// O XmlSerializer ignora membros Obsolete, por isso a leitura usa este membro separado.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("pISEspec", Order = 6)]
        public decimal? pISEspecXml
        {
            get => adRemIS;
            set => adRemIS = value;
        }
        public bool ShouldSerializepISEspecXml() => false;

        // UB09
        [XmlElement(Order = 7)]
        public string uTrib { get; set; }
        public bool ShouldSerializeuTrib() => !string.IsNullOrEmpty(uTrib);

        // UB10
        [XmlElement(Order = 8)]
        public decimal qTrib
        {
            get => _qTrib.Arredondar(4);
            set => _qTrib = value.Arredondar(4);
        }
        public bool ShouldSerializeqTrib() => ShouldSerializeuTrib();

        // UB11
        [XmlElement(Order = 9)]
        public decimal vIS
        {
            get => _vIs.Arredondar(2);
            set => _vIs = value.Arredondar(2);
        }

        /// <summary>
        /// Define o valor de cClassTribIS a partir de um inteiro
        /// </summary>
        public void SetcClassTribIS(int intValue)
        {
            _cClassTribIS = intValue.ToString("D6");
        }

        /// <summary>
        /// Define o valor de CSTIS a partir de um inteiro
        /// </summary>
        /// <param name="intValue"></param>
        public void SetCSTIS(int intValue)
        {
            CSTIS = intValue.ToString("D3");
        }
    }
}
