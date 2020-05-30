CREATE TABLE [dbo].[EventTransaction]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedbyUserId] UNIQUEIDENTIFIER NOT NULL, 
    [TransactionDate] DATE NOT NULL, 
    [TotalAmount] DECIMAL(18, 2) NOT NULL, 
    [Description] VARCHAR(100) NOT NULL, 
    [PaidByUserId] UNIQUEIDENTIFIER NOT NULL, 
    [LastModifiedDate] DATETIME NOT NULL, 
    [TransactionType] INT NOT NULL, 
    [LendToFriendUserId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_EventTransaction_EventFriend] FOREIGN KEY ([PaidByUserId],EventId) REFERENCES [EventFriend](UserId,EventId), 
    CONSTRAINT [FK_EventTransaction_ApplicationUser] FOREIGN KEY ([CreatedbyUserId]) REFERENCES [ApplicationUser]([Id]) 
)
