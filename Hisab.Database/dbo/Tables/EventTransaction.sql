CREATE TABLE [dbo].[EventTransaction]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [EventId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedbyUserId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedDateTime] DATETIME NOT NULL, 
    [TotalAmount] DECIMAL(18, 2) NOT NULL, 
    [Description] VARCHAR(100) NOT NULL, 
    [PaidByUserId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_EventTransaction_EventFriend] FOREIGN KEY ([PaidByUserId],EventId) REFERENCES [EventFriend](UserId,EventId), 
    CONSTRAINT [FK_EventTransaction_ApplicationUser] FOREIGN KEY ([CreatedbyUserId]) REFERENCES [ApplicationUser]([Id]) 
)
