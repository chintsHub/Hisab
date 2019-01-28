CREATE TABLE [dbo].[EventAccount] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        VARCHAR (200)    NOT NULL,
    [AccountType] INT              NOT NULL,
    [EventId]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EventAccount] PRIMARY KEY CLUSTERED ([Id] ASC)
);

