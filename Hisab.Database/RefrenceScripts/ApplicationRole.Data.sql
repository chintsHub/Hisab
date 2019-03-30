
if((select count(*) from ApplicationRole ) = 0)
BEGIN

INSERT INTO [dbo].[ApplicationRole] ([Id] ,[Name] ,[NormalizedName]) VALUES
           (1 ,'Admin' ,'ADMIN')

INSERT INTO [dbo].[ApplicationRole] ([Id] ,[Name] ,[NormalizedName]) VALUES
           (2 ,'App User' ,'APPLICATION USER')



END