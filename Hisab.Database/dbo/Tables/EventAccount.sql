CREATE TABLE [dbo].[EventAccount]
(
	[AccountId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventId] UNIQUEIDENTIFIER NOT NULL, 
    [EventFriendId] UNIQUEIDENTIFIER NULL, 
    [AccountTypeId] INT NOT NULL, 
    CONSTRAINT [FK_EventAccount_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    
    CONSTRAINT [FK_EventAccount_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [AccountType]([Id])
)
