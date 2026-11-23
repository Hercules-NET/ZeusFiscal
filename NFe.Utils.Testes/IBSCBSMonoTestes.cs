using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using Xunit;

namespace NFe.Utils.Testes
{
    /// <summary>
    /// Testes do grupo gIBSCBSMono no leiaute da NT 2025.002 v1.50 (PL_010f v1.04)
    /// </summary>
    public class IBSCBSMonoTestes
    {
        private const string NamespaceNfe = "http://www.portalfiscal.inf.br/nfe";

        //o DFeTiposBasicos não tem targetNamespace; o envelope o inclui no namespace da NF-e e declara o IBSCBS como no leiauteNFe
        private const string SchemaEnvelope =
            "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"" + NamespaceNfe + "\" targetNamespace=\"" + NamespaceNfe + "\" elementFormDefault=\"qualified\">" +
            "<xs:include schemaLocation=\"DFeTiposBasicos_v1.00.xsd\"/>" +
            "<xs:element name=\"IBSCBS\" type=\"TTribNFe\"/>" +
            "</xs:schema>";

        private static string SerializarComNamespaceNfe(IBSCBS ibscbs)
        {
            var serializador = new XmlSerializer(typeof(IBSCBS), NamespaceNfe);
            using (var sw = new StringWriter())
            {
                serializador.Serialize(sw, ibscbs);
                return sw.ToString();
            }
        }

        private static List<string> ValidarContraPl010f(string xml)
        {
            var erros = new List<string>();
            var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
            using (var reader = XmlReader.Create(new StringReader(SchemaEnvelope), new XmlReaderSettings(), Path.Combine(Directory.GetCurrentDirectory(), "Schemas", "envelope.xsd")))
                schemas.Add(NamespaceNfe, reader);

            var cfg = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = schemas };
            cfg.ValidationEventHandler += (sender, e) => erros.Add(e.Message);

            using (var reader = XmlReader.Create(new StringReader(xml), cfg))
                while (reader.Read()) { }

            return erros;
        }

        [Fact]
        public void Serializar_RevendaCobradaAnteriormente620006_GeraXmlValidoNoPl010f()
        {
            // Arrange
            var ibscbs = new IBSCBS
            {
                CST = CSTIBSCBS.cst620,
                cClassTrib = "620006",
                gIBSCBSMono = new gIBSCBSMono
                {
                    gIBSMonoAdValorem = new gIBSMonoAdValorem { gMonoRet = new gMonoRetIBS { vIBSMonoRet = 0.06m } },
                    gCBSMonoAdValorem = new gCBSMonoAdValorem { gMonoRet = new gMonoRetCBS { vCBSMonoRet = 0.54m } },
                    vTotIBSMonoItem = 0m,
                    vTotCBSMonoItem = 0m
                }
            };

            // Act
            var xml = SerializarComNamespaceNfe(ibscbs);

            // Assert
            Assert.Empty(ValidarContraPl010f(xml));
            Assert.Contains("<gIBSMonoAdValorem><gMonoRet><vIBSMonoRet>0.06</vIBSMonoRet></gMonoRet></gIBSMonoAdValorem>", xml.Replace("\r", "").Replace("\n", "").Replace(" ", ""));
            Assert.DoesNotContain("gMonoDif", xml);
        }

        [Fact]
        public void Serializar_MonofasiaAdRemComRetencaoEDiferencaDeBiocombustivel_GeraXmlValidoNoPl010f()
        {
            // Arrange
            var ibscbs = new IBSCBS
            {
                CST = CSTIBSCBS.cst620,
                cClassTrib = "620002",
                gIBSCBSMono = new gIBSCBSMono
                {
                    gIBSMonoAdRem = new gIBSMonoAdRem
                    {
                        gMonoPadrao = new gMonoPadraoIBSAdRem { qBCMono = 1000m, adRemIBS = 0.0123m, vIBSMono = 12.30m },
                        gMonoReten = new gMonoRetenIBSAdRem { qBCMonoReten = 500m, adRemIBSReten = 0.0123m, vIBSMonoReten = 6.15m },
                        gpBioDiferenca = new gpBioDiferencaIBS { qBCBioComb = 10.5m, vIBSDiferenca = 0.13m }
                    },
                    gCBSMonoAdRem = new gCBSMonoAdRem
                    {
                        gMonoPadrao = new gMonoPadraoCBSAdRem { qBCMono = 1000m, adRemCBS = 0.1107m, vCBSMono = 110.70m },
                        gMonoReten = new gMonoRetenCBSAdRem { qBCMonoReten = 500m, adRemCBSReten = 0.1107m, vCBSMonoReten = 55.35m }
                    },
                    vTotIBSMonoItem = 18.45m,
                    vTotCBSMonoItem = 166.05m
                }
            };

            // Act
            var xml = SerializarComNamespaceNfe(ibscbs);

            // Assert
            Assert.Empty(ValidarContraPl010f(xml));
        }

