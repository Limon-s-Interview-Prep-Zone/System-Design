$baseUrl = "http://localhost:5200"
try {
    $null = Invoke-RestMethod -Uri "$baseUrl/api/orders" -Method Get -TimeoutSec 1 -ErrorAction Stop
} catch {
    $baseUrl = "http://localhost:5150"
}
Write-Host "Targeting API at $baseUrl"

Write-Host "=================================================="
Write-Host "TEST 1: Forward Flow (Happy Path)"
Write-Host "=================================================="
$happyBody = @{ customerId = "alice_happy"; amount = 150.00; quantity = 2 } | ConvertTo-Json
$resp1 = Invoke-RestMethod -Uri "$baseUrl/api/orders/checkout" -Method Post -Body $happyBody -ContentType "application/json"
$orderId1 = $resp1.orderId
Write-Host "Submitted Order 1: $orderId1"

Start-Sleep -Seconds 3

$order1 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId1"
$saga1 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId1/saga-state"

Write-Host "Order 1 Status: $($order1.status)"
Write-Host "Saga 1 State:   $($saga1.currentState)"

Write-Host "=================================================="
Write-Host "TEST 2: Failure & Compensation Flow (Payment Fails)"
Write-Host "=================================================="
$resp2 = Invoke-RestMethod -Uri "$baseUrl/api/orders/checkout/fail-payment" -Method Post
$orderId2 = $resp2.orderId
Write-Host "Submitted Order 2: $orderId2"

Start-Sleep -Seconds 3

$order2 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId2"
$saga2 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId2/saga-state"

Write-Host "Order 2 Status:         $($order2.status)"
Write-Host "Order 2 Failure Reason: $($order2.failureReason)"
Write-Host "Saga 2 State:          $($saga2.currentState)"
Write-Host "Saga 2 Failure Reason: $($saga2.failureReason)"

Write-Host "=================================================="
Write-Host "TEST 3: Failure Flow (Inventory Out of Stock)"
Write-Host "=================================================="
$resp3 = Invoke-RestMethod -Uri "$baseUrl/api/orders/checkout/fail-inventory" -Method Post
$orderId3 = $resp3.orderId
Write-Host "Submitted Order 3: $orderId3"

Start-Sleep -Seconds 3

$order3 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId3"
$saga3 = Invoke-RestMethod -Uri "$baseUrl/api/orders/$orderId3/saga-state"

Write-Host "Order 3 Status:         $($order3.status)"
Write-Host "Order 3 Failure Reason: $($order3.failureReason)"
Write-Host "Saga 3 State:          $($saga3.currentState)"
