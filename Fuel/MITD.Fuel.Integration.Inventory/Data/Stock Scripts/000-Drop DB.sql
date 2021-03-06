USE [master]
GO

IF EXISTS (SELECT * FROM sys.sysdatabases s WHERE s.name='MiniStock')
BEGIN
	EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'MiniStock'
	
	USE [master]
	
	ALTER DATABASE [MiniStock] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE

	DROP DATABASE MiniStock
END
GO
