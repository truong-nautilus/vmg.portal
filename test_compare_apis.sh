#!/bin/bash

TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJBY2NvdW50SWQiOiIzIiwiVXNlck5hbWUiOiJhbG90ZXN0NCIsIk5pY2tOYW1lIjoiYWxvdGVzdDQiLCJQbGF0Zm9ybUlkIjoiMSIsIk1lcmNoYW50SWQiOiIxMDAwIiwiSXNBZ2VuY3kiOiIwIiwiSXNPVFAiOiJGYWxzZSIsIkxvY2F0aW9uSWQiOiIxNiIsIlByZUZpeCI6IlZOIiwianRpIjoiZWQxNmMyN2ItNzNlZS00MjljLTg0OGUtZGM3ZmU4ZmQ4YjE0IiwiQ3VycmVuY3lUeXBlIjoiMCIsIm5iZiI6MTc2NTc4ODEwOCwiZXhwIjoxNzY1ODMxMzA4LCJpYXQiOjE3NjU3ODgxMDh9.G5C5d5j1FyZ2aO9nMutgUZ1W84SJHdAAQzk6qfjmgb0"

echo "=========================================="
echo "So sánh API GetAccountBalance"
echo "=========================================="
echo ""

echo "1️⃣  PRODUCTION (api.3b1.site)"
echo "-------------------------------------------"
PROD_RESPONSE=$(curl -s 'https://api.3b1.site/Account/GetAccountBalance' \
  -H 'accept: application/json' \
  -H "authorization: Bearer $TOKEN" \
  -H 'language: vi')

echo "$PROD_RESPONSE" | jq '.'
PROD_CODE=$(echo "$PROD_RESPONSE" | jq -r '.code')

if [ "$PROD_CODE" = "0" ]; then
    echo "✅ Production: SUCCESS"
elif [ "$PROD_CODE" = "1047" ]; then
    echo "❌ Production: ERROR 1047"
else
    echo "⚠️  Production: Code $PROD_CODE"
fi

echo ""
echo "2️⃣  LOCALHOST (localhost:64327)"
echo "-------------------------------------------"
LOCAL_RESPONSE=$(curl -s 'http://localhost:64327/Account/GetAccountBalance' \
  -H 'accept: application/json' \
  -H "authorization: Bearer $TOKEN" \
  -H 'language: vi')

echo "$LOCAL_RESPONSE" | jq '.'
LOCAL_CODE=$(echo "$LOCAL_RESPONSE" | jq -r '.code')

if [ "$LOCAL_CODE" = "0" ]; then
    echo "✅ Localhost: SUCCESS"
elif [ "$LOCAL_CODE" = "1047" ]; then
    echo "❌ Localhost: ERROR 1047"
else
    echo "⚠️  Localhost: Code $LOCAL_CODE"
fi

echo ""
echo "=========================================="
echo "KẾT LUẬN"
echo "=========================================="

if [ "$PROD_CODE" = "$LOCAL_CODE" ]; then
    echo "✅ Production và Localhost trả về cùng kết quả (code: $PROD_CODE)"
else
    echo "⚠️  Production (code: $PROD_CODE) khác Localhost (code: $LOCAL_CODE)"
fi
