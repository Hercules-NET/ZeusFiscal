using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using System.Xml;
using System.Xml.Schema;
using DCe.Utils;
using DCe.Utils.Validacao;
using DCe.Utils.Extensoes;
using DCe.Classes.Servicos.StatusServico;
using DCe.Testes.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DCe.Testes;

/// <summary>
/// O teste de include é o mais importante deste módulo. No .NET Core, XmlResolver nulo faz
/// xs:include falhar EM SILÊNCIO: o schema carrega, a validação roda, e os tipos do arquivo
/// incluído não são verificados. Como dce_v1.00.xsd é um wrapper de 10 linhas que inclui
/// dceTiposBasico_v1.00.xsd, include quebrado significa validar NADA.
/// </summary>
[TestClass]
public class DceSchemasTest
{
    private const string Ns = "http://www.portalfiscal.inf.br/dce";

    private static Stream Recurso(string pasta, string nome)
    {
        var completo = $"DCe.Classes.Schemas.{pasta}.{nome}";
        var stream = typeof(global::DCe.Classes.Informacoes.DCe).Assembly.GetManifestResourceStream(completo);
        Assert.IsTrue(stream is not null, $"recurso embutido não encontrado: {completo}");
        return stream!;
    }

    [DataTestMethod]
    [DataRow("dce", "dce_v1.00.xsd")]
    [DataRow("dce", "dceTiposBasico_v1.00.xsd")]
    [DataRow("dce", "retDCe_v1.00.xsd")]
    [DataRow("dce", "procDCe_v1.00.xsd")]
    [DataRow("consulta", "consSitDCe_v1.00.xsd")]
    [DataRow("consulta", "retConsSitDCe_v1.00.xsd")]
    [DataRow("consulta", "consStatServDCe_v1.00.xsd")]
    [DataRow("consulta", "retConsStatServDCe_v1.00.xsd")]
    [DataRow("consulta", "dceTiposBasico_v1.00.xsd")]
    [DataRow("evento", "eventoDCe_v1.00.xsd")]
    [DataRow("evento", "retEventoDCe_v1.00.xsd")]
    [DataRow("evento", "evCancDCe_v1.00.xsd")]
    [DataRow("evento", "procEventoDCe_v1.00.xsd")]
    [DataRow("evento", "eventoDCeTiposBasico_v1.00.xsd")]
    public void CadaSchemaEstaEmbutido(string pasta, string nome)
    {
        using var stream = Recurso(pasta, nome);
        Assert.IsTrue(stream.Length > 0);
        Assert.IsTrue(DceSchemaLoader.Carregar(pasta, nome).IsCompiled);
    }

    /// <summary>
    /// Os dois dceTiposBasico_v1.00.xsd são recursos DISTINTOS e com conteúdo diferente: o de
    /// consulta/ é a versão pré-NT 2024.001 (tpEmit vai só até 3). O pacote oficial é
    /// inconsistente e espelhamos isso de propósito — unificar pelo mais novo faria a
    /// consulta validar contra schema que a SEFAZ não usa para ela.
    /// </summary>
    [TestMethod]
    public void OsDoisDceTiposBasico_SaoRecursosDistintosEDiferentes()
    {
        using var deDce = new StreamReader(Recurso("dce", "dceTiposBasico_v1.00.xsd"));
        using var deConsulta = new StreamReader(Recurso("consulta", "dceTiposBasico_v1.00.xsd"));

        Assert.AreNotEqual(deDce.ReadToEnd(), deConsulta.ReadToEnd());
    }

    /// <summary>
    /// Prova positiva de que o include resolveu: TDCe é declarado em dceTiposBasico, não em
    /// dce_v1.00.xsd. Se o include falhasse, o SchemaSet compilaria com zero tipos globais
    /// além do elemento raiz — e validaria tudo como válido.
    /// </summary>
    [TestMethod]
    public void IncludeResolve_TiposDoArquivoIncluidoFicamDisponiveis()
    {
        var set = DceSchemaLoader.Carregar("dce", "dce_v1.00.xsd");

        Assert.IsTrue(set.GlobalTypes.Names.Cast<XmlQualifiedName>().Any(
            n => n.Name == "TDCe" && n.Namespace == Ns));
    }

    [TestMethod]
    public void SchemaRejeitaXmlQueNaoEhDCe()
    {
        var set = DceSchemaLoader.Carregar("dce", "dce_v1.00.xsd");
        var doc = new XmlDocument { Schemas = set };
        doc.LoadXml($"<DCe xmlns=\"{Ns}\" versao=\"1.00\"><naoExiste/></DCe>");

        var erros = new List<string>();
        doc.Validate((_, e) => erros.Add(e.Message));

        Assert.IsTrue(erros.Count > 0);
    }
    [TestMethod]
    public void CaminhoNulo_ValidaComSchemasEmbutidos()
    {
        var consulta = new consStatServDCe { tpAmb = TipoAmbiente.Homologacao, cUF = Estado.PE };
        Validador.Valida(consulta.ObterXmlString(), "consulta", "consStatServDCe_v1.00.xsd", null);
    }

