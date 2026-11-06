using System.Globalization;
using DCe.Classes.Retorno;
using BarcodeStandard;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using SkiaSharp.QrCode.Image;

namespace DCe.Dace.QuestPdf;

/// <summary>Modelo de impressão previsto pelo Anexo II da DC-e.</summary>
public enum DaceModelo { Completa, Resumida }

/// <summary>
/// Gera a Declaração Auxiliar de Conteúdo eletrônica a partir de um <c>procDCe</c> autorizado.
/// A versão resumida omite apenas a relação de itens; nenhuma informação é calculada fora do XML.
/// </summary>
public sealed class DaceDocument : IDocument
{
    private static readonly CultureInfo Brasil = CultureInfo.GetCultureInfo("pt-BR");
    private readonly DaceViewModel _dados;
    private readonly DaceModelo _modelo;

    public DaceDocument(string procDCeXml, DaceModelo modelo = DaceModelo.Completa)
        : this(DaceXmlParser.Parse(procDCeXml), modelo) { }

    public DaceDocument(procDCe processado, DaceModelo modelo = DaceModelo.Completa)
        : this(DaceXmlParser.Parse(processado), modelo) { }

    public DaceDocument(DaceViewModel dados, DaceModelo modelo = DaceModelo.Completa)
    {
        _dados = dados ?? throw new ArgumentNullException(nameof(dados));
        _modelo = modelo;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        Title = "DACE " + _dados.ChaveAcesso,
        Author = "ZeusFiscal",
        Subject = "Declaração Auxiliar de Conteúdo eletrônica"
    };

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1, Unit.Centimetre);
            page.DefaultTextStyle(style => style.FontFamily("Arial").FontSize(8));
            page.Content().Column(column =>
            {
                column.Item().Element(Cabecalho);
                column.Item().PaddingTop(4).Element(Identificacao);
                column.Item().PaddingTop(4).Element(RemDest);
                if (_dados.ParteAdicional is not null)
                    column.Item().PaddingTop(4).Element(ParteAdicional);
                if (_modelo == DaceModelo.Completa)
                    column.Item().PaddingTop(4).Element(Itens);
                column.Item().PaddingTop(4).Element(DadosAdicionais);
                column.Item().PaddingTop(8).Element(Rodape);
                if (_modelo == DaceModelo.Resumida)
                    column.Item().PaddingTop(4).Element(Chave);
            });
        });
    }

    private void Cabecalho(IContainer container)
    {
        container.Border(0.5f).Row(row =>
        {
            row.RelativeItem(_modelo == DaceModelo.Completa ? 0.42f : 1).Padding(5).AlignCenter().Column(column =>
            {
                column.Item().Text("DACE").Bold().FontSize(16);
                column.Item().Text("DECLARAÇÃO AUXILIAR DE CONTEÚDO ELETRÔNICA").Bold().FontSize(6);
                column.Item().PaddingTop(3).Text("Nº " + _dados.Numero.ToString(@"000\.000\.000")).Bold();
                column.Item().Text("SÉRIE " + _dados.Serie.ToString("000"));
                if (_dados.EmHomologacao)
                {
                    column.Item().PaddingTop(3).Text("SEM VALOR FISCAL").FontColor(Colors.Red.Medium).Bold().FontSize(7);
                    column.Item().Text("AMBIENTE DE HOMOLOGAÇÃO").FontColor(Colors.Red.Medium).Bold().FontSize(7);
                }
            });
            if (_modelo == DaceModelo.Completa)
                row.RelativeItem().Padding(5).Element(Chave);
        });
    }

    private void Chave(IContainer container) => container.AlignCenter().Column(column =>
    {
        column.Item().Height(35).Image(CodigoBarras()).FitArea();
        column.Item().PaddingTop(2).Text(_dados.ChaveFormatada).FontSize(9);
    });

    private void ParteAdicional(IContainer container) => container.Column(column =>
    {
        column.Item().Element(c => Titulo(c, "IDENTIFICAÇÃO DO FISCO, MARKETPLACE, TRANSPORTADORA OU ECT"));
        column.Item().Row(row =>
        {
            row.RelativeItem(2).Element(c => Campo(c, "CNPJ (" + _dados.ParteAdicionalTipo + ")", _dados.ParteAdicional!.Documento));
            row.RelativeItem(5).Element(c => Campo(c, "NOME / ÓRGÃO", _dados.ParteAdicional!.Nome));
        });
    });

    private void DadosAdicionais(IContainer container) => container.Column(column =>
    {
        column.Item().Element(c => Titulo(c, "DADOS ADICIONAIS"));
        column.Item().Row(row =>
        {
            row.RelativeItem().Element(c => Campo(c, "INFORMAÇÕES COMPLEMENTARES", _dados.InformacoesComplementares ?? string.Empty));
            row.RelativeItem().Element(c => Campo(c, "INFORMAÇÕES ADICIONAIS DO FISCO", _dados.InformacoesAdicionaisFisco ?? string.Empty));
        });
    });

    private void Identificacao(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Element(c => Campo(c, "DATA DE EMISSÃO", _dados.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss", Brasil)));
                row.RelativeItem().Element(c => Campo(c, "MODALIDADE DO TRANSPORTE", _dados.ModalidadeTransporte));
            });
            column.Item().Element(c => Campo(c, "PROTOCOLO DE AUTORIZAÇÃO", _dados.CampoProtocolo));
        });
    }

    private void RemDest(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(c => Pessoa(c, "REMETENTE (USUÁRIO EMITENTE)", _dados.Remetente));
            column.Item().PaddingTop(4).Element(c => Pessoa(c, "DESTINATÁRIO", _dados.Destinatario));
        });
    }

    private void Pessoa(IContainer container, string titulo, DacePessoa pessoa)
    {
        container.Column(column =>
        {
            column.Item().Element(c => Titulo(c, "IDENTIFICAÇÃO DO " + titulo));
            column.Item().Row(row =>
            {
                row.RelativeItem(2).Element(c => Campo(c, "CNPJ/CPF", pessoa.Documento));
                row.RelativeItem(5).Element(c => Campo(c, "NOME", pessoa.Nome));
            });
            column.Item().Row(row =>
            {
                row.RelativeItem(2).Element(c => Campo(c, "CIDADE-UF", pessoa.CidadeUf));
                row.RelativeItem(5).Element(c => Campo(c, "ENDEREÇO", pessoa.Endereco));
            });
        });
    }

    private void Itens(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(c => Titulo(c, "IDENTIFICAÇÃO DOS BENS OU MERCADORIAS"));
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns => { columns.ConstantColumn(28); columns.RelativeColumn(5); columns.RelativeColumn(); columns.RelativeColumn(); columns.RelativeColumn(); });
                table.Header(header => { Header(header.Cell(), "ITEM"); Header(header.Cell(), "DESCRIÇÃO"); Header(header.Cell(), "QUANTIDADE"); Header(header.Cell(), "VALOR UNIT."); Header(header.Cell(), "VALOR TOTAL"); });
                foreach (var item in _dados.Itens)
                {
                    Cell(table.Cell(), item.Numero.ToString()); Cell(table.Cell(), item.Descricao); Cell(table.Cell(), item.Quantidade.ToString("N4", Brasil));
                    Cell(table.Cell(), item.ValorUnitario.ToString("C2", Brasil)); Cell(table.Cell(), item.ValorTotal.ToString("C2", Brasil));
                }
            });
            column.Item().AlignRight().Border(0.5f).Padding(3).Text("VALOR TOTAL: " + _dados.ValorTotal.ToString("C2", Brasil)).Bold();
        });
    }

    private void Rodape(IContainer container)
    {
        container.Border(0.5f).Column(column =>
        {
            column.Item().Row(row => { row.RelativeItem().Element(c => Titulo(c, "QR-CODE")); row.RelativeItem(2).Element(c => Titulo(c, "OBSERVAÇÕES")); });
            column.Item().Row(row =>
            {
                row.RelativeItem().Padding(4).AlignCenter().Height(90).Element(c =>
                {
                    if (!string.IsNullOrWhiteSpace(_dados.UrlQrCode))
                        c.Image(QrCode()).FitArea();
                });
                row.RelativeItem(2).Padding(4).Column(obs => { obs.Item().Text(_dados.ObservacaoLegal1).FontSize(6.5f); obs.Item().PaddingTop(2).Text(_dados.ObservacaoLegal2).FontSize(6.5f); });
            });
        });
    }

    private static void Campo(IContainer container, string titulo, string valor) => container.Border(0.5f).Padding(3).Column(column => { column.Item().Text(titulo).FontSize(6).Bold(); column.Item().Text(valor); });
    private static void Titulo(IContainer container, string valor) => container.Border(0.5f).Background(Colors.Grey.Lighten3).Padding(2).AlignCenter().Text(valor).FontSize(7).Bold();
    private static void Header(IContainer container, string valor) => container.Border(0.5f).Background(Colors.Grey.Lighten3).Padding(2).Text(valor).FontSize(7).Bold();
    private static void Cell(IContainer container, string valor) => container.Border(0.5f).Padding(2).Text(valor).FontSize(7);

    private byte[] QrCode()
    {
        using var stream = new MemoryStream();
        new QrCode(_dados.UrlQrCode!, new Vector2Slim(256, 256), SKEncodedImageFormat.Png).GenerateImage(stream);
        return stream.ToArray();
    }

    private byte[] CodigoBarras()
    {
        var barcode = new Barcode();
        using var image = barcode.Encode(BarcodeStandard.Type.Code128, _dados.ChaveAcesso, 1200, 140);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

}
