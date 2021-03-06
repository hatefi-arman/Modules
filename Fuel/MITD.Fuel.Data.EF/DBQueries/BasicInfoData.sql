USE [StorageSpace]
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (1, N'SAPID', N'SAPID')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (2, N'HAFIZ', N'HAFIZ')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (3, N'IMSENGCO', N'IMSENGCO')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (4, N'IRISL', N'IRISL')
GO
INSERT [BasicInfo].[User] ([Id], [RoleId], [Name], [CompanyId]) VALUES (1, 1, N'Admin', 1)
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (1, N'Metric Ton')
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (2, N'Liter')
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (3, N'Long Ton')
GO
SET IDENTITY_INSERT [BasicInfo].[SharedGood] ON 

GO
INSERT [BasicInfo].[SharedGood] ([Id], [Name], [Code], [MainUnitId]) VALUES (1, N'Shared Good 1', N'SharedGood1', 1)
GO
INSERT [BasicInfo].[SharedGood] ([Id], [Name], [Code], [MainUnitId]) VALUES (2, N'Shared Good 2', N'SharedGood2', 2)
GO
SET IDENTITY_INSERT [BasicInfo].[SharedGood] OFF
GO
INSERT [BasicInfo].[Good] ([Id], [Name], [Code], [CompanyId], [SharedGoodId], [Unit_Id]) VALUES (1, N'Heavy Fuel Oil', N'HFO', 1, 1, NULL)
GO
INSERT [BasicInfo].[Good] ([Id], [Name], [Code], [CompanyId], [SharedGoodId], [Unit_Id]) VALUES (2, N'Marine Diesel Oil', N'MDO', 1, 2, NULL)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (1, N'0123', N'ABBA', N'ABBA In SAPID', 1, 2, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (2, N'0093', N'AGEAN', N'AGEAN In SAPID', 1, 1, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (3, N'AREZOO', N'AREZOO', N'AREZOO In HAFIZ', 2, 2, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (4, N'NESHAT', N'NESHAT', N'NESHAT In HAFIZ', 2, 1, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (5, N'0123', N'ABBA', N'ABBA In IRISL', 4, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (6, N'0093', N'AGEAN', N'AGEAN In IRISL', 4, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (7, N'AREZOO', N'AREZOO', N'AREZOO In IRISL', 4, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (8, N'NESHAT', N'NESHAT', N'NESHAT In IRISL', 4, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (9, N'5002', N'TABUK', N'TABUK In IMSENGCO', 3, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (10, N'5001', N'PAGAS', N'PAGAS In IMSENGCO', 3, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (11, N'1093', N'TINA', N'TINA In SAPID', 1, 4, 1)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (1, N'MT', CAST(1.00 AS Decimal(18, 2)), 1, NULL)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (2, N'Liter', CAST(0.99 AS Decimal(18, 2)), 1, 1)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (3, N'Long Ton', CAST(1200.00 AS Decimal(18, 2)), 1, 2)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (4, N'MT', CAST(1.00 AS Decimal(18, 2)), 2, NULL)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (5, N'Liter', CAST(0.99 AS Decimal(18, 2)), 2, 4)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (6, N'Long Ton', CAST(1200.00 AS Decimal(18, 2)), 2, 5)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (1, N'Tank1', 1)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (2, N'Tank2', 1)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (3, N'Tank3', 2)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (4, N'Tank4', 2)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (5, N'Tank5', 3)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (6, N'Tank6', 11)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (7, N'Tank7', 11)
GO
INSERT [BasicInfo].[ActivityLocation] ([Id], [Code], [Name], [Abbreviation], [Latitude], [Longitude]) VALUES (1, N'IRBND', N'Bandar Abbas', N'B. Abbas', 122.348670959473, 47.619930267334)
GO
INSERT [BasicInfo].[ActivityLocation] ([Id], [Code], [Name], [Abbreviation], [Latitude], [Longitude]) VALUES (2, N'IRBKM', N'Bandar Khomeini', N'B.I.K', 122.396670959473, 47.999930267334)
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (1, N'USD', N'USD')
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (2, N'EUR', N'EUR')
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (3, N'IR Rials', N'IRR')
GO
