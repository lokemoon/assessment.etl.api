CREATE TABLE [dbo].[Customers]
(
	[CustomerId] VARCHAR(24) NOT NULL PRIMARY KEY, 
    [GivenName] NVARCHAR(50) NOT NULL,
	[FamilyName] NVARCHAR(50) NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT(0),
	[CreatedTime] DATETIME2 NOT NULL,
    [CreatedBy] VARCHAR(24) NOT NULL,
    [LastModifyTime] DATETIME2 NOT NULL, 
    [LastModifyBy] VARCHAR(24) NULL,
)
