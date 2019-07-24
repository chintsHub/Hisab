CREATE TABLE [dbo].[EventTranscationJournal]
(
	[JournalId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionId] INT NOT NULL, 
    [Particulars] VARCHAR(100) NOT NULL, 
    [AccountId] INT NOT NULL, 
    [DebitAmount] DECIMAL(18, 4) NOT NULL, 
    [CreditAmount] DECIMAL(18, 4) NOT NULL, 
    CONSTRAINT [FK_EventTranscationJournal_EventTransaction] FOREIGN KEY ([TransactionId]) REFERENCES [EventTransaction]([Id]), 
    CONSTRAINT [FK_EventTranscationJournal_EventAccount] FOREIGN KEY ([AccountId]) REFERENCES [EventAccount]([AccountId])
)
