CREATE TABLE [dbo].[EventUserTransaction] (
    [Id]                INT NOT NULL,
	[EventId]	int Not null,
	[EventUserId] int not null,
    [TransactionDate]   DATE             NOT NULL,
    [TransactionAmount] DECIMAL (18, 2)  NOT NULL,
    CONSTRAINT [PK_EventUserTransaction] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_EventUserTransaction_EventUser] FOREIGN KEY ([EventId],[EventUserId]) REFERENCES [EventUser]([EventId], [UserId])
);

