# This script performs the automation to run the backup SQL script for the odin database in the SQL
# Server docker container.

# Check if the current directory is the `Data` directory before running the script
$invocationDirectory = Split-Path -Leaf -Path $PWD
if ($invocationDirectory -ne "Data") {
    Write-Host "[ERROR]: Script must be executed from the `"Data`" directory" -ForegroundColor Red
    exit
}

# Prompt for MSSQL SA password (the same one in .env file)
$password_secure_string = Read-Host "Enter MSSQL SA Password" -AsSecureString

$date = Get-Date -Format "yyyy_MM_dd"

$backupSqlScriptName = "backup_simple.sql"
$backupFileName = "odin_$date.bak"

$hostScriptPath = "$backupSqlScriptName"
$hostBackupDir = "Backups"

$containerName = "sql1"
$containerScriptsDir = "/scripts"
$containerBackupFile = "/var/opt/mssql/backup/$backupFileName"

# Create the /scripts directory in the container if it doesn't exist
docker exec -it $containerName mkdir -p $containerScriptsDir

# Copy the backup sql script to the container
docker cp $hostScriptPath ${containerName}:$containerScriptsDir/$backupSqlScriptName

# Run the backup script in the container
& {
    param([SecureString] $password_secure_string)

    # Converts it to plain text for use - this is not the most secure way but this app is intended for local use as of writing this
    $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password_secure_string))
    docker exec -it $containerName /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $password -i $containerScriptsDir/$backupSqlScriptName
} $password_secure_string

# Copy the backup file from the container to the host, create the backup directory (ignore existing directory error)
New-Item -Path $hostBackupDir -ItemType Directory -ErrorAction SilentlyContinue
docker cp ${containerName}:$containerBackupFile $hostBackupDir/$backupFileName
