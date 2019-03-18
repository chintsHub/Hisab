
if((select count(*) from AccountType ) = 0)
Begin

INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (1,'Asset')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (2,'Liability')
End