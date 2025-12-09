#!/bin/bash

# VMG Admin API - Comprehensive Test with Validation
# Tests all 52 Admin API endpoints with proper error checking

BASE_URL="http://localhost:64327"
PASS_COUNT=0
FAIL_COUNT=0

echo "========================================="
echo "VMG Admin API - Comprehensive Validation"
echo "========================================="
echo ""

# Helper function to test endpoint
test_endpoint() {
    local name="$1"
    local method="$2"
    local endpoint="$3"
    local data="$4"
    
    echo "🔍 Testing: $name"
    echo "   Endpoint: $method $endpoint"
    
    if [ "$method" = "GET" ]; then
        response=$(curl -s "$BASE_URL$endpoint")
    else
        response=$(curl -s -X POST "$BASE_URL$endpoint" -d "$data")
    fi
    
    # Check if response contains "response" field
    if echo "$response" | grep -q '"response"'; then
        response_code=$(echo "$response" | python3 -c "import sys, json; print(json.load(sys.stdin).get('response', -99))" 2>/dev/null)
        
        if [ "$response_code" = "1" ]; then
            echo "   ✅ PASS (Response: $response_code)"
            ((PASS_COUNT++))
        else
            echo "   ❌ FAIL (Response: $response_code)"
            echo "   Response: $response"
            ((FAIL_COUNT++))
        fi
    else
        echo "   ❌ FAIL (Invalid JSON or missing 'response' field)"
        echo "   Response: $response"
        ((FAIL_COUNT++))
    fi
    echo ""
}

echo "=== 1. UserAPI Controller (3 endpoints) ==="
test_endpoint "Get Users List" "POST" "/UserAPI/getUsers" "pageNumber=1&pageSize=10"
test_endpoint "Get User Detail" "POST" "/UserAPI/getUserDetail" "accountId=1"
test_endpoint "Lock/Unlock User" "POST" "/UserAPI/lockUser" "accountId=1&reason=Test"

echo "=== 2. Dashboard Controller (3 endpoints) ==="
test_endpoint "Daily Report" "POST" "/Dashboard/GetReportTotalDaily" "currencyID=1"
test_endpoint "CCU Report" "POST" "/Dashboard/GetReportCCU" "currencyID=1"
test_endpoint "Account Report" "POST" "/Dashboard/GetReportAccount" "currencyID=1"

echo "=== 3. Group Controller (4 endpoints) ==="
test_endpoint "Get Groups" "GET" "/Group/Index" ""
test_endpoint "Add Group" "POST" "/Group/FunGroup_Add" "txtName=TestGroup"
test_endpoint "Edit Group" "POST" "/Group/FunGroup_Edit" "txtGroupID=1&txtName=Updated"
test_endpoint "Delete Group" "POST" "/Group/Group_Delete" "GroupID=1"

echo "=== 4. Roles Controller (6 endpoints) ==="
test_endpoint "Roles Index" "GET" "/Roles/Index" ""
test_endpoint "Get Permissions" "POST" "/Roles/Roles_GetPart" "groupID=1"
test_endpoint "Update Permission" "POST" "/Roles/UpdatePermission" "groupID=1&functionID=1"
test_endpoint "Add Function" "POST" "/Roles/FunRoles_Add" "txtName=NewFunc"
test_endpoint "Edit Function" "POST" "/Roles/FunRoles_Edit" "txtFunctionID=1"
test_endpoint "Delete Function" "POST" "/Roles/FunRoles_Delete" "functionID=1"

echo "=== 5. Cashout Controller (3 endpoints) ==="
test_endpoint "Get Cashout List" "POST" "/CashoutAPI/getCashoutList" "pageNumber=1"
test_endpoint "Approve Cashout" "POST" "/CashoutAPI/approveCashout" "cashoutId=1"
test_endpoint "Reject Cashout" "POST" "/CashoutAPI/rejectCashout" "cashoutId=1"

echo "=== 6. Report Controller (4 endpoints) ==="
test_endpoint "Top Winners" "POST" "/ReportAPI/GetTopWin" "pageNumber=1"
test_endpoint "Top Losers" "POST" "/ReportAPI/GetTopLose" "pageNumber=1"
test_endpoint "Campaign Sources" "POST" "/ReportAPI/SourceCampainGetList" "source=fb"
test_endpoint "Game Statistics" "POST" "/ReportAPI/GameStatistic" "gameId=1"

echo "=== 7. Game Controller (1 endpoint) ==="
test_endpoint "Update Profit Config" "POST" "/Game/UpdateProfitConfig" "gameId=1&profitPercent=5"

