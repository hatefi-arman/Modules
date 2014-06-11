USE MiniStock
GO     
----------------------------------------------------------------
--بررسي اينکه تاريخ پايان بزرگتر يا مساوي  تاريخ شروع باشد
IF OBJECT_Id ( '[IsActualDateInFinancialYear]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsActualDateInFinancialYear')
			ALTER TABLE FinancialYear
			DROP CONSTRAINT chk_IsActualDateInFinancialYear
		DROP FUNCTION [IsActualDateInFinancialYear];
	END
	GO
	CREATE FUNCTION [IsActualDateInFinancialYear]
	(
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		SET @retValue= CASE WHEN @EndDate<=@StartDate THEN 0 ELSE 1 END 
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('FinancialYear'))
		ALTER TABLE FinancialYear
		ADD  CONSTRAINT chk_IsActualDateInFinancialYear CHECK (dbo.IsActualDateInFinancialYear(StartDate,EndDate)<>0)
GO
-------------------------------------------------------------------
--بررسي اينکه سال حداکثر 366 روز باشد
IF OBJECT_Id ( '[IsValIdDayInFinancialYear]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsValIdDayInFinancialYear')
			ALTER TABLE FinancialYear
			DROP CONSTRAINT chk_IsValIdDayInFinancialYear
		DROP FUNCTION [IsValIdDayInFinancialYear];
	END
	GO
	CREATE FUNCTION [IsValIdDayInFinancialYear]
	(
		@Id INT,
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		SET @retValue=CASE WHEN DATEDIFF(day,@StartDate,@EndDate)>365 THEN 0 ELSE 1 END 
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('FinancialYear'))
		ALTER TABLE FinancialYear
		ADD  CONSTRAINT chk_IsValIdDayInFinancialYear CHECK (dbo.IsValIdDayInFinancialYear(Id,StartDate,EndDate)<>0)
