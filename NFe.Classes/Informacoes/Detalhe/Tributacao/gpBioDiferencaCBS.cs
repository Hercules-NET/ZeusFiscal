using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB99/UB104 - Grupo de informações sobre mistura de EAC com gasolina A em percentual inferior ou superior ao obrigatório, para a CBS (tag gpBioDiferenca)
    /// </summary>
    public class gpBioDiferencaCBS
    {
        private decimal _qBcBioComb;
        private decimal _vCbsDiferenca;

        // UB99a/UB104a
        [XmlElement(Order = 1)]
        public decimal qBCBioComb
        {
            get => _qBcBioComb.Arredondar(4);
            set => _qBcBioComb = value.Arredondar(4);
        }

        // UB99b/UB104b
        [XmlElement(Order = 2)]
        public decimal vCBSDiferenca
        {
            get => _vCbsDiferenca.Arredondar(2);
            set => _vCbsDiferenca = value.Arredondar(2);
        }
    }
}
