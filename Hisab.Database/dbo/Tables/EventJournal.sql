CREATE TABLE [dbo].[EventJournal] (
    [EventTransactionId] UNIQUEIDENTIFIER NOT NULL,
    [EventAccountId]     UNIQUEIDENTIFIER NOT NULL,
    [Action]             INT              NOT NULL,
    [DebitAmount]        DECIMAL (18, 2)  NOT NULL,
    [CreditAmount]       DECIMAL (18, 2)  NOT NULL
);

