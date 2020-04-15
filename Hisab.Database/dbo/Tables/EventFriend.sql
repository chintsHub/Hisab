CREATE TABLE [dbo].[EventFriend]
(
	[EventFriendId] UNIQUEIDENTIFIER NOT NULL, 
	[EventId] UNIQUEIDENTIFIER NOT NULL , 
    [Email] VARCHAR(200) NOT NULL, 
    [Status] INT NOT NULL, 
    [AppUserId] UNIQUEIDENTIFIER NULL, 
    
    CONSTRAINT [FK_EventFriend_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventFriend_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [ApplicationUser]([Id]), 
    CONSTRAINT [PK_EventFriend] PRIMARY KEY ([EventFriendId]) 
)
