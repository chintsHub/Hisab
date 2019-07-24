CREATE TABLE [dbo].[EventAccount]
(
	[AccountId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EventId] INT NOT NULL, 
    [EventFriendId] INT NULL, 
    [AccountTypeId] INT NOT NULL, 
    CONSTRAINT [FK_EventAccount_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventAccount_EventFriend] FOREIGN KEY ([EventFriendId]) REFERENCES [EventFriend]([EventFriendId]), 
    CONSTRAINT [FK_EventAccount_AccountType] FOREIGN KEY ([AccountTypeId]) REFERENCES [AccountType]([Id])
)
