CREATE TABLE [dbo].[EventTransactionSplit]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [EventFriendId] UNIQUEIDENTIFIER NOT NULL, 
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [AmountDue] DECIMAL(18, 4) NOT NULL, 
    
    CONSTRAINT [FK_EventTransactionSplit_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id])
)
