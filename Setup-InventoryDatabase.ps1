param(
    [string]$Server = ".",
    [string]$Database = "inventory",
    [switch]$Reset
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Data

function Invoke-SqlBatch {
    param(
        [string]$ConnectionString,
        [string]$CommandText
    )

    if ([string]::IsNullOrWhiteSpace($CommandText)) {
        return
    }

    $connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $CommandText
        [void]$command.ExecuteNonQuery()
    }
    finally {
        $connection.Dispose()
    }
}

function Invoke-SqlFile {
    param(
        [string]$ConnectionString,
        [string]$FilePath
    )

    if (-not (Test-Path -LiteralPath $FilePath)) {
        throw "SQL file not found: $FilePath"
    }

    Write-Host "Running $FilePath"
    $content = Get-Content -LiteralPath $FilePath -Raw
    $batches = [System.Text.RegularExpressions.Regex]::Split($content, "(?im)^\s*GO\s*$")

    foreach ($batch in $batches) {
        $trimmed = $batch.Trim()
        if ($trimmed.Length -eq 0) {
            continue
        }

        Invoke-SqlBatch -ConnectionString $ConnectionString -CommandText $trimmed
    }
}

function Invoke-SqlQuery {
    param(
        [string]$ConnectionString,
        [string]$CommandText
    )

    $connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 120
        $command.CommandText = $CommandText

        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        return $table
    }
    finally {
        $connection.Dispose()
    }
}

$root = $PSScriptRoot
$masterConnectionString = "Data Source=$Server;Initial Catalog=master;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"
$databaseConnectionString = "Data Source=$Server;Initial Catalog=$Database;Integrated Security=True;Encrypt=False;TrustServerCertificate=True"

$dropDatabaseCommand = @"
IF DB_ID(N'$Database') IS NOT NULL
BEGIN
    ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$Database];
END
"@

$createDatabaseCommand = @"
IF DB_ID(N'$Database') IS NULL
BEGIN
    CREATE DATABASE [$Database];
END
"@

if ($Reset) {
    Write-Host "Resetting database [$Database] on server [$Server]"
    Invoke-SqlBatch -ConnectionString $masterConnectionString -CommandText $dropDatabaseCommand
}

Invoke-SqlBatch -ConnectionString $masterConnectionString -CommandText $createDatabaseCommand

$sqlFiles = @(
    "Development Project\Sparcpoint.Inventory.Database\Instances\Instances.sql",
    "Development Project\Sparcpoint.Inventory.Database\Transactions\Transactions.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\IntegerList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\StringList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\CustomAttributeList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedIntegerList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedStringList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Table Types\CorrelatedCustomAttributeList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Table Types\CorrelatedListItemList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Table Types\CorrelatedProductInstanceList.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\Categories.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\Products.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\CategoryAttributes.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\ProductAttributes.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\ProductCategories.sql",
    "Development Project\Sparcpoint.Inventory.Database\Instances\Tables\CategoryCategories.sql",
    "Development Project\Sparcpoint.Inventory.Database\Transactions\Tables\InventoryTransactions.sql"
)

foreach ($relativePath in $sqlFiles) {
    $absolutePath = Join-Path $root $relativePath
    Invoke-SqlFile -ConnectionString $databaseConnectionString -FilePath $absolutePath
}

$verifyCommand = @"
SELECT
    s.name AS SchemaName,
    t.name AS TableName
FROM sys.tables t
INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE s.name IN ('Instances', 'Transactions')
ORDER BY s.name, t.name;
"@

Write-Host ""
Write-Host "Database setup complete. Created tables:"
$results = Invoke-SqlQuery -ConnectionString $databaseConnectionString -CommandText $verifyCommand
$results | Format-Table -AutoSize
