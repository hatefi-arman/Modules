USE [MiniStock]

DELETE FROM OperationReference;
DELETE FROM TransactionItemPrices;
DELETE FROM TransactionItems;
DELETE FROM Transactions;

DELETE FROM [Goods];
DELETE FROM [StoreTypes];

DELETE FROM [UnitConverts];
DELETE FROM [Units];

DELETE FROM [TimeBucket];
DELETE FROM [FinancialYear];

DELETE FROM [Warehouse];
DELETE FROM [Companies];
DELETE FROM [Users];

GO

DECLARE @UserId INT;
SET @UserId = 1101;

INSERT INTO [dbo].[Users] ([Id], [Code], [Name], [User_Name], [Password], [Active], [Email_Address], [IPAddress], [Login], [SessionId], [UserCreatorId], [CreateDate]) VALUES (@UserId, @UserId, N'user1', N'fueluser', N'fueluser', 1, NULL, NULL, 0, NULL, @UserId, GETDATE())

------------------------------------------------------------------------------------------

INSERT INTO [dbo].[Companies] ([Id], [Code], [Name], [Active], UserCreatorId, CreateDate) VALUES (10, N'IRISL', N'IRISL', 1, @UserId, GETDATE())
INSERT INTO [dbo].[Companies] ([Id], [Code], [Name], [Active], UserCreatorId, CreateDate) VALUES (11, N'SAPID', N'SAPID', 1, @UserId, GETDATE())

------------------------------------------------------------------------------------------

INSERT INTO [dbo].[Warehouse] ([Id], [Code], [Name], [CompanyId], [Active], UserCreatorId, CreateDate) VALUES (13, '0123', N'ABBA in IRISL', 10, 0, @UserId, GETDATE())
INSERT INTO [dbo].[Warehouse] ([Id], [Code], [Name], [CompanyId], [Active], UserCreatorId, CreateDate) VALUES (12, '0123', N'ABBA in SAPID', 11, 0, @UserId, GETDATE())

------------------------------------------------------------------------------------------

SET IDENTITY_INSERT [dbo].[FinancialYear] ON
INSERT INTO [dbo].[FinancialYear] ([Id], [Name], [StartDate], [EndDate], UserCreatorId, CreateDate) VALUES (1, N'93', N'2014-03-21 00:00:00', N'2015-03-20 00:00:00', @UserId, GETDATE())
SET IDENTITY_INSERT [dbo].[FinancialYear] OFF

SET IDENTITY_INSERT [dbo].[TimeBucket] ON
INSERT INTO [dbo].[TimeBucket] ([Id], [Name], [StartDate], [EndDate], [FinancialYearId], [Active], UserCreatorId, CreateDate) VALUES (1, N'93-1', N'2014-03-21 00:00:00', N'2015-03-20 00:00:00', 1, 1, @UserId, GETDATE())
SET IDENTITY_INSERT [dbo].[TimeBucket] OFF

--------------------------------------------------------------------------------------------

SET IDENTITY_INSERT [dbo].[Units] ON
INSERT INTO [dbo].[Units] ([Id], [Code], [Name], [IsCurrency], [IsBaseCurrency], [Active], UserCreatorId, CreateDate) VALUES (1, N'TON', N'تن', 0, 0, 1,  @UserId, GETDATE())
INSERT INTO [dbo].[Units] ([Id], [Code], [Name], [IsCurrency], [IsBaseCurrency], [Active], UserCreatorId, CreateDate) VALUES (2, N'IRR', N'ريال', 1, 1, 1,   @UserId, GETDATE())
INSERT INTO [dbo].[Units] ([Id], [Code], [Name], [IsCurrency], [IsBaseCurrency], [Active], UserCreatorId, CreateDate) VALUES (3, N'USD', N'دلار', 1, 0, 1, @UserId, GETDATE())
SET IDENTITY_INSERT [dbo].[Units] OFF


SET IDENTITY_INSERT [dbo].[UnitConverts] ON
INSERT INTO [dbo].[UnitConverts] ([Id], [UnitId], [SubUnitId], [Coefficient], [EffectiveDate], UserCreatorId, CreateDate) VALUES (1, 3, 2, CAST(2500.000 AS Decimal(18, 3)), N'2014-06-10 00:00:00', @UserId, GETDATE())
SET IDENTITY_INSERT [dbo].[UnitConverts] OFF

--------------------------------------------------------------------------------------------

SET IDENTITY_INSERT [dbo].[StoreTypes] ON
INSERT INTO [dbo].[StoreTypes] ([Id], [Code], [Type], [InputName], [OutputName], UserCreatorId, CreateDate) VALUES (1, 1, 1, N'In', NULL, @UserId, GETDATE())  --Type=1 : Receipt ,    Type=2 : Issue     Usage in current implementation
INSERT INTO [dbo].[StoreTypes] ([Id], [Code], [Type], [InputName], [OutputName], UserCreatorId, CreateDate) VALUES (2, 2, 2, NULL, N'Out', @UserId, GETDATE())  --Type=1 : Receipt ,    Type=2 : Issue
SET IDENTITY_INSERT [dbo].[StoreTypes] OFF

--------------------------------------------------------------------------------------------

INSERT INTO [dbo].[Goods] ([Id], [Code], [Name], [Active], UserCreatorId, CreateDate) VALUES (10, N'HFO', N'سوخت سنگین', 1,   @UserId, GETDATE())
INSERT INTO [dbo].[Goods] ([Id], [Code], [Name], [Active], UserCreatorId, CreateDate) VALUES (11, N'MDO', N'سوخت دیزل', 1,    @UserId, GETDATE())
INSERT INTO [dbo].[Goods] ([Id], [Code], [Name], [Active], UserCreatorId, CreateDate) VALUES (12, N'MGO', N'سوخت گازوئیل', 1, @UserId, GETDATE())

--------------------------------------------------------------------------------------------

GO

raiserror(N'پایان ايجاد داده هاي پيش فرض سيستم.',0,1) with nowait
GO	