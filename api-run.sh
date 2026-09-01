curl -X POST http://localhost:5000/api/ledger/settle \
  -H "Content-Type: application/json" \
  -H "X-POS-Terminal-ID: 77" \
  -H "X-POS-Security-Token: POS-SECURE-KEY-HASH-V2" \
  -d '{"Amount": 1250.00, "TargetAssetPool": "USD Core Ledger Pool"}'
