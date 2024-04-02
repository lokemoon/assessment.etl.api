CREATE TABLE [dbo].[Transactions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountNum] VARCHAR(24) NOT NULL, 
    [CustomerId] VARCHAR(24) NOT NULL, 
    [TransactionDate] DATE NOT NULL, 
    [TransactionTime] TIME(0) NOT NULL, 
    [Description] NVARCHAR(100) NOT NULL, 
    [Amount] DECIMAL(18, 2) NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT(0),
    [CreatedTime] DATETIME2 NOT NULL,
    [CreatedBy] VARCHAR(24) NOT NULL,
    [LastModifyTime] DATETIME2 NOT NULL, 
    [LastModifyBy] VARCHAR(24) NULL,
    CONSTRAINT [FK_Transactions_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([CustomerId]),
    CONSTRAINT [FK_Transactions_Accounts_AccountNum] FOREIGN KEY ([AccountNum]) REFERENCES [Accounts]([AccountNum])
)
