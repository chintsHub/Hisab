
if((select count(*) from AccountType ) = 0)
Begin

INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (1,'Current Asset')
INSERT INTO [dbo].[AccountType] ([Id] ,[Name]) VALUES (2,'Expense')
End