echo "=== 8. Bot Controllers (2 endpoints) ==="
test_endpoint "Get Bot List" "POST" "/BotAPI/GetBotList" "pageNumber=1"
test_endpoint "Add Bot" "POST" "/Bot/AddBot" "username=bot1"

echo "=== 9. Agency Controller (3 endpoints) ==="
test_endpoint "Get Agency List" "POST" "/Agency/GetList" "pageNumber=1"
test_endpoint "Create Agency" "POST" "/Agency/Create" "name=TestAgency"
test_endpoint "Get Currencies" "GET" "/Agency/GetListCurrency" ""

echo "=== 10. Home Controller (4 endpoints) ==="
test_endpoint "Admin Login" "POST" "/Home/ConfirmLogin" "username=admin&password=123"
test_endpoint "Admin Logout" "GET" "/Home/Logout" ""
test_endpoint "Get Session Info" "GET" "/Home/GetSessionInfo" ""
test_endpoint "Get Sidebar Menu" "GET" "/Home/GetSidebarMenu" ""

echo "=== 11. System Controller (15 endpoints) ==="
test_endpoint "Account History" "GET" "/System/AccountHistory" ""
test_endpoint "User Action Logs" "GET" "/System/LogUserAction" ""
test_endpoint "Maintenance List" "GET" "/System/ListMaintenance" ""
test_endpoint "Card Config" "GET" "/System/CardConfigList" ""
test_endpoint "Partner Card" "GET" "/System/PatnerCard" ""
test_endpoint "Partner Config" "GET" "/System/PartnerConfigList" ""
test_endpoint "Card Add History" "GET" "/System/HistoryAddCard" ""
test_endpoint "Card Buy History" "GET" "/System/HistoryBuyCard" ""
test_endpoint "Store Card List" "GET" "/System/StoreCardList" ""
test_endpoint "Store Card Insert" "POST" "/System/StoreCardInsert" ""
test_endpoint "Store Card Detail" "GET" "/System/StoreCardDetail" ""
test_endpoint "Store Card Temp List" "GET" "/System/StoreCardTempList" ""
test_endpoint "Store Card Temp Insert" "POST" "/System/StoreCardTempInsert" ""
test_endpoint "Momo History" "GET" "/System/MomoHistoryLog" ""
test_endpoint "Game Bank Logs" "GET" "/System/LogUpdateGameBank" ""

echo "=== 12. Event Manager Controller (17 endpoints) ==="
test_endpoint "Lucky Spin Config" "GET" "/EventManager/LuckeySpinConfig" ""
test_endpoint "Update Lucky Spin" "POST" "/EventManager/LuckeySpinconfigUpdate" ""
test_endpoint "Lucky Spin List" "GET" "/EventManager/LuckeySpinList" ""
test_endpoint "Lucky Spin Stats" "GET" "/EventManager/LuckeySpinStatistic" ""
test_endpoint "Deposit Config" "GET" "/EventManager/DepositConfig" ""
test_endpoint "Update Deposit Config" "POST" "/EventManager/DepositConfigUpdate" ""
test_endpoint "Deposit History" "GET" "/EventManager/DepositHistory" ""
test_endpoint "Deposit Stats" "GET" "/EventManager/DepositStatistic" ""
test_endpoint "Football Gift Exchange" "GET" "/EventManager/FootballEventExchangeGift" ""
test_endpoint "Football Report" "GET" "/EventManager/FootballReportEventByUser" ""
test_endpoint "Summon Dragon" "GET" "/EventManager/SummonDragon" ""
test_endpoint "Search Dragon" "GET" "/EventManager/SearchDragon" ""
test_endpoint "God of Wealth Stats" "GET" "/EventManager/GodOfWealthStatistic" ""
test_endpoint "Card Double Charge" "GET" "/EventManager/CardEventDoubleCharge" ""
test_endpoint "Play Game Config" "GET" "/EventManager/PlayGameConfig" ""
test_endpoint "Share Profit Config" "GET" "/EventManager/ShareProfitConfigList" ""
test_endpoint "X Wealth Slot Config" "GET" "/EventManager/XWealthSlotConfig" ""

echo "========================================="
echo "Test Summary"
echo "========================================="
echo "✅ Passed: $PASS_COUNT"
echo "❌ Failed: $FAIL_COUNT"
echo "Total: $((PASS_COUNT + FAIL_COUNT))"
echo ""

if [ $FAIL_COUNT -eq 0 ]; then
    echo "🎉 All tests passed!"
    exit 0
else
    echo "⚠️  Some tests failed. Please review the output above."
    exit 1
fi
