using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    public class gIBSUF
    {
        private decimal _pIbsUf;
        private decimal _vIbsUf;

        /// <summary>
        /// UB18 - Alíquota do IBS de competência das UF (em percentual)
        /// </summary>
        [XmlElement(Order = 1)]
        public decimal pIBSUF
        {
            get => _pIbsUf.Arredondar(4);
            set => _pIbsUf = value.Arredondar(4);
        }

        /// <summary>
        /// UB21 - Grupo de Informações do Diferimento
        /// </summary>
        [XmlElement(Order = 2)]
        public gDif gDif { get; set; }

        /// <summary>
        /// UB24 - Grupo de Informações da devolução de tributos
        /// </summary>
        [XmlElement(Order = 3)]
        public gDevTrib gDevTrib { get; set; }

        /// <summary>
        /// UB26 - Grupo de informações da redução da alíquota
        /// </summary>
        [XmlElement(Order = 4)]
        public gRed gRed { get; set; }

        /// <summary>
        /// UB35 - Valor do IBS de competência da UF
        /// </summary>
        [XmlElement(Order = 5)]
        public decimal vIBSUF
        {
            get => _vIbsUf.Arredondar(2);
            set => _vIbsUf = value.Arredondar(2);
        }
    }

    public class gDif
    {
        private decimal _pDif;
        private decimal _vDif;

        /// <summary>
        /// UB22 - Percentual do diferimento
        /// </summary>
        public decimal pDif
        {
            get => _pDif.Arredondar(4);
            set => _pDif = value.Arredondar(4);
        }

        /// <summary>
        /// UB23 - Valor do Diferimento
        /// </summary>
        public decimal vDif
        {
            get => _vDif.Arredondar(2);
            set => _vDif = value.Arredondar(2);
        }
    }

    public class gDevTrib
    {
        private decimal _vDevTrib { get; set; }

        /// <summary>
        /// UB25 - Valor do tributo devolvido
        /// </summary>
        public decimal vDevTrib
        {
            get => _vDevTrib.Arredondar(2);
            set => _vDevTrib = value.Arredondar(2);
        }
    }

    public class gRed
    {
        private decimal _pRedAliq;
        private decimal _pAliqEfet;

        /// <summary>
        /// UB27 - Percentual da redução de alíquota do cClassTrib
        /// </summary>
        public decimal pRedAliq
        {
            get => _pRedAliq.Arredondar(4);
            set => _pRedAliq = value.Arredondar(4);
        }

        /// <summary>
        /// UB28 - Alíquota Efetiva do IBS de competência das UF que será aplicada a Base de Cálculo(em percentual)
        /// </summary>
        public decimal pAliqEfet
        {
            get => _pAliqEfet.Arredondar(4);
            set => _pAliqEfet = value.Arredondar(4);
        }
    }
}