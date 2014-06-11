USE [MiniStock]
GO

/****** Object: Table [dbo].[TransactionReference] Script Date: 8/6/2014 1:56:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OperationReference] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [OperationId]   BIGINT         NOT NULL,
    [OperationType] INT            NOT NULL,
    [ReferenceType]   VARCHAR (512) NOT NULL,
    [ReferenceNumber] VARCHAR (256)  NOT NULL,
    CONSTRAINT [PK_TransactionReference] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TransactionReference_Column]
    ON [dbo].[OperationReference]([ReferenceType] ASC, [ReferenceNumber] ASC);
GO