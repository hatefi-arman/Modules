USE [MiniStock]
GO

EXEC	[dbo].[Cardex]
		@WarehouseId = 12,
		@GoodId = 10,
		@StartDate = N'2014-01-01',
		@EndDate = N'2015-01-01'
GO

select * from OperationReference
select * from Transactions
select * from TransactionItems
select * from TransactionItemPrices
select * from Warehouse