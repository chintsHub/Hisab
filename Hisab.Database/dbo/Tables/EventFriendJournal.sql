CREATE TABLE [dbo].[EventFriendJournal]
(
	[JournalId] BIGINT not null IDENTITY,
	[EventId] UNIQUEIDENTIFIER NOT NULL, 
	[UserId] UNIQUEIDENTIFIER not null,
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [DebitAccount] UNIQUEIDENTIFIER NOT NULL, 
    [CreditAccount] UNIQUEIDENTIFIER NOT NULL, 
    [PayReceiveFriend] UNIQUEIDENTIFIER NULL, 
    [Amount] DECIMAL(18, 2) NOT NULL, 
    CONSTRAINT [FK_EventTransactionSettlement_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]), 
    
    CONSTRAINT [FK_EventTransactionSettlement_EventFriend] FOREIGN KEY ([UserId],[EventId]) REFERENCES [EventFriend]([UserId],[EventId]), 
    CONSTRAINT [FK_EventTransactionSettlement_DebitAccount] FOREIGN KEY ([DebitAccount]) REFERENCES [UserAccount]([AccountId]), 
    CONSTRAINT [FK_EventTransactionSettlement_CreditAccount] FOREIGN KEY ([CreditAccount]) REFERENCES [UserAccount]([AccountId]), 
    CONSTRAINT [FK_EventTransactionSettlement_PayReceiveFriend] FOREIGN KEY ([PayReceiveFriend],[EventId]) REFERENCES [EventFriend]([UserId],[EventId]), 
    CONSTRAINT [PK_EventFriendJournal] PRIMARY KEY ([JournalId]), 
   
)
