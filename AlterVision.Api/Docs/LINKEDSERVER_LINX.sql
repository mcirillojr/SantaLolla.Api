IF EXISTS (
    SELECT 1 
    FROM sys.servers 
    WHERE name = N'LINX_HML'
)
BEGIN
    EXEC master.dbo.sp_dropserver 
        @server = N'LINX_HML',
        @droplogins = 'droplogins';
END
GO

EXEC master.dbo.sp_addlinkedserver
    @server     = N'LINX_HML',
    @srvproduct = N'',
    @provider   = N'MSOLEDBSQL',
    @datasrc    = N'172.26.0.246,1433',
    @catalog    = N'Cliente475_ERP_HML';
GO

EXEC master.dbo.sp_addlinkedsrvlogin
    @rmtsrvname  = N'LINX_HML',
    @useself     = N'False',
    @locallogin  = NULL,
    @rmtuser     = N'475.svc.api01',
    @rmtpassword = N'F77xV@r%D,B8z+SL%o$]';
GO

SELECT *
FROM OPENQUERY(LINX_HML, '
    SELECT TOP 10 
        name,
        create_date
    FROM Cliente475_ERP_HML.sys.tables
');