    [TestMethod]
    public void PastaInexistente_ProduzErroComOCaminho()
    {
        var pasta = Path.Combine(Path.GetTempPath(), "dce-ausente-" + Guid.NewGuid().ToString("N"));
        var erro = Assert.ThrowsException<DirectoryNotFoundException>(() =>
            DceSchemaLoader.Carregar("dce", "dce_v1.00.xsd", pasta));
        StringAssert.Contains(erro.Message, "Diretório de Schemas não encontrado");
        StringAssert.Contains(erro.Message, pasta);
    }

    [TestMethod]
    public void PastaExterna_ResolveIncludesEHomonimosSemFallbackEmbutido()
    {
        var pasta = Path.Combine(Path.GetTempPath(), "dce-schemas-" + Guid.NewGuid().ToString("N"));
        var assembly = typeof(global::DCe.Classes.Informacoes.DCe).Assembly;
        const string prefixo = "DCe.Classes.Schemas.";
        try
        {
            var recursos = assembly.GetManifestResourceNames().Where(n => n.StartsWith(prefixo, StringComparison.Ordinal)).ToArray();
            Assert.AreEqual(22, recursos.Length);
            foreach (var recurso in recursos)
            {
                var relativo = recurso.Substring(prefixo.Length);
                var ponto = relativo.IndexOf('.');
                var destino = Path.Combine(pasta, relativo.Substring(0, ponto), relativo.Substring(ponto + 1));
                Directory.CreateDirectory(Path.GetDirectoryName(destino)!);
                using var origem = assembly.GetManifestResourceStream(recurso)!;
                using var arquivo = File.Create(destino);
                origem.CopyTo(arquivo);
            }
            foreach (var (subpasta, raiz) in new[] { ("dce", "dce_v1.00.xsd"), ("consulta", "consStatServDCe_v1.00.xsd"), ("evento", "evCancDCe_v1.00.xsd") })
                Assert.IsTrue(DceSchemaLoader.Carregar(subpasta, raiz, pasta).IsCompiled);
            var consulta = new consStatServDCe { tpAmb = TipoAmbiente.Homologacao, cUF = Estado.PE };
            Validador.Valida(consulta.ObterXmlString(), "consulta", "consStatServDCe_v1.00.xsd", pasta);

            // Marcadores distintos nos arquivos homonimos provam qual include foi resolvido.
            foreach (var (subpasta, marcador) in new[] { ("dce", "OrigemDeclaracao"), ("consulta", "OrigemConsulta") })
            {
                var homonimo = Path.Combine(pasta, subpasta, "dceTiposBasico_v1.00.xsd");
                var conteudo = File.ReadAllText(homonimo);
                File.WriteAllText(homonimo, conteudo.Replace("</xs:schema>",
                    $"<xs:simpleType name='{marcador}'><xs:restriction base='xs:string'/></xs:simpleType></xs:schema>"));
            }
            var schemaDce = DceSchemaLoader.Carregar("dce", "dce_v1.00.xsd", pasta);
            var schemaConsulta = DceSchemaLoader.Carregar("consulta", "dceTiposBasico_v1.00.xsd", pasta);
            Assert.IsTrue(schemaDce.GlobalTypes.Contains(new XmlQualifiedName("OrigemDeclaracao", Ns)));
            Assert.IsFalse(schemaDce.GlobalTypes.Contains(new XmlQualifiedName("OrigemConsulta", Ns)));
            Assert.IsTrue(schemaConsulta.GlobalTypes.Contains(new XmlQualifiedName("OrigemConsulta", Ns)));
            Assert.IsFalse(schemaConsulta.GlobalTypes.Contains(new XmlQualifiedName("OrigemDeclaracao", Ns)));
            var arquivoStatus = Path.Combine(pasta, "consulta", "consStatServDCeTiposBasico_v1.00.xsd");
            File.WriteAllText(arquivoStatus, File.ReadAllText(arquivoStatus).Replace("STATUS", "OUTRO"));
            Assert.ThrowsException<XmlSchemaValidationException>(() =>
                Validador.Valida(consulta.ObterXmlString(), "consulta", "consStatServDCe_v1.00.xsd", pasta));
            File.Delete(Path.Combine(pasta, "evento", "evCancDCe_v1.00.xsd"));
            Assert.ThrowsException<FileNotFoundException>(() =>
                DceSchemaLoader.Carregar("evento", "evCancDCe_v1.00.xsd", pasta));
        }
        finally { if (Directory.Exists(pasta)) Directory.Delete(pasta, true); }
    }

}
