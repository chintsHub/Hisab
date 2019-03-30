CREATE TABLE [dbo].[ApplicationUserRole]
(
	[UserId] INT NOT NULL , 
    [RoleId] INT NOT NULL, 
    PRIMARY KEY ([UserId], [RoleId]), 
    CONSTRAINT [FK_ApplicationUserRole_ApplicationUser] FOREIGN KEY ([UserId]) REFERENCES [ApplicationUser]([Id]), 
    CONSTRAINT [FK_ApplicationUserRole_ApplicationRole] FOREIGN KEY ([RoleId]) REFERENCES [ApplicationRole]([Id]) 
)
