USE MiniStock
GO 
ALTER DATABASE MiniStock SET RECURSIVE_TRIGGERS OFF
GO
----------------------------------------------------------------------
IF OBJECT_ID (N'[ErrorHandling]',N'P') IS NOT NULL
   DROP PROCEDURE [ErrorHandling];
GO
CREATE PROCEDURE [ErrorHandling] 
--WITH ENCRYPTION
AS
BEGIN
	IF ERROR_NUMBER() IS NULL
		RETURN;
		
	DECLARE 
		@ErrorMessage    NVARCHAR(4000),
		@ErrorNumber     INT,
		@ErrorSeverity   INT,
		@ErrorState      INT,
		@ErrorLine       INT,
		@ErrorProcedure  NVARCHAR(200),
	    @ErrorText NVARCHAR(MAX)=NULL;
	    
	SELECT 
			@ErrorMessage= ERROR_MESSAGE(),
			@ErrorNumber = ERROR_NUMBER(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE(),
			@ErrorLine = ERROR_LINE(),
			@ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '-');
		
	
	IF LEFT(@ErrorMessage,1)=N'@'
	  BEGIN
		SET @ErrorText =SUBSTRING(@ErrorMessage,2,LEN(@ErrorMessage))
		RAISERROR(@ErrorText,16,1,500)
	  END
	ELSE
	 BEGIN
		SET @ErrorText=(SELECT em.TextMessage  
					FROM [ErrorMessages] em
					WHERE (PATINDEX('%'+''''+UPPER(em.ErrorMessage)+''''+'%',UPPER(@ErrorMessage))<>0 OR 
					   PATINDEX('%'+'"'+UPPER(em.ErrorMessage)+'"'+'%',UPPER(@ErrorMessage))<>0)AND
						   PATINDEX('%'+UPPER(em.[Action])+'%',UPPER(@ErrorMessage))<>0)
 
		IF @ErrorText IS NULL OR @ErrorText=''
		  BEGIN
			SELECT @ErrorMessage =N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' + 
									'Message: '+ ERROR_MESSAGE();
			RAISERROR 
			(
			@ErrorMessage, 
			@ErrorSeverity, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
			@ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
			);
		  END
		ELSE
			RAISERROR(@ErrorText,16,1,'500')
	 END
END 
GO
-------- IsValidTransactionCode -------
	IF OBJECT_ID ( 'IsValidTransactionCode', 'P' ) IS NOT NULL 
		DROP PROCEDURE IsValidTransactionCode;
	GO
	CREATE PROC IsValidTransactionCode
	(
		@Action tinyint,
		@Code decimal(20,2),
		@WarehouseId INT,
		@RegistrationDate DATETIME,
		@TimeBucketId INT
	)
	AS
	BEGIN
		    IF @RegistrationDate IS NULL 
				BEGIN
					 RAISERROR(N'@تاریخ معتبر نمی باشد',16,1,'500')
				END
		SET NOCOUNT ON;
		SELECT dbo.IsValidRHCode(@Action,@Code,@WarehouseId,@RegistrationDate,@TimeBucketId)
	END
GO
----------------------------------------------------------------------
IF OBJECT_ID ( '[ChangeWarehouseStatus]', 'P' ) IS NOT NULL 
	DROP PROCEDURE [ChangeWarehouseStatus];
GO
CREATE PROCEDURE [ChangeWarehouseStatus]
(
	@Active BIT,
	--@CompanyId INT,
	@WarehouseId INT,
	@UserCreatorId INT
)	
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
DECLARE @TRANSACTIONCOUNT INT;
    SET @TRANSACTIONCOUNT = @@TRANCOUNT;
BEGIN TRY
	   if @TRANSACTIONCOUNT = 0
            BEGIN TRANSACTION
        else
            SAVE TRANSACTION CurrentTransaction;
	   --DECLARE @WarehouseId INT,
	   --        @CompanyId INT
	   -- SET @CompanyId = (
	   --        SELECT TOP(1)c.Id
	   --        FROM   Companies c 
	   --        WHERE c.Name = @Company
	   -- )
	   --SET @WarehouseId = (
	   --        SELECT TOP(1)w.Id
	   --        FROM   Warehouse w
	   --        WHERE  w.Name = @Warehouse
	   --               AND w.CompanyId = @CompanyId
	   --) 
	   
	   DECLARE @ROB DECIMAL(20,3)
	   SET @ROB=(SELECT SUM(ISNULL(ti.QuantityAmount,0) * CASE WHEN t.[Action]=1 THEN (1) WHEN t.[Action]=2 THEN (-1) END)
							   FROM Transactions t
							   INNER JOIN TransactionItems ti ON ti.TransactionId = t.Id
							   INNER JOIN Warehouse a ON a.Id = t.WarehouseId
							   WHERE --a.CompanyId=@CompanyId
							         --AND 
							         t.WarehouseId=@WarehouseId
							         --AND ti.GoodId=CASE WHEN @GoodId <> NULL THEN @GoodId ELSE ti.GoodId END
							         --AND (@TransactionId IS NULL OR t.Id< @TransactionId )
							         --AND (@RowVersion IS NULL OR ti.RowVersion< @TransactionId )
							         --AND ti.QuantityUnitId=CASE WHEN @QuantityUnitId <> NULL THEN @QuantityUnitId ELSE ti.QuantityUnitId END
				)
	   --DECLARE @CHKNegativ TINYINT
	   --SET @CHKNegativ=dbo.CheckNegetiveWarehouse(@WarehouseId,0,0,0)
	   --IF @CHKNegativ=0
	   IF @ROB<>0
	   BEGIN
	   	   DECLARE @MSG NVARCHAR(256)
	   	   SET @MSG=N'@'+N' براي '+CASE WHEN @Active=0 THEN N' غير فعال ' WHEN @Active=1 THEN N' فعال ' END+ N' کردن انبار ، موجودي آن بايد صفر(0) باشد '
		   RAISERROR(@MSG,16,1,'500')
	   END
	   ELSE
	   BEGIN
	   		UPDATE Warehouse
			SET
				[Active] = @Active,
				UserCreatorId = @UserCreatorId,
				CreateDate = getdate()
			WHERE Id=@WarehouseId
			Select CAST('وضعيت انبار تغيير يافت' as nvarchar(100))
	   END
	   IF @TRANSACTIONCOUNT = 0
			COMMIT TRANSACTION 					
	END try
	BEGIN CATCH
	     IF (XACT_STATE()) = -1 
			 ROLLBACK TRANSACTION
		 IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
			 ROLLBACK TRANSACTION
		 IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
             ROLLBACK TRANSACTION CurrentTransaction
		EXEC ErrorHandling
	END catch
END
--[User_ListGetAll]
GO
----------------------------------------------------------------------------------
if OBJECT_ID('TransactionOperation','P') is Not Null
	drop procedure TransactionOperation;
Go
Create Procedure TransactionOperation
(
@Action nvarchar(10),
@Id int = Null,
@TransactionAction tinyint , -- رسيد 1 - حواله 2 -   فاکتور فروش 3
@Description nvarchar(max)=NULL,
@CompanyId INT,--شرکت
@WarehouseId INT,--انبار
@TimeBucketId INT,
@StoreTypesId INT ,--نوع رسيد و حواله(خريد؛برگشت از فروش؛ انتقال و ... )
@PricingReferenceId INT =NULL,
@Status tinyint =null ,--وضعيت رسيد و حواله و ...
@RegistrationDate DATETIME=NULL,--تاريخ ثبت
@ReferenceType NVARCHAR(100)=NULL,--نوع مرجع
@ReferenceNo NVARCHAR(100)=NULL,--شماره مرجع
@UserCreatorId INT,
@TransactionId INT OUT,
@Code decimal(20,2) OUT,--شماره رسید یا حواله یا درخواست خرید,درخواست کالا 
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
	BEGIN
		set nocount ON;
		DECLARE @TRANSACTIONCOUNT INT;
			SET @TRANSACTIONCOUNT = @@TRANCOUNT;
		BEGIN TRY
			IF @TRANSACTIONCOUNT = 0
				BEGIN TRANSACTION
			ELSE
				SAVE TRANSACTION CurrentTransaction;
			
			   	IF @RegistrationDate =NULL OR @RegistrationDate IS NULL 
					BEGIN
							RAISERROR(N'@تاریخ معتبر نمی باشد',16,1,'500')
					END
	--DECLARE @WarehouseId INT,
	--        @CompanyId INT
	--		SET @CompanyId = (
	--				SELECT TOP(1)c.Id
	--				FROM   Companies c 
	--				WHERE c.Name = @Company
	--		)
	--		SET @WarehouseId = (
	--				SELECT TOP(1)w.Id
	--				FROM   Warehouse w
	--				WHERE  w.Name = @Warehouse
	--						AND w.CompanyId = @CompanyId
	--		) 
	IF @TransactionAction<1 OR @TransactionAction>3
		RAISERROR(N'@سند بايد رسيد ، حواله يا فاکنور باشد',16,1,'500')
	DECLARE @tbId INT
	SET @tbId =(SELECT TOP(1)tb.Id
	             FROM TimeBucket tb WHERE tb.[Active]=1 AND @RegistrationDate  BETWEEN tb.StartDate AND tb.EndDate)
	IF @TimeBucketId =0 OR @TimeBucketId IS NULL
	SET @TimeBucketId=@tbId
	IF @TimeBucketId<>@tbId
		 RAISERROR(N'@تاريخ و بازه زماني مطابقت ندارد',16,1,'500')    
	IF NOT EXISTS (SELECT TOP(1) Id FROM Warehouse w WHERE w.[Active]=1 AND w.Id=@WarehouseId)
	     RAISERROR(N'@انبار معتبر (فعال) نيست',16,1,'500')    
	IF NOT EXISTS (SELECT TOP(1) Id FROM Companies c WHERE c.[Active]=1 AND c.Id=@CompanyId)
	     RAISERROR(N'@شرکت معتبر (فعال) نيست',16,1,'500')
	IF NOT EXISTS (SELECT TOP(1) Id FROM TimeBucket tb WHERE tb.[Active]=1 AND tb.Id=@TimeBucketId)
	     RAISERROR(N'@بازه زماني معتبر (فعال) نيست',16,1,'500')
			if lower(@Action)='insert'
				BEGIN
					IF @Code IS NULL OR @Code =0.000
					   SET @Code=dbo.GetTransactionNewCode(@TransactionAction,@WarehouseId,@RegistrationDate,@TimeBucketId)
					INSERT INTO Transactions
					(
						-- Id -- this column value is auto-generated
						[Action],
						Code,
						[Description],
						--CrossId,
						WarehouseId,
						StoreTypesId,
						TimeBucketId,
						PricingReferenceId,
						[Status],
						RegistrationDate,
						SenderReciver,
						HardCopyNo,
						ReferenceType,
						ReferenceNo,
						ReferenceDate,
						UserCreatorId,
						CreateDate
					)
					VALUES
					(
						@TransactionAction,
						@Code,
						@Description,
						--NULL,	/*{ CrossId }*/
						@WarehouseId,
						@StoreTypesId,
						@TimeBucketId,
						@PricingReferenceId,
						1,/*{ [Status] }*/
						@RegistrationDate,
						NULL,/*{ SenderReciver }*/
						NULL,/*{ HardCopyNo }*/
						@ReferenceType,
						@ReferenceNo,
						getdate(),/*{ ReferenceDate }*/
						@UserCreatorId,
						getdate()/*{ CreateDate }*/
					)
					--Select CAST('ثبت با موفقیت انجام شد' as nvarchar(100))
					SET @TransactionId=@@identity
				END;
			--if LOWER(@Action)='update'
			--BEGIN
			--		UPDATE Transaction
			--		SET
			--			-- Id = ? -- this column value is auto-generated
			--			--[Action] = @TransactionAction,
			--			--Code = @Code,
			--			[Description] = @Description,
			--			WarehouseId = @WarehouseId,
			--			StoreTypesId = @StoreTypesId,
			--          TimeBucketId=@TimeBucketId,
			--          PricingReferenceId=@PricingReferenceId,
			--			RegistrationDate = @RegistrationDate,
			--			ReferenceType = @ReferenceType,
			--			ReferenceNo = @ReferenceNo,
			--			ReferenceDate = getdate(),
			--			UserCreatorId = @UserCreatorId
			--		WHERE Id=@Id
			--END			
			--if LOWER(@Action)='delete'
			--BEGIN
			--	DELETE FROM Transaction
			--	WHERE Id=@Id
			--	Select CAST(N'@اطلاعات با موفقیت حذف شد' as nvarchar(100))
			--END
			 
			SET @Message=N'OperationSuccessful'
		IF @TRANSACTIONCOUNT = 0
		COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
				SET @Message=ERROR_MESSAGE();
				SET @TransactionId=NULL
				SET @Code=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('TransactionItemsOperation','P') is Not Null
	drop procedure TransactionItemsOperation;
Go
Create Procedure TransactionItemsOperation
(
@Action nvarchar(10),
@TransactionId  int,
@UserCreatorId INT,
@TransactionItems TypeTransactionItems READONLY,-- Id , GoodId, QuantityUnitId, QuantityAmount, [Description]
@TransactionItemsId NVARCHAR(256) OUT,
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
	BEGIN
	set nocount ON;
	DECLARE @TRANSACTIONCOUNT INT;
		SET @TRANSACTIONCOUNT = @@TRANCOUNT;
	BEGIN TRY
	IF @TRANSACTIONCOUNT = 0
		BEGIN TRANSACTION
	ELSE
		SAVE TRANSACTION CurrentTransaction;
		SET @TransactionItemsId=N''
	    DECLARE @Id int =Null,
				@GoodId int = null,
				@QuantityUnitId INT,
				@QuantityAmount DECIMAL(20,3) = 0,
				@Description nvarchar(max) =NULL,
				@RowVersion SMALLINT = 0,
				@WarehouseId INT,
				@CompanyId INT,
				@TimeBucketId INT,
				@MSG NVARCHAR(max)
				
		SELECT TOP(1) @WarehouseId=isnull(t.WarehouseId,0),@CompanyId=ISNULL(w.CompanyId,0),@TimeBucketId=ISNULL(t.TimeBucketId,0)
	    FROM Transactions t
	    INNER JOIN Warehouse w ON w.Id=t.WarehouseId
	    WHERE t.Id=@TransactionId

		IF NOT EXISTS (SELECT TOP(1) Id FROM Warehouse w WHERE w.[Active]=1 AND w.Id=@WarehouseId)
			   	BEGIN
			   		SET @MSG=N'@ انبار ' + (SELECT TOP(1) w.Code+N' '+w.Name FROM Warehouse w WHERE w.Id=@WarehouseId) +N' معتبر (فعال) نيست'
			   		RAISERROR(@MSG,16,1,'500')    
			   	END
		IF NOT EXISTS (SELECT TOP(1) Id FROM Companies c WHERE c.[Active]=1 AND c.Id=@CompanyId)
		BEGIN
			SET @MSG=N'@ شرکت ' + (SELECT TOP(1) c.Code+N' '+c.Name FROM Companies c WHERE c.Id=@CompanyId) +N' معتبر (فعال) نيست'
			RAISERROR(@MSG,16,1,'500')    
		END
		IF NOT EXISTS (SELECT TOP(1) Id FROM TimeBucket tb WHERE tb.[Active]=1 AND tb.Id=@TimeBucketId)
		BEGIN
			SET @MSG=N'@ بازه زماني ' + (SELECT TOP(1)tb.Name FROM TimeBucket tb WHERE tb.Id=@TimeBucketId) +N' معتبر (فعال) نيست'
			RAISERROR(@MSG,16,1,'500')    
		END	 
	    IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'TransactionItems')
		DEALLOCATE TransactionItems
		DECLARE TransactionItems CURSOR FOR
		SELECT  t.Id , t.GoodId, t.QuantityUnitId, t.QuantityAmount, t.[Description]
		FROM @TransactionItems t
		OPEN TransactionItems
			FETCH NEXT FROM TransactionItems INTO  @Id , @GoodId, @QuantityUnitId, @QuantityAmount, @Description
			WHILE @@Fetch_Status = 0  
			BEGIN
				
			   	IF NOT EXISTS (SELECT TOP(1) Id FROM Goods g WHERE g.[Active]=1 AND g.Id=@GoodId)
			   	BEGIN
			   		SET @MSG=N'@ کالاي ' + (SELECT TOP(1) g.Code+N' '+g.Name FROM Goods g WHERE g.Id=@GoodId) +N' معتبر (فعال) نيست'
			   		RAISERROR(@MSG,16,1,'500')    
			   	END
				IF NOT EXISTS (SELECT TOP(1) Id FROM Units u WHERE u.[Active]=1 AND u.IsCurrency=0 AND u.Id=@QuantityUnitId)
			   	BEGIN
			   		SET @MSG=N'@ واحد ' + (SELECT TOP(1) u.Code+N' '+u.Name FROM Units u WHERE u.Id=@QuantityUnitId) +N' معتبر (فعال) نيست'
			   		RAISERROR(@MSG,16,1,'500')    
			   	END
	     
			 --  	declare @CHKNegativ TINYINT
				--SET @CHKNegativ=dbo.CheckNegetiveWarehouseValue(@WarehouseId,@GoodId ,@RowVersion,@Action,@TransactionId,@QuantityAmount,@QuantityUnitId,1)
				--IF @CHKNegativ=0
				--BEGIN
				--DECLARE @m NVARCHAR(MAX)
				--SET @m=N'@انبار '+(SELECT top(1) w.Name
				--				FROM Warehouse w WHERE w.Id=@WarehouseId)+N' منفی پذیر نیست و نمیتواند منفی شود '
				--RAISERROR(@m,16,1,'500')
				--END		

			if lower(@Action)='insert'
				BEGIN
					SET @RowVersion=ISNULL((SELECT MAX(ti.[RowVersion]) FROM TransactionItems ti WHERE ti.TransactionId=@TransactionId),0)+1		
					INSERT INTO TransactionItems
					(
						-- Id -- this column value is auto-generated
						RowVersion,
						TransactionId,
						GoodId,
						QuantityUnitId,
						QuantityAmount,
						[Description],
						UserCreatorId,
						CreateDate
					)
					VALUES
					(
						@RowVersion ,
						@TransactionId ,
						@GoodId ,
						@QuantityUnitId ,
						@QuantityAmount ,
						@Description,
						@UserCreatorId ,
						getdate()
					)
					--Select CAST('ثبت با موفقیت انجام شد' as nvarchar(100))
					SET @TransactionItemsId+=CAST(@@identity AS NVARCHAR(15))+N';'
				END;
				--if LOWER(@Action)='update'
				--BEGIN
				--		UPDATE TransactionItems
				--		SET
				--			-- Id = ? -- this column value is auto-generated
				--			GoodId = @GoodId,
				--			QuantityUnitId = @QuantityUnitId,
				--			QuantityAmount = @QuantityAmount,
				--			[Description] = @Description,
				--			UserCreatorId = @UserCreatorId
				--		WHERE Id=@Id
				--END			
				--if LOWER(@Action)='delete'
				--BEGIN
				--	DELETE FROM TransactionItems
				--	WHERE Id=@Id
				--	Select CAST(N'@اطلاعات با موفقیت حذف شد' as nvarchar(100))
				--END
		    FETCH TransactionItems INTO  @Id , @GoodId, @QuantityUnitId, @QuantityAmount, @Description
			END
		
		SET @Message=N'OperationSuccessful'
			IF @TRANSACTIONCOUNT = 0
				COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
			    SET @Message=ERROR_MESSAGE();
				SET @TransactionItemsId=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('TransactionItemPricesOperation','P') is Not Null
	drop procedure TransactionItemPricesOperation;
Go
Create Procedure TransactionItemPricesOperation
(
@Action nvarchar(10),
@UserCreatorId INT,
@TransactionItemPrices TypeTransactionItemPrices READONLY,-- Id ,TransactionItemId, QuantityUnitId ,QuantityAmount ,PriceUnitId ,Fee  ,[Description]
@TransactionItemPriceIds NVARCHAR(MAX) OUT,
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
	BEGIN
	set nocount ON;
	DECLARE @TRANSACTIONCOUNT INT;
		SET @TRANSACTIONCOUNT = @@TRANCOUNT;
	BEGIN TRY
		IF @TRANSACTIONCOUNT = 0
			BEGIN TRANSACTION
		ELSE
			SAVE TRANSACTION CurrentTransaction;
		SET @TransactionItemPriceIds=N''
	    DECLARE @Id int =Null,
				@TransactionId INT,
				@TransactionItemId INT = NULL,
				@PriceUnitId INT = NULL,
				@Fee DECIMAL(20,3) = 0,
				@RegistrationDate DATETIME = getdate(),
				@QuantityUnitId INT,
				@QuantityAmount DECIMAL(20,3) = 0,
				@Description nvarchar(max) =NULL,
				@RowVersion SMALLINT = 0,
				@WarehouseId INT,
				@QuantityAmountTotal DECIMAL(20,3) = 0,
				@QuantityAmountSumSoFar DECIMAL(20,3) = 0,
				@ActionType TINYINT=0,
				@StoreTypeCode SMALLINT = 0,
				@MainCurrencyUnitId INT =NULL,
				@FeeInMainCurrency DECIMAL(20,3) =0,
				@PrimaryCoefficient TINYINT=2,
				@Coefficient DECIMAL(18,3) =0
						                                    					
	    IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'TransactionItemPrices')
		DEALLOCATE TransactionItemPrices
		DECLARE TransactionItemPrices CURSOR FOR
		SELECT  t.Id , t.TransactionItemId, t.QuantityUnitId, t.QuantityAmount, t.PriceUnitId, t.Fee, t.RegistrationDate, t.[Description]
		FROM @TransactionItemPrices t
		OPEN TransactionItemPrices
			FETCH NEXT FROM TransactionItemPrices INTO  @Id , @TransactionItemId ,@QuantityUnitId ,@QuantityAmount ,@PriceUnitId , @Fee, @RegistrationDate, @Description
			WHILE @@Fetch_Status = 0  
			BEGIN
			   	 SELECT TOP(1) @WarehouseId = ISNULL(t.WarehouseId, 0),
			   	        @ActionType = ISNULL(t.Action, 0),
			   	        @StoreTypeCode= ISNULL(st.Code, 0),
			   	        @TransactionId=t.Id
			   	 FROM   Transactions t
			   	        INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
			   	        INNER JOIN TransactionItems ti
			   	             ON  t.Id = ti.TransactionId
			   	 WHERE  ti.Id = @TransactionItemId
			   	 
			   	 IF NOT(@ActionType=1 OR(@ActionType=3 AND @StoreTypeCode=9))--FuelReport Incremental Correction
			   	 BEGIN
				   SET @Message='Failed' 
				   RAISERROR(N'@فقط عمليات رسيد بصورت دستي قابل قيمت گذاري مي باشد ',16,1,'500')			   	 	
			   	 END
		   
				 SET @QuantityAmountTotal=ISNULL((SELECT sum(ti.QuantityAmount)
				                                    FROM TransactionItems ti WHERE ti.Id=@TransactionItemId
				                                                               AND ti.QuantityUnitId=@QuantityUnitId
				                                                               AND ti.TransactionId=@TransactionId),0)
                 SET @MainCurrencyUnitId=ISNULL((SELECT TOP(1) Id FROM Units u WHERE u.IsBaseCurrency=1),0)
                 if @MainCurrencyUnitId=0 
                 	RAISERROR(N'@براي انجام عمليات قيمت گذاري ابتدا بايد ارز پايه تعيين گردد ',16,1,'500')		
                 
                 IF @PriceUnitId<>@MainCurrencyUnitId
						BEGIN
							SELECT @Coefficient=fn.Coefficient ,@PrimaryCoefficient=fn.PrimaryCoefficient
								FROM dbo.[PrimaryCoefficient](@PriceUnitId,@MainCurrencyUnitId,@RegistrationDate) fn	
						END		                      
						ELSE 
						BEGIN
							SET @Coefficient=1
							SET @PrimaryCoefficient=1
						END
		
						IF @Coefficient<>0
						BEGIN
							IF @PrimaryCoefficient=1  
							BEGIN
						   			SET @Coefficient=(1  *  @Coefficient)
							END
							ELSE IF @PrimaryCoefficient=0 
							BEGIN
						   			SET @Coefficient=(1  /  @Coefficient)
							END    
						END
						ELSE
						BEGIN
							RAISERROR(N'@Base Currency Not Declare Relation With Selected Currency ', 16, 1, '500')
						END
			SET @FeeInMainCurrency=@Fee * @Coefficient			
						                                                               
			SET @QuantityAmountSumSoFar=0				                                                               
			if lower(@Action)='insert'
				BEGIN
					SET @QuantityAmountSumSoFar=ISNULL((SELECT sum(tip.QuantityAmount)
					                                    FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@TransactionItemId
																						 AND tip.QuantityUnitId=@QuantityUnitId
																						 AND tip.TransactionId=@TransactionId),0)
					IF (@QuantityAmountTotal-@QuantityAmountSumSoFar)<@QuantityAmount
					     RAISERROR(N'@بيشتر از مقدار قيمت گذاري نشده را براي قيمت گذاري ارسال کرده ايد ',16,1,'500')																						 
					SET @RowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@TransactionItemId),0)+1	
					INSERT INTO TransactionItemPrices
					(
						-- Id -- this column value is auto-generated
						RowVersion,
						TransactionId,
						TransactionItemId,
						QuantityUnitId,
						QuantityAmount,
						PriceUnitId,
						Fee,
						MainCurrencyUnitId,
						FeeInMainCurrency,
						RegistrationDate,
						[Description],
						UserCreatorId,
						CreateDate
					)
					VALUES
					(
						@RowVersion ,
						@TransactionId ,
						@TransactionItemId ,
						@QuantityUnitId,
						@QuantityAmount ,
						@PriceUnitId ,
						@Fee ,
						@MainCurrencyUnitId,
						@FeeInMainCurrency,
						@RegistrationDate,
						@Description ,
						@UserCreatorId ,
						getdate()
					)	
					--Select CAST('ثبت با موفقیت انجام شد' as nvarchar(100))
					SET @TransactionItemPriceIds+=CAST(@@identity AS NVARCHAR(15))+N';'
				END;
				--if LOWER(@Action)='update'
				--BEGIN
				--		SET @QuantityAmountSumSoFar=ISNULL((SELECT sum(tip.QuantityAmount)
				--	                                    FROM TransactionItemPrices tip WHERE tip.Id<>@Id
				--	                                                                     AND tip.TransactionItemId=@TransactionItemId
				--																		 AND tip.QuantityUnitId=@QuantityUnitId
				--																		 AND tip.TransactionId=@TransactionId),0)
				--      IF (@QuantityAmountTotal-@QuantityAmountSumSoFar)<=@QuantityAmount
				--	     RAISERROR(N'@بيشتر از مقدار قيمت گذاري نشده را براي قيمت گذاري ارسال کرده ايد ',16,1,'500')			
				--		UPDATE TransactionItemPrices
				--		SET
				--			-- Id = ? -- this column value is auto-generated
				--			TransactionId=@TransactionId,
				--			TransactionItemId=@TransactionItemId,
				--			QuantityUnitId=@QuantityUnitId,
				--			QuantityAmount=@QuantityAmount,
				--			PriceUnitId=@PriceUnitId,
				--			Fee=@Fee,
				--			MainCurrencyUnitId=@MainCurrencyUnitId,
				--			FeeInMainCurrency=@FeeInMainCurrency,
				--          RegistrationDate=@RegistrationDate,
				--			[Description]=@Description,
				--		WHERE Id=@Id
				--END			
				--if LOWER(@Action)='delete'
				--BEGIN
				--	DELETE FROM TransactionItemPrices
				--	WHERE Id=@Id
				--	Select CAST(N'@اطلاعات با موفقیت حذف شد' as nvarchar(100))
				--END
		    FETCH TransactionItemPrices INTO  @Id , @TransactionItemId ,@QuantityUnitId ,@QuantityAmount ,@PriceUnitId , @Fee, @RegistrationDate, @Description
			END
		
		SET @Message=N'OperationSuccessful'
			IF @TRANSACTIONCOUNT = 0
				COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
		        SET @Message=ERROR_MESSAGE();
				SET @TransactionItemPriceIds=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('PriceGivenIssuedItems','P') is Not Null
	drop procedure PriceGivenIssuedItems;
