#!/bin/bash

# VMG Admin API Test Script

BASE_URL="http://localhost:64327"

echo "========================================="
echo "VMG Admin API Test Suite"
echo "========================================="
echo ""

# Test 1: Dashboard Daily Report
echo "📝 Test 1: Dashboard Daily Report"
echo "Endpoint: POST /Dashboard/GetReportTotalDaily"
curl -s -X POST "$BASE_URL/Dashboard/GetReportTotalDaily" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "currencyID=1" | python3 -m json.tool || echo "Failed"
echo ""

# Test 2: Dashboard CCU Report
echo "📝 Test 2: Dashboard CCU Report"
echo "Endpoint: POST /Dashboard/GetReportCCU"
curl -s -X POST "$BASE_URL/Dashboard/GetReportCCU" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "currencyID=1" | python3 -m json.tool || echo "Failed"
echo ""

# Test 3: Dashboard Account Report
echo "📝 Test 3: Dashboard Account Report"
echo "Endpoint: POST /Dashboard/GetReportAccount"
curl -s -X POST "$BASE_URL/Dashboard/GetReportAccount" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "currencyID=1" | python3 -m json.tool || echo "Failed"
echo ""

# Test 4: Get Users List
echo "📝 Test 4: Get Users List"
echo "Endpoint: POST /UserAPI/getUsers"
curl -s -X POST "$BASE_URL/UserAPI/getUsers" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "pageNumber=1&pageSize=10" | python3 -m json.tool || echo "Failed"
echo ""

# Test 5: Get User Detail
echo "📝 Test 5: Get User Detail (Admin ID=1)"
echo "Endpoint: POST /UserAPI/getUserDetail"
curl -s -X POST "$BASE_URL/UserAPI/getUserDetail" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "accountId=1" | python3 -m json.tool || echo "Failed"
echo ""

# Test 6: Lock User
echo "📝 Test 6: Lock/Unlock User (TestUser)"
echo "Endpoint: POST /UserAPI/lockUser"
# First find testuser ID (mocking it as 2 based on seed data)
curl -s -X POST "$BASE_URL/UserAPI/lockUser" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "accountId=2&reason=Testing lock" | python3 -m json.tool || echo "Failed"
echo ""
