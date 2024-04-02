/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [dbo].[Customers]([CustomerId], [GivenName], [FamilyName], [CreatedTime], [CreatedBy],[LastModifyTime])
SELECT a.value, a.value, a.value, GETDATE(), 'assessment.setup',GETDATE()
FROM STRING_SPLIT('222,333',',') a
LEFT OUTER JOIN
	[dbo].[Customers] b
	ON a.value = b.CustomerId
WHERE b.CustomerId is NULL

