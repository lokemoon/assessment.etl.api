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
INSERT INTO [dbo].[Accounts]([AccountNum], [Name],[AccountType],[Balance],[Outstanding],[CreatedTime], [CreatedBy],[LastModifyTime])
SELECT a.value, a.value, 'SAVING',0, 0, GETDATE(),'assessment.setup', GETDATE()
FROM STRING_SPLIT('6872838260,8872838299,8872838283',',') a
LEFT OUTER JOIN
	[dbo].[Accounts] b
	ON a.value = b.AccountNum
WHERE b.AccountNum is NULL