
if EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventTransactionJournal') 
	BEGIN
		DELETE FROM [dbo].[EventTransactionJournal]
	End

if EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventAccount') 
	BEGIN
		DELETE FROM [dbo].[EventAccount]
	End

if EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EventTransaction') 
	BEGIN
		DELETE FROM [dbo].[EventTransaction] where TransactionType in (2,4) -- Pool related transactions
	End
