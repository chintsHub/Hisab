CREATE TABLE [dbo].[Event] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Name]   VARCHAR (50)     NOT NULL,
    [CreateDate] DATETIME NOT NULL, 
    [Status] INT NOT NULL DEFAULT 1, 
    [EventPic] INT NOT NULL, 
    [CurrencyCode] NVARCHAR(6) NOT NULL DEFAULT 'AUD', 
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Event_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id])
);

