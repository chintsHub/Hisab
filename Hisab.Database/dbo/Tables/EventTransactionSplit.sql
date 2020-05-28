CREATE TABLE [dbo].[EventTransactionSplit]
(
	[EventId] UNIQUEIDENTIFIER NOT NULL , 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [SplitPercentage] DECIMAL(18, 2) NOT NULL, 
    PRIMARY KEY ([EventId], [UserId], [TransactionId]), 
    CONSTRAINT [FK_EventTransactionSplit_EventFriend] FOREIGN KEY ([UserId],[EventId]) REFERENCES [EventFriend]([UserId],[EventId]), 
    CONSTRAINT [FK_EventTransactionSplit_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]) 
    
)
