EXEC master.dbo.sp_addlinkedserver
    @server     = N'POSTGRES_SETA',
    @srvproduct = N'PostgreSQL',
    @provider   = N'MSDASQL',
    @datasrc    = N'POSTGRES_SETA';
GO

EXEC master.dbo.sp_addlinkedsrvlogin
    @rmtsrvname  = N'POSTGRES_SETA',
    @useself     = N'False',
    @locallogin  = NULL,
    @rmtuser     = N'sl_bi',
    @rmtpassword = N'L\j9azm5_#';
GO




SELECT *
FROM OPENQUERY(POSTGRES_SETA, '
    SELECT *
    FROM public.produtos
    LIMIT 10
');