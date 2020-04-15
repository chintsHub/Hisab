CREATE TABLE [dbo].[EventTransaction]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [TotalAmount] DECIMAL(18, 4) NOT NULL, 
    [Description] VARCHAR(200) NOT NULL, 
    [SplitType] INT NOT NULL, 
    [EventId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedbyUserId] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedDateTime] DATETIME NOT NULL, 
    CONSTRAINT [FK_EventTransaction_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventTransaction_ApplicationUser] FOREIGN KEY ([CreatedbyUserId]) REFERENCES [ApplicationUser]([Id])
)
