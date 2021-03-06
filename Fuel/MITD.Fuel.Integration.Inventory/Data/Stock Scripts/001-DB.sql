use master
GO 
IF NOT EXISTS (SELECT * FROM sys.sysdatabases s WHERE s.name='MiniStock')
	CREATE DATABASE MiniStock ON PRIMARY
		(NAME = 'MiniStock', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MiniStock.MDF', SIZE = 6000KB , FILEGROWTH = 1024KB )
		LOG ON
		(NAME = 'MiniStock_log', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\MiniStock_Log.ldf', SIZE = 1024KB , FILEGROWTH = 10%)
GO

USE MASTER
GO
EXEC SP_CONFIGURE 'show advanced options',1
GO
RECONFIGURE
GO
EXEC sp_configure 'xp_cmdshell', 1
GO
RECONFIGURE
GO

ALTER DATABASE MiniStock SET ALLOW_SNAPSHOT_ISOLATION ON;
GO
ALTER DATABASE MiniStock SET READ_COMMITTED_SNAPSHOT ON;
GO