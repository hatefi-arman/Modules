(SELECT SUM(t.s) AS CurrentMojodi
FROM   (
			SELECT 
			SUM(CASE WHEN PATINDEX('%'+cast(t.Action AS NVARCHAR(5))+'%', N'12   ')=0 
				THEN  0 ELSE
				ti.QuantityAmount *(CASE WHEN t.[Action]=1 THEN 1 WHEN t.[Action]=2 THEN -1 END) END) AS s
			FROM TransactionItems ti
					INNER  JOIN Transactions t
						ON t.Id = ti.TransactionId
			WHERE  ti.GoodId=10 --@GoodId
					AND ti.QuantityUnitId=1--@QuantityUnitId
					AND t.WarehouseId=12
					AND t.TimeBucketId=1--@TimeBucketId
			GROUP BY
					t.Action
		) AS t
)