USE MiniStock
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[UpdateTransaction]', 'TR') IS NOT NULL
    DROP TRIGGER [UpdateTransaction];
GO
CREATE TRIGGER [UpdateTransaction]
ON [Transactions]
   AFTER INSERT, UPDATE
AS
BEGIN TRY
	DECLARE @Id        INT,
			@Action tinyint ,
			@Code decimal(20,2) ,
			@Description nvarchar(max),
			--@CrossId int ,
			@PricingReferenceId int ,
			@WarehouseId INT ,
			@TimeBucketId INT,
			@StoreTypesId INT ,
			@Status tinyint ,
			@RegistrationDate DATETIME ,
			@SenderReciver int ,
			@HardCopyNo NVARCHAR(10) ,
			@ReferenceType NVARCHAR(100) ,
			@ReferenceNo NVARCHAR(100) ,
			@ReferenceDate DATETIME ,
			@UserCreatorId int,

			@CHKNegativ TINYINT,
			
			@IdOld        INT,
			@ActionOld tinyint ,
			@CodeOld decimal(20,2) ,
			@DescriptionOld nvarchar(max),
			@PricingReferenceIdOld int ,
			@WarehouseIdOld INT ,
			@TimeBucketIdOld INT,
			@StoreTypesIdOld INT ,
			@StatusOld tinyint ,
			@RegistrationDateOld DATETIME ,
			@SenderReciverOld int ,
			@HardCopyNoOld NVARCHAR(10) ,
			@ReferenceTypeOld NVARCHAR(100) ,
			@ReferenceNoOld NVARCHAR(100) ,
			@ReferenceDateOld DATETIME ,
			@UserCreatorIdOld int
			
	SELECT TOP(1) @Id         = Id,@Action  = Action,@Code  = Code,@Description  = Description,@PricingReferenceId  = PricingReferenceId,
					@WarehouseId  = WarehouseId,@TimeBucketId =TimeBucketId,@StoreTypesId  = StoreTypesId,@Status  = Status,@RegistrationDate  = RegistrationDate,
					@SenderReciver  = SenderReciver,@HardCopyNo  = HardCopyNo,@ReferenceType  = ReferenceType,
					@ReferenceNo  = ReferenceNo,@ReferenceDate  = ReferenceDate,@UserCreatorId  = UserCreatorId	FROM  INSERTED 
					
	SELECT TOP(1) @IdOld         = Id,@ActionOld  = Action,@CodeOld  = Code,
					@DescriptionOld  = Description,@PricingReferenceIdOld  = PricingReferenceId,@WarehouseIdOld  = WarehouseId,
					@TimeBucketIdOld =TimeBucketId, @StoreTypesIdOld  = StoreTypesId,@StatusOld  = Status,
					@RegistrationDateOld  = RegistrationDate,@SenderReciverOld  = SenderReciver,
					@HardCopyNoOld  = HardCopyNo,@ReferenceTypeOld  = ReferenceType,@ReferenceNoOld  = ReferenceNo,
					@ReferenceDateOld  = ReferenceDate,@UserCreatorIdOld  = UserCreatorId	FROM  DELETED
       
        IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
	 		 
		SET @CHKNegativ=dbo.CheckNegetiveWarehouse(@WarehouseId,0,0,@TimeBucketId)
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@Id), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')
END TRY
BEGIN CATCH

		 IF ERROR_MESSAGE() = 'Warehousemanfinist'
		    RAISERROR(
		        N'@عمليات امکان پذير نيست؛ زيرا منجر به منفي شدن انبار ميشود ',
		        16,
		        1,
		        '500'
		    )
		   
	EXEC [ErrorHandling]
END CATCH
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[DeleteTransaction]', 'TR') IS NOT NULL
    DROP TRIGGER [DeleteTransaction];
GO
CREATE TRIGGER [DeleteTransaction]
ON [Transactions]
   AFTER DELETE
