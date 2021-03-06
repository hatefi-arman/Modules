USE MiniStock 
GO 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------------------------
IF OBJECT_ID ( '[CurrentTime]', 'FN' ) IS NOT NULL 
DROP FUNCTION [CurrentTime];
GO
CREATE FUNCTION [CurrentTime]()
RETURNS VARCHAR(10)
--WITH ENCRYPTION
AS
 BEGIN
 	RETURN (SELECT (cast(DATEPART(hour, GETDATE())AS NVARCHAR(3))+':'+cast(DATEPART(minute, GETDATE())AS NVARCHAR(3))+':'+cast(DATEPART(SECOND, GETDATE())AS NVARCHAR(3)))AS CTime)
 END
GO
-----------------------------برای تراز بندی با کاراکتر دلخواه می باشدleft pad
IF OBJECT_ID ( '[LPAD]', 'FN' ) IS NOT NULL 
DROP FUNCTION [LPAD];
GO
CREATE FUNCTION [LPAD]
		(
		@SourceString VARCHAR(MAX),
        @FinalLength  INT,
        @PadChar      VARCHAR(1)
		)
RETURNS VARCHAR(MAX)
--WITH ENCRYPTION
AS
 BEGIN
 	IF @FinalLength - Len(@SourceString)<0
 		RETURN (select(LEFT(@SourceString,@FinalLength)))
    RETURN (select(Replicate(@PadChar,@FinalLength - Len(@SourceString)) + @SourceString))
 END
--SELECT [Galaxy].LPAD('012345678',10,'&')AS name 
GO
-----------------------------برای تراز بندی با کاراکتر دلخواه می باشدright pad
IF OBJECT_ID ( '[RPAD]', 'FN' ) IS NOT NULL 
DROP FUNCTION [RPAD];
GO
CREATE FUNCTION [RPAD]
		(
		@SourceString VARCHAR(MAX),
        @FinalLength  INT,
        @PadChar      VARCHAR(1)
		)
RETURNS VARCHAR(MAX)
--WITH ENCRYPTION
AS
 BEGIN
 	IF @FinalLength - Len(@SourceString)<0
 		RETURN (RIGHT(@SourceString,@FinalLength))
 	RETURN (@SourceString + Replicate(@PadChar,@FinalLength - Len(@SourceString)))
 END
--SELECT [Galaxy].RPAD('1234567890',10,' ')AS name 
GO

-- مقدار فعلی موجودی
IF OBJECT_ID ( 'GetMojodi', 'FN' ) IS NOT NULL 
DROP FUNCTION GetMojodi;
GO
CREATE FUNCTION GetMojodi
(
	@GoodId INT=NULL,
	@WarehouseId INT,
	@TimeBucketId INT ,
	@RegistrationDate DATETIME=NULL,
	@Action TINYINT =NULL,
	@TransactionId INT=NULL,
	@RowVersion SMALLINT=NULL,
	@QuantityUnitId INT = NULL,
	@QuantityAmount DECIMAL(20,3)=NULL
	
)
RETURNS DECIMAL(20,3)
--WITH ENCRYPTION
AS 
BEGIN
	DECLARE @ActionType AS NVARCHAR(5)='12   '
	declare @retValue DECIMAL(20,3)
	SET @retValue =(SELECT SUM(t.s) AS CurrentMojodi
					FROM   (
							   SELECT 
							   SUM(CASE WHEN PATINDEX('%'+cast(t.Action AS NVARCHAR(5))+'%', @ActionType)=0 
								   THEN  0 ELSE
							        ti.QuantityAmount *(CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END) END) AS s
							   FROM TransactionItems ti
									  INNER  JOIN Transactions t
										   ON t.Id = ti.TransactionId
							   WHERE  ti.GoodId=@GoodId
							          AND ti.QuantityUnitId=@QuantityUnitId
									  AND t.WarehouseId=@WarehouseId
									  AND t.TimeBucketId=@TimeBucketId
									  AND ((@TransactionId=0 OR @TransactionId IS NULL OR @RowVersion=0 OR @RowVersion IS NULL OR (CONVERT(NVARCHAR(20),@RegistrationDate,(120))) IS NULL) OR 
									      (
									          CONVERT(NVARCHAR(20), t.RegistrationDate, (120))
									          + dbo.RPAD(
									                        CAST(
									                            CASE 
									                                 WHEN t.Action 
									                                      = 1 THEN 
									                                      2
									                                 WHEN t.Action 
									                                      = 2 THEN 
									                                      1
									                                 ELSE t.Action
									                            END AS NVARCHAR(3) ), ' ', 3 ) 
									          + dbo.RPAD(CAST(t.ID AS NVARCHAR(15)), ' ', 15) 
												+ dbo.RPAD(CAST(ti.RowVersion AS NVARCHAR(10)), ' ', 10)
																						  ) <=(
																								  CONVERT(NVARCHAR(20), @RegistrationDate, (120)) 
																								  + dbo.RPAD(CAST(@Action AS NVARCHAR(3)), ' ', 3)
																								  + dbo.RPAD(CAST(@TransactionId AS NVARCHAR(15)), ' ', 15) 
																								  + dbo.RPAD(CAST(@RowVersion AS NVARCHAR(10)), ' ', 10)
																							  ))
																			   GROUP BY
																					  t.Action
																		   ) AS t
				  )
	return ISNULL(@retValue,0)
