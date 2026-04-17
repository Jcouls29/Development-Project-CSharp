$ServerName = "(localdb)\MSSQLLocalDB"
$DatabaseName = "SparcpointInventory"
$SqlProjPath = "c:\Repos\Development-Project-CSharp\Development Project\Sparcpoint.Inventory.Database"

Write-Host "Creating database $DatabaseName..."
sqlcmd -S $ServerName -Q "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '$DatabaseName') CREATE DATABASE $DatabaseName;"

$orderedFiles = @(
    "Instances\Instances.sql",
    "Transactions\Transactions.sql",
    "Instances\Tables\Categories.sql",
    "Instances\Tables\Products.sql",
    "Instances\Tables\CategoryAttributes.sql",
    "Instances\Tables\ProductAttributes.sql",
    "Instances\Tables\ProductCategories.sql",
    "Instances\Tables\CategoryCategories.sql",
    "Transactions\Tables\InventoryTransactions.sql"
)

# Table types can be tricky with GO statements, but sqlcmd handles 'GO' successfully.
$tableTypeFiles = @(
    "Table Types\IntegerList.sql",
    "Table Types\CustomAttributeList.sql",
    "Table Types\StringList.sql",
    "Table Types\CorrelatedCustomAttributeList.sql",
    "Table Types\CorrelatedIntegerList.sql",
    "Table Types\CorrelatedStringList.sql",
    "Instances\Table Types\CorrelatedProductInstanceList.sql",
    "Instances\Table Types\CorrelatedListItemList.sql"
)

Write-Host "Creating schemas and tables..."
foreach ($file in $orderedFiles) {
    $fullPath = Join-Path $SqlProjPath $file
    Write-Host "Executing $fullPath"
    sqlcmd -S $ServerName -d $DatabaseName -i "`"$fullPath`""
}

Write-Host "Creating table types..."
foreach ($file in $tableTypeFiles) {
    if (Test-Path (Join-Path $SqlProjPath $file)) {
        $fullPath = Join-Path $SqlProjPath $file
        Write-Host "Executing $fullPath"
        sqlcmd -S $ServerName -d $DatabaseName -i "`"$fullPath`""
    }
}

Write-Host "Database Setup Complete!"
