using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DCe.Testes.Fixtures.Entradas;

namespace DCe.Testes.Fixtures;

/// <summary>
/// DC-e canônica dos testes. Tudo fixo e determinístico: datas com DateTimeKind.Utc e
/// nenhuma dependência de relógio. O cDC (aleatório por especificação) NÃO mora aqui —
/// entra pelo IRandomSource injetado, senão nenhum teste de XML seria estável.
/// </summary>
internal static class DceFixture
{
    public static DeclaracaoConteudo Padrao() => new()
    {
        Ambiente = TipoAmbiente.Homologacao,
        CodigoUf = (Estado)26,
        Serie = 1,
        Numero = 1,
        TipoEmissao = 1,
        TipoEmitente = 3,
        SiteAutorizador = 0,
        VersaoAplicativo = "ZeusFiscal 1.0",
        Emitente = new PessoaDce
        {
            Cpf = "12345678909",
            Nome = "USUARIO NAO CONTRIBUINTE",
            Endereco = new EnderecoDce
            {
                Logradouro = "RUA EXEMPLO",
                Numero = "100",
                Bairro = "CENTRO",
                CodigoMunicipio = 2611101,
                Municipio = "PETROLINA",
                Uf = "PE",
                Cep = "56300000"
            }
        },
        Signatario = new SignatarioDce
        {
            Cnpj = "99999999000191",
            Nome = "TRANSPORTADORA EXEMPLO LTDA"
        },
        Destinatario = new PessoaDce
        {
            Cpf = "12345678909",
            Nome = "USUARIO NAO CONTRIBUINTE",
            Endereco = new EnderecoDce
            {
                Logradouro = "AVENIDA EXEMPLO",
                Numero = "200",
                Bairro = "BAIRRO EXEMPLO",
                CodigoMunicipio = 2611606,
                Municipio = "RECIFE",
                Uf = "PE",
                Cep = "51020000"
            }
        },
        Itens = new List<ItemDce>
        {
            new()
            {
                Descricao = "VEICULO USADO DE TESTE",
                Ncm = "87032310",
                Quantidade = 1m,
                ValorUnitario = 210342m,
                ValorTotal = 210342m
            }
        },
        Transporte = new TransporteDce
        {
            Modalidade = "2",                        // 2 = transporte por empresa transportadora
            CnpjTransportador = "99999999000191"
        },
        Solicitacao = "TRANSPORTE DE VEICULO USADO PARTICULAR"
    };
}
