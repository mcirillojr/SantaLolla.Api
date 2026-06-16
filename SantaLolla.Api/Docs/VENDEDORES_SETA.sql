SELECT *
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT
        retorno.rede,
        retorno.empresa,
        retorno.cnpj_empresa,
        retorno.apelido_empresa,
        retorno.nome_empresa,
        retorno.codvendedor,
        retorno.vendedor,
        retorno.nome_vendedor,
        retorno.cpfcnpj_vendedor,
        retorno.descricao_atividade,
        retorno.status
    FROM consultadinamica(''
        SELECT
            current_schema::char(10) AS rede,

            empresas.codigo AS empresa,
            empresas.cpfcnpj AS cnpj_empresa,
            btrim(empresas.apelido)::varchar AS apelido_empresa,
            btrim(empresas.nome)::varchar AS nome_empresa,

            vendedores.codigo AS codvendedor,
            btrim(vendedores.apelido)::varchar AS vendedor,
            btrim(vendedores.nome)::varchar AS nome_vendedor,
            vendedores.cpfcnpj AS cpfcnpj_vendedor,
			            
            btrim(atividades.descricao)::varchar AS descricao_atividade,

            vendedores.status AS status

        FROM pessoas vendedores

        INNER JOIN pessoas empresas
            ON lpad(vendedores.empresa, 8, ''''0'''')::char(8) = empresas.codigo

        LEFT JOIN atividades
            ON atividades.codigo = vendedores.atividade

        WHERE vendedores.empresa IS NOT NULL 
          AND vendedores.funcionario = ''''1''''
          AND btrim(vendedores.empresa) <> ''''''''
          AND vendedores.podevender = ''''1''''

        ORDER BY empresas.codigo, vendedores.codigo
    ''::text) retorno(
        rede character(10),
        empresa character(8),
        cnpj_empresa character(18),
        apelido_empresa character varying,
        nome_empresa character varying,
        codvendedor character(8),
        vendedor character varying,
        nome_vendedor character varying,
        cpfcnpj_vendedor character(18),
        descricao_atividade character varying,
        status character(1)
    )
    WHERE trim(retorno.rede) <> ''rede000003''
');