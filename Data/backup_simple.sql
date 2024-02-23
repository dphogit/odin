-- Performs a full backup of the odin database using the simple recovery model

DECLARE @BackupPath NVARCHAR(500)
DECLARE @Date NVARCHAR(30)

-- Get current date in yyyy-mm-dd format (ISO 8601)
SET @Date = REPLACE(CONVERT(NVARCHAR, GETDATE(), 23), '-', '');

-- Define the path to the backup file
SET @BackupPath = N'/var/opt/mssql/backup/odin_' + @Date + N'.bak';

-- Use simple recovery model
USE master;
ALTER DATABASE odin SET RECOVERY SIMPLE;
GO

-- Backup the database
BACKUP DATABASE odin TO DISK = @BackupPath WITH FORMAT;
GO