END;
GO
------------------------------------------------------------------------------------
IF OBJECT_ID ( 'CheckNegetiveWarehouseValue', 'FN' ) IS NOT NULL  
  DROP FUNCTION CheckNegetiveWarehouseValue;
GO
	CREATE FUNCTION CheckNegetiveWarehouseValue
	(
		@WarehouseId INT,
		@GoodId INT,
		@TimeBucketId INT,
		@RowVersion SMALLINT= NULL,
		@Action tinyint= NULL,
		@TransactionId decimal(20,2)= NULL,
        @QuantityAmount DECIMAL(13, 3)= NULL,
		@QuantityUnitId INT = NULL,
		@CallFromProcedure BIT =0
	)
	RETURNS  TINYINT
	--WITH ENCRYPTION
	AS 
	BEGIN
		declare @retValue TINYINT
		DECLARE @RegistrationDate DATETIME
		DECLARE @CurrentMojodi DECIMAL(20,3)
		DECLARE @Current_Value DECIMAL(20, 3)
		
			
			SET @RegistrationDate=(SELECT t.RegistrationDate FROM Transactions t
							  WHERE t.Action=@Action AND t.Id=@TransactionId AND t.WarehouseId=@WarehouseId AND t.TimeBucketId=@TimeBucketId)
			SET @CurrentMojodi=dbo.GetMojodi(@GoodId, @WarehouseId,@TimeBucketId,@RegistrationDate,@Action,@TransactionId,@RowVersion,@QuantityUnitId,@QuantityAmount)
			
			set @Current_Value=(SELECT ti.QuantityAmount
			                    FROM   TransactionItems ti
			                           INNER JOIN Transactions t
			                                ON  t.Id = ti.TransactionId
			                    WHERE  t.Action = @Action
			                           AND ti.TransactionId = @TransactionId
			                           AND t.WarehouseId = @WarehouseId
			                           AND ti.GoodId = @GoodId
			                           AND t.TimeBucketId=@TimeBucketId
			                           AND ti.RowVersion = @RowVersion)			
			
			IF @CallFromProcedure=1 AND   @Action=2 
				SET @CurrentMojodi=@CurrentMojodi+ISNULL(@Current_Value,0)-@QuantityAmount	
			
			IF @CallFromProcedure=1 AND @Action=1 
				SET @CurrentMojodi=@CurrentMojodi-ISNULL(@Current_Value,0)+@QuantityAmount	
			
			IF @CurrentMojodi<0
			 BEGIN
					SET @retValue=0
			 END
			ELSE 
				SET @retValue=1	--منفی نشده است 
		RETURN @retValue	
	END
