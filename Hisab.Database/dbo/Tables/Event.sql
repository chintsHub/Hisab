﻿CREATE TABLE [dbo].[Event] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Name]   VARCHAR (50)     NOT NULL,
    [CreateDate] DATETIME NULL, 
    [LastModifiedDate] DATETIME NULL, 
    [Status] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Event_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id])
);

