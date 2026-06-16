USE master;
GO

IF DB_ID('AlterVisionIntegracao') IS NULL
BEGIN
    CREATE DATABASE AlterVisionIntegracao;
END;
GO
----------------------------------------------------
USE AlterVisionIntegracao;
GO
-----------------------------------------------------

USE AlterVisionIntegracao;
GO

IF OBJECT_ID('DBO.ALTERVISION_LOJAS', 'U') IS NULL
BEGIN
    CREATE TABLE DBO.ALTERVISION_LOJAS (
        ID_LOJA INT IDENTITY(1,1) NOT NULL,

        REDE VARCHAR(20) NOT NULL,
        CODIGO_EMPRESA VARCHAR(20) NOT NULL,
        CNPJ VARCHAR(20) NOT NULL,

        APELIDO VARCHAR(150) NULL,
        NOME VARCHAR(250) NULL,

        ALIAS_ID VARCHAR(50) NULL,

        ATIVO BIT NOT NULL CONSTRAINT DF_ALTERVISION_LOJAS_ATIVO DEFAULT 1,

        DATA_CRIACAO DATETIME NOT NULL CONSTRAINT DF_ALTERVISION_LOJAS_DATA_CRIACAO DEFAULT GETDATE(),
        DATA_ATUALIZACAO DATETIME NOT NULL CONSTRAINT DF_ALTERVISION_LOJAS_DATA_ATUALIZACAO DEFAULT GETDATE(),

        CONSTRAINT PK_ALTERVISION_LOJAS PRIMARY KEY CLUSTERED (ID_LOJA)
    );
END;
GO

-----------------------------------------------------------------------------
USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_ALTERVISION_LOJAS_REDE_EMPRESA_CNPJ'
      AND object_id = OBJECT_ID('DBO.ALTERVISION_LOJAS')
)
BEGIN
    CREATE UNIQUE INDEX UX_ALTERVISION_LOJAS_REDE_EMPRESA_CNPJ
    ON DBO.ALTERVISION_LOJAS (
        REDE,
        CODIGO_EMPRESA,
        CNPJ
    );
END;
GO

---------------------------------------------------------------------------------------
USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ALTERVISION_LOJAS_ALIAS_ATIVO'
      AND object_id = OBJECT_ID('DBO.ALTERVISION_LOJAS')
)
BEGIN
    CREATE INDEX IX_ALTERVISION_LOJAS_ALIAS_ATIVO
    ON DBO.ALTERVISION_LOJAS (
        ALIAS_ID,
        ATIVO
    )
    INCLUDE (
        REDE,
        CODIGO_EMPRESA,
        CNPJ,
        APELIDO,
        NOME
    );
END;
GO

-------------------------------------------------------------------------------------------------------

GO

IF COL_LENGTH('dbo.ALTERVISION_LOJAS', 'CEP') IS NULL
BEGIN
    ALTER TABLE dbo.ALTERVISION_LOJAS
    ADD CEP VARCHAR(10) NULL;
END;
GO


---------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_ALTERVISION_FILIAIS_ORIGEM AS

SELECT
    LTRIM(RTRIM(CAST(rede AS VARCHAR(20)))) AS REDE,
    LTRIM(RTRIM(CAST(codigo_empresa AS VARCHAR(20)))) AS CODIGO_EMPRESA,
    LTRIM(RTRIM(CAST(cnpj AS VARCHAR(20)))) AS CNPJ,
    LTRIM(RTRIM(CAST(apelido AS VARCHAR(150)))) AS APELIDO,
    LTRIM(RTRIM(CAST(nome AS VARCHAR(250)))) AS NOME,
    CAST(lastupdate AS DATETIME2) AS LASTUPDATE,
    LTRIM(RTRIM(CAST(cep AS VARCHAR(10)))) AS CEP
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT
        retorno.rede,
        retorno.codigo_empresa,
        retorno.cnpj,
        retorno.apelido,
        retorno.nome,
        retorno.lastupdate,
        retorno.cep
    FROM consultadinamica(''
        SELECT
            current_schema::char(10) AS rede,
            pessoas.codigo AS codigo_empresa,
            pessoas.cpfcnpj AS cnpj,
            btrim(pessoas.apelido)::varchar AS apelido,
            btrim(pessoas.nome)::varchar AS nome,
            pessoas.lastupdate::timestamp AS lastupdate,
            btrim(pessoas.cep)::varchar AS cep
        FROM pessoas
        WHERE pessoas.codigo BETWEEN ''''00000001'''' AND ''''00000099''''
          AND pessoas.cpfcnpj IS NOT NULL
          AND pessoas.filial = true
    ''::text) retorno(
        rede character(10),
        codigo_empresa character(8),
        cnpj character(18),
        apelido character varying,
        nome character varying,
        lastupdate timestamp without time zone,
        cep character varying
    )
    WHERE retorno.rede <> ''rede000003''::bpchar
');
GO

-----------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

SELECT *
FROM dbo.VW_ALTERVISION_FILIAIS_ORIGEM
ORDER BY
    REDE,
    CODIGO_EMPRESA;
	
---------------------------------------------------------------------------------------------------

SELECT
    REDE,
    CODIGO_EMPRESA,
    CNPJ,
    COUNT(*) AS QTDE
FROM DBO.VW_ALTERVISION_FILIAIS_ORIGEM
GROUP BY
    REDE,
    CODIGO_EMPRESA,
    CNPJ
HAVING COUNT(*) > 1;

---------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF OBJECT_ID('DBO.ALTERVISION_CONTROLE_CARGA', 'U') IS NULL
BEGIN
    CREATE TABLE DBO.ALTERVISION_CONTROLE_CARGA (
        ID_CONTROLE BIGINT IDENTITY(1,1) NOT NULL,

        TIPO_CARGA VARCHAR(50) NOT NULL,

        DATA_INICIO_EXECUCAO DATETIME NOT NULL CONSTRAINT DF_ALTERVISION_CONTROLE_DATA_INICIO DEFAULT GETDATE(),
        DATA_FIM_EXECUCAO DATETIME NULL,

        STATUS VARCHAR(20) NOT NULL,
        MENSAGEM_ERRO VARCHAR(MAX) NULL,

        REGISTROS_AFETADOS INT NULL,

        CONSTRAINT PK_ALTERVISION_CONTROLE_CARGA PRIMARY KEY CLUSTERED (ID_CONTROLE)
    );
END;
GO

----------------------------------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER PROCEDURE dbo.SP_CARGA_ALTERVISION_LOJAS
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ID_CONTROLE BIGINT;
    DECLARE @REGISTROS_AFETADOS INT = 0;

    BEGIN TRY

        INSERT INTO dbo.ALTERVISION_CONTROLE_CARGA (
            TIPO_CARGA,
            STATUS
        )
        VALUES (
            'LOJAS',
            'INICIADO'
        );

        SET @ID_CONTROLE = SCOPE_IDENTITY();

        MERGE dbo.ALTERVISION_LOJAS AS DESTINO
        USING (
            SELECT
                REDE,
                CODIGO_EMPRESA,
                CNPJ,
                APELIDO,
                NOME,
                LASTUPDATE,
                CEP
            FROM dbo.VW_ALTERVISION_FILIAIS_ORIGEM
        ) AS ORIGEM
        ON  DESTINO.REDE = ORIGEM.REDE
        AND DESTINO.CODIGO_EMPRESA = ORIGEM.CODIGO_EMPRESA
        AND DESTINO.CNPJ = ORIGEM.CNPJ

        WHEN MATCHED AND (
               ISNULL(DESTINO.APELIDO, '') <> ISNULL(ORIGEM.APELIDO, '')
            OR ISNULL(DESTINO.NOME, '') <> ISNULL(ORIGEM.NOME, '')
            OR ISNULL(DESTINO.CEP, '') <> ISNULL(ORIGEM.CEP, '')
            OR ISNULL(DESTINO.LASTUPDATE_ORIGEM, CONVERT(DATETIME2, '19000101')) <> ISNULL(ORIGEM.LASTUPDATE, CONVERT(DATETIME2, '19000101'))
            OR DESTINO.ATIVO <> 1
        )
        THEN
            UPDATE SET
                DESTINO.APELIDO = ORIGEM.APELIDO,
                DESTINO.NOME = ORIGEM.NOME,
                DESTINO.CEP = ORIGEM.CEP,
                DESTINO.LASTUPDATE_ORIGEM = ORIGEM.LASTUPDATE,
                DESTINO.ATIVO = 1,
                DESTINO.DATA_ATUALIZACAO = GETDATE()

        WHEN NOT MATCHED THEN
            INSERT (
                REDE,
                CODIGO_EMPRESA,
                CNPJ,
                APELIDO,
                NOME,
                CEP,
                LASTUPDATE_ORIGEM,
                ALIAS_ID,
                ATIVO,
                DATA_CRIACAO,
                DATA_ATUALIZACAO
            )
            VALUES (
                ORIGEM.REDE,
                ORIGEM.CODIGO_EMPRESA,
                ORIGEM.CNPJ,
                ORIGEM.APELIDO,
                ORIGEM.NOME,
                ORIGEM.CEP,
                ORIGEM.LASTUPDATE,
                NULL,
                1,
                GETDATE(),
                GETDATE()
            );

        SET @REGISTROS_AFETADOS = @@ROWCOUNT;

        UPDATE dbo.ALTERVISION_CONTROLE_CARGA
        SET
            STATUS = 'SUCESSO',
            DATA_FIM_EXECUCAO = GETDATE(),
            REGISTROS_AFETADOS = @REGISTROS_AFETADOS
        WHERE ID_CONTROLE = @ID_CONTROLE;

    END TRY
    BEGIN CATCH

        IF @ID_CONTROLE IS NOT NULL
        BEGIN
            UPDATE dbo.ALTERVISION_CONTROLE_CARGA
            SET
                STATUS = 'ERRO',
                DATA_FIM_EXECUCAO = GETDATE(),
                MENSAGEM_ERRO = ERROR_MESSAGE()
            WHERE ID_CONTROLE = @ID_CONTROLE;
        END;

        THROW;

    END CATCH
