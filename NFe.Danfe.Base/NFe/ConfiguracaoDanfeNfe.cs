using System;

namespace NFe.Danfe.Base.NFe
{
    public enum ImprimirUnidQtdeValor
    {
        Comercial,
        Tributavel,
        Ambos
    }

    public class ConfiguracaoDanfeNfe : ConfiguracaoDanfe
    {
        public ConfiguracaoDanfeNfe(byte[] logomarca = null, bool duasLinhas = true, bool documentoCancelado = false, bool quebrarLinhasObservacao = false) : this()
        {
            Logomarca = logomarca;
            DuasLinhas = duasLinhas;
            DocumentoCancelado = documentoCancelado;
            QuebrarLinhasObservacao = quebrarLinhasObservacao;
        }

        /// <summary>
        /// Construtor sem parâmetros para serialização
        /// </summary>
        private ConfiguracaoDanfeNfe()
        {
            Logomarca = null;
            DuasLinhas = true;
            DocumentoCancelado = false;
            QuebrarLinhasObservacao = true;
            ExibirResumoCanhoto = true;
            ResumoCanhoto = string.Empty;
            ChaveContingencia = string.Empty;
            ExibeCampoFatura = false;
            ExibeRetencoes = false;
            ImprimirISSQN = true;
            ImprimirDescPorc = false;
            ImprimirTotalLiquido = false;
            ImprimirUnidQtdeValor = ImprimirUnidQtdeValor.Comercial;
            ExibirTotalTributos = false;
            DecimaisValorUnitario = 2;
            DecimaisQuantidadeItem = 2;
            DataHoraImpressao = null;
            ExibirCanhoto = true;
        }

        public bool DuasLinhas { get; set; }

        public bool QuebrarLinhasObservacao { get; set; }

        public bool ExibeCampoFatura { get; set; }

        public bool ExibirResumoCanhoto { get; set; }

        public bool ExibeRetencoes { get; set; }

        /// <summary> Exibe o canhoto do DANFE. O canhoto pode ser suprimido quando o emitente não utilizar o bloco (NT 2026.010 - item 4.4). </summary>
        public bool ExibirCanhoto { get; set; }

        public string ResumoCanhoto { get; set; }

        public string ChaveContingencia { get; set; }

        public bool ImprimirISSQN { get; set; }

        public bool ImprimirDescPorc { get; set; }

        public bool ImprimirTotalLiquido { get; set; }

        public ImprimirUnidQtdeValor ImprimirUnidQtdeValor { get; set; }

        public bool ExibirTotalTributos { get; set; }

        public int DecimaisValorUnitario { get; set; }

        public int DecimaisQuantidadeItem { get; set; }

        public DateTime? DataHoraImpressao { get; set; }
    }
}