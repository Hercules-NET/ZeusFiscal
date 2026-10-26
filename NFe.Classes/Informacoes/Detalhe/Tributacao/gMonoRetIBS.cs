using System.Xml.Serialization;

namespace NFe.Classes.Informacoes.Detalhe.Tributacao
{
    /// <summary>
    /// UB88/UB93 - Grupo de informações da Tributação Monofásica Retida Anteriormente do IBS (tag gMonoRet)
    /// </summary>
    public class gMonoRetIBS
    {
        private decimal _vIbsMonoRet;

        // UB88a/UB93a
        [XmlElement(Order = 1)]
        public decimal vIBSMonoRet
        {
            get => _vIbsMonoRet.Arredondar(2);
            set => _vIbsMonoRet = value.Arredondar(2);
        }
    }
}
