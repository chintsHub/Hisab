﻿CREATE TABLE [dbo].[EventTransactionSettlement]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[EventId] UNIQUEIDENTIFIER not null,
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [PayerEventFriendId] UNIQUEIDENTIFIER NOT NULL, 
    [ReceiverEventFriendId] UNIQUEIDENTIFIER NOT NULL, 
    [Amount] DECIMAL(18, 4) NOT NULL, 
    CONSTRAINT [FK_EventTransactionSettlement_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventTransactionSettlement_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]), 
    CONSTRAINT [FK_EventTransactionSettlement_EventFriend] FOREIGN KEY ([PayerEventFriendId]) REFERENCES [EventFriend]([EventFriendId]), 
    CONSTRAINT [FK_EventTransactionSettlement_EventFriend2] FOREIGN KEY ([ReceiverEventFriendId]) REFERENCES [EventFriend]([EventFriendId])
)
