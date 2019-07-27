CREATE TABLE [dbo].[EventTransaction]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TotalAmount] DECIMAL(18, 4) NOT NULL, 
    [Description] VARCHAR(200) NOT NULL, 
    [SplitType] INT NOT NULL, 
    [EventId] INT NOT NULL, 
    [CreatedbyUserId] INT NOT NULL, 
    [CreatedDateTime] DATETIME NOT NULL, 
    CONSTRAINT [FK_EventTransaction_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventTransaction_ApplicationUser] FOREIGN KEY ([CreatedbyUserId]) REFERENCES [ApplicationUser]([Id])
)
