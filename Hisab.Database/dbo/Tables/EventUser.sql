CREATE TABLE [dbo].[EventUser] (
    [EventId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [AdultCount] INT              NULL,
    [KidCount]   INT              NULL,
    CONSTRAINT [PK_EventUser] PRIMARY KEY CLUSTERED ([EventId] ASC, [UserId] ASC)
);

