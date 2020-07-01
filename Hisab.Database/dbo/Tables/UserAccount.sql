CREATE TABLE [dbo].[UserAccount]
(
	[AccountId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [AccountTypeId] INT NOT NULL, 
    CONSTRAINT [FK_UserAccount_ApplicationUser] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id]), 
    CONSTRAINT [FK_UserAccount_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [ApplicationAccountType]([Id])
)
