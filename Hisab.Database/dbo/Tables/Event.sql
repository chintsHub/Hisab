CREATE TABLE [dbo].[Event] (
    [Id]     INT NOT NULL,
    [UserId] INT NOT NULL,
    [Name]   VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Event_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id])
);

