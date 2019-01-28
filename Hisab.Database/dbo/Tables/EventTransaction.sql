CREATE TABLE [dbo].[EventTransaction] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [TransactionDate]   DATE             NOT NULL,
    [TransactionAmount] DECIMAL (18, 2)  NOT NULL,
    [EventId]           UNIQUEIDENTIFIER NOT NULL,
    [EventUserId]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EventTransaction] PRIMARY KEY CLUSTERED ([Id] ASC)
);

