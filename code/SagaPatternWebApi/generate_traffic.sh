#!/usr/bin/env bash
set -e

API_URL="http://localhost:5200"

echo "=== 🚀 Generating Saga Observability Traffic for Grafana ==="

CUSTOMERS=("alice" "bob" "charlie" "david" "emma" "frank" "grace" "hannah" "ian" "julia")

# 1. Forward Flows (Happy Path - Completed Orders)
echo "--- 🟢 Generating 12 Successful Checkout Flows ---"
ORDER_IDS=()
for i in {1..12}; do
  CUST=${CUSTOMERS[$((RANDOM % ${#CUSTOMERS[@]}))]}
  AMT=$(( (RANDOM % 800) + 50 ))
  QTY=$(( (RANDOM % 5) + 1 ))
  RESP=$(curl -s -X POST "$API_URL/api/orders/checkout" \
    -H "Content-Type: application/json" \
    -d "{\"customerId\":\"$CUST\",\"amount\":$AMT.00,\"quantity\":$QTY}")
  ORDER_ID=$(echo "$RESP" | grep -o '"orderId":"[^"]*' | cut -d'"' -f4)
  if [ -n "$ORDER_ID" ]; then
    ORDER_IDS+=("$ORDER_ID")
    echo "  [Happy Path] Order $ORDER_ID created for $CUST ($$AMT, qty: $QTY)"
  fi
  sleep 0.2
done

# 2. Payment Failure Flows (Compensation Triggered)
echo "--- 🔴 Generating 6 Payment Decline & Compensation Flows ---"
for i in {1..6}; do
  CUST="${CUSTOMERS[$((RANDOM % ${#CUSTOMERS[@]}))]}_payment_fail"
  AMT=$(( (RANDOM % 1500) + 1100 )) # > $1000 limit
  RESP=$(curl -s -X POST "$API_URL/api/orders/checkout/fail-payment?customerId=$CUST&amount=$AMT")
  ORDER_ID=$(echo "$RESP" | grep -o '"orderId":"[^"]*' | cut -d'"' -f4)
  echo "  [Payment Declined] Order $ORDER_ID ($$AMT > $$1000 limit) -> Compensation executed"
  sleep 0.2
done

# 3. Inventory Out-of-Stock Flows
echo "--- 🟠 Generating 4 Inventory Out-of-Stock Flows ---"
for i in {1..4}; do
  CUST="${CUSTOMERS[$((RANDOM % ${#CUSTOMERS[@]}))]}_stock_fail"
  QTY=$(( (RANDOM % 30) + 15 )) # > 10 stock limit
  RESP=$(curl -s -X POST "$API_URL/api/orders/checkout/fail-inventory?customerId=$CUST&quantity=$QTY")
  ORDER_ID=$(echo "$RESP" | grep -o '"orderId":"[^"]*' | cut -d'"' -f4)
  echo "  [Stock Depleted] Order $ORDER_ID (qty $QTY > 10 stock) -> Auto-cancelled"
  sleep 0.2
done

# 4. Status and Saga State Queries
echo "--- 🔍 Generating Read Queries for Saga State & Order History ---"
for ID in "${ORDER_IDS[@]:0:6}"; do
  curl -s "$API_URL/api/orders/$ID" > /dev/null
  curl -s "$API_URL/api/orders/$ID/saga-state" > /dev/null
  sleep 0.1
done
curl -s "$API_URL/api/orders" > /dev/null

echo "=== ✅ Traffic Generation Completed Successfully! ==="

