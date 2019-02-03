CREATE TABLE [dbo].[EventUser] (
    [EventId]    INT NOT NULL,
    [UserId]     INT NOT NULL,
    [AdultCount] INT              NULL,
    [KidCount]   INT              NULL,
    CONSTRAINT [PK_EventUser] PRIMARY KEY CLUSTERED ([EventId] ASC, [UserId] ASC)
);

