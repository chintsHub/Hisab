CREATE TABLE [dbo].[EventTransactionJournal]
(
	[JournalId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionId] UNIQUEIDENTIFIER NOT NULL, 
    [Particulars] VARCHAR(100) NOT NULL, 
    [AccountId] UNIQUEIDENTIFIER NOT NULL, 
    [DebitAmount] DECIMAL(18, 4) NOT NULL, 
    [CreditAmount] DECIMAL(18, 4) NOT NULL, 
    CONSTRAINT [FK_EventTransactionJournal_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]), 
    CONSTRAINT [FK_EventTransactionJournal_EventAccount] FOREIGN KEY ([AccountId]) REFERENCES [EventAccount]([AccountId])
)
