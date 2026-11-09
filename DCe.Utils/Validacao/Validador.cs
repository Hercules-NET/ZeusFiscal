using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace DCe.Utils.Validacao;

/// <summary>Validação de XML pelos XSDs oficiais embutidos ou externos da DC-e.</summary>
public static class Validador
{
    public static void Valida(string xml, string pasta, string schema, string? caminhoSchemas = null)
    {
        var configuracao = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            ValidationType = ValidationType.Schema,
            Schemas = DceSchemaLoader.Carregar(pasta, schema, caminhoSchemas),
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        };
        configuracao.ValidationEventHandler += (_, evento) =>
            throw new XmlSchemaValidationException("Erro de validação da DC-e: " + evento.Message, evento.Exception);
        using (var texto = new StringReader(xml))
        using (var leitor = XmlReader.Create(texto, configuracao))
            while (leitor.Read()) { }
    }
}
