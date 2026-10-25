# Generate a new unique 32-hex Trace ID and 16-hex Span ID
$traceId = [System.Guid]::NewGuid().ToString('N')
$spanId = [System.Guid]::NewGuid().ToString('N').Substring(0, 16)
$traceparent = "00-$traceId-$spanId-01"

Write-Host "Assigned Trace ID: $traceId"
Write-Host "W3C traceparent:   $traceparent"

$body = @{
    customerId = 'alice_checkout_test'
    amount = 250.00
    quantity = 2
} | ConvertTo-Json

# Submit checkout request with explicit traceparent header
$resp = Invoke-RestMethod -Uri 'http://localhost:5200/api/orders/checkout' -Method Post -Body $body -ContentType 'application/json' -Headers @{ traceparent = $traceparent }

$orderId = $resp.orderId
Write-Host "Order Submitted:   $orderId"

# Wait 3 seconds for Saga flow (Inventory -> Payment -> Complete) to finish
Start-Sleep -Seconds 3

# Verify Order completion in SQLite DB
$order = Invoke-RestMethod -Uri "http://localhost:5200/api/orders/$orderId"
$saga = Invoke-RestMethod -Uri "http://localhost:5200/api/orders/$orderId/saga-state"

Write-Host "Order Status:      $($order.status)"
Write-Host "Saga State:        $($saga.currentState)"

# Verify in Tempo
Start-Sleep -Seconds 3
$tempoFound = $false
for ($i = 0; $i -lt 5; $i++) {
    try {
        $res = Invoke-RestMethod -Uri "http://localhost:3000/api/datasources/proxy/uid/tempo/api/traces/$traceId" -Headers @{ Authorization = 'Basic YWRtaW46YWRtaW4=' } -ErrorAction Stop
        if ($res.batches) {
            $tempoFound = $true
            break
        }
    } catch {
        Start-Sleep -Seconds 2
    }
}

Write-Host "=================================================="
Write-Host "ORDER ID: $orderId"
Write-Host "TRACE ID: $traceId"
Write-Host "TEMPO STATUS: $(if ($tempoFound) { 'FOUND AND READY IN TEMPO!' } else { 'INDEXING IN TEMPO' })"
Write-Host "=================================================="