GO
------------------------------------------------------------------------------------
IF OBJECT_ID ( 'CheckNegetiveWarehouse', 'FN' ) IS NOT NULL  
  DROP FUNCTION CheckNegetiveWarehouse;
GO
CREATE FUNCTION CheckNegetiveWarehouse
(
	@WarehouseId INT,
	@GoodId INT,
	@TimeBucketId INT,
	@QuantityAmount DECIMAL(20,3)=0 
)
	RETURNS TINYINT
	--WITH ENCRYPTION
	AS 
	BEGIN
		DECLARE @retValue TINYINT
			
			DECLARE @value NVARCHAR(MAX)=''
			SET @value=dbo.negWarehouse(@WarehouseId,@GoodId,@TimeBucketId) 
			IF not @value=''
			BEGIN
				SET @retValue=0
			END	
			ELSE 
				SET @retValue=1	--منفي نشده است 
		RETURN @retValue	
	END
GO--print dbo.CheckNegetiveWarehouse(2,1,1,0)
------------------------------------------------------
IF OBJECT_ID ( 'negWarehouse', 'FN' ) IS NOT NULL 
DROP FUNCTION negWarehouse;
GO
CREATE FUNCTION negWarehouse
(
	@WarehouseId INT,
	@GoodId INT,
	@TimeBucketId INT
)
RETURNS NVARCHAR(MAX)
--WITH ENCRYPTION
AS 
BEGIN	
	IF @WarehouseId IS NULL 
		SET @WarehouseId=0
		
	IF @GoodId IS NULL 
		SET @GoodId=0
	IF @TimeBucketId IS NULL 
		SET @TimeBucketId=0	
		
	DECLARE @value VARCHAR(MAX),
	        @tmpWarehouseId INT,
	        @tmpGoodId int,
	        @c DECIMAL(20,3)
	SET @value=''
	SET @tmpGoodId=0
	SET @tmpWarehouseId=0
	SET @TimeBucketId=0
	SET @c=0
	
	SELECT @c=CASE WHEN @tmpGoodId<>t.GoodId OR @tmpWarehouseId<>t.WarehouseId THEN t.sumValue ELSE @c+t.sumValue END,
		   @tmpGoodId=t.GoodId,@tmpWarehouseId=t.WarehouseId,
		   @value=CASE WHEN @c<0 THEN (@value+CAST(t.Action AS NVARCHAR(1))+';'+CAST(t.Id AS NVARCHAR(20))+'@'+
						 cast(t.WarehouseId AS NVARCHAR(13))+'#'+CONVERT(NVARCHAR(20),t.RegistrationDate,(120))+'$'+Cast(t.GoodId AS NVARCHAR(10))+CHAR(13)+char(10)) ELSE @value END
	FROM
	(SELECT  t.Action,t.Id,t.WarehouseId,t.RegistrationDate,ti.GoodId,
			 ti.QuantityAmount * (CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END) sumValue      
			 ,ROW_NUMBER() OVER(PARTITION BY ti.GoodId
				  			    ORDER BY t.WarehouseId,ti.GoodId, t.RegistrationDate, t.Action,
								         t.Id )RNo
	 FROM Transactions t
		   INNER JOIN TransactionItems ti
				ON  t.Id = ti.TransactionId
	 WHERE @WarehouseId=CASE WHEN @WarehouseId=0 THEN 0 ELSE t.WarehouseId END AND
		   @GoodId=CASE WHEN @GoodId=0 THEN 0 ELSE ti.GoodId END AND
		   (t.Action=1 OR t.Action=2)		
	)t      
	ORDER BY WarehouseId,GoodId,RegistrationDate,Action,Id	
	RETURN @Value 
END;
GO
------------------------------------------------------------------------------------
IF OBJECT_ID ( 'CheckOverFlowQuantityPricing', 'FN' ) IS NOT NULL  
  DROP FUNCTION CheckOverFlowQuantityPricing;
