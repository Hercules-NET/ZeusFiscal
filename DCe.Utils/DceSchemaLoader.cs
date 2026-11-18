using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace DCe.Utils;

/// <summary>
/// Carrega os XSD da DC-e dos recursos embutidos ou de uma pasta, resolvendo os xs:include.
/// Sem o resolver, o include falha em silêncio no .NET Core e a validação vira decorativa.
/// </summary>
public static class DceSchemaLoader
{
    private const string Prefixo = "DCe.Classes.Schemas.";
    private const string Ns = "http://www.portalfiscal.inf.br/dce";

    /// <param name="pasta">"dce", "consulta" ou "evento" — a subpasta do pacote oficial.</param>
    /// <param name="arquivoRaiz">Nome do XSD raiz dentro dessa pasta.</param>
    /// <param name="caminhoSchemas">Pasta dos schemas com suas subpastas oficiais; nulo usa os embutidos.</param>
    public static XmlSchemaSet Carregar(string pasta, string arquivoRaiz, string? caminhoSchemas = null)
    {
        if (!string.IsNullOrWhiteSpace(caminhoSchemas))
        {
            if (!Directory.Exists(caminhoSchemas))
                throw new DirectoryNotFoundException("Diretório de Schemas não encontrado: " + caminhoSchemas);
            var externos = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
            // O caminho completo mantém os includes relativos na subpasta correta.
            var arquivo = Path.GetFullPath(Path.Combine(caminhoSchemas, pasta, arquivoRaiz));
            using var leitor = XmlReader.Create(arquivo, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null
            });
            externos.Add(Ns, leitor);
            externos.Compile();
            return externos;
        }

        var resolver = new RecursoEmbutidoResolver(pasta);
        var set = new XmlSchemaSet { XmlResolver = resolver };

        using var stream = Abrir(pasta, arquivoRaiz);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { XmlResolver = resolver });
        set.Add(Ns, reader);
        set.Compile();

        return set;
    }

    internal static Stream Abrir(string pasta, string arquivo)
    {
        var assembly = typeof(DCe.Classes.Informacoes.DCe).Assembly;
        var nome = $"{Prefixo}{pasta}.{arquivo}";
        return assembly.GetManifestResourceStream(nome)
               ?? throw new FileNotFoundException($"Schema XSD embutido não encontrado: {nome}");
    }

    /// <summary>
    /// Resolve os xs:include dentro da MESMA subpasta do schema raiz. A pasta é parte da
    /// identidade: dceTiposBasico_v1.00.xsd existe em dce/ e em consulta/ com conteúdos
    /// diferentes, e resolver só pelo nome do arquivo colidiria os dois.
    /// </summary>
    private sealed class RecursoEmbutidoResolver : XmlUrlResolver
    {
        private readonly string _pasta;

        public RecursoEmbutidoResolver(string pasta) => _pasta = pasta;

        public override object GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            => Abrir(_pasta, Path.GetFileName(absoluteUri.LocalPath));

        public override Uri ResolveUri(Uri? baseUri, string? relativeUri)
            => new Uri("file:///" + (relativeUri ?? string.Empty));
    }
}
