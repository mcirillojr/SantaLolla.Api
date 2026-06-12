SELECT *
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

            vendas.empresa,
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
          AND vendas.lastupdate > now() - interval ''''61 day''''
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
        empresa character(2),
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