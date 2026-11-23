using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB89/UB94 - Grupo de informações sobre mistura de EAC com gasolina A em percentual inferior ou superior ao obrigatório, para o IBS (tag gpBioDiferenca)
    /// </summary>
    public class gpBioDiferencaIBS
    {
        private decimal _qBcBioComb;
        private decimal _vIbsDiferenca;

        // UB89a/UB94a
        [XmlElement(Order = 1)]
        public decimal qBCBioComb
        {
            get => _qBcBioComb.Arredondar(4);
            set => _qBcBioComb = value.Arredondar(4);
        }

        // UB89b/UB94b
        [XmlElement(Order = 2)]
        public decimal vIBSDiferenca
        {
            get => _vIbsDiferenca.Arredondar(2);
            set => _vIbsDiferenca = value.Arredondar(2);
        }
    }
}
