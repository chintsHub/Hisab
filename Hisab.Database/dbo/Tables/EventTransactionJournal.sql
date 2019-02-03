CREATE TABLE [dbo].[EventTransactionJournal] (
    [EventTransactionId] INT NOT NULL,
    [EventUserAccountId]     INT NOT NULL,
    [Action]             INT              NOT NULL,
    [DebitAmount]        DECIMAL (18, 2)  NOT NULL,
    [CreditAmount]       DECIMAL (18, 2)  NOT NULL, 
    CONSTRAINT [PK_EventTransactionJournal] PRIMARY KEY ([EventTransactionId], [EventUserAccountId], [Action]), 
    CONSTRAINT [FK_EventTransactionJournal_EventUserAccount] FOREIGN KEY ([EventUserAccountId]) REFERENCES [EventUserAccount]([AccountId]), 
    CONSTRAINT [FK_EventTransactionJournal_EventUserTransaction] FOREIGN KEY ([EventTransactionId]) REFERENCES [EventUserTransaction]([Id])
);

