CREATE TABLE [dbo].[EventFriend]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL, 
	[EventId] UNIQUEIDENTIFIER NOT NULL , 
    [Status] INT NOT NULL, 
    
    CONSTRAINT [PK_EventFriend] PRIMARY KEY ([UserId], [EventId]), 
    CONSTRAINT [FK_EventFriend_UserId] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id]), 
    CONSTRAINT [FK_EventFriend_EventId] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]) 
)