AS
BEGIN TRY
	DECLARE @Id        INT,
			@Action tinyint ,
			@Code decimal(20,2) ,
			@Description nvarchar(max),
			@PricingReferenceId int ,
			@WarehouseId INT ,
			@TimeBucketId INT,
			@StoreTypesId INT ,
			@Status tinyint ,
			@RegistrationDate DATETIME ,
			@SenderReciver int ,
			@HardCopyNo NVARCHAR(10) ,
			@ReferenceType NVARCHAR(100) ,
			@ReferenceNo NVARCHAR(100) ,
			@ReferenceDate DATETIME ,
			@UserCreatorId int,
			@CHKNegativ TINYINT
	SELECT TOP(1) @Id         = Id,@Action  = Action,@Code  = Code,@Description  = Description,@PricingReferenceId  = PricingReferenceId,
					@WarehouseId  = WarehouseId,@TimeBucketId =TimeBucketId,@StoreTypesId  = StoreTypesId,@Status  = Status,@RegistrationDate  = RegistrationDate,
					@SenderReciver  = SenderReciver,@HardCopyNo  = HardCopyNo,@ReferenceType  = ReferenceType,
					@ReferenceNo  = ReferenceNo,@ReferenceDate  = ReferenceDate,@UserCreatorId  = UserCreatorId	FROM    DELETED
	   
	   IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
	
		SET @CHKNegativ=dbo.CheckNegetiveWarehouse(@WarehouseId,0,0,@TimeBucketId)
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@Id), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')
END TRY
BEGIN CATCH
		 IF ERROR_MESSAGE() = 'Warehousemanfinist'
		    RAISERROR(
		        N'@عمليات امکان پذير نيست؛ زيرا موجودي انبار نبايد منفي شود ',
		        16,
		        1,
		        '500'
		    )
	EXEC [ErrorHandling]
END CATCH
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[UpdateTransactionItems]', 'TR') IS NOT NULL
    DROP TRIGGER [UpdateTransactionItems];
GO
CREATE TRIGGER [UpdateTransactionItems]
ON [TransactionItems]
   AFTER INSERT, UPDATE
AS
BEGIN TRY
	DECLARE @TransactionId        INT,
			@Id int =Null,
			@GoodId int = null,
			@QuantityUnitId INT,
			@QuantityAmount DECIMAL(20,3) = 0,
			@Description nvarchar(max) =NULL,
			@RowVersion SMALLINT,
			
			@WarehouseId INT,
			@Action TINYINT,
			@CHKNegativ TINYINT,
			@CHKOverFlow TINYINT,
			@TotalQuantityAmount DECIMAL(20,3),
			@PricedQuantityAmount DECIMAL(20,3),
			
			@TransactionIdOld        INT,
			@IdOld int =Null,
			@GoodIdOld int = null,
			@QuantityUnitIdOld INT,
			@QuantityAmountOld DECIMAL(20,3) = 0,
			@DescriptionOld nvarchar(max) =NULL,
			@RowVersionOld SMALLINT,
			@TimeBucketId INT
			
	SELECT TOP(1) @TransactionId=TransactionId, @Id=Id,@GoodId=GoodId,@QuantityUnitId=QuantityUnitId,
	              @QuantityAmount=QuantityAmount,@Description=Description,@RowVersion=[RowVersion]	FROM  INSERTED 
	SELECT TOP(1) @TransactionIdOld=TransactionId, @IdOld=Id,@GoodIdOld=GoodId,@QuantityUnitIdOld=QuantityUnitId,
	              @QuantityAmountOld=QuantityAmount,@DescriptionOld=Description,@RowVersionOld=RowVersion	FROM  DELETED

	    SELECT TOP(1) @WarehouseId=t.WarehouseId,@Action=t.[Action] FROM Transactions t WHERE t.Id=@TransactionId
	    SELECT @TimeBucketId=TimeBucketId FROM Transactions t WHERE t.Id=@TransactionId
	 	IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
		SET @CHKNegativ=dbo.CheckNegetiveWarehouseValue(@WarehouseId,@GoodId ,@TimeBucketId ,@RowVersion,@Action,@TransactionId,@QuantityAmount,@QuantityUnitId,0)
		
		IF (SELECT TOP(1)u.IsCurrency FROM Units u WHERE u.Id=@QuantityUnitId)=1
		BEGIN
			RAISERROR(N'QuantityUnitNotUseForAmount', 16, 1)
			RETURN
		END
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		SET @CHKNegativ=dbo.CheckNegetiveWarehouse(@WarehouseId,@GoodId,@TimeBucketId ,@QuantityAmount)
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		SET @CHKOverFlow=dbo.CheckOverFlowQuantityPricing(@Action,@Id,@TransactionId,@QuantityUnitId)
		IF @CHKOverFlow=0
		BEGIN
			RAISERROR(N'OverFlowPriceing', 16, 1)
			RETURN
		END
		SET @TotalQuantityAmount=ISNULL((SELECT SUM(ti.QuantityAmount) FROM TransactionItems ti WHERE ti.TransactionId=@TransactionId),0)
		SET @PricedQuantityAmount=ISNULL((SELECT SUM(tip.QuantityAmount) FROM TransactionItemPrices tip WHERE tip.TransactionId=@TransactionId),0)
		IF @TotalQuantityAmount=@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 3
		ELSE IF @TotalQuantityAmount<>@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 2
			
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@TransactionId), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')
END TRY
BEGIN CATCH

		 IF ERROR_MESSAGE() = 'Warehousemanfinist'
		    RAISERROR(
		        N'@عمليات امکان پذير نيست؛ زيرا موجودي انبار نبايد منفي شود ',
		        16,
		        1,
		        '500'
		    )
		 IF ERROR_MESSAGE() = 'QuantityUnitNotUseForAmount'
		    RAISERROR(
		        N'@واحد که براي مقدار انتخاب ميشود فقط بايد از نوع شمارشي باشد ',
		        16,
		        1,
		        '500'
		    )
		 DECLARE @MSG NVARCHAR(256)
		 IF ERROR_MESSAGE() = 'OverFlowPriceing'
		 BEGIN
		 	SET @MSG= CASE WHEN @Action=1 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار رسيد شده است'
         				WHEN @Action=2 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار حواله شده است'
         		      END
         	RAISERROR(
		       @MSG,
		        16,
		        1,
		        '500'
		    )
		 END  
	EXEC [ErrorHandling]