        [Fact]
        public void Serializar_MonofasiaAdValoremPadrao_GeraXmlValidoNoPl010f()
        {
            // Arrange
            var ibscbs = new IBSCBS
            {
                CST = CSTIBSCBS.cst620,
                cClassTrib = "620001",
                gIBSCBSMono = new gIBSCBSMono
                {
                    gIBSMonoAdValorem = new gIBSMonoAdValorem
                    {
                        gMonoPadrao = new gMonoPadraoIBSAdValorem { vBCMono = 1000m, pAliqMonoUF = 0.1m, vIBSMonoUF = 1m, pAliqMonoMun = 0m, vIBSMonoMun = 0m, vIBSMono = 1m }
                    },
                    gCBSMonoAdValorem = new gCBSMonoAdValorem
                    {
                        gMonoPadrao = new gMonoPadraoCBSAdValorem { vBCMono = 1000m, pAliqMonoCBS = 0.9m, vCBSMono = 9m }
                    },
                    vTotIBSMonoItem = 1m,
                    vTotCBSMonoItem = 9m
                }
            };

            // Act
            var xml = SerializarComNamespaceNfe(ibscbs);

            // Assert
            Assert.Empty(ValidarContraPl010f(xml));
        }

        [Fact]
        public void Serializar_LeiauteAnteriorAV150_EhRejeitadoPeloPl010f()
        {
            // Arrange
            var ibscbs = new IBSCBS
            {
                CST = CSTIBSCBS.cst620,
                cClassTrib = "620006",
                gIBSCBSMono = new gIBSCBSMono
                {
                    gMonoRet = new gMonoRet { qBCMonoRet = 100m, adRemIBSRet = 0.0006m, vIBSMonoRet = 0.06m, adRemCBSRet = 0.0054m, vCBSMonoRet = 0.54m },
                    vTotIBSMonoItem = 0m,
                    vTotCBSMonoItem = 0m
                }
            };

            // Act
            var xml = SerializarComNamespaceNfe(ibscbs);

            // Assert
            Assert.NotEmpty(ValidarContraPl010f(xml));
        }

        [Fact]
        public void Deserializar_XmlAutorizadoNoLeiauteAnterior_PreencheOsGruposAntigos()
        {
            // Arrange
            const string xml =
                "<IBSCBS><CST>620</CST><cClassTrib>620006</cClassTrib><gIBSCBSMono>" +
                "<gMonoRet><qBCMonoRet>100.0000</qBCMonoRet><adRemIBSRet>0.0006</adRemIBSRet><vIBSMonoRet>0.06</vIBSMonoRet><adRemCBSRet>0.0054</adRemCBSRet><vCBSMonoRet>0.54</vCBSMonoRet></gMonoRet>" +
                "<vTotIBSMonoItem>0.00</vTotIBSMonoItem><vTotCBSMonoItem>0.00</vTotCBSMonoItem>" +
                "</gIBSCBSMono></IBSCBS>";

            // Act
            var ibscbs = DFe.Utils.FuncoesXml.XmlStringParaClasse<IBSCBS>(xml);

            // Assert
            Assert.Equal(0.06m, ibscbs.gIBSCBSMono.gMonoRet.vIBSMonoRet);
            Assert.Equal(0.54m, ibscbs.gIBSCBSMono.gMonoRet.vCBSMonoRet);
            Assert.Null(ibscbs.gIBSCBSMono.gIBSMonoAdValorem);
        }

        [Fact]
        public void Deserializar_XmlNoLeiauteV150_PreencheOsGruposNovos()
        {
            // Arrange
            const string xml =
                "<IBSCBS><CST>620</CST><cClassTrib>620006</cClassTrib><gIBSCBSMono>" +
                "<gIBSMonoAdValorem><gMonoRet><vIBSMonoRet>0.06</vIBSMonoRet></gMonoRet></gIBSMonoAdValorem>" +
                "<gCBSMonoAdRem><gMonoRet><vCBSMonoRet>0.54</vCBSMonoRet></gMonoRet></gCBSMonoAdRem>" +
                "<vTotIBSMonoItem>0.00</vTotIBSMonoItem><vTotCBSMonoItem>0.00</vTotCBSMonoItem>" +
                "</gIBSCBSMono></IBSCBS>";

            // Act
            var ibscbs = DFe.Utils.FuncoesXml.XmlStringParaClasse<IBSCBS>(xml);

            // Assert
            Assert.Equal(0.06m, ibscbs.gIBSCBSMono.gIBSMonoAdValorem.gMonoRet.vIBSMonoRet);
            Assert.Equal(0.54m, ibscbs.gIBSCBSMono.gCBSMonoAdRem.gMonoRet.vCBSMonoRet);
            Assert.Null(ibscbs.gIBSCBSMono.gMonoRet);
        }
    }
}