GO
----------------------------------------------------------------
--بررسي اينکه تاريخ پايان بزرگتر يا مساوي  تاريخ شروع باشد
IF OBJECT_Id ( '[IsActualDateInTimeBucket]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsActualDateInTimeBucket')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsActualDateInTimeBucket
		DROP FUNCTION [IsActualDateInTimeBucket];
	END
	GO
	CREATE FUNCTION [IsActualDateInTimeBucket]
	(
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		SET @retValue= CASE WHEN @EndDate<=@StartDate THEN 0 ELSE 1 END 
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsActualDateInTimeBucket CHECK (dbo.IsActualDateInTimeBucket(StartDate,EndDate)<>0)
GO
----------------------------------------------------------------
--بررسي اينکه تاريخ پايان 29 روز بزرگتر از تاريخ شروع باشد
IF OBJECT_Id ( '[IsDiffDateGreaterThan29DaysInTimeBucket]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsDiffDateGreaterThan29DaysInTimeBucket')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsDiffDateGreaterThan29DaysInTimeBucket
		DROP FUNCTION [IsDiffDateGreaterThan29DaysInTimeBucket];
	END
	GO
	CREATE FUNCTION [IsDiffDateGreaterThan29DaysInTimeBucket]
	(
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		SET @retValue= CASE WHEN DATEADD(day,28,@StartDate)>@EndDate THEN 0 ELSE 1 END 
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsDiffDateGreaterThan29DaysInTimeBucket CHECK (dbo.IsDiffDateGreaterThan29DaysInTimeBucket(StartDate,EndDate)<>0)
GO
----------------------------------------------------------------
--تاريخ شروع اولين بازه برابر با تاريخ شروع سال و تاربخ پايان آخرين بازه برابر با تاريخ پايان سال باشد
IF OBJECT_Id ( '[IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket
		DROP FUNCTION [IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket];
	END
	GO
	CREATE FUNCTION [IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket]
	(
		@Id INT,
		@FinancialYearId INT,
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=1
		
		IF (SELECT TOP (1) ISNULL(tb.Id,0) 
	                  FROM TimeBucket tb
				      WHERE tb.FinancialYearId=@FinancialYearId
	        ORDER BY tb.Id ASC)=isnull(@Id,0) OR NOT EXISTS (SELECT tb.Id 
	                  FROM TimeBucket tb
				      WHERE tb.FinancialYearId=@FinancialYearId)
		  BEGIN
		  	 DECLARE @StartDateYear DATETIME
		  	        SET @StartDateYear=(SELECT TOP(1) fy.[StartDate] FROM FinancialYear fy WHERE fy.Id=@FinancialYearId)
		  	        IF @StartDate<>@StartDateYear
		  	        BEGIN
		  				set @retValue=0
		  			END
		  END
		  IF @retValue=1 AND (SELECT count(ISNULL(tb.Id,0))
	                  FROM TimeBucket tb
				      WHERE tb.FinancialYearId=@FinancialYearId)=12
				      AND (SELECT TOP(1) tb.Id
	                  FROM TimeBucket tb
				      WHERE tb.FinancialYearId=@FinancialYearId
				           ORDER BY tb.Id desc)=@Id
		  BEGIN
		  	 DECLARE @EndDateYear DATETIME
		  	        SET @EndDateYear=(SELECT TOP(1) fy.EndDate
		  	                            FROM FinancialYear fy WHERE fy.Id=@FinancialYearId)
		  	        IF @EndDate<>@EndDateYear
		  	        BEGIN
		  				set @retValue=0
		  			END
		  END
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket CHECK (dbo.IsStartDateEQStartYear_EndDateEQEndYearInTimeBucket(Id,FinancialYearId,StartDate,EndDate)<>0)
GO
-------------------------------------------------------------------
--بررسي اينکه تاريخ شروع بازه در يک سال از تاريخ پايان بازه قبل در همان سال بزرگتر باشد
-- و يا تعداد بازه ها در يک سال نبايد از 12 مورد بيشتر باشد
IF OBJECT_Id ( '[IsValIdDateInTimeBucket]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsValIdDateInTimeBucket')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsValIdDateInTimeBucket
		DROP FUNCTION [IsValIdDateInTimeBucket];
	END
	GO
	CREATE FUNCTION [IsValIdDateInTimeBucket]
	(
		@Id INT, 
		@FinancialYearId INT,
		@StartDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		
		SET @retValue=(SELECT CASE WHEN isnull(count(tb.Id),0)>0 THEN 1 ELSE 0 END FROM TimeBucket tb
							  WHERE tb.EndDate>=@StartDate AND tb.FinancialYearId=@FinancialYearId AND tb.Id<@Id)
		
		IF @retValue=0
		BEGIN
			SET @retValue=(SELECT CASE WHEN isnull(count(tb.Id),0)>12 THEN 1 ELSE 0 END FROM TimeBucket tb
							  WHERE tb.FinancialYearId=@FinancialYearId)
		END
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsValIdDateInTimeBucket CHECK (dbo.IsValIdDateInTimeBucket(Id,FinancialYearId,StartDate)<>1)
GO
-------------------------------------------------------------------
-- بررسي اينکه تاريخ شروع هر بازه بايد بزرگتر يا مساوي تاريخ شروع سال 
-- و تاريخ پايا ن بازه بايد کوچکتر يا مساوي تاريخ پايان سال باشد
IF OBJECT_Id ( '[IsValIdDateInTimeBucket_Than_FinancialYear]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsValIdDateInTimeBucket_Than_FinancialYear')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsValIdDateInTimeBucket_Than_FinancialYear
		DROP FUNCTION [IsValIdDateInTimeBucket_Than_FinancialYear];
	END
	GO
	CREATE FUNCTION [IsValIdDateInTimeBucket_Than_FinancialYear]
	(
		@Id INT, 
		@FinancialYearId INT,
		@StartDate DATETIME,
		@EndDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		
		SET @retValue=(SELECT CASE WHEN isnull(count(fy.Id),0)>0 THEN 1 ELSE 0 END FROM FinancialYear fy
							  WHERE (fy.[StartDate]>@StartDate OR fy.[EndDate]<@EndDate) AND fy.Id=@FinancialYearId)
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsValIdDateInTimeBucket_Than_FinancialYear CHECK (dbo.IsValIdDateInTimeBucket_Than_FinancialYear(Id,FinancialYearId,StartDate,EndDate)<>1)
GO
-------------------------------------------------------------------
--بررسي اينکه تاريخ بازه ها گپ نداشته باشند
IF OBJECT_Id ( '[IsNoGapInTimeBucket]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_IsNoGapInTimeBucket')
			ALTER TABLE TimeBucket
			DROP CONSTRAINT chk_IsNoGapInTimeBucket
		DROP FUNCTION [IsNoGapInTimeBucket];
	END
	GO
	CREATE FUNCTION [IsNoGapInTimeBucket]
	(
		@Id INT,
		@FinancialYearId INT,
		@StartDate DATETIME
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		declare @PreEndDate DATETIME
		SELECT TOP (1) @PreEndDate=tb.EndDate
				FROM TimeBucket tb
				WHERE tb.Id<@Id AND tb.FinancialYearId=@FinancialYearId
		ORDER BY tb.Id DESC
		IF @PreEndDate IS NOT NULL
		BEGIN
			    SET	@PreEndDate=DATEADD(day,1,@PreEndDate)
		        SET @retValue=CASE WHEN @StartDate<>@PreEndDate THEN 1 ELSE 0 END
		END
		return @retValue
	END;
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('TimeBucket'))
		ALTER TABLE TimeBucket
		ADD  CONSTRAINT chk_IsNoGapInTimeBucket CHECK (dbo.IsNoGapInTimeBucket(Id,FinancialYearId,[StartDate])<>1)
GO
----------------------------------------------------------------
--براي بررسي اينکه واحد اصلي و فرعي در تبديل واحدها يکسان نباشند
IF OBJECT_Id ( '[NoMatch2UnitInUnitConverts]', 'FN' ) IS NOT NULL 
	BEGIN
		IF EXISTS(SELECT * FROM sys.check_constraints WHERE  NAME = 'chk_NoMatch2UnitInUnitConverts')
			ALTER TABLE UnitConverts
			DROP CONSTRAINT chk_NoMatch2UnitInUnitConverts
		DROP FUNCTION [NoMatch2UnitInUnitConverts];
	END
	GO
	CREATE FUNCTION [NoMatch2UnitInUnitConverts]
	(
		@UnitId INT,
		@SubUnitId INT 
	)
	RETURNS bit
	AS 
	BEGIN
		declare @retValue bit		
		SET @retValue=0
		SET @retValue= CASE WHEN @UnitId=@SubUnitId THEN 0 ELSE 1 END 
		return @retValue
	END
GO
	IF EXISTS(SELECT * FROM sys.tables t WHERE  upper(NAME) = UPPER('UnitConverts'))
		ALTER TABLE UnitConverts
		ADD  CONSTRAINT chk_NoMatch2UnitInUnitConverts CHECK (dbo.NoMatch2UnitInUnitConverts(UnitId,SubUnitId)<>0)
GO
