using DFe.Classes;
using CTe.Classes.Informacoes.Impostos.ReformaTributaria;

namespace CTe.Classes.Informacoes.Impostos
{
    public class imp
    {
        public Tributacao.ICMS ICMS { get; set; }

        private decimal? _vTotTrib;
        public decimal? vTotTrib
        {
            get { return _vTotTrib.Arredondar(2); }
            set { _vTotTrib = value.Arredondar(2); }
        }

        public bool vTotTribSpecified { get { return vTotTrib.HasValue; } }

        public string infAdFisco { get; set; }

        public ICMSUFFim ICMSUFFim { get; set; }

        public infTribFed infTribFed { get; set; }

        /// <summary>
        /// Grupo de Tributação Monofásica Padrão
        /// Reforma Tributária - NT 2025.001
        /// </summary>
        public gMonoPadrao gMonoPadrao { get; set; }

        /// <summary>
        /// Grupo de Tributação Monofásica com Alíquota Diferenciada
        /// Reforma Tributária - NT 2025.001
        /// </summary>
        public gMonoDif gMonoDif { get; set; }

        /// <summary>
        /// Grupo de Valores Totais da Tributação Monofásica
        /// Reforma Tributária - NT 2025.001
        /// </summary>
        public gMono gMono { get; set; }

        /// <summary>
        /// Indicador de alto desempenho logístico
        /// Tag opcional para reforma tributária (NT 2025.001)
        /// Valores: 0 - Não; 1 - Sim
        /// </summary>
        public byte? indAltoDesemp { get; set; }

        /// <summary>
        /// Se null, não aparece no xml
        /// </summary>
        public bool indAltoDesempSpecified { get { return indAltoDesemp.HasValue; } }
    }
}