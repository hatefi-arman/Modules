USE MiniStock
GO 
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ErrorMessages]') 
					AND name = N'idxErrorMessage')
		CREATE INDEX idxErrorMessage ON ErrorMessages(ErrorMessage)
GO 

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[User_List]') 
					AND name = N'idxUser_List')					
		CREATE INDEX idxUser_List ON User_List(Code,[User_Name],Role_ID)
GO 
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[OperationReference]') 
					AND name = N'idxOperationReference')					
		CREATE INDEX idxOperationReference ON OperationReference(ReferenceType,ReferenceNumber)
GO 
raiserror(N'پایان ایجاد ایندکس ها.',0,1) with nowait
GO