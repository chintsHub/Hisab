﻿CREATE TABLE [dbo].[EventFriend]
(
	[EventFriendId] INT NOT NULL IDENTITY, 
	[EventId] INT NOT NULL , 
    [Email] VARCHAR(200) NOT NULL, 
    [NickName] VARCHAR(200) NOT NULL, 
    [Status] INT NOT NULL, 
    [AppUserId] INT NULL, 
    [AdultCount] INT NOT NULL, 
    [KidsCount] INT NOT NULL, 
    
    CONSTRAINT [FK_EventFriend_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]), 
    CONSTRAINT [FK_EventFriend_AppUser] FOREIGN KEY ([AppUserId]) REFERENCES [ApplicationUser]([Id]), 
    CONSTRAINT [PK_EventFriend] PRIMARY KEY ([EventFriendId]) 
)
