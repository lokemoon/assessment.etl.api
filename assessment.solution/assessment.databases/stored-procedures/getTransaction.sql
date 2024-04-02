CREATE PROCEDURE [dbo].[getTransactionById]
	@Id INT
AS
BEGIN

SELECT  * FROM [dbo].[Transactions]
WHERE [IsDeleted] = 0 AND [Id] = @Id

END