END CATCH
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[DeleteTransactionItems]', 'TR') IS NOT NULL
    DROP TRIGGER [DeleteTransactionItems];
GO
CREATE TRIGGER [DeleteTransactionItems]
ON [TransactionItems]
   AFTER DELETE
AS
BEGIN TRY
	DECLARE @TransactionId        INT,
			@Id int =Null,
			@GoodId int = null,
			@QuantityUnitId INT,
			@QuantityAmount DECIMAL(20,3) = 0,
			@Description nvarchar(max) =NULL,
			@RowVersion SMALLINT,
			@WarehouseId INT,
			@Action TINYINT,
			@CHKNegativ TINYINT,
			@CHKOverFlow TINYINT,
			@TimeBucketId INT,
			@TotalQuantityAmount DECIMAL(20,3),
			@PricedQuantityAmount DECIMAL(20,3)

			
	SELECT TOP(1) @TransactionId=TransactionId, @Id=Id,@GoodId=GoodId,@QuantityUnitId=QuantityUnitId,
	              @QuantityAmount=QuantityAmount,@Description=Description,@RowVersion=[RowVersion]	FROM   DELETED
	   
	  SELECT TOP(1) @WarehouseId=t.WarehouseId,@Action=t.[Action] FROM Transactions t WHERE t.Id=@TransactionId
	   SELECT @TimeBucketId=TimeBucketId FROM Transactions t WHERE t.Id=@TransactionId
	  IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
	 		 
	  SET @CHKNegativ=dbo.CheckNegetiveWarehouseValue(@WarehouseId,@GoodId ,@TimeBucketId ,@RowVersion,@Action,@TransactionId,@QuantityAmount,@QuantityUnitId,0)
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		SET @CHKNegativ=dbo.CheckNegetiveWarehouse(@WarehouseId,@GoodId,@TimeBucketId ,@QuantityAmount)
		IF @CHKNegativ=0
		BEGIN
			RAISERROR(N'Warehousemanfinist', 16, 1)
			RETURN
		END
		SET @CHKOverFlow=dbo.CheckOverFlowQuantityPricing(@Action,@Id,@TransactionId,@QuantityUnitId)
		IF @CHKOverFlow=0
		BEGIN
			RAISERROR(N'OverFlowPriceing', 16, 1)
			RETURN
		END
		SET @TotalQuantityAmount=ISNULL((SELECT SUM(ti.QuantityAmount) FROM TransactionItems ti WHERE ti.TransactionId=@TransactionId),0)
		SET @PricedQuantityAmount=ISNULL((SELECT SUM(tip.QuantityAmount) FROM TransactionItemPrices tip WHERE tip.TransactionId=@TransactionId),0)
		IF @TotalQuantityAmount=@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 3
		ELSE IF @TotalQuantityAmount<>@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 2
			
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@TransactionId), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')
END TRY
BEGIN CATCH
		 IF ERROR_MESSAGE() = 'Warehousemanfinist'
		    RAISERROR(
		        N'@عمليات امکان پذير نيست؛ زيرا موجودي انبار نبايد منفي شود ',
		        16,
		        1,
		        '500'
		    )
		 DECLARE @MSG NVARCHAR(256)
		 IF ERROR_MESSAGE() = 'OverFlowPriceing'
		 BEGIN
		 	SET @MSG= CASE WHEN @Action=1 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار رسيد شده است'
         				WHEN @Action=2 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار حواله شده است'
         				WHEN @Action=3 THEN
         				N'@ مقدار فاکتور قيمت گذاري شده بيشتر از مقدار حواله شده است'
         		      END
         	RAISERROR(
		       @MSG,
		        16,
		        1,
		        '500'
		    )
		 END
	EXEC [ErrorHandling]