END;
GO
-------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

EXEC dbo.SP_CARGA_ALTERVISION_LOJAS;

------------------------------------------------------------------------

SELECT *
FROM DBO.ALTERVISION_LOJAS
ORDER BY
    REDE,
    CODIGO_EMPRESA;

---------------------------------------------------------------------------

SELECT TOP 20 *
FROM dbo.ALTERVISION_CONTROLE_CARGA
ORDER BY ID_CONTROLE DESC;
----------------------------------------------------------------------------

SELECT
    ID_LOJA,
    REDE,
    CODIGO_EMPRESA,
    CNPJ,
    APELIDO,
    NOME,
    ALIAS_ID
FROM dbo.ALTERVISION_LOJAS
WHERE ALIAS_ID IS NULL
ORDER BY
    REDE,
    CODIGO_EMPRESA;

	---------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_API_ALTERVISION_LOJAS AS

SELECT
    ID_LOJA AS id_loja,
    REDE AS rede,
    CODIGO_EMPRESA AS codigo_empresa,
    CNPJ AS cnpj,
    APELIDO AS apelido,
    NOME AS nome,
    CEP AS cep,
    ALIAS_ID AS alias_id,
    ATIVO AS ativo,
    LASTUPDATE_ORIGEM AS lastupdate_origem,
    DATA_ATUALIZACAO AS data_atualizacao
FROM dbo.ALTERVISION_LOJAS
WHERE ATIVO = 1;
GO
--------------------------------------------------------------------------------

SELECT *
FROM DBO.VW_API_ALTERVISION_LOJAS
ORDER BY
    rede,
    codigo_empresa;

---------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF COL_LENGTH('dbo.ALTERVISION_LOJAS', 'LASTUPDATE_ORIGEM') IS NULL
BEGIN
    ALTER TABLE dbo.ALTERVISION_LOJAS
    ADD LASTUPDATE_ORIGEM DATETIME2 NULL;
END;
GO

-------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ALTERVISION_LOJAS_ALIAS_ATIVO'
      AND object_id = OBJECT_ID('dbo.ALTERVISION_LOJAS')
)
BEGIN
    DROP INDEX IX_ALTERVISION_LOJAS_ALIAS_ATIVO
    ON dbo.ALTERVISION_LOJAS;
END;
GO

CREATE INDEX IX_ALTERVISION_LOJAS_ALIAS_ATIVO
ON dbo.ALTERVISION_LOJAS (
    ALIAS_ID,
    ATIVO
)
INCLUDE (
    REDE,
    CODIGO_EMPRESA,
    CNPJ,
    APELIDO,
    NOME,
    CEP
);
GO

------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF OBJECT_ID('dbo.ALTERVISION_VENDEDORES', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ALTERVISION_VENDEDORES (
        ID_VENDEDOR INT IDENTITY(1,1) NOT NULL,

        REDE VARCHAR(20) NOT NULL,

        EMPRESA VARCHAR(20) NULL,
        CNPJ_EMPRESA VARCHAR(20) NULL,
        APELIDO_EMPRESA VARCHAR(150) NULL,
        NOME_EMPRESA VARCHAR(250) NULL,

        EMPRESA_ACESSO VARCHAR(20) NULL,
        CNPJ_EMPRESA_ACESSO VARCHAR(20) NULL,
        APELIDO_EMPRESA_ACESSO VARCHAR(150) NULL,
        NOME_EMPRESA_ACESSO VARCHAR(250) NULL,

        ALIAS_ID VARCHAR(50) NULL,

        CODVENDEDOR VARCHAR(20) NOT NULL,
        VENDEDOR VARCHAR(150) NULL,
        NOME_VENDEDOR VARCHAR(250) NULL,
        CPFCNPJ_VENDEDOR VARCHAR(20) NULL,

        DESCRICAO_ATIVIDADE VARCHAR(150) NULL,
        STATUS VARCHAR(10) NULL,

        ADMISSAO DATE NULL,
        DEMISSAO DATE NULL,

        LASTUPDATE_ORIGEM DATETIME2 NULL,

        ATIVO BIT NOT NULL CONSTRAINT DF_ALTERVISION_VENDEDORES_ATIVO DEFAULT 1,

        DATA_CRIACAO DATETIME NOT NULL CONSTRAINT DF_ALTERVISION_VENDEDORES_DATA_CRIACAO DEFAULT GETDATE(),
        DATA_ATUALIZACAO DATETIME NOT NULL CONSTRAINT DF_ALTERVISION_VENDEDORES_DATA_ATUALIZACAO DEFAULT GETDATE(),

        CONSTRAINT PK_ALTERVISION_VENDEDORES PRIMARY KEY CLUSTERED (ID_VENDEDOR)
    );
END;
GO

---------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_ALTERVISION_VENDEDORES_REDE_EMPRESA_ACESSO_COD'
      AND object_id = OBJECT_ID('dbo.ALTERVISION_VENDEDORES')
)
BEGIN
    CREATE UNIQUE INDEX UX_ALTERVISION_VENDEDORES_REDE_EMPRESA_ACESSO_COD
    ON dbo.ALTERVISION_VENDEDORES (
        REDE,
        EMPRESA_ACESSO,
        CODVENDEDOR
    );
END;
GO

---------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ALTERVISION_VENDEDORES_ALIAS_ATIVO'
      AND object_id = OBJECT_ID('dbo.ALTERVISION_VENDEDORES')
)
BEGIN
    CREATE INDEX IX_ALTERVISION_VENDEDORES_ALIAS_ATIVO
    ON dbo.ALTERVISION_VENDEDORES (
        ALIAS_ID,
        ATIVO
    )
    INCLUDE (
        REDE,
        EMPRESA,
        EMPRESA_ACESSO,
        CNPJ_EMPRESA_ACESSO,
        CODVENDEDOR,
        VENDEDOR,
        NOME_VENDEDOR,
        CPFCNPJ_VENDEDOR,
        STATUS
    );
END;
GO

------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_ALTERVISION_VENDEDORES_ORIGEM AS