Go
Create Procedure PriceGivenIssuedItems
(
@UserCreatorId INT,
@IssueItemIds Ids READONLY,-- Id 
@TransactionItemPriceIds NVARCHAR(MAX) OUT,
@Message NVARCHAR(MAX) OUT,
@NotPricedTransactionId INT OUT --First Receipt Number Without Pricing 
)
--WITH ENCRYPTION
AS 
BEGIN
set nocount ON;
DECLARE @TRANSACTIONCOUNT INT;
	SET @TRANSACTIONCOUNT = @@TRANCOUNT;
BEGIN TRY
	IF @TRANSACTIONCOUNT = 0
		BEGIN TRANSACTION
	ELSE
		SAVE TRANSACTION CurrentTransaction;
	DECLARE @IssueItemId INT,
	        @Description NVARCHAR(MAX),
		    @QuantityAmount DECIMAL(20,3)	
		SET @TransactionItemPriceIds=N''
		
		
	IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'IssueItemPrices')
		DEALLOCATE IssueItemPrices
	DECLARE IssueItemPrices CURSOR FOR
	SELECT  t.Id,t.Description
	FROM @IssueItemIds t
	OPEN IssueItemPrices
		FETCH NEXT FROM IssueItemPrices INTO  @IssueItemId, @Description
		WHILE @@Fetch_Status = 0  
		BEGIN
			DECLARE @WarehouseId INT,
					@GoodId int ,
					@QuantityUnitId INT ,
					
					@TimeBucketId INT,
					@RegistrationDate DATETIME,
					@IssueId INT=NULL,
					@IssueItemRowVersion SMALLINT=NULL,
		
					@IssueItemPriceId INT=NULL,
					@IssueItemPriceRowVersion SMALLINT=0,
					@CurrentQuantityAmount DECIMAL(20,3),
					@ActionType SMALLINT=NULL	
				
				SELECT TOP(1) @WarehouseId = ISNULL(t.WarehouseId, 0),
			   			@ActionType = ISNULL(t.Action, 0),
			   			@GoodId=ti.GoodId,
			   			@QuantityUnitId=ti.QuantityUnitId,
			   			@QuantityAmount=ti.QuantityAmount,
			   			@TimeBucketId=t.TimeBucketId,
			   			@RegistrationDate=t.RegistrationDate,
			   			@IssueItemRowVersion=ti.RowVersion,
			   			@IssueId=t.Id
			   		FROM   Transactions t
			   			INNER JOIN TransactionItems ti
			   					ON  t.Id = ti.TransactionId
			   		WHERE  ti.Id = @IssueItemId
		IF @ActionType<>2
		BEGIN
				SET @Message='Failed' 
				RAISERROR(N'@فقط عمليات حواله بصورت سيستمي قابل قيمت گذاري مي باشد ',16,1,'500')	
		END
		 
		IF OBJECT_ID('tempdb..##TempIssuePriceing') IS NOT NULL
					EXEC('DROP TABLE ##TempIssuePriceing')
			
		SELECT TOP(1) @IssueItemPriceId=ISNULL(MAX(tip.Id),0)+1  FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId
		IF @IssueItemPriceId=1 SET @IssueItemPriceRowVersion=1
		ELSE  SET @IssueItemPriceRowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	

		SELECT t.Id AS TransactionId, t.RegistrationDate AS TransactionRegistrationDate,ti.Id AS TransactionItemId, ti.RowVersion AS TransactionItemRowVersion, 
				ti.QuantityAmount AS TransactionItemQuantityAmount,tip.Id AS TransactionItemPriceId, tip.RowVersion AS TransactionItemPriceRowVersion, 
				tip.RegistrationDate AS TransactionItemPriceRegistrationDate, tip.QuantityAmount AS TransactionItemPriceQuantityAmount, 
				tip.PriceUnitId AS TransactionItemPricePriceUnitId, tip.Fee AS TransactionItemPriceFee, tip.MainCurrencyUnitId AS TransactionItemPriceMainCurrencyUnitId, 
				tip.FeeInMainCurrency AS TransactionItemPriceExchangeRate, tip.QuantityAmountUseFIFO AS TransactionItemPriceQuantityAmountUseFIFO
		INTO ##TempIssuePriceing
		FROM Transactions t
			INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
			INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
			INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
		WHERE a.Id=@WarehouseId
				AND ti.GoodId=@GoodId
				AND ti.QuantityUnitId=@QuantityUnitId
				AND t.TimeBucketId=@TimeBucketId
				AND t.[Action]=1
				AND tip.QuantityAmountUseFIFO<tip.QuantityAmount
				AND tip.Fee>0
				AND tip.FeeInMainCurrency>0
				AND ( CONVERT(NVARCHAR(20),t.RegistrationDate,(120))
					--+ dbo.LPAD(CAST(t.Action AS nvarCHAR(3)) ,' ',3)
					+ dbo.LPAD(CAST(t.Id AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(ti.Id AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(ti.RowVersion AS NVARCHAR(10)),' ',10) 
					+ CONVERT(NVARCHAR(20),tip.RegistrationDate,(120))
					+ dbo.LPAD(CAST(tip.Id AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(tip.RowVersion AS NVARCHAR(15)),' ',15))<=
         
					( CONVERT(NVARCHAR(20),@RegistrationDate,(120))
					--+ dbo.LPAD(CAST(t.Action AS NVARCHAR(3)),' ',3)
					+ dbo.LPAD(CAST(@IssueId AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(@IssueItemId AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(@IssueItemRowVersion AS NVARCHAR(10)),' ',10) 
					+ CONVERT(NVARCHAR(20),GETDATE(),(120))
					+ dbo.LPAD(CAST(@IssueItemPriceId AS NVARCHAR(15)),' ',15)
					+ dbo.LPAD(CAST(@IssueItemPriceRowVersion AS NVARCHAR(15)),' ',15)
					)
			ORDER BY 
				t.RegistrationDate,
				t.Action ,
				t.Id ,
				ti.Id ,
				ti.RowVersion ,
				tip.RegistrationDate,
				tip.Id ,
				tip.RowVersion 
		--SELECT * FROM ##TempIssuePriceing
		DECLARE @RegistrationDateFirstFree DATETIME,
				@TransactionIdFirstFree INT=NULL,
				@TransactionItemIdFirstFree INT=NULL,
				@TransactionItemRowVersionFirstFree SMALLINT=NULL,
				@TransactionItemPriceRegistrationDateFirstFree DATETIME,
				@TransactionItemPriceIdFirstFree INT=NULL,
				@TransactionItemPriceRowVersionFirstFree SMALLINT=0 ,
				@TransactionId INT, 
				@TransactionRegistrationDate DATETIME,
				@TransactionItemId INT,
				@TransactionItemRowVersion SMALLINT, 
				@TransactionItemQuantityAmount DECIMAL(20,3),
				@TransactionItemPriceId INT,
				@TransactionItemPriceRowVersion SMALLINT, 
				@TransactionItemPriceRegistrationDate DATETIME,
				@TransactionItemPriceQuantityAmount DECIMAL(20,3),
				@TransactionItemPricePriceUnitId INT,
				@TransactionItemPriceFee DECIMAL(20,3),
				@TransactionItemPriceMainCurrencyUnitId INT, 
				@TransactionItemPriceExchangeRate DECIMAL(20,3),
				@TransactionItemPriceQuantityAmountUseFIFO DECIMAL(20,3);

		SELECT TOP(1) @RegistrationDateFirstFree=tip.TransactionRegistrationDate, @TransactionIdFirstFree=TransactionId, 
						@TransactionItemIdFirstFree=TransactionItemId, @TransactionItemRowVersionFirstFree=TransactionItemRowVersion, 
						@TransactionItemPriceRegistrationDateFirstFree=TransactionItemPriceRegistrationDate, 
						@TransactionItemPriceIdFirstFree=TransactionItemPriceId, 
						@TransactionItemPriceRowVersionFirstFree=TransactionItemPriceRowVersion
		FROM ##TempIssuePriceing tip				 
		SET @NotPricedTransactionId= ISNULL((
						SELECT TOP(1)t.Id
						FROM Transactions t
						INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
						INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
						INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
						WHERE a.Id=@WarehouseId
								AND ti.GoodId=@GoodId
								AND ti.QuantityUnitId=@QuantityUnitId
								AND t.TimeBucketId=@TimeBucketId
								AND t.[Action]=1
								AND tip.QuantityAmountUseFIFO<tip.QuantityAmount
								AND (tip.Fee=0 OR tip.FeeInMainCurrency=0)
								AND ( CONVERT(NVARCHAR(20),t.RegistrationDate,(120))
								+ dbo.LPAD(CAST(t.Id AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(ti.Id AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(ti.RowVersion AS NVARCHAR(10)),' ',10) 
								+ CONVERT(NVARCHAR(20),tip.RegistrationDate,(120))
								+ dbo.LPAD(CAST(tip.Id AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(tip.RowVersion AS NVARCHAR(15)),' ',15))<=
         
								( CONVERT(NVARCHAR(20),@RegistrationDateFirstFree,(120))
								+ dbo.LPAD(CAST(@TransactionIdFirstFree AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(@TransactionItemIdFirstFree AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(@TransactionItemRowVersionFirstFree AS NVARCHAR(10)),' ',10) 
								+ CONVERT(NVARCHAR(20),@TransactionItemPriceRegistrationDateFirstFree,(120))
								+ dbo.LPAD(CAST(@TransactionItemPriceIdFirstFree AS NVARCHAR(15)),' ',15)
								+ dbo.LPAD(CAST(@TransactionItemPriceRowVersionFirstFree AS NVARCHAR(15)),' ',15)
								)
						)  ,0)
			IF @NotPricedTransactionId<>0
			BEGIN
				RAISERROR(N'@رسيدي که لازم است در قيمت گذاري از آن استفاده شود؛ هنوز قيمت نخورده است',16,1,'500')
				IF OBJECT_ID('tempdb..##TempIssuePriceing') IS NOT NULL
						EXEC('DROP TABLE ##TempIssuePriceing')
				RETURN
			END       
			ELSE
			BEGIN
			
				IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'AssignIssuePrice')
					DEALLOCATE AssignIssuePrice
				DECLARE AssignIssuePrice CURSOR FOR
				SELECT  t.TransactionId, 
						t.TransactionRegistrationDate,
						t.TransactionItemId,
						t.TransactionItemRowVersion, 
						t.TransactionItemQuantityAmount,
						t.TransactionItemPriceId,
						t.TransactionItemPriceRowVersion, 
						t.TransactionItemPriceRegistrationDate,
						t.TransactionItemPriceQuantityAmount, 
						t.TransactionItemPricePriceUnitId,
						t.TransactionItemPriceFee,
						t.TransactionItemPriceMainCurrencyUnitId, 
						t.TransactionItemPriceExchangeRate,
						t.TransactionItemPriceQuantityAmountUseFIFO
				FROM ##TempIssuePriceing t
				OPEN AssignIssuePrice
					FETCH NEXT FROM AssignIssuePrice INTO   @TransactionId, 
															@TransactionRegistrationDate,
															@TransactionItemId,
															@TransactionItemRowVersion, 
															@TransactionItemQuantityAmount,
															@TransactionItemPriceId,
															@TransactionItemPriceRowVersion, 
															@TransactionItemPriceRegistrationDate,
															@TransactionItemPriceQuantityAmount, 
															@TransactionItemPricePriceUnitId,
															@TransactionItemPriceFee,
															@TransactionItemPriceMainCurrencyUnitId, 
															@TransactionItemPriceExchangeRate,
															@TransactionItemPriceQuantityAmountUseFIFO
				WHILE @@Fetch_Status = 0  
				BEGIN
					SET @CurrentQuantityAmount= @TransactionItemPriceQuantityAmount-@TransactionItemPriceQuantityAmountUseFIFO;
					IF @CurrentQuantityAmount>(@QuantityAmount)
					BEGIN
						SET @CurrentQuantityAmount=@QuantityAmount
					END
					DECLARE @RowVersion SMALLINT
					SET @RowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	
					INSERT INTO TransactionItemPrices
					(
						-- Id -- this column value is auto-generated
						RowVersion,
						TransactionId,
						TransactionItemId,
						[Description],
						QuantityUnitId,
						QuantityAmount,
						PriceUnitId,
						Fee,
						MainCurrencyUnitId,
						FeeInMainCurrency,
						RegistrationDate,
						--QuantityAmountUseFIFO,
						TransactionReferenceId,
						UserCreatorId,
						CreateDate
					)
					VALUES
					(
						@RowVersion ,
						@IssueId ,
						@IssueItemId ,
						@Description,
						@QuantityUnitId ,
						@CurrentQuantityAmount,
						@TransactionItemPricePriceUnitId,
						@TransactionItemPriceFee,
						@TransactionItemPriceMainCurrencyUnitId,
						@TransactionItemPriceExchangeRate,
						@RegistrationDate,
						/*{ QuantityAmountUseFIFO }*/
						@TransactionItemPriceId,
						@UserCreatorId ,
						getdate()
					)
					
					SET @TransactionItemPriceIds+=cast(@@identity AS NVARCHAR(15))+N';'
						
					UPDATE TransactionItemPrices
					SET
						QuantityAmountUseFIFO += @CurrentQuantityAmount,
						IssueReferenceIds = isnull (IssueReferenceIds,N'')+cast(@@identity AS NVARCHAR(15))+N';'
					WHERE Id=@TransactionItemPriceId
									
					SET @QuantityAmount-=@CurrentQuantityAmount
					IF @QuantityAmount=0
						BREAK;
									
         			FETCH AssignIssuePrice INTO @TransactionId, 
												@TransactionRegistrationDate,
												@TransactionItemId,
												@TransactionItemRowVersion, 
												@TransactionItemQuantityAmount,
												@TransactionItemPriceId,
												@TransactionItemPriceRowVersion, 
												@TransactionItemPriceRegistrationDate,
												@TransactionItemPriceQuantityAmount, 
												@TransactionItemPricePriceUnitId,
												@TransactionItemPriceFee,
												@TransactionItemPriceMainCurrencyUnitId, 
												@TransactionItemPriceExchangeRate,
												@TransactionItemPriceQuantityAmountUseFIFO
				END
			END
			IF @QuantityAmount<>0
				RAISERROR(N'@موجودي رسيدهاي با قيمت براي قيمت گزاري حواله کافي نيست',16,1,'500')
						
			FETCH IssueItemPrices INTO  @IssueItemId , @Description
		END
			SET @Message=N'OperationSuccessful'
		IF @TRANSACTIONCOUNT = 0
				COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
				SET @Message=ERROR_MESSAGE();
				SET @NotPricedTransactionId=NULL
				SET @TransactionItemPriceIds=NULL			    
			EXEC ErrorHandling
		END CATCH
END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('PriceAllSuspendedIssuedItems','P') is Not Null
	drop procedure PriceAllSuspendedIssuedItems;
Go
Create Procedure PriceAllSuspendedIssuedItems
(
@WarehouseId INT,
@UserCreatorId INT,
@TransactionItemPriceIds NVARCHAR(MAX) OUT,
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
BEGIN
set nocount ON;
DECLARE @TRANSACTIONCOUNT INT;
	SET @TRANSACTIONCOUNT = @@TRANCOUNT;
BEGIN TRY
	IF @TRANSACTIONCOUNT = 0
		BEGIN TRANSACTION
	ELSE
		SAVE TRANSACTION CurrentTransaction;
	DECLARE @IssueItemId INT,
			@GoodId int ,
			@QuantityUnitId INT ,
			@QuantityAmount DECIMAL(20,3),
			@TimeBucketId INT,
			@RegistrationDate DATETIME,
			@IssueId INT=NULL,
			@IssueItemRowVersion SMALLINT=NULL,
			@ActionType SMALLINT=NULL,
			@NotPricedTransactionId INT
			
			
		SET @TransactionItemPriceIds=N''
	IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'CursorIssueItemPricesAutomatically')
	DEALLOCATE CursorIssueItemPricesAutomatically
	DECLARE CursorIssueItemPricesAutomatically CURSOR FOR
	SELECT  
		ISNULL(t.Action, 0) AS ActionType,
		ti.GoodId AS GoodId,
		ti.QuantityUnitId AS QuantityUnitId,
		ti.QuantityAmount AS QuantityAmount,
		t.TimeBucketId AS TimeBucketId,
		t.RegistrationDate AS RegistrationDate,
		ti.RowVersion AS IssueItemRowVersion,
		ti.Id AS IssueItemId,
		t.Id AS IssueId
	FROM   Transactions t
		INNER JOIN TransactionItems ti
			   	ON  t.Id = ti.TransactionId
	WHERE  t.WarehouseId=@WarehouseId 
	       AND t.[Action]=2
	       AND t.[Status]<3
	ORDER BY t.RegistrationDate,
			 t.Action ,
			 t.Id ,
			 ti.Id ,
			 ti.RowVersion 
	OPEN CursorIssueItemPricesAutomatically
		FETCH NEXT FROM CursorIssueItemPricesAutomatically INTO  @ActionType,
																 @GoodId,
																 @QuantityUnitId,
																 @QuantityAmount,
															     @TimeBucketId,
															     @RegistrationDate,
															     @IssueItemRowVersion,
															     @IssueItemId,
															     @IssueId
		WHILE @@Fetch_Status = 0  
		BEGIN
			DECLARE @IssueItemPriceId INT=NULL,
					@IssueItemPriceRowVersion SMALLINT=0,
					@CurrentQuantityAmount DECIMAL(20,3)
		--IF @ActionType<>2
		--BEGIN
		--	   SET @Message='Failed' 
		--	   RAISERROR(N'@فقط عمليات حواله بصورت سيستمي قابل قيمت گذاري مي باشد ',16,1,'500')	
		--END
		 
		 
		 SET @QuantityAmount=@QuantityAmount- ISNULL((SELECT SUM(ISNULL(tip.QuantityAmount,0)) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)
		 IF @QuantityAmount>0
		 BEGIN
			IF OBJECT_ID('tempdb..##TempIssuePriced') IS NOT NULL
					   EXEC('DROP TABLE ##TempIssuePriced')
			
					SELECT TOP(1) @IssueItemPriceId=ISNULL(MAX(tip.Id),0)+1  FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId
					IF @IssueItemPriceId=1 SET @IssueItemPriceRowVersion=1
					ELSE  SET @IssueItemPriceRowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	

			SELECT t.Id AS TransactionId, t.RegistrationDate AS TransactionRegistrationDate,ti.Id AS TransactionItemId, ti.RowVersion AS TransactionItemRowVersion, 
				   ti.QuantityAmount AS TransactionItemQuantityAmount,tip.Id AS TransactionItemPriceId, tip.RowVersion AS TransactionItemPriceRowVersion, 
				   tip.RegistrationDate AS TransactionItemPriceRegistrationDate, tip.QuantityAmount AS TransactionItemPriceQuantityAmount, 
				   tip.PriceUnitId AS TransactionItemPricePriceUnitId, tip.Fee AS TransactionItemPriceFee, tip.MainCurrencyUnitId AS TransactionItemPriceMainCurrencyUnitId, 
				   tip.FeeInMainCurrency AS TransactionItemPriceExchangeRate, tip.QuantityAmountUseFIFO AS TransactionItemPriceQuantityAmountUseFIFO
			INTO ##TempIssuePriced
			FROM Transactions t
			INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
			INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
			INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
			WHERE a.Id=@WarehouseId
				  AND ti.GoodId=@GoodId
				  AND ti.QuantityUnitId=@QuantityUnitId
				  AND t.TimeBucketId=@TimeBucketId
				  AND t.[Action]=1   --case when @ActionType=1 then 2 else 1 end
				  AND tip.QuantityAmountUseFIFO<tip.QuantityAmount
				  AND tip.Fee>0
				  AND tip.FeeInMainCurrency>0
				  AND ( CONVERT(NVARCHAR(20),t.RegistrationDate,(120))
					 --+ dbo.LPAD(CAST(t.Action AS nvarCHAR(3)) ,' ',3)
					 + dbo.LPAD(CAST(t.Id AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(ti.Id AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(ti.RowVersion AS NVARCHAR(10)),' ',10) 
					 + CONVERT(NVARCHAR(20),tip.RegistrationDate,(120))
					 + dbo.LPAD(CAST(tip.Id AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(tip.RowVersion AS NVARCHAR(15)),' ',15))<=
         
					 ( CONVERT(NVARCHAR(20),@RegistrationDate,(120))
					 --+ dbo.LPAD(CAST(t.Action AS NVARCHAR(3)),' ',3)
					 + dbo.LPAD(CAST(@IssueId AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(@IssueItemId AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(@IssueItemRowVersion AS NVARCHAR(10)),' ',10) 
					 + CONVERT(NVARCHAR(20),GETDATE(),(120))
					 + dbo.LPAD(CAST(@IssueItemPriceId AS NVARCHAR(15)),' ',15)
					 + dbo.LPAD(CAST(@IssueItemPriceRowVersion AS NVARCHAR(15)),' ',15)
					 )
				ORDER BY 
					t.RegistrationDate,
					t.Action ,
					t.Id ,
					ti.Id ,
					ti.RowVersion ,
					tip.RegistrationDate,
					tip.Id ,
					tip.RowVersion 
       
					DECLARE @RegistrationDateFirstFree DATETIME,
							@TransactionIdFirstFree INT=NULL,
							@TransactionItemIdFirstFree INT=NULL,
							@TransactionItemRowVersionFirstFree SMALLINT=NULL,
							@TransactionItemPriceRegistrationDateFirstFree DATETIME,
							@TransactionItemPriceIdFirstFree INT=NULL,
							@TransactionItemPriceRowVersionFirstFree SMALLINT=0 ,
							@TransactionId INT, 
							@TransactionRegistrationDate DATETIME,
							@TransactionItemId INT,
							@TransactionItemRowVersion SMALLINT, 
							@TransactionItemQuantityAmount DECIMAL(20,3),
							@TransactionItemPriceId INT,
							@TransactionItemPriceRowVersion SMALLINT, 
							@TransactionItemPriceRegistrationDate DATETIME,
							@TransactionItemPriceQuantityAmount DECIMAL(20,3),
							@TransactionItemPricePriceUnitId INT,
							@TransactionItemPriceFee DECIMAL(20,3),
							@TransactionItemPriceMainCurrencyUnitId INT, 
							@TransactionItemPriceExchangeRate DECIMAL(20,3),
							@TransactionItemPriceQuantityAmountUseFIFO DECIMAL(20,3);

					SELECT TOP(1) @RegistrationDateFirstFree=tip.TransactionRegistrationDate, @TransactionIdFirstFree=TransactionId, 
								  @TransactionItemIdFirstFree=TransactionItemId, @TransactionItemRowVersionFirstFree=TransactionItemRowVersion, 
								  @TransactionItemPriceRegistrationDateFirstFree=TransactionItemPriceRegistrationDate, 
								  @TransactionItemPriceIdFirstFree=TransactionItemPriceId, 
								  @TransactionItemPriceRowVersionFirstFree=TransactionItemPriceRowVersion
					FROM ##TempIssuePriced tip				 
					SET @NotPricedTransactionId= ISNULL((
								 SELECT TOP(1)t.Id
								  FROM Transactions t
									INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
									INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
									INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
									WHERE a.Id=@WarehouseId
										  AND ti.GoodId=@GoodId
										  AND ti.QuantityUnitId=@QuantityUnitId
										  AND t.TimeBucketId=@TimeBucketId
										  AND t.[Action]=1
										  AND tip.QuantityAmountUseFIFO<tip.QuantityAmount
										  AND (tip.Fee=0 OR tip.FeeInMainCurrency=0)
										  AND ( CONVERT(NVARCHAR(20),t.RegistrationDate,(120))
										 + dbo.LPAD(CAST(t.Id AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(ti.Id AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(ti.RowVersion AS NVARCHAR(10)),' ',10) 
										 + CONVERT(NVARCHAR(20),tip.RegistrationDate,(120))
										 + dbo.LPAD(CAST(tip.Id AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(tip.RowVersion AS NVARCHAR(15)),' ',15))<=
         
										 ( CONVERT(NVARCHAR(20),@RegistrationDateFirstFree,(120))
										 + dbo.LPAD(CAST(@TransactionIdFirstFree AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(@TransactionItemIdFirstFree AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(@TransactionItemRowVersionFirstFree AS NVARCHAR(10)),' ',10) 
										 + CONVERT(NVARCHAR(20),@TransactionItemPriceRegistrationDateFirstFree,(120))
										 + dbo.LPAD(CAST(@TransactionItemPriceIdFirstFree AS NVARCHAR(15)),' ',15)
										 + dbo.LPAD(CAST(@TransactionItemPriceRowVersionFirstFree AS NVARCHAR(15)),' ',15)
										 )
									)  ,0)
					--IF @NotPricedTransactionId<>0
					--BEGIN
					--	RAISERROR(N'@رسيدي که لازم است در قيمت گذاري از آن استفاده شود؛ هنوز قيمت نخورده است',16,1,'500')
					--	IF OBJECT_ID('tempdb..##TempIssuePriced') IS NOT NULL
					--		 EXEC('DROP TABLE ##TempIssuePriced')
					--	RETURN
					--END       
					--ELSE
					IF @NotPricedTransactionId=0
					BEGIN
			
						IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'AssignIssuePrice')
								DEALLOCATE AssignIssuePrice
							DECLARE AssignIssuePrice CURSOR FOR
								SELECT  t.TransactionId, 
										t.TransactionRegistrationDate,
										t.TransactionItemId,
										t.TransactionItemRowVersion, 
										t.TransactionItemQuantityAmount,
										t.TransactionItemPriceId,
										t.TransactionItemPriceRowVersion, 
										t.TransactionItemPriceRegistrationDate,
										t.TransactionItemPriceQuantityAmount, 
										t.TransactionItemPricePriceUnitId,
										t.TransactionItemPriceFee,
										t.TransactionItemPriceMainCurrencyUnitId, 
										t.TransactionItemPriceExchangeRate,
										t.TransactionItemPriceQuantityAmountUseFIFO
								FROM ##TempIssuePriced t
							OPEN AssignIssuePrice
								FETCH NEXT FROM AssignIssuePrice INTO   @TransactionId, 
																		@TransactionRegistrationDate,
																		@TransactionItemId,
																		@TransactionItemRowVersion, 
																		@TransactionItemQuantityAmount,
																		@TransactionItemPriceId,
																		@TransactionItemPriceRowVersion, 
																		@TransactionItemPriceRegistrationDate,
																		@TransactionItemPriceQuantityAmount, 
																		@TransactionItemPricePriceUnitId,
																		@TransactionItemPriceFee,
																		@TransactionItemPriceMainCurrencyUnitId, 
																		@TransactionItemPriceExchangeRate,
																		@TransactionItemPriceQuantityAmountUseFIFO
								WHILE @@Fetch_Status = 0  
								BEGIN
									SET @CurrentQuantityAmount= @TransactionItemPriceQuantityAmount-@TransactionItemPriceQuantityAmountUseFIFO;
									IF @CurrentQuantityAmount>(@QuantityAmount)
									BEGIN
										SET @CurrentQuantityAmount=@QuantityAmount
									END
									DECLARE @RowVersion SMALLINT
									SET @RowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	
									INSERT INTO TransactionItemPrices
									(
										-- Id -- this column value is auto-generated
										RowVersion,
										TransactionId,
										TransactionItemId,
										--[Description],
										QuantityUnitId,
										QuantityAmount,
										PriceUnitId,
										Fee,
										MainCurrencyUnitId,
										FeeInMainCurrency,
										RegistrationDate,
										--QuantityAmountUseFIFO,
										TransactionReferenceId,
										UserCreatorId,
										CreateDate
									)
									VALUES
									(
										@RowVersion ,
										@IssueId ,
										@IssueItemId ,
										/*{ [Description] }*/
										@QuantityUnitId ,
										@CurrentQuantityAmount,
										@TransactionItemPricePriceUnitId,
										@TransactionItemPriceFee,
										@TransactionItemPriceMainCurrencyUnitId,
										@TransactionItemPriceExchangeRate,
										@RegistrationDate,
										/*{ QuantityAmountUseFIFO }*/
										@TransactionItemPriceId,
										@UserCreatorId ,
										getdate()
									)
									 SET @TransactionItemPriceIds+=cast(@@identity AS NVARCHAR(15))+N';'
						
									UPDATE TransactionItemPrices
									SET
										QuantityAmountUseFIFO += @CurrentQuantityAmount,
										IssueReferenceIds = isnull (IssueReferenceIds,N'')+cast(@@identity AS NVARCHAR(15))+N';'
									WHERE Id=@TransactionItemPriceId
									
									SET @QuantityAmount-=@CurrentQuantityAmount
									IF @QuantityAmount=0
									   BREAK;
         							FETCH AssignIssuePrice INTO @TransactionId, 
																@TransactionRegistrationDate,
																@TransactionItemId,
																@TransactionItemRowVersion, 
																@TransactionItemQuantityAmount,
																@TransactionItemPriceId,
																@TransactionItemPriceRowVersion, 
																@TransactionItemPriceRegistrationDate,
																@TransactionItemPriceQuantityAmount, 
																@TransactionItemPricePriceUnitId,
																@TransactionItemPriceFee,
																@TransactionItemPriceMainCurrencyUnitId, 
																@TransactionItemPriceExchangeRate,
																@TransactionItemPriceQuantityAmountUseFIFO
								END
						END
				FETCH CursorIssueItemPricesAutomatically INTO  @ActionType,
															   @GoodId,
															   @QuantityUnitId,
															   @QuantityAmount,
															   @TimeBucketId,
													           @RegistrationDate,
														       @IssueItemRowVersion,
														       @IssueItemId,
														       @IssueId
		 END
		 --IF @QuantityAmount<>0
			--    RAISERROR(N'@موجودي رسيدهاي با قيمت براي قيمت گزاري حواله کافي نيست',16,1,'500')
		END
		
		SET @Message=N'OperationSuccessful'
		IF @TRANSACTIONCOUNT = 0
			COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
			    SET @Message=ERROR_MESSAGE();
				SET @TransactionItemPriceIds=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('PriceAllSuspendedTransactionItemsUsingReference','P') is Not Null
	drop procedure PriceAllSuspendedTransactionItemsUsingReference;
Go
Create Procedure PriceAllSuspendedTransactionItemsUsingReference
(
@WarehouseId INT,
@UserCreatorId INT,
@Action TINYINT,
@TransactionItemPriceIds NVARCHAR(MAX) OUT,
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
BEGIN
set nocount ON;
DECLARE @TRANSACTIONCOUNT INT;
	SET @TRANSACTIONCOUNT = @@TRANCOUNT;
BEGIN TRY
	IF @TRANSACTIONCOUNT = 0
		BEGIN TRANSACTION
	ELSE
		SAVE TRANSACTION CurrentTransaction;
	DECLARE @IssueItemId INT,
			@GoodId int ,
			@QuantityUnitId INT ,
			@QuantityAmount DECIMAL(20,3),
			@TimeBucketId INT,
			@RegistrationDate DATETIME,
			@IssueId INT=NULL,
			@PricingReferenceId INT,
			@IssueItemRowVersion SMALLINT=NULL,
			@ActionType SMALLINT=NULL,
			
			@NotPricedTransactionId INT
			
			
		SET @TransactionItemPriceIds=N''
	IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'CursorIssueItemPricesBackPayAutomatically')
	DEALLOCATE CursorIssueItemPricesBackPayAutomatically
	DECLARE CursorIssueItemPricesBackPayAutomatically CURSOR FOR
	SELECT  
		ISNULL(t.Action, 0) AS ActionType,
		ti.GoodId AS GoodId,
		ti.QuantityUnitId AS QuantityUnitId,
		ti.QuantityAmount AS QuantityAmount,
		t.TimeBucketId AS TimeBucketId,
		t.PricingReferenceId ,
		t.RegistrationDate AS RegistrationDate,
		ti.RowVersion AS IssueItemRowVersion,
		ti.Id AS IssueItemId,
		t.Id AS IssueId
	FROM   Transactions t
		INNER JOIN TransactionItems ti
			   	ON  t.Id = ti.TransactionId
	WHERE  t.WarehouseId=@WarehouseId 
	       AND t.[Action]=@Action
	       AND t.[Status]<3
	       AND ISNULL(t.PricingReferenceId,0)<>0  
	ORDER BY t.RegistrationDate,
			 t.Action ,
			 t.Id ,
			 ti.Id ,
			 ti.RowVersion 
			 
	OPEN CursorIssueItemPricesBackPayAutomatically
		FETCH NEXT FROM CursorIssueItemPricesBackPayAutomatically INTO  @ActionType,
																 @GoodId,
																 @QuantityUnitId,
																 @QuantityAmount,
															     @TimeBucketId,
															     @PricingReferenceId,
															     @RegistrationDate,
															     @IssueItemRowVersion,
															     @IssueItemId,
															     @IssueId
		WHILE @@Fetch_Status = 0  
		BEGIN
			DECLARE @IssueItemPriceId INT=NULL,
					@IssueItemPriceRowVersion SMALLINT=0,
					@CurrentQuantityAmount DECIMAL(20,3)
		IF ISNULL(@PricingReferenceId,0)=0
		BEGIN
			   SET @Message='Failed' 
			    RAISERROR(N'@فقط عمليات رسيد / حواله هاي برگشتي يا فاکتور بصورت سيستمي و با استفاده از مرجع قابل قيمت گذاري مي باشد ',16,1,'500')	
		END
		IF ISNULL(@PricingReferenceId,0)<>ISNULL((SELECT TOP(1)tip.TransactionReferenceId
		                                            FROM TransactionItemPrices tip 
		                                          WHERE tip.TransactionReferenceId<>@PricingReferenceId 
														AND tip.TransactionItemId=@IssueItemId),@PricingReferenceId)
		BEGIN
			   SET @Message='Failed' 
			   RAISERROR(N'@رسيد / حواله هاي برگشتي يا فاکتور فقط از يک مرجع قابل قيمت گذاري مي باشند ',16,1,'500')	
		END 
		 
		 SET @QuantityAmount=@QuantityAmount- ISNULL((SELECT SUM(ISNULL(tip.QuantityAmount,0)) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)
		 IF @QuantityAmount>0
		 BEGIN
			IF OBJECT_ID('tempdb..##TempIssuePriced') IS NOT NULL
					   EXEC('DROP TABLE ##TempIssuePriced')
			
					SELECT TOP(1) @IssueItemPriceId=ISNULL(MAX(tip.Id),0)+1  FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId
					IF @IssueItemPriceId=1 SET @IssueItemPriceRowVersion=1
					ELSE  SET @IssueItemPriceRowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	

			SELECT t.Id AS TransactionId, t.RegistrationDate AS TransactionRegistrationDate,ti.Id AS TransactionItemId, ti.RowVersion AS TransactionItemRowVersion, 
				   ti.QuantityAmount AS TransactionItemQuantityAmount,tip.Id AS TransactionItemPriceId, tip.RowVersion AS TransactionItemPriceRowVersion, 
				   tip.RegistrationDate AS TransactionItemPriceRegistrationDate, tip.QuantityAmount AS TransactionItemPriceQuantityAmount, 
				   tip.PriceUnitId AS TransactionItemPricePriceUnitId, tip.Fee AS TransactionItemPriceFee, tip.MainCurrencyUnitId AS TransactionItemPriceMainCurrencyUnitId, 
				   tip.FeeInMainCurrency AS TransactionItemPriceExchangeRate, tip.QuantityAmountUseFIFO AS TransactionItemPriceQuantityAmountUseFIFO
			INTO ##TempIssuePriced
			FROM Transactions t
			INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
			INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
			INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
			WHERE a.Id=@WarehouseId
				  AND ti.GoodId=@GoodId
				  AND ti.QuantityUnitId=@QuantityUnitId
				  AND t.TimeBucketId=@TimeBucketId
				  AND t.[Action]=case when @ActionType=1 then 2  when @ActionType=2 then  1  when @ActionType=3 then 2 end
				  --AND tip.QuantityAmountUseFIFO<tip.QuantityAmount    --  با جناب هاتفي هماهنگ کرده و نظر ايشان اعمال شد
				  AND tip.Fee>0
				  AND tip.FeeInMainCurrency>0
				  AND t.Id=@PricingReferenceId
				ORDER BY 
				   t.RegistrationDate DESC,
				   t.Action DESC,
				   t.Id DESC,
				   ti.Id DESC,
				   ti.RowVersion DESC,
				   tip.RegistrationDate DESC,
				   tip.Id DESC,
				   tip.RowVersion DESC --بصورت LIFO قيمت گذاري برگشتي انجام مي شود
					
				DECLARE     @TransactionId INT, 
							@TransactionRegistrationDate DATETIME,
							@TransactionItemId INT,
							@TransactionItemRowVersion SMALLINT, 
							@TransactionItemQuantityAmount DECIMAL(20,3),
							@TransactionItemPriceId INT,
							@TransactionItemPriceRowVersion SMALLINT, 
							@TransactionItemPriceRegistrationDate DATETIME,
							@TransactionItemPriceQuantityAmount DECIMAL(20,3),
							@TransactionItemPricePriceUnitId INT,
							@TransactionItemPriceFee DECIMAL(20,3),
							@TransactionItemPriceMainCurrencyUnitId INT, 
							@TransactionItemPriceExchangeRate DECIMAL(20,3),
							@TransactionItemPriceQuantityAmountUseFIFO DECIMAL(20,3);	
					--**************************
					--قسمتي که براي بررسي رسيدهاي قبلي قيمت گذاري شده باشد براي اين سناريو حذف کردم
					--**************************
						IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'AssignIssuePrice')
								DEALLOCATE AssignIssuePrice
							DECLARE AssignIssuePrice CURSOR FOR
								SELECT  t.TransactionId, 
										t.TransactionRegistrationDate,
										t.TransactionItemId,
										t.TransactionItemRowVersion, 
										t.TransactionItemQuantityAmount,
										t.TransactionItemPriceId,
										t.TransactionItemPriceRowVersion, 
										t.TransactionItemPriceRegistrationDate,
										t.TransactionItemPriceQuantityAmount, 
										t.TransactionItemPricePriceUnitId,
										t.TransactionItemPriceFee,
										t.TransactionItemPriceMainCurrencyUnitId, 
										t.TransactionItemPriceExchangeRate,
										t.TransactionItemPriceQuantityAmountUseFIFO
								FROM ##TempIssuePriced t
								ORDER BY
									   t.TransactionRegistrationDate DESC,
									   t.TransactionId DESC,
									   t.TransactionItemId DESC,
									   t.TransactionItemRowVersion DESC,
									   t.TransactionItemPriceRegistrationDate DESC,
									   t.TransactionItemPriceId DESC,
									   t.TransactionItemPriceRowVersion DESC --بصورت LIFO قيمت گذاري برگشتي انجام مي شود
							OPEN AssignIssuePrice
								FETCH NEXT FROM AssignIssuePrice INTO   @TransactionId, 
																		@TransactionRegistrationDate,
																		@TransactionItemId,
																		@TransactionItemRowVersion, 
																		@TransactionItemQuantityAmount,
																		@TransactionItemPriceId,
																		@TransactionItemPriceRowVersion, 
																		@TransactionItemPriceRegistrationDate,
																		@TransactionItemPriceQuantityAmount, 
																		@TransactionItemPricePriceUnitId,
																		@TransactionItemPriceFee,
																		@TransactionItemPriceMainCurrencyUnitId, 
																		@TransactionItemPriceExchangeRate,
																		@TransactionItemPriceQuantityAmountUseFIFO
								WHILE @@Fetch_Status = 0  
								BEGIN
									SET @CurrentQuantityAmount= @TransactionItemPriceQuantityAmount--  -@TransactionItemPriceQuantityAmountUseFIFO;
									IF @CurrentQuantityAmount>(@QuantityAmount)
									BEGIN
										SET @CurrentQuantityAmount=@QuantityAmount
									END
									DECLARE @RowVersion SMALLINT
									SET @RowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	
									INSERT INTO TransactionItemPrices
									(
										-- Id -- this column value is auto-generated
										RowVersion,
										TransactionId,
										TransactionItemId,
										--[Description],
										QuantityUnitId,
										QuantityAmount,
										PriceUnitId,
										Fee,
										MainCurrencyUnitId,
										FeeInMainCurrency,
										RegistrationDate,
										--QuantityAmountUseFIFO,
										TransactionReferenceId,
										UserCreatorId,
										CreateDate
									)
									VALUES
									(
										@RowVersion ,
										@IssueId ,
										@IssueItemId ,
										/*{ [Description] }*/
										@QuantityUnitId ,
										@CurrentQuantityAmount,
										@TransactionItemPricePriceUnitId,
										@TransactionItemPriceFee,
										@TransactionItemPriceMainCurrencyUnitId,
										@TransactionItemPriceExchangeRate,
										@RegistrationDate,
										/*{ QuantityAmountUseFIFO }*/
										@TransactionItemPriceId,
										@UserCreatorId ,
										getdate()
									)
									 SET @TransactionItemPriceIds+=cast(@@identity AS NVARCHAR(15))+N';'
						
									UPDATE TransactionItemPrices
									SET
										QuantityAmountUseFIFO += @CurrentQuantityAmount,
										IssueReferenceIds = isnull (IssueReferenceIds,N'')+cast(@@identity AS NVARCHAR(15))+N';'
									WHERE Id=@TransactionItemPriceId
									
									SET @QuantityAmount-=@CurrentQuantityAmount
									IF @QuantityAmount=0
									   BREAK;
         							FETCH AssignIssuePrice INTO @TransactionId, 
																@TransactionRegistrationDate,
																@TransactionItemId,
																@TransactionItemRowVersion, 
																@TransactionItemQuantityAmount,
																@TransactionItemPriceId,
																@TransactionItemPriceRowVersion, 
																@TransactionItemPriceRegistrationDate,
																@TransactionItemPriceQuantityAmount, 
																@TransactionItemPricePriceUnitId,
																@TransactionItemPriceFee,
																@TransactionItemPriceMainCurrencyUnitId, 
																@TransactionItemPriceExchangeRate,
																@TransactionItemPriceQuantityAmountUseFIFO
								END
				FETCH CursorIssueItemPricesBackPayAutomatically INTO  @ActionType,
																 @GoodId,
																 @QuantityUnitId,
																 @QuantityAmount,
															     @TimeBucketId,
															     @PricingReferenceId,
															     @RegistrationDate,
															     @IssueItemRowVersion,
															     @IssueItemId,
															     @IssueId
		 END
			 --IF @QuantityAmount<>0
				--	RAISERROR(N'@موجودي رسيدهاي با قيمت براي قيمت گزاري حواله کافي نيست',16,1,'500')
		END 
		SET @Message=N'OperationSuccessful'
		IF @TRANSACTIONCOUNT = 0
				COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
				 SET @Message=ERROR_MESSAGE();
				SET @TransactionItemPriceIds=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------------------
if OBJECT_ID('PriceSuspendedTransactionUsingReference','P') is Not Null
	drop procedure PriceSuspendedTransactionUsingReference;
Go
Create Procedure PriceSuspendedTransactionUsingReference
(
@TransactionId INT,
@Description NVARCHAR(MAX)=NULL,
@UserCreatorId INT,
@TransactionItemPriceIds NVARCHAR(MAX) OUT,
@Message NVARCHAR(MAX) OUT
)
--WITH ENCRYPTION
AS 
BEGIN
set nocount ON;
DECLARE @TRANSACTIONCOUNT INT;
	SET @TRANSACTIONCOUNT = @@TRANCOUNT;
BEGIN TRY
	IF @TRANSACTIONCOUNT = 0
		BEGIN TRANSACTION
	ELSE
		SAVE TRANSACTION CurrentTransaction;
	DECLARE @IssueItemId INT,
			@GoodId int ,
			@QuantityUnitId INT ,
			@QuantityAmount DECIMAL(20,3),
			@TimeBucketId INT,
			@RegistrationDate DATETIME,
			@IssueId INT=NULL,
			@PricingReferenceId INT,
			@IssueItemRowVersion SMALLINT=NULL,
			@WarehouseId INT,
			@ActionType SMALLINT=NULL,
			@StoreTypesId INT=NULL
			
			
		SET @TransactionItemPriceIds=N''
	IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'CursorPriceSuspendedTransactionUsingReference')
	DEALLOCATE CursorPriceSuspendedTransactionUsingReference
	DECLARE CursorPriceSuspendedTransactionUsingReference CURSOR FOR
	SELECT  
		ISNULL(t.Action, 0) AS ActionType,
		ti.GoodId AS GoodId,
		ti.QuantityUnitId AS QuantityUnitId,
		ti.QuantityAmount AS QuantityAmount,
		t.TimeBucketId AS TimeBucketId,
		t.PricingReferenceId ,
		t.RegistrationDate AS RegistrationDate,
		ti.RowVersion AS IssueItemRowVersion,
		ti.Id AS IssueItemId,
		t.Id AS IssueId,
		t.WarehouseId,
		t.StoreTypesId
	FROM   Transactions t
		INNER JOIN TransactionItems ti
			   	ON  t.Id = ti.TransactionId
	WHERE  t.Id=@TransactionId 
	       AND t.[Status]<3
	       AND ISNULL(t.PricingReferenceId,0)<>0  
	ORDER BY t.RegistrationDate,
			 t.Action ,
			 t.Id ,
			 ti.Id ,
			 ti.RowVersion 
			 
	OPEN CursorPriceSuspendedTransactionUsingReference
		FETCH NEXT FROM CursorPriceSuspendedTransactionUsingReference INTO  @ActionType,
																 @GoodId,
																 @QuantityUnitId,
																 @QuantityAmount,
															     @TimeBucketId,
															     @PricingReferenceId,
															     @RegistrationDate,
															     @IssueItemRowVersion,
															     @IssueItemId,
															     @IssueId,
															     @WarehouseId,
															     @StoreTypesId
		WHILE @@Fetch_Status = 0  
		BEGIN
			DECLARE @IssueItemPriceId INT=NULL,
					@IssueItemPriceRowVersion SMALLINT=0,
					@CurrentQuantityAmount DECIMAL(20,3)
		IF ISNULL(@PricingReferenceId,0)=0
		BEGIN
			   SET @Message='Failed' 
			   RAISERROR(N'@فقط عمليات رسيد / حواله هاي برگشتي يا فاکتور بصورت سيستمي و با استفاده از مرجع قابل قيمت گذاري مي باشد ',16,1,'500')	
		END
		IF ISNULL(@PricingReferenceId,0)<>ISNULL((SELECT TOP(1)tip.TransactionReferenceId
		                                            FROM TransactionItemPrices tip 
		                                          WHERE tip.TransactionReferenceId<>@PricingReferenceId 
														AND tip.TransactionItemId=@IssueItemId),@PricingReferenceId)
		BEGIN
			   SET @Message='Failed' 
			   RAISERROR(N'@رسيد / حواله هاي برگشتي يا فاکتور فقط از يک مرجع قابل قيمت گذاري مي باشند ',16,1,'500')	
		END 
		 
		 SET @QuantityAmount=@QuantityAmount- ISNULL((SELECT SUM(ISNULL(tip.QuantityAmount,0)) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)
		 IF @QuantityAmount>0
		 BEGIN
			IF OBJECT_ID('tempdb..##TempIssuePriced') IS NOT NULL
					   EXEC('DROP TABLE ##TempIssuePriced')
			
					SELECT TOP(1) @IssueItemPriceId=ISNULL(MAX(tip.Id),0)+1  FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId
					IF @IssueItemPriceId=1 SET @IssueItemPriceRowVersion=1
					ELSE  SET @IssueItemPriceRowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	

			SELECT t.Id AS TransactionId,t.[Action], t.RegistrationDate AS TransactionRegistrationDate,ti.Id AS TransactionItemId, ti.RowVersion AS TransactionItemRowVersion, 
				   ti.QuantityAmount AS TransactionItemQuantityAmount,tip.Id AS TransactionItemPriceId, tip.RowVersion AS TransactionItemPriceRowVersion, 
				   tip.RegistrationDate AS TransactionItemPriceRegistrationDate, tip.QuantityAmount AS TransactionItemPriceQuantityAmount, 
				   tip.PriceUnitId AS TransactionItemPricePriceUnitId, tip.Fee AS TransactionItemPriceFee, tip.MainCurrencyUnitId AS TransactionItemPriceMainCurrencyUnitId, 
				   tip.FeeInMainCurrency AS TransactionItemPriceExchangeRate, tip.QuantityAmountUseFIFO AS TransactionItemPriceQuantityAmountUseFIFO
			INTO ##TempIssuePriced
			FROM Transactions t
			INNER JOIN TransactionItems ti ON ti.TransactionId=t.Id
			INNER JOIN TransactionItemPrices tip ON tip.TransactionItemId=ti.Id
			INNER JOIN Warehouse a ON a.Id = t.WarehouseId AND a.[Active]=1
			WHERE a.Id=@WarehouseId
				  AND ti.GoodId=@GoodId
				  AND ti.QuantityUnitId=@QuantityUnitId
				  AND t.TimeBucketId=@TimeBucketId
				  --AND t.[Action]=case when @ActionType=1 then 2 else 1 end
				  --AND tip.QuantityAmountUseFIFO<tip.QuantityAmount    --  با جناب هاتفي هماهنگ کرده و نظر ايشان اعمال شد
				  AND tip.Fee>0
				  AND tip.FeeInMainCurrency>0
				  AND t.Id=@PricingReferenceId
				ORDER BY
				   t.RegistrationDate DESC,
				   t.Action DESC,
				   t.Id DESC,
				   ti.Id DESC,
				   ti.RowVersion DESC,
				   tip.RegistrationDate DESC,
				   tip.Id DESC,
				   tip.RowVersion DESC --بصورت LIFO قيمت گذاري برگشتي انجام مي شود
					
				DECLARE     @TransactionRegistrationDate DATETIME,
							@TransactionItemId INT,
							@Action SMALLINT,
							@TransactionItemRowVersion SMALLINT, 
							@TransactionItemQuantityAmount DECIMAL(20,3),
							@TransactionItemPriceId INT,
							@TransactionItemPriceRowVersion SMALLINT, 
							@TransactionItemPriceRegistrationDate DATETIME,
							@TransactionItemPriceQuantityAmount DECIMAL(20,3),
							@TransactionItemPricePriceUnitId INT,
							@TransactionItemPriceFee DECIMAL(20,3),
							@TransactionItemPriceMainCurrencyUnitId INT, 
							@TransactionItemPriceExchangeRate DECIMAL(20,3),
							@TransactionItemPriceQuantityAmountUseFIFO DECIMAL(20,3);	
					--**************************
					--قسمتي که براي بررسي رسيدهاي قبلي قيمت گذاري شده باشد براي اين سناريو حذف کردم
					--**************************
						IF EXISTS(SELECT cursor_name FROM sys.syscursors WHERE cursor_name = 'AssignIssuePrice')
								DEALLOCATE AssignIssuePrice
							DECLARE AssignIssuePrice CURSOR FOR
								SELECT  t.TransactionId, 
								        t.Action,
										t.TransactionRegistrationDate,
										t.TransactionItemId,
										t.TransactionItemRowVersion, 
										t.TransactionItemQuantityAmount,
										t.TransactionItemPriceId,
										t.TransactionItemPriceRowVersion, 
										t.TransactionItemPriceRegistrationDate,
										t.TransactionItemPriceQuantityAmount, 
										t.TransactionItemPricePriceUnitId,
										t.TransactionItemPriceFee,
										t.TransactionItemPriceMainCurrencyUnitId, 
										t.TransactionItemPriceExchangeRate,
										t.TransactionItemPriceQuantityAmountUseFIFO
								FROM ##TempIssuePriced t
								ORDER BY
									   t.TransactionRegistrationDate DESC,
									   t.TransactionId DESC,
									   t.TransactionItemId DESC,
									   t.TransactionItemRowVersion DESC,
									   t.TransactionItemPriceRegistrationDate DESC,
									   t.TransactionItemPriceId DESC,
									   t.TransactionItemPriceRowVersion DESC --بصورت LIFO قيمت گذاري برگشتي انجام مي شود
							OPEN AssignIssuePrice
								FETCH NEXT FROM AssignIssuePrice INTO   @TransactionId, 
																		@Action,
																		@TransactionRegistrationDate,
																		@TransactionItemId,
																		@TransactionItemRowVersion, 
																		@TransactionItemQuantityAmount,
																		@TransactionItemPriceId,
																		@TransactionItemPriceRowVersion, 
																		@TransactionItemPriceRegistrationDate,
																		@TransactionItemPriceQuantityAmount, 
																		@TransactionItemPricePriceUnitId,
																		@TransactionItemPriceFee,
																		@TransactionItemPriceMainCurrencyUnitId, 
																		@TransactionItemPriceExchangeRate,
																		@TransactionItemPriceQuantityAmountUseFIFO
								WHILE @@Fetch_Status = 0  
								BEGIN
									--به گفته جناب هاتفي(خانم رضاييان)در تاريخ 1393-04-03
									--تمام مقادير Correction+
									-- با في يک آرتيکل از مرجع انتخاب شده قيمت گذاري گردد و کاري با ميزان و موجودي آن مرجع نداريم
									IF (SELECT TOP(1) Code FROM StoreTypes st WHERE st.Id=@StoreTypesId)=7 AND @Action=1
									BEGIN
										SET @CurrentQuantityAmount=@QuantityAmount
									END
									ELSE
									BEGIN
										-- روند عادي قيمت گذاري از روي مرجع و سطر به سطر بر اساس تعداد و قيمت مرجع
										SET @CurrentQuantityAmount= @TransactionItemPriceQuantityAmount--  -@TransactionItemPriceQuantityAmountUseFIFO;
										IF @CurrentQuantityAmount>(@QuantityAmount)
										BEGIN
											SET @CurrentQuantityAmount=@QuantityAmount
										END	
									END
									
									DECLARE @RowVersion SMALLINT
									SET @RowVersion=ISNULL((SELECT MAX(tip.[RowVersion]) FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@IssueItemId),0)+1	
									INSERT INTO TransactionItemPrices
									(
										-- Id -- this column value is auto-generated
										RowVersion,
										TransactionId,
										TransactionItemId,
										[Description],
										QuantityUnitId,
										QuantityAmount,
										PriceUnitId,
										Fee,
										MainCurrencyUnitId,
										FeeInMainCurrency,
										RegistrationDate,
										--QuantityAmountUseFIFO,
										TransactionReferenceId,
										UserCreatorId,
										CreateDate
									)
									VALUES
									(
										@RowVersion ,
										@IssueId ,
										@IssueItemId ,
										@Description,
										@QuantityUnitId ,
										@CurrentQuantityAmount,
										@TransactionItemPricePriceUnitId,
										@TransactionItemPriceFee,
										@TransactionItemPriceMainCurrencyUnitId,
										@TransactionItemPriceExchangeRate,
										@RegistrationDate,
										/*{ QuantityAmountUseFIFO }*/
										@TransactionItemPriceId,
										@UserCreatorId ,
										getdate()
									)
									 SET @TransactionItemPriceIds+=cast(@@identity AS NVARCHAR(15))+N';'
						
									UPDATE TransactionItemPrices
									SET
										QuantityAmountUseFIFO += @CurrentQuantityAmount,
										IssueReferenceIds = isnull (IssueReferenceIds,N'')+cast(@@identity AS NVARCHAR(15))+N';'
									WHERE Id=@TransactionItemPriceId
									
									SET @QuantityAmount-=@CurrentQuantityAmount
									IF @QuantityAmount=0
									   BREAK;
         							FETCH AssignIssuePrice INTO @TransactionId, 
																@Action, 
																@TransactionRegistrationDate,
																@TransactionItemId,
																@TransactionItemRowVersion, 
																@TransactionItemQuantityAmount,
																@TransactionItemPriceId,
																@TransactionItemPriceRowVersion, 
																@TransactionItemPriceRegistrationDate,
																@TransactionItemPriceQuantityAmount, 
																@TransactionItemPricePriceUnitId,
																@TransactionItemPriceFee,
																@TransactionItemPriceMainCurrencyUnitId, 
																@TransactionItemPriceExchangeRate,
																@TransactionItemPriceQuantityAmountUseFIFO
								END
				FETCH CursorPriceSuspendedTransactionUsingReference INTO  @ActionType,
																 @GoodId,
																 @QuantityUnitId,
																 @QuantityAmount,
															     @TimeBucketId,
															     @PricingReferenceId,
															     @RegistrationDate,
															     @IssueItemRowVersion,
															     @IssueItemId,
															     @IssueId,
															     @WarehouseId,
															     @StoreTypesId
		 END
			 --IF @QuantityAmount<>0
				--	RAISERROR(N'@موجودي رسيدهاي با قيمت براي قيمت گزاري حواله کافي نيست',16,1,'500')
		END 
		SET @Message=N'OperationSuccessful'
		IF @TRANSACTIONCOUNT = 0
				COMMIT TRANSACTION 					
		END try
		BEGIN CATCH
				IF (XACT_STATE()) = -1 
					ROLLBACK TRANSACTION
				IF (XACT_STATE()) = 1 AND @TRANSACTIONCOUNT=0
					ROLLBACK TRANSACTION
				IF  (XACT_STATE()) = 1 and @TRANSACTIONCOUNT > 0
					ROLLBACK TRANSACTION CurrentTransaction
				 SET @Message=ERROR_MESSAGE();
				SET @TransactionItemPriceIds=NULL
			EXEC ErrorHandling
		END CATCH
	END
GO	
----------------------------------------------------------------------
IF OBJECT_ID ( '[TransactionsGetAll]', 'P' ) IS NOT NULL 
	DROP PROCEDURE [TransactionsGetAll];
GO
CREATE PROCEDURE [TransactionsGetAll]

--WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON;
	SELECT t.Id, t.[Action], t.Code, t.[Description], t.WarehouseId, --t.CrossId,
	       t.StoreTypesId, t.TimeBucketId, t.[Status], t.RegistrationDate,
	       t.SenderReciver, t.HardCopyNo, t.ReferenceType, t.ReferenceNo,
	       t.ReferenceDate, t.UserCreatorId, t.CreateDate,
	       st.Code AS StoreTypeCode, st.InputName AS StoreTypeInputName, st.OutputName AS StoreTypeOutputName,
	       w.Code AS WarehouseCode, w.Name AS WarehouseName, w.CompanyId, w.[Active] AS WarehouseStatus,
	       c.Code AS CompanyCode, c.Name AS CompanyName, c.[Active] AS CompanyStatus
	FROM Transactions t
	INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
	INNER JOIN Warehouse w ON w.Id = t.WarehouseId
	INNER JOIN Companies c ON c.Id = w.CompanyId
END
GO
----------------------------------------------------------------------
IF OBJECT_ID ( '[TransactionItemsGetAll]', 'P' ) IS NOT NULL 
	DROP PROCEDURE [TransactionItemsGetAll];
GO
CREATE PROCEDURE [TransactionItemsGetAll]

--WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON;
	SELECT ti.Id AS tiId, ti.RowVersion AS tiRowVersion, ti.GoodId AS tiGoodId, ti.QuantityUnitId AS tiQuantityUnitId,
	       ti.QuantityAmount AS tiQuantityAmount, ti.[Description] AS tiDescription, 
	       ti.UserCreatorId AS tiUserCreatorId, ti.CreateDate AS tiCreateDate,
	       ti.TransactionId AS tId, t.[Action] AS tAction, t.Code AS tCode, t.[Description] AS tDescription, t.PricingReferenceId AS tPricingReferenceId,--t.CrossId AS tCrossId, 
	       t.WarehouseId AS tWarehouseId,t.StoreTypesId AS tStoreTypesId, t.TimeBucketId AS tTimeBucketId, t.[Status] AS tStatus,
	       t.RegistrationDate AS tRegistrationDate,t.SenderReciver AS tSenderReciver, t.HardCopyNo AS tHardCopyNo, 
	       t.ReferenceType AS tReferenceType, t.ReferenceNo AS tReferenceNo,t.ReferenceDate AS tReferenceDate, 
	       t.UserCreatorId AS tUserCreatorId, t.CreateDate AS tCreateDate,
	       st.Code AS StoreTypeCode, st.InputName AS StoreTypeInputName, st.OutputName AS StoreTypeOutputName,
	       w.Code AS WarehouseCode, w.Name AS WarehouseName, w.CompanyId, w.[Active] AS WarehouseStatus,
	       c.Code AS CompanyCode, c.Name AS CompanyName, c.[Active] AS CompanyStatus,
	       g.Code AS GoodCode, g.Name AS GoodName, g.[Active] AS GoodStatus,
	       u.Code AS UnitCode, u.Name AS UnitName, u.IsCurrency AS UnitIsCurrency, u.IsBaseCurrency AS UnitIsBaseCurrency, u.[Active] AS UnitStatus
	FROM TransactionItems ti 
	INNER JOIN Transactions t ON t.Id = ti.TransactionId
	INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
	INNER JOIN Warehouse w ON w.Id = t.WarehouseId
	INNER JOIN Companies c ON c.Id = w.CompanyId
	INNER JOIN Goods g ON g.Id = ti.GoodId
	INNER JOIN Units u ON u.Id = ti.QuantityUnitId
END
GO
----------------------------------------------------------------------
IF OBJECT_ID ( '[TransactionItemPricesGetAll]', 'P' ) IS NOT NULL 
	DROP PROCEDURE [TransactionItemPricesGetAll];
GO
CREATE PROCEDURE [TransactionItemPricesGetAll]

--WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON;
	SELECT tip.Id AS tipId, tip.RowVersion AS tipRowVersion, tip.[Description] AS tipDescription, tip.QuantityUnitId AS tipQuantityUnitId,
	       tip.QuantityAmount AS tipQuantityAmount, tip.PriceUnitId AS tipPriceUnitId, tip.Fee AS tipFee, tip.MainCurrencyUnitId AS tipMainCurrencyUnitId,
	       tip.FeeInMainCurrency AS tipFeeInMainCurrency, tip.RegistrationDate AS tipRegistrationDate,
	       tip.QuantityAmountUseFIFO AS tipQuantityAmountUseFIFO, tip.TransactionReferenceId AS tipTransactionReferenceId,
	       tip.IssueReferenceIds AS tipIssueReferenceIds, tip.UserCreatorId AS tipUserCreatorId, tip.CreateDate AS tipCreateDate ,
	       
		   ti.Id AS tiId, ti.RowVersion AS tiRowVersion, ti.GoodId AS tiGoodId, ti.QuantityUnitId AS tiQuantityUnitId,
	       ti.QuantityAmount AS tiQuantityAmount, ti.[Description] AS tiDescription, 
	       ti.UserCreatorId AS tiUserCreatorId, ti.CreateDate AS tiCreateDate,
	       ti.TransactionId AS tId, t.[Action] AS tAction, t.Code AS tCode, t.[Description] AS tDescription,t.PricingReferenceId AS tPricingReferenceId, --t.CrossId AS tCrossId, 
	       t.WarehouseId AS tWarehouseId,t.StoreTypesId AS tStoreTypesId, t.TimeBucketId AS tTimeBucketId, t.[Status] AS tStatus,
	       t.RegistrationDate AS tRegistrationDate,t.SenderReciver AS tSenderReciver, t.HardCopyNo AS tHardCopyNo, 
	       t.ReferenceType AS tReferenceType, t.ReferenceNo AS tReferenceNo,t.ReferenceDate AS tReferenceDate, 
	       t.UserCreatorId AS tUserCreatorId, t.CreateDate AS tCreateDate,
	       
	       st.Code AS StoreTypeCode, st.InputName AS StoreTypeInputName, st.OutputName AS StoreTypeOutputName,
	       w.Code AS WarehouseCode, w.Name AS WarehouseName, w.CompanyId, w.[Active] AS WarehouseStatus,
	       c.Code AS CompanyCode, c.Name AS CompanyName, c.[Active] AS CompanyStatus,
	       g.Code AS GoodCode, g.Name AS GoodName, g.[Active] AS GoodStatus,
	       u.Code AS QuantityUnitCode, u.Name AS QuantityUnitName, u.[Active] AS QuantityUnitStatus,
	       u2.Code AS PriceUnitCode, u2.Name AS PriceUnitName, u2.[Active] AS PriceUnitStatus,
	       u3.Code AS MainCurrencyCode, u3.Name AS MainCurrencyName, u3.[Active] AS BaseUnitStatus
	FROM TransactionItemPrices tip
	INNER JOIN TransactionItems ti ON ti.Id = tip.TransactionItemId
	INNER JOIN Transactions t ON t.Id = ti.TransactionId
	INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
	INNER JOIN Warehouse w ON w.Id = t.WarehouseId
	INNER JOIN Companies c ON c.Id = w.CompanyId
	INNER JOIN Goods g ON g.Id = ti.GoodId
	LEFT JOIN Units u ON u.Id = tip.QuantityUnitId
	LEFT JOIN Units u2 ON u2.Id = tip.PriceUnitId AND u2.IsCurrency=1
	LEFT JOIN Units u3 ON u3.Id = tip.MainCurrencyUnitId AND u3.IsCurrency=1
END
GO
----------------------------------------------------------------------
IF OBJECT_ID ( '[Cardex]', 'P' ) IS NOT NULL 
	DROP PROCEDURE [Cardex];
GO
CREATE PROCEDURE [Cardex]
(
	--@CompanyId INT,
	@WarehouseId INT,
	@GoodId INT,
	@StartDate DATETIME,
	@EndDate DATETIME
)
--WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @StartDateTimeBucket DATETIME,
	        @EndDatePeriod DATETIME
	SET @StartDateTimeBucket =(SELECT TOP(1)tb.StartDate FROM TimeBucket tb WHERE (@StartDate BETWEEN tb.StartDate AND tb.EndDate))
	SET @EndDatePeriod = DATEADD([day],-1,@StartDate)
	
	SET @EndDate=DATEADD([day],1,@EndDate)
	
	DECLARE @Cardex TABLE
	(
	   RegistrationDate DATETIME,
	   Description NVARCHAR(MAX),
	   Code DECIMAL(20,2),
	   Action TINYINT,
	   SignAction SMALLINT,
	   QuantityUnitId INT,
	   QuantityUnitCode NVARCHAR(15),
	   QuantityUnitName NVARCHAR(MAX),
	   PriceUnitId INT,
	   PriceUnitCode NVARCHAR(15), 
	   PriceUnitName NVARCHAR(MAX),
	   MainCurrencyUnitId INT,
	   MainCurrencyCode NVARCHAR(15), 
	   MainCurrencyName NVARCHAR(MAX),
	   FeeInMainCurrency DECIMAL(20,3),
	   QuantityAmount DECIMAL(20,3),
	   QuantityAmountPriced DECIMAL(20,3),
	   TotalPrice DECIMAL(20,3)
	)
	
	INSERT INTO @Cardex
	(
		RegistrationDate,
		Description,
		Code,
		Action,
		SignAction,
		QuantityUnitId,
		QuantityUnitCode,
		QuantityUnitName,
		PriceUnitId,
		PriceUnitCode,
		PriceUnitName,
		MainCurrencyUnitId,
		MainCurrencyCode,
		MainCurrencyName,
		FeeInMainCurrency,
		QuantityAmount,
		QuantityAmountPriced,
		TotalPrice
	)
	SELECT NULL,/*{ @RegistrationDate }*/
		   NULL,/*{ @Description }*/
		   NULL,/*{ @Code }*/
		   NULL,/*{ @Action }*/
		   NULL,/*{ @SignAction }*/
		   tip.QuantityUnitId,
		   u.Code AS QuantityUnitCode, 
		   u.Name AS QuantityUnitName, 
		   tip.PriceUnitId, 
		   u2.Code AS PriceUnitCode, 
		   u2.Name AS PriceUnitName,
		   tip.MainCurrencyUnitId, 
		   u3.Code AS MainCurrencyCode, 
		   u3.Name AS MainCurrencyName,
		   NULL AS FeeInMainCurrency,
	       SUM(ISNULL(ti.QuantityAmount*(CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END),0)) AS QuantityAmount,
		   SUM(ISNULL(tip.QuantityAmount*(CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END),0)) AS QuantityAmountPriced,
	       SUM(ISNULL(tip.FeeInMainCurrency,0)*ISNULL(tip.QuantityAmount,0)*(CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END)) AS TotalPrice
	FROM TransactionItemPrices tip
	INNER JOIN TransactionItems ti ON ti.Id = tip.TransactionItemId
	INNER JOIN Transactions t ON t.Id = ti.TransactionId
	INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
	LEFT JOIN Units u ON u.Id = tip.QuantityUnitId
	LEFT JOIN Units u2 ON u2.Id = tip.PriceUnitId AND u2.IsCurrency=1
	LEFT JOIN Units u3 ON u3.Id = tip.MainCurrencyUnitId AND u3.IsCurrency=1
	WHERE t.WarehouseId=@WarehouseId
			AND (t.RegistrationDate BETWEEN @StartDateTimeBucket AND @EndDatePeriod)
			AND ti.GoodId=@GoodId
	GROUP BY 
	         tip.QuantityUnitId,u.Code , u.Name , 
		     tip.PriceUnitId, u2.Code, u2.Name,
		     tip.MainCurrencyUnitId, u3.Code, u3.Name,
		     tip.FeeInMainCurrency	
	;
	
	UPDATE @Cardex
	SET
		RegistrationDate = @EndDatePeriod,
		[Description] = N'اول دوره',
		Code = NULL,
		[Action] = CASE WHEN QuantityAmount>0 THEN 1 ELSE 2 END,
		SignAction =  CASE WHEN [Action] = 1 THEN 1 WHEN [Action] = 2 THEN -1 END,
		FeeInMainCurrency=TotalPrice/QuantityAmount
	;	 
	
	INSERT INTO @Cardex
	(
		RegistrationDate,
		Description,
		Code,
		Action,
		SignAction,
		QuantityUnitId,
		QuantityUnitCode,
		QuantityUnitName,
		PriceUnitId,
		PriceUnitCode,
		PriceUnitName,
		MainCurrencyUnitId,
		MainCurrencyCode,
		MainCurrencyName,
		FeeInMainCurrency,
		QuantityAmount,
		QuantityAmountPriced,
		TotalPrice
	)-- t.[Description],
	SELECT t.RegistrationDate,st.InputName,t.Code,t.[Action],CASE WHEN t.[Action] = 1 THEN 1 WHEN t.[Action] = 2 THEN -1 END AS SignAction ,
		   tip.QuantityUnitId,u.Code AS QuantityUnitCode, u.Name AS QuantityUnitName, 
		   tip.PriceUnitId, u2.Code AS PriceUnitCode, u2.Name AS PriceUnitName,
		   tip.MainCurrencyUnitId, u3.Code AS MainCurrencyCode, u3.Name AS MainCurrencyName,
		   tip.FeeInMainCurrency AS FeeInMainCurrency,
		   SUM(ISNULL(ti.QuantityAmount,0)) AS QuantityAmount,
	       SUM(ISNULL(tip.QuantityAmount,0)) AS QuantityAmountPriced,
	       SUM(ISNULL(tip.FeeInMainCurrency,0)*ISNULL(tip.QuantityAmount,0)) AS TotalPrice
	FROM TransactionItemPrices tip
	INNER JOIN TransactionItems ti ON ti.Id = tip.TransactionItemId
	INNER JOIN Transactions t ON t.Id = ti.TransactionId
	INNER JOIN StoreTypes st ON st.Id = t.StoreTypesId
	--INNER JOIN Warehouse w ON w.Id = t.WarehouseId
	--INNER JOIN Companies c ON c.Id = w.CompanyId
	--INNER JOIN Goods g ON g.Id = ti.GoodId
	LEFT JOIN Units u ON u.Id = tip.QuantityUnitId
	LEFT JOIN Units u2 ON u2.Id = tip.PriceUnitId AND u2.IsCurrency=1
	LEFT JOIN Units u3 ON u3.Id = tip.MainCurrencyUnitId AND u3.IsCurrency=1
	WHERE t.WarehouseId=@WarehouseId
			AND (t.RegistrationDate BETWEEN @StartDate AND @EndDate)
			AND ti.GoodId=@GoodId
	GROUP BY t.RegistrationDate, st.InputName,t.Code,t.[Action],CASE WHEN t.[Action] = 1 THEN 1 WHEN t.[Action] = 2 THEN -1 END 	,--t.[Description],
	         tip.QuantityUnitId,u.Code , u.Name , 
		     tip.PriceUnitId, u2.Code, u2.Name,
		     tip.MainCurrencyUnitId, u3.Code, u3.Name,
		     tip.FeeInMainCurrency	
	SELECT c.RegistrationDate, c.[Description], c.Code, c.[Action], c.SignAction,
	       c.QuantityUnitId, c.QuantityUnitCode, c.QuantityUnitName, c.PriceUnitId,
	       c.PriceUnitCode, c.PriceUnitName, c.MainCurrencyUnitId, c.MainCurrencyCode,
	       c.MainCurrencyName, c.QuantityAmount, c.QuantityAmountPriced, c.FeeInMainCurrency, c.TotalPrice 
	FROM @Cardex c
	ORDER BY  c.RegistrationDate,
			  c.Action,
			  c.Code
			  
	DELETE FROM @Cardex
END
GO
GRANT EXECUTE ON [dbo].[Cardex] TO [public] AS [dbo]
GO
------------------------------------------------------------	
RAISERROR('پایان اجرای پروسيجرها.',0,1) WITH NOWAIT
 -----
 