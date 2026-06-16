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