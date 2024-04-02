CREATE PROCEDURE [dbo].[searchTransactions]
	@Offset int = 0,
	@Limit int = 10,
	@CustomerId varchar(24) = null,
	@AccountNum varchar(24) = null,
	@Description nvarchar(100) = null	
AS
BEGIN

SELECT  COUNT(1) FROM [dbo].[Transactions]
WHERE [IsDeleted] = 0 AND CustomerId = ISNULL(@CustomerId, CustomerId) AND 
AccountNum = ISNULL(@AccountNum, AccountNum) AND 
(@Description is NULL OR Description LIKE '%'+@Description+'%');

SELECT  * FROM [dbo].[Transactions]
WHERE [IsDeleted] = 0 AND CustomerId = ISNULL(@CustomerId, CustomerId) AND 
AccountNum = ISNULL(@AccountNum, AccountNum) AND 
(@Description is NULL OR Description LIKE '%'+@Description+'%')
ORDER BY ID
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;

END