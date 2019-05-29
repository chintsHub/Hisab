CREATE TABLE [dbo].[Event] (
    [Id]     INT NOT NULL IDENTITY,
    [UserId] INT NOT NULL,
    [Name]   VARCHAR (50)     NOT NULL,
    [CreateDate] DATETIME NULL, 
    [LastModifiedDate] DATETIME NULL, 
    [Status] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Event_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id])
);

