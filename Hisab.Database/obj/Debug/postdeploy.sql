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

if((select count(*) from AccountType ) = 0)
Begin

INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (1,'Asset')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (2,'Liability')
End

if((select count(*) from ApplicationRole ) = 0)
BEGIN

INSERT INTO [dbo].[ApplicationRole] ([Id] ,[Name] ,[NormalizedName]) VALUES
           (1 ,'Admin' ,'ADMIN')

INSERT INTO [dbo].[ApplicationRole] ([Id] ,[Name] ,[NormalizedName]) VALUES
           (2 ,'App User' ,'APPLICATION USER')



END
GO
