using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gTribCompraGov
    {
        private decimal _pAliqIbsUf;
        private decimal _vTribIbsUf;
        private decimal _pAliqIbsMun;
        private decimal _vTribIbsMun;
        private decimal _pAliqCbs;
        private decimal _vTribCbs;

        /// <summary>
        /// UB82b - Alíquota do IBS de competência do Estado (em percentual)
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal pAliqIBSUF
        {
            get => _pAliqIbsUf.Arredondar(4);
            set => _pAliqIbsUf = value.Arredondar(4);
        }

        /// <summary>
        /// UB82c - Valor do Tributo do IBS da UF calculado
        /// </summary>
        [XmlElement(Order = 2)]
        public decimal vTribIBSUF
        {
            get => _vTribIbsUf.Arredondar(2);
            set => _vTribIbsUf = value.Arredondar(2);
        }

        /// <summary>
        /// UB82d - Alíquota do IBS de competência do Município (em percentual)
        /// </summary>
        [XmlElement(Order = 3)]
        public decimal pAliqIBSMun
        {
            get => _pAliqIbsMun.Arredondar(4);
            set => _pAliqIbsMun = value.Arredondar(4);
        }

        /// <summary>
        /// UB82e - Valor do Tributo do IBS do Município calculado
        /// </summary>
        [XmlElement(Order = 4)]
        public decimal vTribIBSMun
        {
            get => _vTribIbsMun.Arredondar(2);
            set => _vTribIbsMun = value.Arredondar(2);
        }

        /// <summary>
        /// UB82f - Alíquota da CBS (em percentual)
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal pAliqCBS
        {
            get => _pAliqCbs.Arredondar(4);
            set => _pAliqCbs = value.Arredondar(4);
        }

        /// <summary>
        /// UB82g - Valor do Tributo da CBS calculado
        /// </summary>
        [XmlElement(Order = 6)]
        public decimal vTribCBS
        {
            get => _vTribCbs.Arredondar(2);
            set => _vTribCbs = value.Arredondar(2);
        }
    }
}