GO
CREATE FUNCTION CheckOverFlowQuantityPricing
(
	@Action TINYINT,
	@TransactionItemId INT,
	@TransactionId INT,
	@QuantityUnitId  INT 
)
	RETURNS TINYINT
	--WITH ENCRYPTION
	AS 
	BEGIN
		DECLARE @retValue TINYINT,
		        @value DECIMAL(20,3)=0,
		        @QuantityAmountTotal DECIMAL(20,3) = 0,
				@QuantityAmountSumSoFar DECIMAL(20,3) = 0 
		
		SET @QuantityAmountTotal=ISNULL((SELECT sum(ti.QuantityAmount)
				                                    FROM TransactionItems ti WHERE ti.Id=@TransactionItemId
																			   AND ti.QuantityUnitId=@QuantityUnitId
				                                                               AND ti.TransactionId=@TransactionId),0)	
        IF @Action=2
			SET @QuantityAmountSumSoFar=ISNULL((SELECT sum(tip.QuantityAmount)
					                                    FROM TransactionItemPrices tip WHERE tip.TransactionItemId=@TransactionItemId
																						 AND tip.QuantityUnitId=@QuantityUnitId
																						 AND tip.TransactionId=@TransactionId),0)	
		ELSE IF @Action=1
			SET @QuantityAmountSumSoFar=ISNULL((SELECT sum(tip.QuantityAmount)
					                                    FROM TransactionItemPrices tip WHERE tip.TransactionReferenceId=@TransactionItemId
																						 AND tip.QuantityUnitId=@QuantityUnitId
																						 --AND tip.TransactionId=@TransactionId
																						 ),0)
																						 	 
			IF @QuantityAmountTotal<@QuantityAmountSumSoFar
			BEGIN
				SET @retValue=0--سرريز شده است 
			END	
			ELSE 
				SET @retValue=1	--سرريز نشده است 
		RETURN @retValue	
	END
GO
-----------------------------------------------------------------------------------------------
IF OBJECT_ID ('[PrimaryCoefficient]') IS NOT NULL 
	DROP FUNCTION PrimaryCoefficient;
GO
CREATE FUNCTION PrimaryCoefficient
(
	@OriginalUnitId INT,
	@SubsidiaryUnitId INT,
	@EffectiveDate DATETIME
)
RETURNS @tbl TABLE 
(
	Coefficient DECIMAL(18,0),
    PrimaryCoefficient TINYINT
)
--WITH ENCRYPTION
AS
BEGIN
	DECLARE @SubsidiaryUnit_Coefficient DECIMAL(18,0)
		SET @SubsidiaryUnit_Coefficient=0
	DECLARE @PrimaryCoefficient TINYINT
		SET @PrimaryCoefficient=2

		SET @SubsidiaryUnit_Coefficient = ISNULL((
									SELECT TOP(1)uc.Coefficient
									FROM   UnitConverts uc
									WHERE uc.UnitId = @OriginalUnitId
									      AND uc.SubUnitId = @SubsidiaryUnitId
										  AND uc.EffectiveDate <=@EffectiveDate
									ORDER BY uc.EffectiveDate DESC
							),0)
	   IF @SubsidiaryUnit_Coefficient=0
	   BEGIN
	   		SET @SubsidiaryUnit_Coefficient = ISNULL((
									SELECT TOP(1)uc.Coefficient
									FROM   UnitConverts uc
									WHERE uc.UnitId = @SubsidiaryUnitId
										  AND uc.SubUnitId = @OriginalUnitId
										  AND uc.EffectiveDate <=@EffectiveDate
								    ORDER BY uc.EffectiveDate DESC
							),0)
			IF @SubsidiaryUnit_Coefficient<>0 SET @PrimaryCoefficient=0
	   END
	   ELSE  SET @PrimaryCoefficient=1
   INSERT INTO @tbl
    (
    	Coefficient,
    	PrimaryCoefficient
    )
    VALUES
    (
    	@SubsidiaryUnit_Coefficient,
    	@PrimaryCoefficient
    )
    RETURN 