SELECT
    LTRIM(RTRIM(CAST(rede AS VARCHAR(20)))) AS REDE,

    LTRIM(RTRIM(CAST(empresa AS VARCHAR(20)))) AS EMPRESA,
    LTRIM(RTRIM(CAST(cnpj_empresa AS VARCHAR(20)))) AS CNPJ_EMPRESA,
    LTRIM(RTRIM(CAST(apelido_empresa AS VARCHAR(150)))) AS APELIDO_EMPRESA,
    LTRIM(RTRIM(CAST(nome_empresa AS VARCHAR(250)))) AS NOME_EMPRESA,

    LTRIM(RTRIM(CAST(empresa_acesso AS VARCHAR(20)))) AS EMPRESA_ACESSO,
    LTRIM(RTRIM(CAST(cnpj_empresa_acesso AS VARCHAR(20)))) AS CNPJ_EMPRESA_ACESSO,
    LTRIM(RTRIM(CAST(apelido_empresa_acesso AS VARCHAR(150)))) AS APELIDO_EMPRESA_ACESSO,
    LTRIM(RTRIM(CAST(nome_empresa_acesso AS VARCHAR(250)))) AS NOME_EMPRESA_ACESSO,

    LTRIM(RTRIM(CAST(codvendedor AS VARCHAR(20)))) AS CODVENDEDOR,
    LTRIM(RTRIM(CAST(vendedor AS VARCHAR(150)))) AS VENDEDOR,
    LTRIM(RTRIM(CAST(nome_vendedor AS VARCHAR(250)))) AS NOME_VENDEDOR,
    LTRIM(RTRIM(CAST(cpfcnpj_vendedor AS VARCHAR(20)))) AS CPFCNPJ_VENDEDOR,

    LTRIM(RTRIM(CAST(descricao_atividade AS VARCHAR(150)))) AS DESCRICAO_ATIVIDADE,
    LTRIM(RTRIM(CAST(status AS VARCHAR(10)))) AS STATUS,

    CAST(admissao AS DATE) AS ADMISSAO,
    CAST(demissao AS DATE) AS DEMISSAO,
    CAST(lastupdate AS DATETIME2) AS LASTUPDATE
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT
        retorno.rede,
        retorno.empresa,
        retorno.cnpj_empresa,
        retorno.apelido_empresa,
        retorno.nome_empresa,
        retorno.empresa_acesso,
        retorno.cnpj_empresa_acesso,
        retorno.apelido_empresa_acesso,
        retorno.nome_empresa_acesso,
        retorno.codvendedor,
        retorno.vendedor,
        retorno.nome_vendedor,
        retorno.cpfcnpj_vendedor,
        retorno.descricao_atividade,
        retorno.status,
        retorno.admissao,
        retorno.demissao,
        retorno.lastupdate
    FROM consultadinamica(''
        SELECT
            current_schema::char(10) AS rede,

            empresas.codigo AS empresa,
            empresas.cpfcnpj AS cnpj_empresa,
            btrim(empresas.apelido)::varchar AS apelido_empresa,
            btrim(empresas.nome)::varchar AS nome_empresa,

            empresaacesso.codigo AS empresa_acesso,
            empresaacesso.cpfcnpj AS cnpj_empresa_acesso,
            btrim(empresaacesso.apelido)::varchar AS apelido_empresa_acesso,
            btrim(empresaacesso.nome)::varchar AS nome_empresa_acesso,

            vendedores.codigo AS codvendedor,
            btrim(vendedores.apelido)::varchar AS vendedor,
            btrim(vendedores.nome)::varchar AS nome_vendedor,
            vendedores.cpfcnpj AS cpfcnpj_vendedor,

            btrim(atividades.descricao)::varchar AS descricao_atividade,

            vendedores.status AS status,
            vendedores.admissao AS admissao,
            vendedores.demissao AS demissao,
            vendedores.lastupdate::timestamp AS lastupdate

        FROM pessoas vendedores

        LEFT JOIN pessoas empresas
            ON lpad(vendedores.empresa, 8, ''''0'''')::char(8) = empresas.codigo

        LEFT JOIN pessoas empresaacesso
            ON lpad(vendedores.empresasacesso, 8, ''''0'''')::char(8) = empresaacesso.codigo

        LEFT JOIN atividades
            ON atividades.codigo = vendedores.atividade

        WHERE vendedores.funcionario = ''''1''''
          AND vendedores.podevender = ''''1''''
		  AND vendedores.codigo <> ''''00000101''''
		  and empresas.codigo is not null

        ORDER BY 
            empresas.codigo,
            vendedores.codigo
    ''::text) retorno(
        rede character(10),
        empresa character(8),
        cnpj_empresa character(18),
        apelido_empresa character varying,
        nome_empresa character varying,
        empresa_acesso character(8),
        cnpj_empresa_acesso character(18),
        apelido_empresa_acesso character varying,
        nome_empresa_acesso character varying,
        codvendedor character(8),
        vendedor character varying,
        nome_vendedor character varying,
        cpfcnpj_vendedor character(18),
        descricao_atividade character varying,
        status character(1),
        admissao date,
        demissao date,
        lastupdate timestamp without time zone
    )
    WHERE trim(retorno.rede) <> ''rede000003''
');
GO

-------------------------------------------------------------------------------------------------------------

SELECT *
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
ORDER BY
    REDE,
    EMPRESA_ACESSO,
    CODVENDEDOR;


-------------------------------------------------------------------------------------------------------------

SELECT
    REDE,
    EMPRESA_ACESSO,
    CODVENDEDOR,
    COUNT(*) AS QTDE
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
GROUP BY
    REDE,
    EMPRESA_ACESSO,
    CODVENDEDOR
HAVING COUNT(*) > 1;

-------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER PROCEDURE dbo.SP_CARGA_ALTERVISION_VENDEDORES
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ID_CONTROLE BIGINT;
    DECLARE @REGISTROS_AFETADOS INT = 0;

    BEGIN TRY

        INSERT INTO dbo.ALTERVISION_CONTROLE_CARGA (
            TIPO_CARGA,
            STATUS
        )
        VALUES (
            'VENDEDORES',
            'INICIADO'
        );

        SET @ID_CONTROLE = SCOPE_IDENTITY();

        MERGE dbo.ALTERVISION_VENDEDORES AS DESTINO
        USING (
            SELECT *
            FROM (
                SELECT
                    V.REDE,

                    V.EMPRESA,
                    V.CNPJ_EMPRESA,
                    V.APELIDO_EMPRESA,
                    V.NOME_EMPRESA,

                    ISNULL(NULLIF(V.EMPRESA_ACESSO, ''), V.EMPRESA) AS EMPRESA_ACESSO,

                    V.CNPJ_EMPRESA_ACESSO,
                    V.APELIDO_EMPRESA_ACESSO,
                    V.NOME_EMPRESA_ACESSO,

                    L.ALIAS_ID,

                    V.CODVENDEDOR,
                    V.VENDEDOR,
                    V.NOME_VENDEDOR,
                    V.CPFCNPJ_VENDEDOR,

                    V.DESCRICAO_ATIVIDADE,
                    V.STATUS,
                    V.ADMISSAO,
                    V.DEMISSAO,
                    V.LASTUPDATE,

                    ROW_NUMBER() OVER (
                        PARTITION BY
                            V.REDE,
                            ISNULL(NULLIF(V.EMPRESA_ACESSO, ''), V.EMPRESA),
                            V.CODVENDEDOR
                        ORDER BY
                            V.LASTUPDATE DESC,
                            V.EMPRESA
                    ) AS RN

                FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM V

                LEFT JOIN dbo.ALTERVISION_LOJAS L
                    ON  L.REDE = V.REDE
                    AND L.CODIGO_EMPRESA = ISNULL(NULLIF(V.EMPRESA_ACESSO, ''), V.EMPRESA)
                    AND L.ATIVO = 1
            ) X
            WHERE X.RN = 1
        ) AS ORIGEM
        ON  DESTINO.REDE = ORIGEM.REDE
        AND DESTINO.EMPRESA_ACESSO = ORIGEM.EMPRESA_ACESSO
        AND DESTINO.CODVENDEDOR = ORIGEM.CODVENDEDOR

        WHEN MATCHED AND (
               ISNULL(DESTINO.EMPRESA, '') <> ISNULL(ORIGEM.EMPRESA, '')
            OR ISNULL(DESTINO.CNPJ_EMPRESA, '') <> ISNULL(ORIGEM.CNPJ_EMPRESA, '')
            OR ISNULL(DESTINO.APELIDO_EMPRESA, '') <> ISNULL(ORIGEM.APELIDO_EMPRESA, '')
            OR ISNULL(DESTINO.NOME_EMPRESA, '') <> ISNULL(ORIGEM.NOME_EMPRESA, '')

            OR ISNULL(DESTINO.CNPJ_EMPRESA_ACESSO, '') <> ISNULL(ORIGEM.CNPJ_EMPRESA_ACESSO, '')
            OR ISNULL(DESTINO.APELIDO_EMPRESA_ACESSO, '') <> ISNULL(ORIGEM.APELIDO_EMPRESA_ACESSO, '')
            OR ISNULL(DESTINO.NOME_EMPRESA_ACESSO, '') <> ISNULL(ORIGEM.NOME_EMPRESA_ACESSO, '')

            OR ISNULL(DESTINO.ALIAS_ID, '') <> ISNULL(ORIGEM.ALIAS_ID, '')

            OR ISNULL(DESTINO.VENDEDOR, '') <> ISNULL(ORIGEM.VENDEDOR, '')
            OR ISNULL(DESTINO.NOME_VENDEDOR, '') <> ISNULL(ORIGEM.NOME_VENDEDOR, '')
            OR ISNULL(DESTINO.CPFCNPJ_VENDEDOR, '') <> ISNULL(ORIGEM.CPFCNPJ_VENDEDOR, '')

            OR ISNULL(DESTINO.DESCRICAO_ATIVIDADE, '') <> ISNULL(ORIGEM.DESCRICAO_ATIVIDADE, '')
            OR ISNULL(DESTINO.STATUS, '') <> ISNULL(ORIGEM.STATUS, '')

            OR ISNULL(DESTINO.ADMISSAO, CONVERT(DATE, '19000101')) <> ISNULL(ORIGEM.ADMISSAO, CONVERT(DATE, '19000101'))
            OR ISNULL(DESTINO.DEMISSAO, CONVERT(DATE, '19000101')) <> ISNULL(ORIGEM.DEMISSAO, CONVERT(DATE, '19000101'))

            OR ISNULL(DESTINO.LASTUPDATE_ORIGEM, CONVERT(DATETIME2, '19000101')) <> ISNULL(ORIGEM.LASTUPDATE, CONVERT(DATETIME2, '19000101'))

            OR DESTINO.ATIVO <> 1
        )
        THEN
            UPDATE SET
                DESTINO.EMPRESA = ORIGEM.EMPRESA,
                DESTINO.CNPJ_EMPRESA = ORIGEM.CNPJ_EMPRESA,
                DESTINO.APELIDO_EMPRESA = ORIGEM.APELIDO_EMPRESA,
                DESTINO.NOME_EMPRESA = ORIGEM.NOME_EMPRESA,

                DESTINO.EMPRESA_ACESSO = ORIGEM.EMPRESA_ACESSO,
                DESTINO.CNPJ_EMPRESA_ACESSO = ORIGEM.CNPJ_EMPRESA_ACESSO,
                DESTINO.APELIDO_EMPRESA_ACESSO = ORIGEM.APELIDO_EMPRESA_ACESSO,
                DESTINO.NOME_EMPRESA_ACESSO = ORIGEM.NOME_EMPRESA_ACESSO,

                DESTINO.ALIAS_ID = ORIGEM.ALIAS_ID,

                DESTINO.VENDEDOR = ORIGEM.VENDEDOR,
                DESTINO.NOME_VENDEDOR = ORIGEM.NOME_VENDEDOR,
                DESTINO.CPFCNPJ_VENDEDOR = ORIGEM.CPFCNPJ_VENDEDOR,

                DESTINO.DESCRICAO_ATIVIDADE = ORIGEM.DESCRICAO_ATIVIDADE,
                DESTINO.STATUS = ORIGEM.STATUS,

                DESTINO.ADMISSAO = ORIGEM.ADMISSAO,
                DESTINO.DEMISSAO = ORIGEM.DEMISSAO,

                DESTINO.LASTUPDATE_ORIGEM = ORIGEM.LASTUPDATE,

                DESTINO.ATIVO = 1,
                DESTINO.DATA_ATUALIZACAO = GETDATE()

        WHEN NOT MATCHED THEN
            INSERT (
                REDE,

                EMPRESA,
                CNPJ_EMPRESA,
                APELIDO_EMPRESA,
                NOME_EMPRESA,

                EMPRESA_ACESSO,
                CNPJ_EMPRESA_ACESSO,
                APELIDO_EMPRESA_ACESSO,
                NOME_EMPRESA_ACESSO,

                ALIAS_ID,

                CODVENDEDOR,
                VENDEDOR,
                NOME_VENDEDOR,
                CPFCNPJ_VENDEDOR,

                DESCRICAO_ATIVIDADE,
                STATUS,

                ADMISSAO,
                DEMISSAO,

                LASTUPDATE_ORIGEM,

                ATIVO,
                DATA_CRIACAO,
                DATA_ATUALIZACAO
            )
            VALUES (
                ORIGEM.REDE,

                ORIGEM.EMPRESA,
                ORIGEM.CNPJ_EMPRESA,
                ORIGEM.APELIDO_EMPRESA,
                ORIGEM.NOME_EMPRESA,

                ORIGEM.EMPRESA_ACESSO,
                ORIGEM.CNPJ_EMPRESA_ACESSO,
                ORIGEM.APELIDO_EMPRESA_ACESSO,
                ORIGEM.NOME_EMPRESA_ACESSO,

                ORIGEM.ALIAS_ID,

                ORIGEM.CODVENDEDOR,
                ORIGEM.VENDEDOR,
                ORIGEM.NOME_VENDEDOR,
                ORIGEM.CPFCNPJ_VENDEDOR,

                ORIGEM.DESCRICAO_ATIVIDADE,
                ORIGEM.STATUS,

                ORIGEM.ADMISSAO,
                ORIGEM.DEMISSAO,

                ORIGEM.LASTUPDATE,

                1,
                GETDATE(),
                GETDATE()
            );

        SET @REGISTROS_AFETADOS = @@ROWCOUNT;

        UPDATE dbo.ALTERVISION_CONTROLE_CARGA
        SET
            STATUS = 'SUCESSO',
            DATA_FIM_EXECUCAO = GETDATE(),
            REGISTROS_AFETADOS = @REGISTROS_AFETADOS
        WHERE ID_CONTROLE = @ID_CONTROLE;

    END TRY
    BEGIN CATCH

        IF @ID_CONTROLE IS NOT NULL
        BEGIN
            UPDATE dbo.ALTERVISION_CONTROLE_CARGA
            SET
                STATUS = 'ERRO',
                DATA_FIM_EXECUCAO = GETDATE(),
                MENSAGEM_ERRO = ERROR_MESSAGE()
            WHERE ID_CONTROLE = @ID_CONTROLE;
        END;

        THROW;

    END CATCH
END;
GO

----------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_API_ALTERVISION_VENDEDORES AS

SELECT
    ID_VENDEDOR AS id_vendedor,

    REDE AS rede,

    EMPRESA AS empresa,
    CNPJ_EMPRESA AS cnpj_empresa,
    APELIDO_EMPRESA AS apelido_empresa,
    NOME_EMPRESA AS nome_empresa,

    EMPRESA_ACESSO AS empresa_acesso,
    CNPJ_EMPRESA_ACESSO AS cnpj_empresa_acesso,
    APELIDO_EMPRESA_ACESSO AS apelido_empresa_acesso,
    NOME_EMPRESA_ACESSO AS nome_empresa_acesso,

    ALIAS_ID AS alias_id,

    CODVENDEDOR AS codvendedor,
    VENDEDOR AS vendedor,
    NOME_VENDEDOR AS nome_vendedor,
    CPFCNPJ_VENDEDOR AS cpfcnpj_vendedor,

    DESCRICAO_ATIVIDADE AS descricao_atividade,
    STATUS AS status,

    ADMISSAO AS admissao,
    DEMISSAO AS demissao,

    LASTUPDATE_ORIGEM AS lastupdate_origem,
    DATA_ATUALIZACAO AS data_atualizacao
FROM dbo.ALTERVISION_VENDEDORES
WHERE ATIVO = 1;
GO

--------------------------------------------------------------------------------
UPDATE dbo.ALTERVISION_VENDEDORES
SET EMPRESA_ACESSO = EMPRESA
WHERE EMPRESA_ACESSO IS NULL;
---------------------------------------------------------------------------------


EXEC dbo.SP_CARGA_ALTERVISION_VENDEDORES;

-------------------------------------------------------------------------------
SELECT *
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
WHERE REDE = 'rede000009'
  AND CODVENDEDOR = '00000101';

-------------------------------------------------------------------------------

SELECT *
FROM dbo.ALTERVISION_VENDEDORES
ORDER BY
    REDE,
    EMPRESA_ACESSO,
    CODVENDEDOR;

---------------------------------------------------------------------------

SELECT TOP 20 *
FROM dbo.ALTERVISION_CONTROLE_CARGA
ORDER BY ID_CONTROLE DESC;

-------------------------------------------------------------------------------------

SELECT
    REDE,
    EMPRESA,
    EMPRESA_ACESSO,
    CODVENDEDOR,
    VENDEDOR,
    NOME_VENDEDOR,
    COUNT(*) AS QTDE
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
GROUP BY
    REDE,
    EMPRESA,
    EMPRESA_ACESSO,
    CODVENDEDOR,
    VENDEDOR,
    NOME_VENDEDOR
HAVING COUNT(*) > 1
ORDER BY QTDE DESC;

SELECT *
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
WHERE REDE = 'rede000001'AND 
CODVENDEDOR = '00397856'
ORDER BY
    EMPRESA,
    EMPRESA_ACESSO;

SELECT
    REDE,
    ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA) AS EMPRESA_ACESSO_CHAVE,
    CODVENDEDOR,
    COUNT(*) AS QTDE
FROM dbo.VW_ALTERVISION_VENDEDORES_ORIGEM
GROUP BY
    REDE,
    ISNULL(NULLIF(EMPRESA_ACESSO, ''), EMPRESA),
    CODVENDEDOR
HAVING COUNT(*) > 1
ORDER BY QTDE DESC;
-------------------------------------------------------------------------------------



SELECT
    ID_VENDEDOR,
    REDE,
    EMPRESA,
    EMPRESA_ACESSO,
    CNPJ_EMPRESA_ACESSO,
    CODVENDEDOR,
    VENDEDOR,
    NOME_VENDEDOR,
    ALIAS_ID
FROM dbo.ALTERVISION_VENDEDORES
WHERE ALIAS_ID IS NULL
ORDER BY
    REDE,
    EMPRESA_ACESSO,
    CODVENDEDOR;

	---------------------------------------------------------------------------------

	SELECT *
FROM dbo.VW_API_ALTERVISION_VENDEDORES
WHERE alias_id = '01'
ORDER BY nome_vendedor;

-----------------------------------------------------------------------------------

SELECT *
FROM dbo.VW_API_ALTERVISION_VENDEDORES
ORDER BY
    rede,
    empresa_acesso,
    nome_vendedor;

------------------------------------------------------------------------------------
CREATE TABLE dbo.ALTERVISION_VENDAS_DETALHE (
    ID_VENDA_DETALHE BIGINT IDENTITY(1,1) NOT NULL,

    REDE VARCHAR(20) NOT NULL,
    CODIGO_EMPRESA VARCHAR(20) NOT NULL,
    CNPJ VARCHAR(20) NULL,
    ALIAS_ID VARCHAR(50) NULL,

    CODIGO_VENDA VARCHAR(20) NOT NULL,
    DATA_VENDA DATE NOT NULL,
    HORA INT NULL,

    EMISSAONF DATE NULL,
    LASTUPDATE_ORIGEM DATETIME2 NULL,

    CODCLIENTE VARCHAR(20) NULL,
    CLIENTE VARCHAR(250) NULL,

    CODVENDEDOR VARCHAR(20) NOT NULL,
    VENDEDOR VARCHAR(150) NULL,

    CONDICOES VARCHAR(150) NULL,

    QTDE_ITENS NUMERIC(18,4) NOT NULL DEFAULT 0,
    AVISTA NUMERIC(18,2) NOT NULL DEFAULT 0,
    APRAZO NUMERIC(18,2) NOT NULL DEFAULT 0,
    TOTAL NUMERIC(18,2) NOT NULL DEFAULT 0,
    FRETE NUMERIC(18,2) NOT NULL DEFAULT 0,
    CUSTO NUMERIC(18,2) NOT NULL DEFAULT 0,

    VENDA_IMPORTADA VARCHAR(3) NULL,
    STATUS VARCHAR(10) NULL,

    DATA_CRIACAO DATETIME NOT NULL DEFAULT GETDATE(),
    DATA_ATUALIZACAO DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_ALTERVISION_VENDAS_DETALHE
        PRIMARY KEY CLUSTERED (ID_VENDA_DETALHE)
);

---------------------------------------------------------------------------------
CREATE OR ALTER VIEW dbo.VW_API_ALTERVISION_VENDAS_HORA AS

SELECT
    ALIAS_ID AS alias_id,
    DATA_VENDA AS localDate,
    ISNULL(HORA, 0) AS localHour,

    CODVENDEDOR AS id_sellerPDV,
    VENDEDOR AS sellerName,

    COUNT(DISTINCT CODIGO_VENDA) AS nbSales,
    SUM(QTDE_ITENS) AS nbItems,
    SUM(TOTAL) AS total
FROM dbo.ALTERVISION_VENDAS_DETALHE
WHERE
    ALIAS_ID IS NOT NULL
    AND STATUS IN ('S', 'O')
GROUP BY
    ALIAS_ID,
    DATA_VENDA,
    ISNULL(HORA, 0),
    CODVENDEDOR,
    VENDEDOR;

--------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_ALTERVISION_VENDAS_ORIGEM AS

SELECT
    LTRIM(RTRIM(CAST(rede AS VARCHAR(20)))) AS REDE,
    LTRIM(RTRIM(CAST(empresa AS VARCHAR(20)))) AS CODIGO_EMPRESA,
    LTRIM(RTRIM(CAST(cnpj AS VARCHAR(20)))) AS CNPJ,
    LTRIM(RTRIM(CAST(apelido AS VARCHAR(150)))) AS APELIDO,
    LTRIM(RTRIM(CAST(nome AS VARCHAR(250)))) AS NOME,

    LTRIM(RTRIM(CAST(codigo AS VARCHAR(20)))) AS CODIGO_VENDA,
    CAST(data AS DATE) AS DATA_VENDA,
    CAST(emissaonf AS DATE) AS EMISSAONF,
    CAST(lastupdate AS DATETIME2) AS LASTUPDATE,

    LTRIM(RTRIM(CAST(codcliente AS VARCHAR(20)))) AS CODCLIENTE,
    LTRIM(RTRIM(CAST(cliente AS VARCHAR(250)))) AS CLIENTE,

    LTRIM(RTRIM(CAST(codvendedor AS VARCHAR(20)))) AS CODVENDEDOR,
    LTRIM(RTRIM(CAST(vendedor AS VARCHAR(150)))) AS VENDEDOR,

    LTRIM(RTRIM(CAST(condicoes AS VARCHAR(150)))) AS CONDICOES,

    CAST(qtde_itens AS NUMERIC(18,4)) AS QTDE_ITENS,
    CAST(avista AS NUMERIC(18,2)) AS AVISTA,
    CAST(aprazo AS NUMERIC(18,2)) AS APRAZO,
    CAST(total AS NUMERIC(18,2)) AS TOTAL,
    CAST(frete AS NUMERIC(18,2)) AS FRETE,
    CAST(custo AS NUMERIC(18,2)) AS CUSTO,

    LTRIM(RTRIM(CAST(venda_importada AS VARCHAR(3)))) AS VENDA_IMPORTADA,
    LTRIM(RTRIM(CAST(status AS VARCHAR(10)))) AS STATUS

FROM OPENQUERY(POSTGRES_SETA, '
    SELECT 
        retorno.rede,
        retorno.empresa,
        retorno.cnpj,
        retorno.apelido,
        retorno.nome,
        retorno.codigo,
        retorno.data,
        retorno.emissaonf,
        retorno.lastupdate,
        retorno.codcliente,
        retorno.cliente,
        retorno.codvendedor,
        retorno.vendedor,
        retorno.condicoes,
        retorno.qtde_itens,
        retorno.avista,
        retorno.aprazo,
        retorno.total,
        retorno.frete,
        retorno.custo,
        retorno.venda_importada,
        retorno.status
    FROM consultadinamica(''
        SELECT 
            current_schema::char(10) AS rede,

            lpad(vendas.empresa, 8, ''''0'''')::char(8) AS empresa,
            empresas.cpfcnpj AS cnpj,
            btrim(empresas.apelido)::varchar AS apelido,
            btrim(empresas.nome)::varchar AS nome,

            vendas.codigo,
            vendas.data,

            (
                SELECT max(emissao)
                FROM nfregistro
                WHERE (''''VE'''' || vendas.codigo)::char(10) = nfregistro.auxiliar
                  AND nfregistro.nfeautorizada
            ) AS emissaonf,

            vendas.lastupdate::timestamp AS lastupdate,

            clientes.codigo AS codcliente,
            btrim(clientes.nome)::varchar AS cliente,

            vendedores.codigo AS codvendedor,
            btrim(vendedores.apelido)::varchar AS vendedor,

            btrim(condicoes.descricao)::varchar AS condicoes,

            SUM(
                CASE 
                    WHEN movimento.operacao = ''''DV'''' THEN movimento.quantidade * -1
                    ELSE movimento.quantidade
                END
            )::numeric(10,2) AS qtde_itens,

            SUM(
                movimento.total * 
                (
                    vendas.avista / 
                    CASE 
                        WHEN vendas.subtotal = 0 THEN 1 
                        ELSE vendas.subtotal 
                    END
                )
            )::numeric(10,2) AS avista,

            SUM(
                movimento.total * 
                (
                    vendas.aprazo / 
                    CASE 
                        WHEN vendas.subtotal = 0 THEN 1 
                        ELSE vendas.subtotal 
                    END
                )
            )::numeric(10,2) AS aprazo,

            SUM(
                movimento.total * 
                (
                    vendas.total / 
                    CASE 
                        WHEN vendas.subtotal = 0 THEN 1 
                        ELSE vendas.subtotal 
                    END
                )
            )::numeric(10,2) AS total,

            SUM(movimento.frete)::numeric(10,2) AS frete,

            SUM(movimento.quantidade * movimento.custo)::numeric(10,2) AS custo,

            CASE 
                WHEN vendas.obs LIKE ''''%VENDA IMPORTADA EM%'''' 
                    THEN ''''Sim'''' 
                ELSE ''''Não'''' 
            END::char(3) AS venda_importada,

            vendas.status

        FROM vendas

        INNER JOIN movimento 
            ON (''''VE'''' || vendas.codigo)::char(10) = movimento.auxiliar

        INNER JOIN pessoas empresas 
            ON lpad(vendas.empresa, 8, ''''0'''')::char(8) = empresas.codigo

        INNER JOIN pessoas clientes 
            ON vendas.cliente = clientes.codigo

        INNER JOIN pessoas vendedores 
            ON movimento.vendedorm = vendedores.codigo

        INNER JOIN condicoes 
            ON vendas.condicoes = condicoes.codigo

        LEFT JOIN pessoas avalista 
            ON vendas.avalista = avalista.codigo

        WHERE movimento.estoque = true
          AND vendas.status IN (''''S'''', ''''O'''', ''''C'''')
          AND movimento.operacao IN (''''VE'''', ''''DV'''')
          AND vendas.lastupdate > now() - interval ''''7 day''''
          AND vendas.tipo = ''''01''''

        GROUP BY 
            current_schema,
            vendas.empresa,
            empresas.cpfcnpj,
            empresas.apelido,
            empresas.nome,
            vendas.codigo,
            vendas.data,
            vendas.lastupdate,
            clientes.codigo,
            clientes.nome,
            vendedores.codigo,
            vendedores.apelido,
            condicoes.descricao,
            vendas.avista,
            vendas.aprazo,
            vendas.total,
            vendas.subtotal,
            vendas.obs,
            vendas.status
    ''::text) retorno(
        rede character(10),
        empresa character(8),
        cnpj character(18),
        apelido character varying,
        nome character varying,
        codigo character(8),
        data date,
        emissaonf date,
        lastupdate timestamp without time zone,
        codcliente character(8),
        cliente character varying,
        codvendedor character(8),
        vendedor character varying,
        condicoes character varying,
        qtde_itens numeric(10,2),
        avista numeric(10,2),
        aprazo numeric(10,2),
        total numeric(10,2),
        frete numeric(10,2),
        custo numeric(10,2),
        venda_importada character(3),
        status character(1)
    )
    WHERE trim(retorno.rede) <> ''rede000003''
');
GO


----------------------------------------------------------------------------------------------------------------------------

SELECT TOP 100 *
FROM dbo.VW_ALTERVISION_VENDAS_ORIGEM
ORDER BY
    REDE,
    CODIGO_EMPRESA,
    DATA_VENDA DESC,
    CODIGO_VENDA;
-----------------------------------------------------------------------------------------------------------------------------
SELECT
    REDE,
    CODIGO_EMPRESA,
    CODIGO_VENDA,
    CODVENDEDOR,
    COUNT(*) AS QTDE
FROM dbo.VW_ALTERVISION_VENDAS_ORIGEM
GROUP BY
    REDE,
    CODIGO_EMPRESA,
    CODIGO_VENDA,
    CODVENDEDOR
HAVING COUNT(*) > 1;

-----------------------------------------------------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF OBJECT_ID('dbo.ALTERVISION_VENDAS_DETALHE', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ALTERVISION_VENDAS_DETALHE (
        ID_VENDA_DETALHE BIGINT IDENTITY(1,1) NOT NULL,

        REDE VARCHAR(20) NOT NULL,
        CODIGO_EMPRESA VARCHAR(20) NOT NULL,
        CNPJ VARCHAR(20) NULL,
        ALIAS_ID VARCHAR(50) NULL,

        APELIDO VARCHAR(150) NULL,
        NOME VARCHAR(250) NULL,

        CODIGO_VENDA VARCHAR(20) NOT NULL,
        DATA_VENDA DATE NOT NULL,
        EMISSAONF DATE NULL,

        LASTUPDATE_ORIGEM DATETIME2 NULL,

        CODCLIENTE VARCHAR(20) NULL,
        CLIENTE VARCHAR(250) NULL,

        CODVENDEDOR VARCHAR(20) NOT NULL,
        VENDEDOR VARCHAR(150) NULL,

        CONDICOES VARCHAR(150) NULL,

        QTDE_ITENS NUMERIC(18,4) NOT NULL DEFAULT 0,
        AVISTA NUMERIC(18,2) NOT NULL DEFAULT 0,
        APRAZO NUMERIC(18,2) NOT NULL DEFAULT 0,
        TOTAL NUMERIC(18,2) NOT NULL DEFAULT 0,
        FRETE NUMERIC(18,2) NOT NULL DEFAULT 0,
        CUSTO NUMERIC(18,2) NOT NULL DEFAULT 0,

        VENDA_IMPORTADA VARCHAR(3) NULL,
        STATUS VARCHAR(10) NULL,

        DATA_CRIACAO DATETIME NOT NULL DEFAULT GETDATE(),
        DATA_ATUALIZACAO DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT PK_ALTERVISION_VENDAS_DETALHE
            PRIMARY KEY CLUSTERED (ID_VENDA_DETALHE)
    );
END;
GO

-----------------------------------------------------------------------------------

USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_ALTERVISION_VENDAS_DETALHE'
      AND object_id = OBJECT_ID('dbo.ALTERVISION_VENDAS_DETALHE')
)
BEGIN
    CREATE UNIQUE INDEX UX_ALTERVISION_VENDAS_DETALHE
    ON dbo.ALTERVISION_VENDAS_DETALHE (
        REDE,
        CODIGO_EMPRESA,
        CODIGO_VENDA,
        CODVENDEDOR
    );
END;
GO

------------------------------------------------------------------------------------
USE AlterVisionIntegracao;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ALTERVISION_VENDAS_DETALHE_API'
      AND object_id = OBJECT_ID('dbo.ALTERVISION_VENDAS_DETALHE')
)
BEGIN
    CREATE INDEX IX_ALTERVISION_VENDAS_DETALHE_API
    ON dbo.ALTERVISION_VENDAS_DETALHE (
        ALIAS_ID,
        DATA_VENDA,
        CODVENDEDOR
    )
    INCLUDE (
        CODIGO_VENDA,
        VENDEDOR,
        QTDE_ITENS,
        TOTAL,
        STATUS,
        LASTUPDATE_ORIGEM
    );
END;
GO

---------------------------------------------------------------------------
USE AlterVisionIntegracao;
GO

CREATE OR ALTER PROCEDURE dbo.SP_CARGA_ALTERVISION_VENDAS_DETALHE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ID_CONTROLE BIGINT;
    DECLARE @REGISTROS_AFETADOS INT = 0;

    BEGIN TRY

        INSERT INTO dbo.ALTERVISION_CONTROLE_CARGA (
            TIPO_CARGA,
            STATUS
        )
        VALUES (
            'VENDAS_DETALHE',
            'INICIADO'
        );

        SET @ID_CONTROLE = SCOPE_IDENTITY();

        MERGE dbo.ALTERVISION_VENDAS_DETALHE AS DESTINO
        USING (
            SELECT
                V.REDE,
                V.CODIGO_EMPRESA,
                V.CNPJ,
                L.ALIAS_ID,

                V.CODIGO_VENDA,
                V.DATA_VENDA,
                V.EMISSAONF,
                V.LASTUPDATE,

                V.CODCLIENTE,
                V.CLIENTE,

                V.CODVENDEDOR,
                V.VENDEDOR,

                V.CONDICOES,

                V.QTDE_ITENS,
                V.AVISTA,
                V.APRAZO,
                V.TOTAL,
                V.FRETE,
                V.CUSTO,

                V.VENDA_IMPORTADA,
                V.STATUS
            FROM dbo.VW_ALTERVISION_VENDAS_ORIGEM V
            LEFT JOIN dbo.ALTERVISION_LOJAS L
                ON  L.REDE = V.REDE
                AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
                AND L.ATIVO = 1
        ) AS ORIGEM
        ON  DESTINO.REDE = ORIGEM.REDE
        AND DESTINO.CODIGO_EMPRESA = ORIGEM.CODIGO_EMPRESA
        AND DESTINO.CODIGO_VENDA = ORIGEM.CODIGO_VENDA
        AND DESTINO.CODVENDEDOR = ORIGEM.CODVENDEDOR

        WHEN MATCHED AND (
               ISNULL(DESTINO.CNPJ, '') <> ISNULL(ORIGEM.CNPJ, '')
            OR ISNULL(DESTINO.ALIAS_ID, '') <> ISNULL(ORIGEM.ALIAS_ID, '')

            OR ISNULL(DESTINO.DATA_VENDA, CONVERT(DATE, '19000101')) <> ISNULL(ORIGEM.DATA_VENDA, CONVERT(DATE, '19000101'))
            OR ISNULL(DESTINO.EMISSAONF, CONVERT(DATE, '19000101')) <> ISNULL(ORIGEM.EMISSAONF, CONVERT(DATE, '19000101'))
            OR ISNULL(DESTINO.LASTUPDATE_ORIGEM, CONVERT(DATETIME2, '19000101')) <> ISNULL(ORIGEM.LASTUPDATE, CONVERT(DATETIME2, '19000101'))

            OR ISNULL(DESTINO.CODCLIENTE, '') <> ISNULL(ORIGEM.CODCLIENTE, '')
            OR ISNULL(DESTINO.CLIENTE, '') <> ISNULL(ORIGEM.CLIENTE, '')

            OR ISNULL(DESTINO.VENDEDOR, '') <> ISNULL(ORIGEM.VENDEDOR, '')
            OR ISNULL(DESTINO.CONDICOES, '') <> ISNULL(ORIGEM.CONDICOES, '')

            OR ISNULL(DESTINO.QTDE_ITENS, 0) <> ISNULL(ORIGEM.QTDE_ITENS, 0)
            OR ISNULL(DESTINO.AVISTA, 0) <> ISNULL(ORIGEM.AVISTA, 0)
            OR ISNULL(DESTINO.APRAZO, 0) <> ISNULL(ORIGEM.APRAZO, 0)
            OR ISNULL(DESTINO.TOTAL, 0) <> ISNULL(ORIGEM.TOTAL, 0)
            OR ISNULL(DESTINO.FRETE, 0) <> ISNULL(ORIGEM.FRETE, 0)
            OR ISNULL(DESTINO.CUSTO, 0) <> ISNULL(ORIGEM.CUSTO, 0)

            OR ISNULL(DESTINO.VENDA_IMPORTADA, '') <> ISNULL(ORIGEM.VENDA_IMPORTADA, '')
            OR ISNULL(DESTINO.STATUS, '') <> ISNULL(ORIGEM.STATUS, '')
        )
        THEN
            UPDATE SET
                DESTINO.CNPJ = ORIGEM.CNPJ,
                DESTINO.ALIAS_ID = ORIGEM.ALIAS_ID,

                DESTINO.DATA_VENDA = ORIGEM.DATA_VENDA,
                DESTINO.EMISSAONF = ORIGEM.EMISSAONF,
                DESTINO.LASTUPDATE_ORIGEM = ORIGEM.LASTUPDATE,

                DESTINO.CODCLIENTE = ORIGEM.CODCLIENTE,
                DESTINO.CLIENTE = ORIGEM.CLIENTE,

                DESTINO.VENDEDOR = ORIGEM.VENDEDOR,
                DESTINO.CONDICOES = ORIGEM.CONDICOES,

                DESTINO.QTDE_ITENS = ORIGEM.QTDE_ITENS,
                DESTINO.AVISTA = ORIGEM.AVISTA,
                DESTINO.APRAZO = ORIGEM.APRAZO,
                DESTINO.TOTAL = ORIGEM.TOTAL,
                DESTINO.FRETE = ORIGEM.FRETE,
                DESTINO.CUSTO = ORIGEM.CUSTO,

                DESTINO.VENDA_IMPORTADA = ORIGEM.VENDA_IMPORTADA,
                DESTINO.STATUS = ORIGEM.STATUS,

                DESTINO.DATA_ATUALIZACAO = GETDATE()

        WHEN NOT MATCHED THEN
            INSERT (
                REDE,
                CODIGO_EMPRESA,
                CNPJ,
                ALIAS_ID,

                CODIGO_VENDA,
                DATA_VENDA,
                EMISSAONF,
                LASTUPDATE_ORIGEM,

                CODCLIENTE,
                CLIENTE,

                CODVENDEDOR,
                VENDEDOR,

                CONDICOES,

                QTDE_ITENS,
                AVISTA,
                APRAZO,
                TOTAL,
                FRETE,
                CUSTO,

                VENDA_IMPORTADA,
                STATUS,

                DATA_CRIACAO,
                DATA_ATUALIZACAO
            )
            VALUES (
                ORIGEM.REDE,
                ORIGEM.CODIGO_EMPRESA,
                ORIGEM.CNPJ,
                ORIGEM.ALIAS_ID,

                ORIGEM.CODIGO_VENDA,
                ORIGEM.DATA_VENDA,
                ORIGEM.EMISSAONF,
                ORIGEM.LASTUPDATE,

                ORIGEM.CODCLIENTE,
                ORIGEM.CLIENTE,

                ORIGEM.CODVENDEDOR,
                ORIGEM.VENDEDOR,

                ORIGEM.CONDICOES,

                ORIGEM.QTDE_ITENS,
                ORIGEM.AVISTA,
                ORIGEM.APRAZO,
                ORIGEM.TOTAL,
                ORIGEM.FRETE,
                ORIGEM.CUSTO,

                ORIGEM.VENDA_IMPORTADA,
                ORIGEM.STATUS,

                GETDATE(),
                GETDATE()
            );

        SET @REGISTROS_AFETADOS = @@ROWCOUNT;

        UPDATE dbo.ALTERVISION_CONTROLE_CARGA
        SET
            STATUS = 'SUCESSO',
            DATA_FIM_EXECUCAO = GETDATE(),
            REGISTROS_AFETADOS = @REGISTROS_AFETADOS
        WHERE ID_CONTROLE = @ID_CONTROLE;

    END TRY
    BEGIN CATCH

        IF @ID_CONTROLE IS NOT NULL
        BEGIN
            UPDATE dbo.ALTERVISION_CONTROLE_CARGA
            SET
                STATUS = 'ERRO',
                DATA_FIM_EXECUCAO = GETDATE(),
                MENSAGEM_ERRO = ERROR_MESSAGE()
            WHERE ID_CONTROLE = @ID_CONTROLE;
        END;

        THROW;

    END CATCH
END;
GO

-----------------------------------------------

SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ALTERVISION_VENDAS_DETALHE'
ORDER BY ORDINAL_POSITION;

-----------------------------------------------------

USE AlterVisionIntegracao;
GO

CREATE OR ALTER VIEW dbo.VW_API_ALTERVISION_VENDAS_HORA AS

SELECT
    V.ALIAS_ID AS alias_id,
    L.APELIDO AS store_name,
    L.CNPJ AS store_document,

    V.DATA_VENDA AS localDate,
    0 AS localHour,

    V.CODVENDEDOR AS id_sellerPDV,
    V.VENDEDOR AS sellerName,

    COUNT(DISTINCT V.CODIGO_VENDA) AS nbSales,
    SUM(V.QTDE_ITENS) AS nbItems,
    SUM(V.TOTAL) AS total,

    MAX(V.LASTUPDATE_ORIGEM) AS lastupdate_origem,
    MAX(V.DATA_ATUALIZACAO) AS data_atualizacao
FROM dbo.ALTERVISION_VENDAS_DETALHE V
LEFT JOIN dbo.ALTERVISION_LOJAS L
    ON  L.REDE = V.REDE
    AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
    AND L.ATIVO = 1
WHERE
    V.ALIAS_ID IS NOT NULL
    AND V.STATUS IN ('S', 'O', 'C')
GROUP BY
    V.ALIAS_ID,
    L.APELIDO,
    L.CNPJ,
    V.DATA_VENDA,
    V.CODVENDEDOR,
    V.VENDEDOR;
GO

---------------------------------------------------------------------------

EXEC dbo.SP_CARGA_ALTERVISION_VENDAS_DETALHE;

-------------------------------------------------------------------------

SELECT TOP 1000 *
FROM dbo.ALTERVISION_CONTROLE_CARGA
WHERE TIPO_CARGA = 'VENDAS_DETALHE'
ORDER BY ID_CONTROLE DESC;

-------------------------------------------------------------------------

SELECT
    REDE,
    CODIGO_EMPRESA,
    CNPJ,
    COUNT(*) AS QTDE
FROM dbo.ALTERVISION_VENDAS_DETALHE
WHERE ALIAS_ID IS NULL
GROUP BY
    REDE,
    CODIGO_EMPRESA,
    CNPJ
ORDER BY QTDE DESC;

----------------------------------------------------------------------------

SELECT
    V.REDE,
    V.CODIGO_EMPRESA,
    V.CNPJ,
    L.ID_LOJA,
    L.APELIDO,
    L.NOME,
    L.ALIAS_ID,
    COUNT(*) AS QTDE_VENDAS
FROM dbo.ALTERVISION_VENDAS_DETALHE V
LEFT JOIN dbo.ALTERVISION_LOJAS L
    ON  L.REDE = V.REDE
    AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
    AND L.ATIVO = 1
WHERE V.ALIAS_ID IS NULL
GROUP BY
    V.REDE,
    V.CODIGO_EMPRESA,
    V.CNPJ,
    L.ID_LOJA,
    L.APELIDO,
    L.NOME,
    L.ALIAS_ID
ORDER BY QTDE_VENDAS DESC;

---------------------------------------------------------------------------------------------------------------------

SELECT
    V.REDE,
    V.CODIGO_EMPRESA,
    V.CNPJ,
    COUNT(*) AS QTDE_VENDAS
FROM dbo.ALTERVISION_VENDAS_DETALHE V
LEFT JOIN dbo.ALTERVISION_LOJAS L
    ON  L.REDE = V.REDE
    AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
    AND L.ATIVO = 1
WHERE 
    V.ALIAS_ID IS NULL
    AND L.ID_LOJA IS NULL
GROUP BY
    V.REDE,
    V.CODIGO_EMPRESA,
    V.CNPJ
ORDER BY QTDE_VENDAS DESC;

--------------------------------------------------------------------------

SELECT
    CASE 
        WHEN L.ID_LOJA IS NULL THEN 'LOJA NAO EXISTE EM ALTERVISION_LOJAS'
        WHEN L.ALIAS_ID IS NULL THEN 'LOJA EXISTE MAS SEM ALIAS_ID'
        ELSE 'OK'
    END AS SITUACAO,
    COUNT(*) AS QTDE
FROM dbo.ALTERVISION_VENDAS_DETALHE V
LEFT JOIN dbo.ALTERVISION_LOJAS L
    ON  L.REDE = V.REDE
    AND L.CODIGO_EMPRESA = V.CODIGO_EMPRESA
    AND L.ATIVO = 1
WHERE V.ALIAS_ID IS NULL
GROUP BY
    CASE 
        WHEN L.ID_LOJA IS NULL THEN 'LOJA NAO EXISTE EM ALTERVISION_LOJAS'
        WHEN L.ALIAS_ID IS NULL THEN 'LOJA EXISTE MAS SEM ALIAS_ID'
        ELSE 'OK'
    END;

---------------------------------------------------------------------------------------


SELECT
    REDE,
    CODIGO_EMPRESA,
    CODIGO_VENDA,
    COUNT(DISTINCT CODVENDEDOR) AS QTDE_VENDEDORES,
    COUNT(*) AS QTDE_LINHAS,
    SUM(TOTAL) AS TOTAL_SOMADO
FROM dbo.ALTERVISION_VENDAS_DETALHE
GROUP BY
    REDE,
    CODIGO_EMPRESA,
    CODIGO_VENDA
HAVING COUNT(DISTINCT CODVENDEDOR) > 1
ORDER BY QTDE_VENDEDORES DESC, QTDE_LINHAS DESC;

--------------------------------------------------------------------------------------------

SELECT
    REDE,
    CODIGO_EMPRESA,
    CODIGO_VENDA,
    DATA_VENDA,
    CODVENDEDOR,
    VENDEDOR,
    QTDE_ITENS,
    TOTAL,
    STATUS
FROM dbo.ALTERVISION_VENDAS_DETALHE
WHERE REDE = 'rede000101'
  AND CODIGO_EMPRESA = '00000009'
  AND CODIGO_VENDA = '00647613'
ORDER BY CODVENDEDOR;

select * from ALTERVISION_VENDAS_DETALHE
select * from ALTERVISION_LOJAS
select * from ALTERVISION_VENDEDORES
select * from ALTERVISION_CONTROLE_CARGA

-------------------------------------------------------------------------------------

DECLARE @DATA_INICIO DATETIME;
DECLARE @DATA_FIM DATETIME;

SELECT TOP 1
    @DATA_INICIO = DATA_INICIO_EXECUCAO,
    @DATA_FIM = DATA_FIM_EXECUCAO
FROM dbo.ALTERVISION_CONTROLE_CARGA
WHERE TIPO_CARGA = 'VENDAS_DETALHE'
  AND STATUS = 'SUCESSO'
ORDER BY ID_CONTROLE DESC;

SELECT
    ID_VENDA_DETALHE,
    REDE,
    CODIGO_EMPRESA,
    CNPJ,
    ALIAS_ID,
    CODIGO_VENDA,
    DATA_VENDA,
    EMISSAONF,
    LASTUPDATE_ORIGEM,
    CODCLIENTE,
    CLIENTE,
    CODVENDEDOR,
    VENDEDOR,
    QTDE_ITENS,
    TOTAL,
    STATUS,
    DATA_CRIACAO,
    DATA_ATUALIZACAO
FROM dbo.ALTERVISION_VENDAS_DETALHE
WHERE DATA_ATUALIZACAO BETWEEN @DATA_INICIO AND @DATA_FIM
ORDER BY DATA_ATUALIZACAO DESC;


---------------------------------------------------------------------------------------------

CREATE TABLE dbo.ALTERVISION_API_TERCEIROS (
    ID_TERCEIRO BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NOME VARCHAR(100) NOT NULL,
    CLIENT_ID VARCHAR(100) NOT NULL,
    CLIENT_SECRET_HASH VARCHAR(500) NOT NULL,
    ATIVO BIT NOT NULL DEFAULT 1,
    DATA_CADASTRO DATETIME NOT NULL DEFAULT GETDATE(),
    DATA_ULTIMO_ACESSO DATETIME NULL
);

CREATE UNIQUE INDEX UX_ALTERVISION_API_TERCEIROS_CLIENT_ID
ON dbo.ALTERVISION_API_TERCEIROS (CLIENT_ID);

select * from ALTERVISION_API_TERCEIROS

-------------------------------------------------------------------------------------

DECLARE @NOME VARCHAR(100) = 'AlterVisionApi';
DECLARE @CLIENT_ID VARCHAR(100) = 'altervision';
DECLARE @CLIENT_SECRET_HASH VARCHAR(500) = '$2a$11$DMoG7/LJP2gtaqqPaRHtO.dkBdtGuyS5Nn1IHQiUs0bjMbWufsDka';

IF EXISTS (
    SELECT 1
    FROM dbo.ALTERVISION_API_TERCEIROS
    WHERE CLIENT_ID = @CLIENT_ID
)
BEGIN
    UPDATE dbo.ALTERVISION_API_TERCEIROS
       SET NOME = @NOME,
           CLIENT_SECRET_HASH = @CLIENT_SECRET_HASH,
           ATIVO = 1
     WHERE CLIENT_ID = @CLIENT_ID;
END
ELSE
BEGIN
    INSERT INTO dbo.ALTERVISION_API_TERCEIROS (
        NOME,
        CLIENT_ID,
        CLIENT_SECRET_HASH,
        ATIVO
    )
    VALUES (
        @NOME,
        @CLIENT_ID,
        @CLIENT_SECRET_HASH,
        1
    );
END;

SELECT
    ID_TERCEIRO,
    NOME,
    CLIENT_ID,
    CLIENT_SECRET_HASH,
    ATIVO,
    DATA_CADASTRO,
    DATA_ULTIMO_ACESSO
FROM dbo.ALTERVISION_API_TERCEIROS
WHERE CLIENT_ID = @CLIENT_ID;


SELECT
    ID_TERCEIRO,
    NOME,
    CLIENT_ID,
    CLIENT_SECRET_HASH,
    LEN(CLIENT_SECRET_HASH) AS TAMANHO_HASH,
    ATIVO,
    DATA_CADASTRO,
    DATA_ULTIMO_ACESSO
FROM dbo.ALTERVISION_API_TERCEIROS
WHERE CLIENT_ID = 'altervision';