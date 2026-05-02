param(
    [string]$BaseUrl = "http://localhost:5000"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-JsonRequest {
    param(
        [string]$Method,
        [string]$Uri,
        [object]$Body = $null
    )

    if ($null -eq $Body) {
        return Invoke-RestMethod -Method $Method -Uri $Uri
    }

    $json = $Body | ConvertTo-Json -Depth 10 -Compress
    return Invoke-RestMethod -Method $Method -Uri $Uri -ContentType "application/json" -Body $json
}

Write-Host "Checking API availability at $BaseUrl"
$products = Invoke-JsonRequest -Method Get -Uri "$BaseUrl/api/v1/products"
Write-Host "API responded. Existing product count: $($products.Count)"

$categoryRequest = @{
    name = "Hardware"
    description = "General hardware items"
    attributes = @{
        department = "Tools"
    }
    parentCategoryIds = @()
}

$category = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/categories" -Body $categoryRequest
Write-Host "Created category $($category.categoryId): $($category.name)"

$productRequest = @{
    name = "Blue Hammer"
    description = "16oz claw hammer"
    productImageUris = @()
    validSkus = @("HAM-001")
    attributes = @{
        color = "Blue"
        brand = "Acme"
    }
    categoryIds = @($category.categoryId)
}

$product = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/products" -Body $productRequest
Write-Host "Created product $($product.productId): $($product.name)"

$inventoryAddRequest = @{
    typeCategory = "initial-load"
    items = @(
        @{
            productId = $product.productId
            quantity = 25
        }
    )
}

$addTransactions = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/inventory/add" -Body $inventoryAddRequest
$addTransaction = @($addTransactions)[0]
Write-Host "Added inventory transaction $($addTransaction.transactionId) with quantity $($addTransaction.quantity)"

$searchRequest = @{
    attributes = @{
        brand = "Acme"
    }
}

$searchResults = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/products/search" -Body $searchRequest
Write-Host "Search by brand returned $($searchResults.Count) product(s)"

$countRequest = @{
    productId = $product.productId
}

$countBeforeRemoval = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/inventory/counts/query" -Body $countRequest
Write-Host "Inventory before removal: totalQuantity=$($countBeforeRemoval.totalQuantity)"

$inventoryRemoveRequest = @{
    typeCategory = "sale"
    items = @(
        @{
            productId = $product.productId
            quantity = 5
        }
    )
}

$removeTransactions = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/inventory/remove" -Body $inventoryRemoveRequest
$removeTransaction = @($removeTransactions)[0]
Write-Host "Removed inventory transaction $($removeTransaction.transactionId) with quantity $($removeTransaction.quantity)"

$countAfterRemoval = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/inventory/counts/query" -Body $countRequest
Write-Host "Inventory after removal: totalQuantity=$($countAfterRemoval.totalQuantity)"

Invoke-RestMethod -Method Delete -Uri "$BaseUrl/api/v1/inventory/transactions/$($removeTransaction.transactionId)" | Out-Null
$countAfterUndo = Invoke-JsonRequest -Method Post -Uri "$BaseUrl/api/v1/inventory/counts/query" -Body $countRequest
Write-Host "Inventory after deleting the removal transaction: totalQuantity=$($countAfterUndo.totalQuantity)"

Write-Host ""
Write-Host "Demo complete."
Write-Host "CategoryId: $($category.categoryId)"
Write-Host "ProductId: $($product.productId)"
Write-Host "Initial transactionId: $($addTransaction.transactionId)"
Write-Host "Removal transactionId: $($removeTransaction.transactionId)"
