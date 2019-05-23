﻿/*
Deployment script for Hisab4

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "Hisab4"
:setvar DefaultFilePrefix "Hisab4"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL13.CHINTS\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL13.CHINTS\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
/*
The column [dbo].[EventFriend].[AdultCount] on table [dbo].[EventFriend] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.

The column [dbo].[EventFriend].[KidsCount] on table [dbo].[EventFriend] must be added, but the column has no default value and does not allow NULL values. If the table contains data, the ALTER script will not work. To avoid this issue you must either: add a default value to the column, mark it as allowing NULL values, or enable the generation of smart-defaults as a deployment option.
*/

IF EXISTS (select top 1 1 from [dbo].[EventFriend])
    RAISERROR (N'Rows were detected. The schema update is terminating because data loss might occur.', 16, 127) WITH NOWAIT

GO
PRINT N'Dropping unnamed constraint on [dbo].[ApplicationUser]...';


GO
ALTER TABLE [dbo].[ApplicationUser] DROP CONSTRAINT [DF__Applicati__NickN__2D27B809];


GO
PRINT N'Altering [dbo].[EventFriend]...';


GO
ALTER TABLE [dbo].[EventFriend]
    ADD [AdultCount] INT NOT NULL,
        [KidsCount]  INT NOT NULL;


GO
PRINT N'Creating unnamed constraint on [dbo].[ApplicationUser]...';


GO
ALTER TABLE [dbo].[ApplicationUser]
    ADD DEFAULT user FOR [NickName];


GO
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

GO
PRINT N'Update complete.';


GO
