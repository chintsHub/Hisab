CREATE TABLE [dbo].[UserFeedback]
(
	[FeedbackId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [Message] VARCHAR(MAX) NOT NULL, 
    [FeedbackType] INT NOT NULL, 
    [FeedbackDate] DATETIME NOT NULL, 
    [ShowAsTestimony] BIT NULL, 
    CONSTRAINT [FK_UserFeedback_User] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser](Id)
)
