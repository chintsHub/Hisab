CREATE TABLE [dbo].[EventTransactionJournal]
(
	[EventId] UNIQUEIDENTIFIER NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [EventAccountId] UNIQUEIDENTIFIER NOT NULL, 
    [EventAccountAction] INT NOT NULL, 
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL, 
	[EventFriendAccountAction] INT NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL, 
    PRIMARY KEY ([EventId], [UserId], [TransactionId]), 
    CONSTRAINT [FK_EventTransactionJournal_EventFriend] FOREIGN KEY ([UserId],[EventId]) REFERENCES [EventFriend]([UserId],[EventId]), 
    CONSTRAINT [FK_EventTransactionJournal_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]), 
    CONSTRAINT [FK_EventTransactionJournal_EventAccount] FOREIGN KEY ([EventAccountId]) REFERENCES [EventAccount](AccountId), 
    CONSTRAINT [FK_EventTransactionJournal_EventFriendAccount] FOREIGN KEY ([UserAccountId]) REFERENCES [UserAccount]([AccountId]) 
)
