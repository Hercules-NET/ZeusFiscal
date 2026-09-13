using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using DFe.Utils;
using NFe.Classes.Servicos.Evento;
using NFe.Classes.Servicos.Tipos;
using NFe.Utils.Assinatura;
using Shared.DFe.Utils;

namespace NFe.Utils.Evento
{
    public static class Extevento
    {
        /// <summary>
        ///     Converte o objeto evento para uma string no formato XML
        /// </summary>
        /// <param name="pedEvento"></param>
        /// <returns>Retorna uma string no formato XML com os dados do objeto evento</returns>
        public static string ObterXmlString(this evento pedEvento)
        {
            return FuncoesXml.ClasseParaXmlString(pedEvento);
        }

        /// <summary>
        ///     Obtém o Id de um evento (infEvento/@Id): literal "ID" + tpEvento + chNFe + nSeqEvento com 2 dígitos
        ///     <para>
        ///         Uso opcional, para quando o evento for montado fora da biblioteca — por exemplo, para assinar com o
        ///         certificado numa máquina cliente e depois transmitir com a sobrecarga que recebe o evento já
        ///         assinado. Os métodos que assinam internamente continuam calculando o Id sozinhos.
        ///     </para>
        /// </summary>
        /// <param name="tpEvento">Código do evento</param>
        /// <param name="chNFe">Chave de acesso da NF-e vinculada ao evento</param>
        /// <param name="nSeqEvento">Sequencial do evento para o mesmo tipo de evento</param>
        /// <returns>Retorna o conteúdo do atributo infEvento/@Id</returns>
        public static string ObterId(NFeTipoEvento tpEvento, string chNFe, int nSeqEvento)
        {
            return "ID" + ((int)tpEvento) + chNFe + nSeqEvento.ToString().PadLeft(2, '0');
        }

        /// <summary>
        ///     Assina um objeto evento
        /// </summary>
        /// <param name="evento"></param>
        /// <param name="certificadoDigital">Informe o certificado digital, se já possuir esse em cache, evitando novo acesso ao certificado</param>
        /// <param name="signatureMethodSignedXml"></param>
        /// <param name="digestMethodReference"></param>
        /// <param name="removerAcentos"></param>
        /// <returns>Retorna um objeto do tipo evento assinado</returns>
        public static evento Assina(this evento evento, X509Certificate2 certificadoDigital,
            string signatureMethodSignedXml = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
            string digestMethodReference = "http://www.w3.org/2000/09/xmldsig#sha1", bool removerAcentos = false)
        {
            var eventoLocal = evento;
            if (eventoLocal.infEvento.Id == null)
                throw new Exception("Não é possível assinar um objeto evento sem sua respectiva Id!");

            var assinatura = Assinador.ObterAssinatura(eventoLocal, eventoLocal.infEvento.Id, certificadoDigital, false, signatureMethodSignedXml, digestMethodReference, removerAcentos);
            eventoLocal.Signature = assinatura;
            return eventoLocal;
        }

        /// <summary>
        ///     descEvento que o XSD do evento só aceita acentuado, indexados pela forma sem acentos.
        ///     Os demais eventos (ex.: Carta de Correção, cujo XSD aceita as duas formas) seguem tendo os acentos removidos.
        /// </summary>
        private static readonly Dictionary<string, string> DescEventoSomenteComAcento = new[]
        {
            NFeTipoEvento.TeNfeCancConciliacaoFinanceiraNFe,
            NFeTipoEvento.TeNfePagamentoIntegralNFe,
            NFeTipoEvento.TeNfeImportacaoAlcZfmNFe,
            NFeTipoEvento.TeNfeFornecimentoNaoRealizadoNFe,
            NFeTipoEvento.TeNfeApropriacaoCredPresumidoNFe,
            NFeTipoEvento.TeNfeAceiteDebitoNotaCreditoNFe,
            NFeTipoEvento.TeNfeImobilizacaoItemNFe,
            NFeTipoEvento.TeNfeApropriacaoCreditoCombustivelNFe,
            NFeTipoEvento.TeNfeApropriacaoCreditoBensServicosNFe,
            NFeTipoEvento.TeNfeManifestacaoTransfCredIBSNFe,
            NFeTipoEvento.TeNfeManifestacaoTransfCredCBSNFe,
            NFeTipoEvento.TeNfeManifestacaoFiscoTransfCredIBSNFe,
            NFeTipoEvento.TeNfeManifestacaoFiscoTransfCredCBSNFe
        }.Select(t => t.Descricao()).ToDictionary(d => d.RemoverAcentos(), d => d);

        /// <summary>
        ///     Remove os acentos do XML, mantendo o descEvento dos eventos cujo XSD só aceita a descrição acentuada
        ///     (ex.: "Imobilização de Item")
        /// </summary>
        public static string RemoverAcentosPreservandoDescEvento(this string xml)
        {
            var semAcentos = xml.RemoverAcentos();
            if (string.IsNullOrEmpty(semAcentos) || semAcentos.IndexOf("<descEvento>", StringComparison.Ordinal) < 0)
                return semAcentos;

            return Regex.Replace(semAcentos, "<descEvento>([^<]*)</descEvento>", m =>
            {
                string descricao;
                return DescEventoSomenteComAcento.TryGetValue(m.Groups[1].Value, out descricao)
                    ? "<descEvento>" + descricao + "</descEvento>"
                    : m.Value;
            });
        }
    }
}