CREATE TABLE [dbo].[EventInvites]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL , 
    [EventId] UNIQUEIDENTIFIER NOT NULL, 
    [InviteStatus] INT NOT NULL, 
    CONSTRAINT [FK_EventInvites_ApplicationUser] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([ID]), 
    CONSTRAINT [FK_EventInvites_Event] FOREIGN KEY ([EventId]) REFERENCES [Event]([ID]), 
    PRIMARY KEY ([UserId], [EventId])
)
