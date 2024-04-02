CREATE PROCEDURE [dbo].[UpdateTransaction]
    @Id INT,	
    @Description NVARCHAR(100),     
    @LastModifyBy VARCHAR(24),
	--@PreviousLastModifyTime datetime2,
    @LastModifyTime datetime2
AS
BEGIN

DECLARE @IsExists BIT = 0;
DECLARE @IsConflict BIT = 0;
SELECT @IsExists = 1 FROM [dbo].[Transactions] WHERE [IsDeleted] = 0 AND [Id] = @Id;

IF @IsExists = 0
BEGIN
    SELECT 404;
    RETURN 0
END
--SELECT @IsConflict = 1 FROM [dbo].[Transactions] WHERE [IsDeleted] = 0 AND [Id] = @Id AND [LastModifyTime] <> @PreviousLastModifyTime;
--IF @IsConflict = 1
--BEGIN
--    SELECT 409;
--    RETURN 0
--END

UPDATE [dbo].[Transactions]
SET [LastModifyTime] = @LastModifyTime, [Description] = @Description, [LastModifyBy] = @LastModifyBy
WHERE [IsDeleted] = 0 AND [Id] = @Id
SELECT 0;
RETURN 0;

END