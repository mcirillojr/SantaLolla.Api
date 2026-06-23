using Dapper;
using SantaLolla.Api.Data;
using SantaLolla.Api.Models.ClientesVarejo;
using SantaLolla.Api.Repositories.Interfaces;

namespace SantaLolla.Api.Repositories
{
    public class ClienteVarejoRepository : IClienteVarejoRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ClienteVarejoRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ClienteVarejoResponse>> ListarAsync(
            ClienteVarejoFiltroRequest filtro
        )
        {
            filtro.Pagina = filtro.Pagina <= 0 ? 1 : filtro.Pagina;

            if (filtro.TamanhoPagina <= 0)
            {
                filtro.TamanhoPagina = 500;
            }

            if (filtro.TamanhoPagina > 5000)
            {
                filtro.TamanhoPagina = 5000;
            }

            var offset = (filtro.Pagina - 1) * filtro.TamanhoPagina;

            var nome = PrepararFiltroLike(filtro.Nome);
            var cpfCnpj = PrepararFiltroLike(filtro.CpfCnpj);

            const string sql = @"
                SELECT
                    REDE AS Rede,
                    CODIGO_CLIENTE AS CodigoCliente,
                    NOME AS Nome,
                    APELIDO AS Apelido,
                    PESSOA AS Pessoa,

                    CPFCNPJ AS CpfCnpj,
                    RGIE AS RgIe,
                    DOCAUXILIAR AS DocAuxiliar,

                    EMAIL AS Email,
                    TELEFONE1 AS Telefone1,
                    TELEFONE2 AS Telefone2,
                    TELEFONE3 AS Telefone3,
                    TELEFONE4 AS Telefone4,

                    CEP AS Cep,
                    ENDERECO AS Endereco,
                    BAIRRO AS Bairro,
                    CIDADE AS Cidade,
                    UF AS Uf,
                    COMPLEMENTO AS Complemento,
                    ENDERECO_COMPLETO AS EnderecoCompleto,

                    NATURALIDADE AS Naturalidade,
                    ORIGEM AS Origem,
                    ESTADOCIVIL AS EstadoCivil,
                    SEXO AS Sexo,
                    NASCIMENTO AS Nascimento,
                    ANIVERSARIO AS Aniversario,

                    STATUS AS Status,
                    GRUPO AS Grupo,
                    ATIVIDADE AS Atividade,
                    DESCRICAO_ATIVIDADE AS DescricaoAtividade,

                    RESPONSAVEL AS Responsavel,
                    NOME_RESPONSAVEL AS NomeResponsavel,

                    EMPRESA AS Empresa,
                    CLIENTE AS Cliente,
                    FORNECEDOR AS Fornecedor,
                    FUNCIONARIO AS Funcionario,
                    TRANSPORTADORA AS Transportadora,
                    CONVENIADO AS Conveniado,

                    CREDITO AS Credito,
                    BLOQUEIA AS Bloqueia,
                    CADASTRO AS Cadastro,
                    ATUALIZADO AS Atualizado,

                    OBS AS Obs
                FROM dbo.SETA_CLIENTES_VAREJO
                WHERE ATIVO = 1
                  AND (@Rede IS NULL OR REDE = @Rede)
                  AND (@CodigoCliente IS NULL OR CODIGO_CLIENTE = @CodigoCliente)
                  AND (@Nome IS NULL OR NOME LIKE @Nome)
                  AND (@CpfCnpj IS NULL OR CPFCNPJ LIKE @CpfCnpj)
                  AND (@AtualizadoInicio IS NULL OR ATUALIZADO >= @AtualizadoInicio)
                  AND (@AtualizadoFim IS NULL OR ATUALIZADO <= @AtualizadoFim)
                ORDER BY
                    REDE,
                    CODIGO_CLIENTE
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY;
            ";

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<ClienteVarejoResponse>(
                sql,
                new
                {
                    Rede = NormalizarTexto(filtro.Rede),
                    CodigoCliente = NormalizarTexto(filtro.CodigoCliente),
                    Nome = nome,
                    CpfCnpj = cpfCnpj,
                    filtro.AtualizadoInicio,
                    filtro.AtualizadoFim,
                    Offset = offset,
                    filtro.TamanhoPagina
                }
            );
        }

        private static string? NormalizarTexto(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static string? PrepararFiltroLike(string? valor)
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