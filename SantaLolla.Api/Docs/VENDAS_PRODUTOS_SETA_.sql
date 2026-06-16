SELECT *
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT 
        retorno.rede,
        retorno.venda,
        retorno.codcliente,
        retorno.codvendedor,
        retorno.referencia,
        retorno.barras,
        retorno.produto,
        retorno.tamanho,
        retorno.quantidade,
        retorno.empresa,
        retorno.cnpj,
        retorno.apelido,
        retorno.nome,
        retorno.data,
        retorno.lastupdate,
        retorno.cliente,
        retorno.vendedor,
        retorno.condicoes,
        retorno.unitario,
        retorno.desconto,
        retorno.total,
        retorno.frete,
        retorno.total_frete,
        retorno.custo,
        retorno.venda_importada,
        retorno.status,
        retorno.colecao,
        retorno.fornecedor
    FROM consultadinamica(''
        SELECT 
            current_schema::char(10) AS rede,
            vendas.codigo AS venda,
            vendas.cliente AS codcliente,
            movimento.vendedorm AS codvendedor,

            produtos.referencia AS referencia,

            (
                SELECT codigo 
                FROM barras 
                WHERE produto = movimento.produto 
                LIMIT 1
            ) AS barras,

            left(movimento.produto, 6)::char(6) AS produto,
            right(movimento.produto, 2)::char(2) AS tamanho,

            movimento.quantidade,
            vendas.empresa,

            empresas.cpfcnpj AS cnpj,
            btrim(empresas.apelido)::varchar AS apelido,
            btrim(empresas.nome)::varchar AS nome,

            vendas.data,
            vendas.lastupdate::timestamp AS lastupdate,

            btrim(clientes.nome)::varchar AS cliente,
            btrim(vendedores.apelido)::varchar AS vendedor,
            btrim(condicoes.descricao)::varchar AS condicoes,

            movimento.unitario,
            movimento.desconto,
            movimento.total,
            movimento.frete,

            (movimento.total + movimento.frete)::numeric(12,2) AS total_frete,
            (movimento.quantidade * movimento.custo)::numeric(10,2) AS custo,

            CASE 
                WHEN vendas.obs LIKE ''''%VENDA IMPORTADA EM%'''' 
                    THEN ''''Sim'''' 
                ELSE ''''Não'''' 
            END::char(3) AS venda_importada,

            vendas.status,

            colecoes.descricao::char(30) AS colecao,
            fornecedor.cpfcnpj AS fornecedor

        FROM vendas

        INNER JOIN movimento 
            ON (''''VE'''' || vendas.codigo)::char(10) = movimento.auxiliar

        INNER JOIN produtos 
            ON left(movimento.produto, 6)::char(6) = produtos.codigo

        INNER JOIN pessoas empresas 
            ON lpad(vendas.empresa, 8, ''''0'''')::char(8) = empresas.codigo

        INNER JOIN pessoas clientes 
            ON vendas.cliente = clientes.codigo

        INNER JOIN pessoas vendedores 
            ON movimento.vendedorm = vendedores.codigo

        INNER JOIN condicoes 
            ON vendas.condicoes = condicoes.codigo

        LEFT JOIN colecoes 
            ON colecoes.codigo = produtos.colecao

        LEFT JOIN pessoas fornecedor 
            ON fornecedor.codigo = produtos.fornecedor

        LEFT JOIN pessoas avalista 
            ON vendas.avalista = avalista.codigo

        WHERE movimento.estoque = true
          AND vendas.status IN (''''S'''', ''''O'''', ''''C'''')
          AND movimento.operacao IN (''''VE'''', ''''DV'''')
          AND vendas.lastupdate > now() - interval ''''11 day''''
          AND vendas.tipo = ''''01''''
    ''::text) retorno(
        rede character(10),
        venda character(8),
        codcliente character(8),
        codvendedor character(8),
        referencia character(25),
        barras character(24),
        produto character(6),
        tamanho character(2),
        quantidade numeric(9,3),
        empresa character(2),
        cnpj character(18),
        apelido character varying,
        nome character varying,
        data date,
        lastupdate timestamp without time zone,
        cliente character varying,
        vendedor character varying,
        condicoes character varying,
        unitario numeric(10,4),
        desconto numeric(8,2),
        total numeric(8,2),
        frete numeric(10,2),
        total_frete numeric(12,2),
        custo numeric(10,2),
        venda_importada character(3),
        status character(1),
        colecao character(30),
        fornecedor character(18)
    )
    WHERE trim(retorno.rede) <> ''rede000003''
');