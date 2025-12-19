using CTe.Classes.Informacoes.Impostos.IBSCBS;
using CTe.Classes.Informacoes.Tipos;
using System.Xml.Serialization;

namespace CTe.Classes.Informacoes.Impostos.Tributacao
{
    public class IBSCBS
    {
        private string _cClassTrib;

        [XmlElement(Order = 1)]
        public CSTIBSCBS CST { get; set; }

        [XmlElement(Order = 2)]
        public string cClassTrib
        {
            get => _cClassTrib;
            set => _cClassTrib = value;
        }

        [XmlElement(Order = 3)]
        public short? indDoacao { get; set; }

        [XmlElement(Order = 4)]
        public gIBSCBS gIBSCBS { get; set; }

        //define se o campo indDoacao deve ser serializado
        public bool ShouldSerializeindDoacao()
        {
            return indDoacao.HasValue;
        }

        /// <summary>
        /// Define o valor de cClassTrib a partir de um inteiro
        /// </summary>
        public void SetcClassTrib(int intValue)
        {
            _cClassTrib = intValue.ToString("D6");
        }
    }
}