END CATCH
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[UpdateTransactionItemPrices]', 'TR') IS NOT NULL
    DROP TRIGGER [UpdateTransactionItemPrices];
GO
CREATE TRIGGER [UpdateTransactionItemPrices]
ON [TransactionItemPrices]
   AFTER INSERT, UPDATE
AS
BEGIN TRY
	DECLARE @Id                      int ,
			@RowVersion              smallint ,
			@TransactionId               int ,
			@TransactionItemId           int ,
			@Description             nvarchar(max),
			@QuantityUnitId           INT ,
			@QuantityAmount          DECIMAL(20,3) ,
			@PriceUnitId             INT ,
			@Fee                     DECIMAL(20,3) ,
			@RegistrationDate         DATETIME,
			@QuantityAmountUseFIFO   DECIMAL(20,3) ,
			@IdOld                      int ,
			@RowVersionOld           	smallint ,
			@TransactionIdOld            	int ,
			@TransactionItemIdOld        	int ,
			@DescriptionOld          	nvarchar(max),
			@QuantityUnitIdOld           INT ,
			@QuantityAmountOld       	DECIMAL(20,3) ,
			@PriceUnitIdOld          	INT ,
			@FeeOld                  	DECIMAL(20,3) ,
			@RegistrationDateOld         DATETIME,
			@QuantityAmountUseFIFOOld	DECIMAL(20,3) ,
			@TotalQuantityAmount DECIMAL(20,3),
			@PricedQuantityAmount DECIMAL(20,3),
			
			@WarehouseId INT,
			@Action TINYINT,
			@CHKOverFlow TINYINT,
			@GoodId      INT
			
	SELECT TOP(1) @Id  =Id , @RowVersion  =RowVersion , @TransactionId  =TransactionId , @TransactionItemId  =TransactionItemId,
					@Description  =DESCRIPTION ,@QuantityUnitId=QuantityUnitId, @QuantityAmount  = QuantityAmount , @PriceUnitId  =PriceUnitId ,
					@Fee  =Fee ,@RegistrationDate=RegistrationDate, @QuantityAmountUseFIFO  =QuantityAmountUseFIFO	FROM  INSERTED 
	SELECT TOP(1) @IdOld=Id,@RowVersionOld	=RowVersion,@TransactionIdOld	=TransactionId,@TransactionItemIdOld	=TransactionItemId,
					@DescriptionOld	=Description,@QuantityUnitIdOld=QuantityUnitId,@QuantityAmountOld	=QuantityAmount,@PriceUnitIdOld	=PriceUnitId,
					@FeeOld	=Fee,@RegistrationDateOld=RegistrationDate,@QuantityAmountUseFIFOOld	=QuantityAmountUseFIFO	FROM  DELETED

	    SELECT TOP(1) @WarehouseId=t.WarehouseId,@Action=t.[Action] FROM Transactions t WHERE t.Id=@TransactionId
	    SELECT TOP(1) @GoodId=ti.GoodId FROM TransactionItems ti WHERE ti.Id=@TransactionItemId
	 	IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
		SET @CHKOverFlow=dbo.CheckOverFlowQuantityPricing(@Action,@TransactionItemId,@TransactionId,@QuantityUnitId)
		IF NOT EXISTS (SELECT TOP(1)u.IsBaseCurrency
		      FROM Units u WHERE u.IsBaseCurrency = 1)
		BEGIN
			RAISERROR(N'NeedBaseCurrency', 16, 1)
			RETURN
		END
		IF (SELECT TOP(1)u.IsCurrency FROM Units u WHERE u.Id=@QuantityUnitId)=1
		BEGIN
			RAISERROR(N'QuantityUnitNotUseForAmount', 16, 1)
			RETURN
		END
		IF (SELECT TOP(1)u.IsCurrency FROM Units u WHERE u.Id=@PriceUnitId)=0
		BEGIN
			RAISERROR(N'@PriceUnitNotUseForMony', 16, 1)
			RETURN
		END
		IF @Action=1 AND  @QuantityAmountUseFIFO>0 AND (@QuantityAmount<@QuantityAmountUseFIFO OR @Fee<>@FeeOld)
		BEGIN
			RAISERROR(N'ThisRecordUseForIssueAndNotChange', 16, 1)
			RETURN
		END
		IF @CHKOverFlow=0
		BEGIN
			RAISERROR(N'OverFlowPriceing', 16, 1)
			RETURN
		END
		IF @Action=1 AND @QuantityAmountUseFIFO>@QuantityAmount
		BEGIN
			RAISERROR(N'ThisRecordUseForFIFOSystem', 16, 1)
			RETURN
		END	
		IF @Action=1 
		   AND @QuantityAmountUseFIFO>@QuantityAmount 
		   AND EXISTS (SELECT TOP(1) t.Id FROM Transactions t WHERE t.PricingReferenceId=@TransactionId)
		BEGIN
			RAISERROR(N'ValidAnyRecordUsingThisReference',16,1)
			RETURN
		END
		IF @Action=2 AND (EXISTS (SELECT TOP(1) tip.Id
		                         FROM TransactionItems ti
								 INNER JOIN TransactionItemPrices tip
									ON tip.TransactionItemId = ti.Id
								 INNER JOIN Transactions t 
									ON t.Id = ti.TransactionId
		                         WHERE t.Action=2 
		                               AND tip.Id>=@Id
									   AND tip.RowVersion>@RowVersion
								       AND ti.GoodId=@GoodId
								       AND t.WarehouseId=@WarehouseId
								)
								OR
								EXISTS(SELECT TOP(1) t.Id 
								       FROM Transactions t WHERE t.PricingReferenceId=@TransactionId)
							)
		BEGIN
			RAISERROR(N'ValidAnyRecordAfterThisRecordAndUseFIFO', 16, 1)
			RETURN
		END
		SET @TotalQuantityAmount=ISNULL((SELECT SUM(ti.QuantityAmount) FROM TransactionItems ti WHERE ti.TransactionId=@TransactionId),0)
		SET @PricedQuantityAmount=ISNULL((SELECT SUM(tip.QuantityAmount) FROM TransactionItemPrices tip WHERE tip.TransactionId=@TransactionId),0)
		IF @TotalQuantityAmount=@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 3
		ELSE IF @TotalQuantityAmount<>@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 2
			
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@TransactionId), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')			
END TRY
BEGIN CATCH
         DECLARE @MSG NVARCHAR(256)
         IF ERROR_MESSAGE() = 'NeedBaseCurrency'
		    RAISERROR(
		        N'@براي قيمت گذاري حتما بايد ارز پايه مشخص باشد ',
		        16,
		        1,
		        '500'
		    )
         IF ERROR_MESSAGE() = 'QuantityUnitNotUseForAmount'
		    RAISERROR(
		        N'@واحد که براي مقدار انتخاب ميشود فقط بايد از نوع شمارشي باشد ',
		        16,
		        1,
		        '500'
		    )
		  IF ERROR_MESSAGE() = 'PriceUnitNotUseForMony'
		    RAISERROR(
		        N'@واحد که براي پول انتخاب ميشود فقط بايد از نوع ارزشي باشد ',
		        16,
		        1,
		        '500'
		    )
		 IF ERROR_MESSAGE() = 'OverFlowPriceing'
		 BEGIN
		 	SET @MSG= CASE WHEN @Action=1 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار رسيد شده است'
         				WHEN @Action=2 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار حواله شده است'
         		      END
         	RAISERROR(
		       @MSG,
		        16,
		        1,
		        '500'
		    )
		 END
		 IF ERROR_MESSAGE() = 'ThisRecordUseForFIFOSystem' OR ERROR_MESSAGE() = 'ThisRecordUseForFIFOSystem'
		    RAISERROR(
		        N'@اين رکورد در قيمت گذاري حواله استفاده شده و قابل تغيير نمي باشد ',
		        16,
		        1,
		        '500'
		    )
		 IF ERROR_MESSAGE() = 'ValidAnyRecordAfterThisRecordAndUseFIFO'
		    RAISERROR(
		        N'@بعد از اين حواله، حواله يا فاکتور ديگري با همين مشخصات قيمت گذاري شده و نبايد اين حواله را تغيير دهيد ',
		        16,
		        1,
		        '500'
		    )
		    IF ERROR_MESSAGE() = 'ValidAnyRecordUsingThisReference'
		    RAISERROR(
		        N'@اين رسيد بعنوان مرجع استفاده شده و نبايد تغيير داده شود ',
		        16,
		        1,
		        '500'
		    )
		    
	EXEC [ErrorHandling]
