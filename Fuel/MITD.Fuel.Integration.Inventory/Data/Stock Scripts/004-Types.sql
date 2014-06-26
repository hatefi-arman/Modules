USE MiniStock
GO 
 -------------------------------------------------------------
IF NOT EXISTS(
   SELECT NAME
   FROM   sys.types
   WHERE  NAME ='TypeTransactionItems'
)
CREATE TYPE TypeTransactionItems AS TABLE 
(
    Id int Null,
	GoodId int not null,
	QuantityUnitId INT,
	QuantityAmount DECIMAL(20,3) DEFAULT 0,
	[Description] NVARCHAR(max) NULL
)
 GO
 -------------------------------------------------------------
IF NOT EXISTS(
   SELECT NAME
   FROM   sys.types
   WHERE  NAME ='Ids'
)
CREATE TYPE Ids AS TABLE 
(
    Id int NULL,
    [Description] NVARCHAR(MAX) NULL
)
 GO
 -------------------------------------------------------------
IF NOT EXISTS(
   SELECT NAME
   FROM   sys.types
   WHERE  NAME ='TypeTransactionItemPrices'
)
CREATE TYPE TypeTransactionItemPrices AS TABLE 
(
    Id int Null,
	--GoodId int not null,
	TransactionItemId INT NOT NULL,
	QuantityUnitId INT NOT NULL,
	QuantityAmount DECIMAL(20,3) DEFAULT 0,
	PriceUnitId INT NOT NULL,
	Fee DECIMAL(20,3) DEFAULT 0,
	RegistrationDate DATETIME DEFAULT (CONVERT([date],getdate(),(111))),
	[Description] nvarchar(max) NULL
)
 GO
 -------------------------------------
raiserror(N'جدول نوع هاي داده با موفقیت ایجاد شدند.',0,1) with nowait
GO

