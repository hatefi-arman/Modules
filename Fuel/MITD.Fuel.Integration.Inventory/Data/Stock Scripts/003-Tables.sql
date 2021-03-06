ALTER DATABASE MiniStock SET MULTI_USER
USE MiniStock
GO 
-------------------------------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[ErrorMessages]') AND type in (N'U'))
BEGIN
Create table ErrorMessages
(
	ErrorMessage NVARCHAR(200) NOT NULL,
	TextMessage NVARCHAR(200) NOT NULL,
	[Action] NVARCHAR(20),
	CONSTRAINT PK_Key PRIMARY KEY(ErrorMessage,TextMessage,Action)
)
END	
GO
---------------------User----------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[Users]') AND type in (N'U'))
BEGIN
Create table Users
(
	Id INT  NOT NULL,--IdENTITY(1,1)
	Code int NOT NULL, 
	Name nvarchar(100) NOT NULL,
	[User_Name] VARCHAR(256) NOT NULL,
	[Password] nvarchar(100) NOT NULL,
	Active BIT DEFAULT 1,
	Email_Address VARCHAR(256) NULL,
	IPAddress VARCHAR(15) NULL,
	Login BIT DEFAULT 0,
	SessionId NVARCHAR(88) NULL,
	UserCreatorId int NULL,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Users_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Users_Code UNIQUE (Code),
	CONSTRAINT UQ_Users_User_Name UNIQUE ([User_Name]),
	CONSTRAINT FK_Users_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
) 
END
GO	
--------------------------------------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[FinancialYear]') AND type in (N'U'))
BEGIN
Create table FinancialYear
(
	Id INT IdENTITY(1,1) NOT NULL,
	Name nvarchar(256) NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	UserCreatorId INT NOT NULL,
	CreateDate DATETIME DEFAULT getdate(),
	--Is_Active BIT DEFAULT 0,
	CONSTRAINT PK_FinancialYear_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_FinancialYear_Name UNIQUE (Name),
    CONSTRAINT UQ_FinancialYear_StartDate UNIQUE ([StartDate]),
    CONSTRAINT UQ_FinancialYear_EndDate UNIQUE ([EndDate]),
	CONSTRAINT FK_FinancialYear_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id),
	CONSTRAINT chk_IsActualDateInFinancialYear CHECK (dbo.IsActualDateInFinancialYear([StartDate],[EndDate])<>0),
	CONSTRAINT chk_IsValIdDayInFinancialYear CHECK (dbo.IsValIdDayInFinancialYear(Id,[StartDate],[EndDate])<>0)
) 
END
GO	
-------------------------------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[TimeBucket]') AND type in (N'U'))
BEGIN
Create table TimeBucket
(
	Id INT IdENTITY(1,1) NOT NULL,
	Name nvarchar(256) NOT NULL,
	[StartDate] DATETIME  DEFAULT getdate(),
	[EndDate] DATETIME  DEFAULT getdate(),
	FinancialYearId INT NOT NULL,
	UserCreatorId INT NOT NULL,
	CreateDate DATETIME DEFAULT getdate(),
	Active BIT DEFAULT 0,
	CONSTRAINT PK_TimeBucket_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_TimeBucket_Name_FinancialYearId UNIQUE (NAME,FinancialYearId),
    CONSTRAINT UQ_TimeBucket_StartDate UNIQUE ([StartDate]),
    CONSTRAINT UQ_TimeBucket_EndDate UNIQUE ([EndDate]),
	CONSTRAINT FK_TimeBucket_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id),
	CONSTRAINT FK_TimeBucket_FinancialYearId FOREIGN KEY(FinancialYearId) REFERENCES FinancialYear(Id),
	CONSTRAINT chk_IsActualDateInTimeBucket CHECK (dbo.IsActualDateInTimeBucket([StartDate],[EndDate])<>0),
	CONSTRAINT chk_IsValIdDateInTimeBucket CHECK (dbo.IsValIdDateInTimeBucket(Id,FinancialYearId,[StartDate])<>1),
	CONSTRAINT chk_IsValIdDateInTimeBucket_Than_FinancialYear CHECK (dbo.IsValIdDateInTimeBucket_Than_FinancialYear(Id,FinancialYearId,[StartDate],EndDate)<>1),
	CONSTRAINT chk_IsNoGapInTimeBucket CHECK (dbo.IsNoGapInTimeBucket(Id,FinancialYearId,[StartDate])<>1),
	CONSTRAINT chk_IsDiffDateGreaterThan29DaysInTimeBucket CHECK (dbo.IsDiffDateGreaterThan29DaysInTimeBucket([STARTDATE],EndDate)<>0),
	CONSTRAINT chk_IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket CHECK (dbo.IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket(Id,FinancialYearId,StartDate,EndDate)<>0)
) 
END
GO	
----------------------Company---------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[Companies]') AND type in (N'U'))
BEGIN
Create table Companies
(
	Id INT  NOT NULL,--IdENTITY(1,1)
	Code  NVARCHAR(10) NOT NULL, 
	Name nvarchar(256) NOT NULL,
	Active BIT DEFAULT 1,
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Companies_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Companies_Code UNIQUE (Code),
	CONSTRAINT UQ_Companies_Name UNIQUE ([Name]),
	CONSTRAINT FK_Companies_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
) 
END
GO
/************************* انبار **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'Warehouse'))
BEGIN
Create table Warehouse
(
	Id INT Not Null,-- Identity(1,1)
	Code  NVARCHAR(10)	NOT NULL,
	Name NVARCHAR(256),
	CompanyId INT Not NULL,
	Active BIT DEFAULT 1,
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Warehouse_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Warehouse_Code UNIQUE (Code,NAME,CompanyId),
	CONSTRAINT FK_Warehouse_CompanyId FOREIGN KEY(CompanyId) REFERENCES Companies(Id),
	CONSTRAINT FK_Warehouse_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
)
END
GO
/************************* کالا **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'Goods'))
BEGIN
Create table Goods
(
	Id int  Not Null,--Identity(1,1)
	Code nvarchar(100) NOT NULL,
	Name  nvarchar(200) NOT NULL,
	Active bit default 1 NOT NULL,
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Goods_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Goods_Code UNIQUE (Code),
	CONSTRAINT UQ_Goods_Name UNIQUE (Name),
	CONSTRAINT FK_Goods_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
)
END
GO
/************************* واحدهای اندازه گیری  **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'Units'))
BEGIN
Create table Units
(
	Id INT Identity(1,1) Not Null,
	Code NVARCHAR(15) NOT NULL,
	Name NVARCHAR(100)NOT NUll,
	IsCurrency BIT DEFAULT 0,
	IsBaseCurrency BIT DEFAULT 0,
	Active bit default 1 NOT NULL,
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Units_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Units_Code UNIQUE (Code),
	CONSTRAINT UQ_Units_Name UNIQUE (Name),
	CONSTRAINT FK_Units_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
)
END	
GO
-------------------------------------
if not Exists(SELECT * FROM sys.objects WHERE object_Id = OBJECT_Id(N'[UnitConverts]') AND type in (N'U'))
BEGIN
Create table UnitConverts
(
	Id INT IdENTITY(1,1) NOT NULL,
	UnitId INT NOT NULL,
	SubUnitId INT NOT NULL,
	Coefficient DECIMAL(18,3) NOT NULL,
	--FinancialYearId INT,
	EffectiveDate DATETIME  DEFAULT getdate(),
	UserCreatorId INT NOT NULL,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_UnitConverts_Id PRIMARY KEY(Id),
    CONSTRAINT UQ_UnitConverts_PrimarySecondary_FinancialYear UNIQUE (UnitId,SubUnitId,EffectiveDate),
    CONSTRAINT FK_UnitConverts_UnitId FOREIGN KEY(UnitId) REFERENCES Units(Id),
    CONSTRAINT FK_UnitConverts_SubUnitId FOREIGN KEY(SubUnitId) REFERENCES Units(Id),
	CONSTRAINT FK_UnitConverts_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id),
	--CONSTRAINT FK_UnitConverts_FinancialYearId FOREIGN KEY(FinancialYearId) REFERENCES FinancialYear(Id),
	CONSTRAINT chk_NoMatch2UnitInUnitConverts CHECK (dbo.NoMatch2UnitInUnitConverts(UnitId,SubUnitId)<>0)
) 
END
/************************* انواع رسید و حواله **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'StoreTypes'))
BEGIN
Create table StoreTypes
(
	Id INT Identity(1,1) Not Null,
	Code smallint NOT NULL,
	Type TINYINT NOT NULL DEFAULT 0,--      1-Person   2-Warehouse     3-CostCenter
	InputName nvarchar(100) ,
	OutputName nvarchar(100) ,
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	constraint PK_StoreTypes_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_StoreTypes_Code UNIQUE (Code),
	--CONSTRAINT UQ_StoreTypes_InputName UNIQUE (InputName),
	--CONSTRAINT UQ_StoreTypes_OutputName UNIQUE (OutputName),
	CONSTRAINT FK_StoreTypes_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
)
END
GO
/************************* رسید,حواله,درخواست خرید,درخواست کالا  **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'Transactions'))
BEGIN
Create table Transactions
(
	Id int Identity(1,1) Not Null,
	[Action] tinyint Not Null,--رسيد -حواله - درخواست خريد - درخواست کالا
	Code decimal(20,2) NULL,--شماره رسید یا حواله یا درخواست خرید,درخواست کالا 
	[Description] nvarchar(max),
	--CrossId int ,--شماره رسيد يا حواله مقابل که خودکار ثبت شده
	PricingReferenceId INT NULL,--شماره مرجع براي قيمتگذاري - رسيد يا حواله هاي برگشتي
	WarehouseId INT Not Null,-- شماره انبار
	StoreTypesId INT Not Null,--نوع رسيد و حواله(خريد؛برگشت از فروش؛ انتقال و ... )
	TimeBucketId INT not null,
	Status tinyint DEFAULT 1 ,--  1-Normal   2-Partial Priced  3-Full Priced  4-Vouchered
	RegistrationDate DATETIME DEFAULT getdate(),--تاريخ ثبت
	SenderReciver int null,--شماره طرف حساب
	HardCopyNo NVARCHAR(10),--شماره فاکتور خريد کاغذي
	ReferenceType NVARCHAR(100),--نوع مرجع
	ReferenceNo NVARCHAR(100),--شماره مرجع
	ReferenceDate DATETIME DEFAULT getdate(),--تاريخ استفاده از مرجع
	UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
	CONSTRAINT PK_Transaction_Id PRIMARY KEY(Id),
	CONSTRAINT UQ_Transaction_Code UNIQUE ([Action],Code),
	CONSTRAINT UQ_Transaction_ReferenceNo UNIQUE (ReferenceType,ReferenceNo,[Action]),
	--ONSTRAINT FK_Transaction_CrossId FOREIGN KEY(CrossId) REFERENCES Transactions(Id),
	CONSTRAINT FK_Transaction_PricingReferenceId FOREIGN KEY(PricingReferenceId) REFERENCES Transactions(Id),
	CONSTRAINT FK_Transaction_WarehouseId FOREIGN KEY(WarehouseId) REFERENCES Warehouse(Id),
	CONSTRAINT FK_Transaction_StoreTypesId FOREIGN KEY(StoreTypesId) REFERENCES StoreTypes(Id),
	CONSTRAINT FK_Transaction_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id),
	CONSTRAINT chk_IsValidAction check(Action>=1 and Action<=3)
)
END
GO
/************************* بدنه رسید,حواله,درخواست خرید,درخواست کالا   **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'TransactionItems'))
BEGIN
CREATE TABLE TransactionItems
(
	Id int Identity(1,1) Not Null,
	[RowVersion] smallint not null,
	--Action tinyint Not Null,
	TransactionId int not null,--شماره رسید یا حواله یا درخواست خرید,درخواست کالا 
	--RegistrationDate DATETIME DEFAULT getdate(),--تاريخ ثبت
	GoodId int not null,
    QuantityUnitId INT NOT NULL,
    QuantityAmount DECIMAL(20,3) DEFAULT 0,
    Description nvarchar(max),
    UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
    CONSTRAINT PK_TransactionItems_Id PRIMARY KEY(Id),
    CONSTRAINT UQ_TransactionItems_RowVersion UNIQUE (RowVersion,TransactionId),
	CONSTRAINT FK_TransactionItems_TransactionId FOREIGN KEY(TransactionId) REFERENCES Transactions(Id),
	CONSTRAINT FK_TransactionItems_GoodId FOREIGN KEY(GoodId) REFERENCES Goods(Id),
	CONSTRAINT FK_TransactionItems_QuantityUnitId FOREIGN KEY(QuantityUnitId) REFERENCES Units(Id),
	CONSTRAINT FK_TransactionItems_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id)
)
END
GO
/************************* قيمت گذاري بدنه رسید,حواله,درخواست خرید,درخواست کالا   **************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'TransactionItemPrices'))
BEGIN
CREATE TABLE TransactionItemPrices
(
	Id int Identity(1,1) Not Null,
	[RowVersion] smallint not null,
	--Action tinyint Not Null,
	TransactionId int not null,--شماره رسید یا حواله یا درخواست خرید,درخواست کالا 
	TransactionItemId int not null,--شماره آيتم رسید یا حواله یا درخواست خرید,درخواست کالا 
	Description nvarchar(max),
	QuantityUnitId INT NOT NULL,
    QuantityAmount DECIMAL(20,3) DEFAULT 0,
    PriceUnitId INT NOT NULL,
    Fee DECIMAL(20,3) DEFAULT 0,
    MainCurrencyUnitId INT NOT NULL,
    FeeInMainCurrency DECIMAL(20,3) DEFAULT 0,
    --Coefficient DECIMAL(18,3) NOT NULL,
    RegistrationDate DATETIME DEFAULT getdate(),--تاريخ قيمت گذاري
    QuantityAmountUseFIFO DECIMAL(20,3) DEFAULT 0,
    TransactionReferenceId int NULL,
    IssueReferenceIds NVARCHAR(MAX) DEFAULT N'',
    UserCreatorId int,
	CreateDate DATETIME DEFAULT getdate(),
    CONSTRAINT PK_TransactionItemPrices_Id PRIMARY KEY(Id),
    CONSTRAINT UQ_TransactionItemPrices_RowVersion UNIQUE (RowVersion,TransactionItemId),
	--CONSTRAINT FK_TransactionItemPrices_TransactionId FOREIGN KEY(TransactionId) REFERENCES Transaction(Id),
	CONSTRAINT FK_TransactionItemPrices_TransactionItemsId FOREIGN KEY(TransactionItemId) REFERENCES TransactionItems(Id),
	CONSTRAINT FK_TransactionItemPrices_TransactionReferenceId FOREIGN KEY(TransactionReferenceId) REFERENCES TransactionItemPrices(Id),
	CONSTRAINT FK_TransactionItemPrices_QuantityUnitId FOREIGN KEY(QuantityUnitId) REFERENCES Units(Id),
	CONSTRAINT FK_TransactionItemPrices_PriceUnitId FOREIGN KEY(PriceUnitId) REFERENCES Units(Id),
	CONSTRAINT FK_TransactionItemPrices_UserCreatorId FOREIGN KEY(UserCreatorId) REFERENCES Users(Id),
	Constraint chk_IsValIdQuantityAmountUseFIFO check(QuantityAmountUseFIFO<=QuantityAmount)
)
END
GO
/**************************************************/
if not Exists(select * from sys.tables where UPPER(NAME) = UPPER(N'OperationReference'))
BEGIN
CREATE TABLE OperationReference
(
	[Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [OperationId]   BIGINT         NOT NULL,
    [OperationType] INT            NOT NULL,
    [ReferenceType]   VARCHAR (512) NOT NULL,
    [ReferenceNumber] VARCHAR (256)  NOT NULL,
	[RegistrationDate] DATETIME NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_TransactionReference] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO
GO
-------------------------------------
raiserror(N'جدول های دیتابیس با موفقیت ایجاد شدند.',0,1) with nowait
GO