END CATCH
GO
------------------------------------------------------------------------------------
IF OBJECT_ID('[DeleteTransactionItemPrices]', 'TR') IS NOT NULL
    DROP TRIGGER [DeleteTransactionItemPrices];
GO
CREATE TRIGGER [DeleteTransactionItemPrices]
ON [TransactionItemPrices]
   AFTER DELETE
AS
BEGIN TRY
	DECLARE @Id                      int ,
			@RowVersion              smallint ,
			@TransactionId               int ,
			@TransactionItemId           int ,
			@Description             nvarchar(max),
			@QuantityUnitId           INT ,
			@QuantityAmount          DECIMAL(20,3) ,
			@PriceUnitId             INT ,
			@Fee                     DECIMAL(20,3) ,
			@RegistrationDate         DATETIME,
			@QuantityAmountUseFIFO   DECIMAL(20,3) ,
			
			
			@WarehouseId INT,
			@Action TINYINT,
			@CHKOverFlow TINYINT,
			@GoodId      INT,
			@TotalQuantityAmount DECIMAL(20,3),
			@PricedQuantityAmount DECIMAL(20,3)
			
    	SELECT TOP(1) @Id  =Id , @RowVersion  =RowVersion , @TransactionId  =TransactionId , @TransactionItemId  =TransactionItemId,
					@Description  =DESCRIPTION ,@QuantityUnitId=QuantityUnitId, @QuantityAmount  = QuantityAmount , @PriceUnitId  =PriceUnitId ,
					@Fee  =Fee ,@RegistrationDate=RegistrationDate, @QuantityAmountUseFIFO  =QuantityAmountUseFIFO FROM   DELETED
	   
	  SELECT TOP(1) @WarehouseId=t.WarehouseId,@Action=t.[Action] FROM Transactions t WHERE t.Id=@TransactionId
	  SELECT TOP(1) @GoodId=ti.GoodId
			  FROM TransactionItems ti WHERE ti.Id=@TransactionItemId
			  
	 	IF (SELECT w.[Active] FROM Warehouse w WHERE w.Id=@WarehouseId)=0
	 		 RAISERROR(N'@انباري که درحال عمليات روي آن هستيد، غير فعال است ', 16, 1, '500')
		SET @CHKOverFlow=dbo.CheckOverFlowQuantityPricing(@Action,@TransactionItemId,@TransactionId,@QuantityUnitId)
		IF @CHKOverFlow=0
		BEGIN
			RAISERROR(N'OverFlowPriceing', 16, 1)
			RETURN
		END
		IF @Action=1 AND @QuantityAmountUseFIFO>0
		BEGIN
			RAISERROR(N'ThisRecordUseForFIFOSystem', 16, 1)
			RETURN
		END
		IF @Action=1 
		   AND @QuantityAmountUseFIFO>@QuantityAmount 
		   AND EXISTS (SELECT TOP(1) t.Id FROM Transactions t WHERE t.PricingReferenceId=@TransactionId)
		BEGIN
			RAISERROR(N'ValidAnyRecordUsingThisReference',16,1)
			RETURN
		END
		IF @Action=2 AND (EXISTS (SELECT TOP(1) tip.Id
		                         FROM TransactionItems ti
								 INNER JOIN TransactionItemPrices tip
									ON tip.TransactionItemId = ti.Id
								 INNER JOIN Transactions t 
									ON t.Id = ti.TransactionId
		                         WHERE t.Action=2 
		                               AND tip.Id>=@Id
									   AND tip.RowVersion>@RowVersion
								       AND ti.GoodId=@GoodId
								       AND t.WarehouseId=@WarehouseId
								)
								OR
								EXISTS(SELECT TOP(1) t.Id 
								       FROM Transactions t WHERE t.PricingReferenceId=@TransactionId)
							)
		BEGIN
			RAISERROR(N'ValidAnyRecordAfterThisRecordAndUseFIFO', 16, 1)
			RETURN
		END
		IF @Action=2 AND EXISTS (SELECT TOP(1) tip.Id
		                         FROM TransactionItems ti
								 INNER JOIN TransactionItemPrices tip
									ON tip.TransactionItemId = ti.Id
								 INNER JOIN Transactions t 
									ON t.Id = ti.TransactionId
		                         WHERE t.Action=2 
		                               AND tip.Id>=@Id
									   AND tip.RowVersion>@RowVersion
								       AND ti.GoodId=@GoodId
								       AND t.WarehouseId=@WarehouseId
		)
		BEGIN
			RAISERROR(N'ValidAnyRecordAfterThisRecordAndUseFIFO', 16, 1)
			RETURN
		END
		SET @TotalQuantityAmount=ISNULL((SELECT SUM(ti.QuantityAmount) FROM TransactionItems ti WHERE ti.TransactionId=@TransactionId),0)
		SET @PricedQuantityAmount=ISNULL((SELECT SUM(tip.QuantityAmount) FROM TransactionItemPrices tip WHERE tip.TransactionId=@TransactionId),0)
		IF @TotalQuantityAmount=@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 3
		ELSE IF @TotalQuantityAmount<>@PricedQuantityAmount
			UPDATE Transactions SET [Status] = 2
			
		IF ISNULL((SELECT TOP(1)t.Status FROM Transactions t WHERE t.Id=@TransactionId), 0)=4
			RAISERROR(N'@اين ديتا در سيستم مالي سند خورده و هيچ تغييري نمي توان در آن ايجاد کرد',16,1,'500')
