
if((select count(*) from AccountType ) = 0)
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (1,'Asset')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (2,'Liability')