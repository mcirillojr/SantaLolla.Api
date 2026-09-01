using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.ClientesVarejo;
using SantaLolla.Api.Models.PagedResponse;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class ClienteVarejoRepository : IClienteVarejoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ClienteVarejoRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedResponse<ClienteVarejoResponse>> ListarAsync(
            ClienteVarejoFiltroRequest filtro
        )
        {
            filtro.Pagina =
                filtro.Pagina <= 0
                    ? 1
                    : filtro.Pagina;

            if (filtro.TamanhoPagina <= 0)
            {
                filtro.TamanhoPagina = 500;
            }

            if (filtro.TamanhoPagina > 5000)
            {
                filtro.TamanhoPagina = 5000;
            }

            var offset =
                (filtro.Pagina - 1) *
                filtro.TamanhoPagina;

            var nome =
                PrepararFiltroLike(filtro.Nome);

            var cpfCnpj =
                PrepararFiltroLike(filtro.CpfCnpj);

            const string sql = @"
                SELECT
                    COUNT(1)
                FROM dbo.SETA_CLIENTES_VAREJO C WITH (NOLOCK)
                WHERE C.ATIVO = 1
                  AND (
                        @Rede IS NULL
                        OR C.REDE = @Rede
                      )
                  AND (
                        @CodigoCliente IS NULL
                        OR C.CODIGO_CLIENTE = @CodigoCliente
                      )
                  AND (
                        @Nome IS NULL
                        OR C.NOME LIKE @Nome
                      )
                  AND (
                        @CpfCnpj IS NULL
                        OR C.CPFCNPJ LIKE @CpfCnpj
                      )
                  AND (
                        @AtualizadoInicio IS NULL
                        OR C.ATUALIZADO >= @AtualizadoInicio
                      )
                  AND (
                        @AtualizadoFim IS NULL
                        OR C.ATUALIZADO <= @AtualizadoFim
                      );

                SELECT
                    C.REDE AS Rede,
                    C.CODIGO_CLIENTE AS CodigoCliente,

                    ULTIMA_LOJA.MARCA AS Marca,

                    C.NOME AS Nome,
                    C.APELIDO AS Apelido,
                    C.PESSOA AS Pessoa,

                    C.CPFCNPJ AS CpfCnpj,
                    C.RGIE AS RgIe,
                    C.DOCAUXILIAR AS DocAuxiliar,

                    C.EMAIL AS Email,
                    C.TELEFONE1 AS Telefone1,
                    C.TELEFONE2 AS Telefone2,
                    C.TELEFONE3 AS Telefone3,
                    C.TELEFONE4 AS Telefone4,

                    C.CEP AS Cep,
                    C.ENDERECO AS Endereco,
                    C.BAIRRO AS Bairro,
                    C.CIDADE AS Cidade,
                    C.UF AS Uf,
                    C.COMPLEMENTO AS Complemento,
                    C.ENDERECO_COMPLETO AS EnderecoCompleto,

                    C.NATURALIDADE AS Naturalidade,
                    C.ORIGEM AS Origem,
                    C.ESTADOCIVIL AS EstadoCivil,
                    C.SEXO AS Sexo,
                    C.NASCIMENTO AS Nascimento,
                    C.ANIVERSARIO AS Aniversario,

                    C.STATUS AS Status,
                    C.GRUPO AS Grupo,
                    C.ATIVIDADE AS Atividade,
                    C.DESCRICAO_ATIVIDADE AS DescricaoAtividade,

                    C.RESPONSAVEL AS Responsavel,
                    C.NOME_RESPONSAVEL AS NomeResponsavel,

                    C.EMPRESA AS Empresa,
                    C.CLIENTE AS Cliente,
                    C.FORNECEDOR AS Fornecedor,
                    C.FUNCIONARIO AS Funcionario,
                    C.TRANSPORTADORA AS Transportadora,
                    C.CONVENIADO AS Conveniado,

                    C.CREDITO AS Credito,
                    C.BLOQUEIA AS Bloqueia,

                    C.CADASTRO AS Cadastro,
                    C.ATUALIZADO AS Atualizado,

                    ULTIMA_VENDA.DATA_VENDA AS DataUltimaCompra,

                    C.OBS AS Obs

                FROM dbo.SETA_CLIENTES_VAREJO C WITH (NOLOCK)

                OUTER APPLY
                (
                    SELECT TOP 1
                        V.DATA_VENDA,
                        V.CODIGO_EMPRESA
                    FROM dbo.SETA_VENDAS_DETALHE V WITH (NOLOCK)
                    WHERE V.REDE = C.REDE
                      AND V.CODCLIENTE = C.CODIGO_CLIENTE
                    ORDER BY
                        V.DATA_VENDA DESC,
                        V.LASTUPDATE_ORIGEM DESC,
                        V.ID_VENDA_DETALHE DESC
                ) ULTIMA_VENDA

                OUTER APPLY
                (
                    SELECT TOP 1
                        L.MARCA
                    FROM dbo.SETA_LOJAS L WITH (NOLOCK)
                    WHERE L.REDE = C.REDE
                      AND L.CODIGO_EMPRESA =
                          ULTIMA_VENDA.CODIGO_EMPRESA
                      AND L.ATIVO = 1
                    ORDER BY
                        L.ID_LOJA DESC
                ) ULTIMA_LOJA

                WHERE C.ATIVO = 1
                  AND (
                        @Rede IS NULL
                        OR C.REDE = @Rede
                      )
                  AND (
                        @CodigoCliente IS NULL
                        OR C.CODIGO_CLIENTE = @CodigoCliente
                      )
                  AND (
                        @Nome IS NULL
                        OR C.NOME LIKE @Nome
                      )
                AND (
                      @CpfCnpj IS NULL
                      OR C.CPFCNPJ = @CpfCnpj
                    )
                  AND (
                        @AtualizadoInicio IS NULL
                        OR C.ATUALIZADO >= @AtualizadoInicio
                      )
                  AND (
                        @AtualizadoFim IS NULL
                        OR C.ATUALIZADO <= @AtualizadoFim
                      )

                ORDER BY
                    C.REDE,
                    C.CODIGO_CLIENTE

                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            var parametros = new
            {
                Rede =
                    NormalizarTexto(filtro.Rede),

                CodigoCliente =
                    NormalizarTexto(filtro.CodigoCliente),

                Nome = nome,
                CpfCnpj = NormalizarTexto(filtro.CpfCnpj),

                filtro.AtualizadoInicio,
                filtro.AtualizadoFim,

                Offset = offset,
                filtro.TamanhoPagina
            };

            using var connection =
                _connectionFactory.CreateConnection();

            using var resultado =
                await connection.QueryMultipleAsync(
                    sql,
                    parametros
                );

            var total =
                await resultado.ReadSingleAsync<int>();

            var clientes = (
                await resultado.ReadAsync<ClienteVarejoResponse>()
            ).ToList();

            return PagedResponse<ClienteVarejoResponse>.Create(
                clientes,
                total,
                filtro.Pagina,
                filtro.TamanhoPagina
            );
        }

        private static string? NormalizarTexto(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static string? PrepararFiltroLike(
            string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            valor = valor.Trim();

            if (valor.Contains('%'))
            {
                return valor;
            }

            return $"%{valor}%";
        }
    }
}