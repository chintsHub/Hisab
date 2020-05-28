if((select count(*) from AccountType ) = 0)
Begin

INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (1,'Asset')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (2,'Liability')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (3,'Income')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (4,'Expense')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (5,'Capital')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (6,'Contra')
End


if((select count(*) from ApplicationAccountType ) = 0)
Begin

INSERT INTO [dbo].[ApplicationAccountType] ([Id] ,[Name], [AccountType]) VALUES (1,'Cash',1)
INSERT INTO [dbo].[ApplicationAccountType] ([Id] ,[Name], [AccountType]) VALUES (2,'Expense', 4)
INSERT INTO [dbo].[ApplicationAccountType] ([Id] ,[Name], [AccountType]) VALUES (3,'Account Recievable', 1)
INSERT INTO [dbo].[ApplicationAccountType] ([Id] ,[Name], [AccountType]) VALUES (4,'Account Payable', 2)
End