END
GO
--------------------تولید کد جدید رسید یا حواله
IF OBJECT_ID ( 'GenerateTransactionNewCode', 'FN' ) IS NOT NULL 
DROP FUNCTION GenerateTransactionNewCode;
GO
CREATE FUNCTION GenerateTransactionNewCode
(
	@Code decimal(20,2)
)
RETURNS decimal(20,2)
--WITH ENCRYPTION
AS 
BEGIN
	declare @retValue decimal(20,2)
	
	--IF @WarehouseToWarehouse=1
	--	SET @retValue=FLOOR(@RHCode)+CAST(((@RHCode - floor(@RHCode))*100+1)/100 AS DECIMAL(10,2))
	--ELSE
		SET @retValue=FLOOR(@Code)+1
	
	IF @retValue IS NULL
		SET @retValue= 1 
	RETURN @retValue
END;
GO
--PRINT dbo.GenerateTransactionNewCode(1.99)
--PRINT dbo.GenerateResidHavaleNewCode(2,1)
--PRINT dbo.GenerateResidHavaleNewCode(NULL,1)
--PRINT dbo.GenerateResidHavaleNewCode(NULL,0)

--------------------تولید کد جدید رسید یا حواله
IF OBJECT_ID ( 'GetTransactionNewCode', 'FN' ) IS NOT NULL 
DROP FUNCTION GetTransactionNewCode;
GO
CREATE FUNCTION GetTransactionNewCode
(
	@Action TINYINT,
	@WarehouseId INT,
	@RegistrationDate DATETIME,
	@TimeBucketId INT
)
RETURNS decimal(20,2)
--WITH ENCRYPTION
AS 
BEGIN
	declare @retValue decimal(20,2)
	
	SET @retValue=(SELECT CASE WHEN @RegistrationDate>=MAX(t.RegistrationDate) OR MAX(t.RegistrationDate) IS NULL THEN 
										dbo.GenerateTransactionNewCode(MAX(t.Code)) 
									ELSE	
										(SELECT dbo.GenerateTransactionNewCode(MAX(t.Code)) 
										 FROM Transactions t WHERE t.Action=@Action AND t.WarehouseId=@WarehouseId AND t.RegistrationDate<=@RegistrationDate AND t.TimeBucketId=@TimeBucketId) 
									END
							 FROM Transactions t
							 WHERE  t.Action=@Action AND t.WarehouseId=@WarehouseId AND t.TimeBucketId=@TimeBucketId)
	
	RETURN @retValue
END;
GO
-------------------------------------------------------------------------
IF OBJECT_ID ( 'IsValidRHCode', 'FN' ) IS NOT NULL 
DROP FUNCTION IsValidRHCode;
GO
CREATE FUNCTION IsValidRHCode
(
	@Action tinyint ,
	@Code decimal(20,2),
	@WarehouseId INT,
	@RegistrationDate DATETIME,
	@TimeBucketId INT
)
RETURNS BIT
--WITH ENCRYPTION
AS 
BEGIN
	declare @retValue bit		
	DECLARE @c SMALLINT
			
	SET @c=(SELECT COUNT(t.Code) FROM Transactions t
			WHERE t.Action=@Action AND t.WarehouseId=@WarehouseId AND t.TimeBucketId=@TimeBucketId AND(
			  (t.Code<=@Code AND t.RegistrationDate>@RegistrationDate AND t.Code<>@Code) 
			  OR (t.Code>=@Code AND t.RegistrationDate<@RegistrationDate AND t.Code<>@Code)))			
	SET @retValue=CASE WHEN @c=0 THEN 1 ELSE 0 END
	return @retValue
END;
GO
------------------------------------------------------
raiserror(N'پایان ایجاد توابع.',0,1) with nowait
GO
