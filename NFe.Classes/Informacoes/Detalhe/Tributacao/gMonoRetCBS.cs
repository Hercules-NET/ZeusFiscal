using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB98/UB103 - Grupo de informações da Tributação Monofásica Retida Anteriormente da CBS (tag gMonoRet)
    /// </summary>
    public class gMonoRetCBS
    {
        private decimal _vCbsMonoRet;

        // UB98a/UB103a
        [XmlElement(Order = 1)]
        public decimal vCBSMonoRet
        {
            get => _vCbsMonoRet.Arredondar(2);
            set => _vCbsMonoRet = value.Arredondar(2);
        }
    }
}
