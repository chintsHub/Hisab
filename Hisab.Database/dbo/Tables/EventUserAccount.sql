CREATE TABLE [dbo].[EventUserAccount]
(
	[AccountId] INT NOT NULL , 
    [EventId] INT NOT NULL, 
    [UserId] INT NULL, 
    [AccountType] INT NOT NULL, 
    [Name] VARCHAR(500) NOT NULL, 
    PRIMARY KEY ([AccountId]), 
    CONSTRAINT [FK_EventUserAccount_EventUser] FOREIGN KEY ([EventId], [UserId]) REFERENCES [EventUser]([EventId],[UserId])
)
