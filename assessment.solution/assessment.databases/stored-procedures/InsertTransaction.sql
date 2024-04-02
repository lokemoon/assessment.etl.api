CREATE PROCEDURE [dbo].[InsertTransaction]
	@AccountNum VARCHAR(24), 
    @CustomerId VARCHAR(24), 
    @TransactionDate DATE, 
    @TransactionTime TIME(0), 
    @Description NVARCHAR(100), 
    @Amount DECIMAL(18, 2),
    @CreatedTime datetime2 = null,
    @CreatedBy VARCHAR(24),
	@LastModifyTime datetime2 = null
AS
INSERT INTO [dbo].[Transactions](
 [AccountNum], 
    [CustomerId], 
    [TransactionDate], 
    [TransactionTime], 
    [Description], 
    [Amount],
    [CreatedTime],
    [CreatedBy],
    [LastModifyTime])
VALUES(
    @AccountNum, 
    @CustomerId, 
    @TransactionDate, 
    @TransactionTime, 
    @Description, 
    @Amount,
    ISNULL(@CreatedTime, GETDATE()),
    @CreatedBy,
	ISNULL(@LastModifyTime, GETDATE()));

	SELECT SCOPE_IDENTITY()
RETURN 0
