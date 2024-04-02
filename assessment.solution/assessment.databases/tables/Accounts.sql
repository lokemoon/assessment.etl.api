CREATE TABLE [dbo].[Accounts]
(
	[AccountNum] VARCHAR(24) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [AccountType] VARCHAR(24) NOT NULL, 
    [Balance] DECIMAL(18, 2) NOT NULL,
    [Outstanding] DECIMAL(18, 2) NOT NULL,    
    [IsDeleted] BIT NOT NULL DEFAULT(0),
    [CreatedTime] DATETIME2 NOT NULL,
    [CreatedBy] VARCHAR(24) NOT NULL,
    [LastModifyTime] DATETIME2 NOT NULL, 
    [LastModifyBy] VARCHAR(24) NULL,
)
