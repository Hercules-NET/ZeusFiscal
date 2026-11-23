using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using Xunit;

namespace NFe.Utils.Testes
{
    public class ISTestes
    {
        private const string NamespaceNFe = "http://www.portalfiscal.inf.br/nfe";
        private static readonly XmlSerializer Serializer = new XmlSerializer(
            typeof(IS), new XmlRootAttribute("IS") { Namespace = NamespaceNFe });

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Serializar_AdValoremSemUnidade_ValidaNoXsd(string unidade)
        {
            var imposto = CriarIS();
            imposto.uTrib = unidade;
            var xml = Serializar(imposto);

            AssertValido(xml);
            Assert.Null(xml.Element(XName.Get("uTrib", NamespaceNFe)));
            Assert.Null(xml.Element(XName.Get("qTrib", NamespaceNFe)));
            Assert.Null(xml.Element(XName.Get("adRemIS", NamespaceNFe)));
        }

        [Fact]
        public void Serializar_AliquotaEspecifica_ValidaNoXsd()
        {
            var imposto = CriarIS();
            imposto.adRemIS = 1.0000m;
            imposto.uTrib = "UN";
            imposto.qTrib = 100m;

            var xml = Serializar(imposto);

            AssertValido(xml);
            Assert.Equal(1m, (decimal)xml.Element(XName.Get("adRemIS", NamespaceNFe)));
            Assert.Null(xml.Element(XName.Get("pISEspec", NamespaceNFe)));
            Assert.Equal(new[] { "CSTIS", "cClassTribIS", "vBCIS", "pIS", "adRemIS", "uTrib", "qTrib", "vIS" },
                xml.Elements().Select(element => element.Name.LocalName));
        }

        [Fact]
        public void Desserializar_PISEspecAntigo_LeAliquotaEEmiteSomenteAdRemIS()
        {
            var xmlAntigo = XElement.Parse("<IS xmlns='http://www.portalfiscal.inf.br/nfe'>" +
                "<CSTIS>000</CSTIS><cClassTribIS>000001</cClassTribIS><vBCIS>1000</vBCIS><pIS>1</pIS>" +
                "<pISEspec>1.0000</pISEspec><uTrib>UN</uTrib><qTrib>100</qTrib><vIS>10</vIS></IS>");

            var imposto = Desserializar(xmlAntigo);

            Assert.Equal(1m, imposto.adRemIS);
            Assert.Equal("UN", imposto.uTrib);
            Assert.Equal(100m, imposto.qTrib);
            Assert.Equal(10m, imposto.vIS);
            var xmlAtual = Serializar(imposto);
            AssertValido(xmlAtual);
            Assert.Null(xmlAtual.Element(XName.Get("pISEspec", NamespaceNFe)));
            Assert.Equal(1m, (decimal)xmlAtual.Element(XName.Get("adRemIS", NamespaceNFe)));
        }

        [Fact]
        public void SerializarEDesserializar_PreservaAdRemISEDemaisCampos()
        {
            var imposto = CriarIS();
            imposto.adRemIS = 1.2345m;
            imposto.uTrib = "UN";
            imposto.qTrib = 100m;
            var primeiroXml = Serializar(imposto);

            var restaurado = Desserializar(primeiroXml);
            var segundoXml = Serializar(restaurado);

            Assert.Equal(1.2345m, restaurado.adRemIS);
            Assert.True(XNode.DeepEquals(primeiroXml, segundoXml));
            AssertValido(segundoXml);
        }

        [Theory]
        [InlineData(1.23456, 1.2346)]
        [InlineData(0, 0)]
        public void AdRemIS_ArredondaQuatroCasasEEmiteZero(decimal valor, decimal esperado)
        {
            var imposto = CriarIS();
            imposto.adRemIS = valor;

            var xml = Serializar(imposto);

            Assert.Equal(esperado, imposto.adRemIS);
            Assert.Equal(esperado, (decimal)xml.Element(XName.Get("adRemIS", NamespaceNFe)));
            AssertValido(xml);
        }

        [Fact]
        public void PISEspec_AliasDeCodigo_CompartilhaValorComAdRemIS()
        {
            var imposto = CriarIS();
            // A API obsoleta é exercitada intencionalmente para garantir compatibilidade de código.
#pragma warning disable CS0618
            imposto.pISEspec = 1.23456m;
            Assert.Equal(1.2346m, imposto.adRemIS);
            imposto.adRemIS = 2m;
            Assert.Equal(2m, imposto.pISEspec);
            imposto.pISEspec = null;
#pragma warning restore CS0618
            Assert.Null(imposto.adRemIS);

            var xml = Serializar(imposto);
            Assert.Null(xml.Element(XName.Get("pISEspec", NamespaceNFe)));
            Assert.Null(xml.Element(XName.Get("adRemIS", NamespaceNFe)));
            AssertValido(xml);
        }

        private static IS Desserializar(XElement xml)
        {
            using (var reader = xml.CreateReader())
                return (IS)Serializer.Deserialize(reader);
        }

        private static void AssertValido(XElement xml)
        {
            var erros = Validar(xml);
            Assert.True(erros.Count == 0, string.Join(Environment.NewLine, erros));
        }

        private static IS CriarIS()
        {
            return new IS { CSTIS = "000", cClassTribIS = "000001", vBCIS = 1000m, pIS = 1m, vIS = 10m };
        }

        private static XElement Serializar(IS imposto)
        {
            using (var writer = new StringWriter())
            {
                Serializer.Serialize(writer, imposto);
                return XElement.Parse(writer.ToString());
            }
        }

        private static List<string> Validar(XElement xml)
        {
            // O wrapper declara somente a raiz; o contrato TIS vem do XSD da biblioteca.
            var schema = new XmlSchema { TargetNamespace = NamespaceNFe, ElementFormDefault = XmlSchemaForm.Qualified };
            schema.Includes.Add(new XmlSchemaInclude
            {
                SchemaLocation = Path.Combine(AppContext.BaseDirectory, "Schemas", "DFeTiposBasicos_v1.00.xsd")
            });
            schema.Items.Add(new XmlSchemaElement { Name = "IS", SchemaTypeName = new XmlQualifiedName("TIS", NamespaceNFe) });
            var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
            schemas.Add(schema);
            schemas.Compile();

            var erros = new List<string>();
            new XDocument(xml).Validate(schemas, (sender, args) => erros.Add(args.Message));
            return erros;
        }
    }
}
