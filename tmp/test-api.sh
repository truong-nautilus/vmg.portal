#!/bin/bash

# VMG Portal API Test Script
# Sử dụng tài khoản admin/admin

BASE_URL="http://localhost:64327"
TOKEN=""

echo "========================================="
echo "VMG Portal API Test Suite"
echo "========================================="
echo ""

# Test 1: Login API
echo "📝 Test 1: Login API"
echo "Endpoint: POST /Authen/Login"
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/Authen/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "admin",
    "password": "admin",
    "platformId": 1,
    "merchantId": 1,
    "uiid": "test-device-001",
    "deviceName": "Test Device"
  }')

echo "$LOGIN_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$LOGIN_RESPONSE"
echo ""

# Extract token if login successful
TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"accessToken":"[^"]*"' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    echo "❌ Login failed! Cannot proceed with other tests."
    echo "Response: $LOGIN_RESPONSE"
    exit 1
fi

echo "✅ Login successful!"
echo "Token: ${TOKEN:0:50}..."
echo ""

# Test 2: RefreshToken API
echo "📝 Test 2: RefreshToken API"
echo "Endpoint: GET /Authen/RefreshToken"
REFRESH_RESPONSE=$(curl -s -X GET "$BASE_URL/Authen/RefreshToken" \
  -H "Authorization: Bearer $TOKEN")

echo "$REFRESH_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$REFRESH_RESPONSE"
echo ""

# Test 3: Test API (Health Check)
echo "📝 Test 3: Test API (Health Check)"
echo "Endpoint: GET /Authen/Test"
TEST_RESPONSE=$(curl -s -X GET "$BASE_URL/Authen/Test")

echo "$TEST_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$TEST_RESPONSE"
echo ""

# Test 4: Get Captcha
echo "📝 Test 4: Get Captcha"
echo "Endpoint: GET /Captcha/Get"
CAPTCHA_RESPONSE=$(curl -s -X GET "$BASE_URL/Captcha/Get?length=4")

echo "$CAPTCHA_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$CAPTCHA_RESPONSE"
echo ""

# Test 5: Account API (with auth)
echo "📝 Test 5: Get Location (Account API)"
echo "Endpoint: GET /Account/GetLocation"
LOCATION_RESPONSE=$(curl -s -X GET "$BASE_URL/Account/GetLocation" \
  -H "Authorization: Bearer $TOKEN")

echo "$LOCATION_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$LOCATION_RESPONSE"
echo ""

# Test 6: Get Mobile Codes
echo "📝 Test 6: Get Mobile Codes"
echo "Endpoint: GET /Account/GetMobileCodes"
MOBILE_RESPONSE=$(curl -s -X GET "$BASE_URL/Account/GetMobileCodes" \
  -H "Authorization: Bearer $TOKEN")

echo "$MOBILE_RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$MOBILE_RESPONSE"
echo ""

echo "========================================="
echo "✅ Test Suite Completed!"
echo "========================================="
echo ""
echo "Summary:"
echo "- Login: ✅"
echo "- RefreshToken: Check response above"
echo "- Health Check: Check response above"
echo "- Captcha: Check response above"
echo "- Account APIs: Check responses above"
echo ""
echo "Access Swagger UI: $BASE_URL/swagger/index.html"