END TRY
BEGIN CATCH
		DECLARE @MSG NVARCHAR(256)
         
		 IF ERROR_MESSAGE() = 'OverFlowPriceing'
		 BEGIN
		 	SET @MSG= CASE WHEN @Action=1 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار رسيد شده است'
         				WHEN @Action=2 THEN
         				N'@ مقدار قيمت گذاري شده بيشتر از مقدار حواله شده است'
         		      END
         	RAISERROR(
		       @MSG,
		        16,
		        1,
		        '500'
		    )
		 END
		 IF ERROR_MESSAGE() = 'ThisRecordUseForFIFOSystem'
		    RAISERROR(
		        N'@اين رکورد در قيمت گذاري حواله استفاده شده و قابل حذف نمي باشد ',
		        16,
		        1,
		        '500'
		    )
		IF ERROR_MESSAGE() = 'ValidAnyRecordAfterThisRecordAndUseFIFO'
		    RAISERROR(
		        N'@بعد از اين حواله، حواله يا فاکتور ديگري با همين مشخصات قيمت گذاري شده و نبايد اين حواله را تغيير دهيد ',
		        16,
		        1,
		        '500'
		    )
		    IF ERROR_MESSAGE() = 'ValidAnyRecordUsingThisReference'
		    RAISERROR(
		        N'@اين رسيد بعنوان مرجع استفاده شده و نبايد تغيير داده شود ',
		        16,
		        1,
		        '500'
		    )
	EXEC [ErrorHandling]
END CATCH
GO
----------------------------------------------------------------------------
	RAISERROR(N'تریگرها با موفقیت ایجاد شدند.', 0, 1) WITH NOWAIT
GO