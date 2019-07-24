CREATE TABLE [dbo].[EventTransaction]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TotalAmount] DECIMAL(18, 4) NOT NULL, 
    [Description] VARCHAR(200) NOT NULL, 
    [SplitType] INT NOT NULL, 
    [EventId] INT NOT NULL, 
    CONSTRAINT [FK_EventTransaction_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id])
)
