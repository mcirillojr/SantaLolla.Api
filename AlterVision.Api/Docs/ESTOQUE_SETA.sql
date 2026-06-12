SELECT *
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT 
        retorno.rede,
        retorno.empresa,
        retorno.cnpj,
        retorno.apelido,
        retorno.nome,
        retorno.codigo,
        retorno.descricao,
        retorno.tamanho,
        retorno.cor,
        retorno.grade,
        retorno.referencia,
        retorno.marca,
        retorno.grupo,
        retorno.quantidade,
        retorno.custo,
        retorno.preco,
        retorno.preco1,
        retorno.preco2
    FROM consultadinamica(''
        SELECT 
            current_schema::char(10) AS rede,

            m.empresa,
            empresas.cpfcnpj AS cnpj,
            btrim(empresas.apelido)::varchar AS apelido,
            btrim(empresas.nome)::varchar AS nome,

            vprodutos.codigo,
            btrim(vprodutos.descricao)::varchar AS descricao,

            substr(m.produto, 7, 2)::char(2) AS tamanho,
            btrim(vprodutos.corx)::varchar AS cor,

            vprodutos.grade,
            btrim(vprodutos.referencia)::varchar AS referencia,
            btrim(marcas.descricao)::varchar AS marca,
            btrim(grupos.descricao)::varchar AS grupo,

            SUM(
                CASE 
                    WHEN m.movimento = ''''E''''::bpchar 
                        THEN m.quantidade 
                    ELSE -m.quantidade 
                END
            )::integer AS quantidade,

            vprodutos.custo,
            vprodutos.preco,
            vprodutos.preco1,
            vprodutos.preco2

        FROM produtos AS vprodutos

        LEFT JOIN marcas 
            ON vprodutos.marca = marcas.codigo

        LEFT JOIN pessoas AS fornecedores 
            ON vprodutos.fornecedor = fornecedores.codigo

        LEFT JOIN departamentos 
            ON vprodutos.departamento = departamentos.codigo

        LEFT JOIN grupos 
            ON vprodutos.grupo = grupos.codigo

        LEFT JOIN subgrupos 
            ON vprodutos.subgrupo = subgrupos.codigo

        LEFT JOIN grades 
            ON vprodutos.grade = grades.codigo

        LEFT JOIN colecoes 
            ON vprodutos.colecao = colecoes.codigo

        LEFT JOIN movimento AS m 
            ON vprodutos.codigo = substr(m.produto, 1, 6)::char(6)
           AND m.estoque

        LEFT JOIN pessoas empresas 
            ON empresas.codigo = lpad(m.empresa, 8, ''''0'''')::char(8)

        LEFT JOIN linhas 
            ON vprodutos.linha = linhas.codigo

        WHERE vprodutos.cadastro < current_date
          AND vprodutos.desativar = false
          AND m.data < current_date
		  and vprodutos.referencia in (''''0453.4062.0378.0001'''',''''0452.1E48.0378.0001'''')

        GROUP BY 
            current_schema,
            empresas.cpfcnpj,
            empresas.nome,
            empresas.apelido,
            m.empresa,
            vprodutos.codigo,
            vprodutos.descricao,
            m.produto,
            vprodutos.corx,
            vprodutos.grade,
            vprodutos.referencia,
            marcas.descricao,
            grupos.descricao,
            vprodutos.custo,
            vprodutos.preco,
            vprodutos.preco1,
            vprodutos.preco2
    ''::text) retorno(
        rede character(10),
        empresa character(2),
        cnpj character(18),
        apelido character varying,
        nome character varying,
        codigo character(6),
        descricao character varying,
        tamanho character(2),
        cor character varying,
        grade character(3),
        referencia character varying,
        marca character varying,
        grupo character varying,
        quantidade integer,
        custo numeric(8,2),
        preco numeric(8,2),
        preco1 numeric(8,2),
        preco2 numeric(8,2)
    )
    WHERE trim(retorno.rede) <> ''rede000003''
      AND retorno.quantidade <> 0
');