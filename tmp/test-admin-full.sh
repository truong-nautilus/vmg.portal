#!/bin/bash

BASE_URL="http://localhost:64327"
echo "========================================="
echo "VMG Admin API FULL Test Suite (50+ Endpoints)"
echo "========================================="

# Helper function to print result
check_status() {
    if [[ $1 == *"Response"* ]]; then
        echo "✅ PASS"
    else
        echo "❌ FAIL"
        echo "Response: $1"
    fi
    echo "-----------------------------------------"
}

# 1. UserAPI
echo "👉 Testing UserAPI..."
curl -s -X POST "$BASE_URL/UserAPI/getUsers" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/UserAPI/getUserDetail" -d "accountId=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/UserAPI/lockUser" -d "accountId=1&reason=Test" | python3 -m json.tool || echo "Failed"

# 2. Group
echo "👉 Testing GroupController..."
curl -s "$BASE_URL/Group/Index" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Group/FunGroup_Add" -d "txtName=TestGroup" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Group/FunGroup_Edit" -d "txtGroupID=1&txtName=Edit" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Group/Group_Delete" -d "GroupID=1" | python3 -m json.tool || echo "Failed"

# 3. Roles
echo "👉 Testing RolesController..."
curl -s "$BASE_URL/Roles/Index" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Roles/Roles_GetPart" -d "groupID=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Roles/UpdatePermission" -d "groupID=1&functionID=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Roles/FunRoles_Add" -d "txtName=Func" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Roles/FunRoles_Edit" -d "txtFunctionID=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Roles/FunRoles_Delete" -d "functionID=1" | python3 -m json.tool || echo "Failed"

# 4. Cashout
echo "👉 Testing CashoutAPIController..."
curl -s -X POST "$BASE_URL/CashoutAPI/getCashoutList" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/CashoutAPI/approveCashout" -d "cashoutId=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/CashoutAPI/rejectCashout" -d "cashoutId=1" | python3 -m json.tool || echo "Failed"

# 5. Report
echo "👉 Testing ReportAPIController..."
curl -s -X POST "$BASE_URL/ReportAPI/GetTopWin" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/ReportAPI/GetTopLose" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/ReportAPI/SourceCampainGetList" -d "source=fb" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/ReportAPI/GameStatistic" -d "gameId=1" | python3 -m json.tool || echo "Failed"

# 6. Dashboard
echo "👉 Testing DashboardController..."
curl -s -X POST "$BASE_URL/Dashboard/GetReportTotalDaily" -d "currencyID=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Dashboard/GetReportCCU" -d "currencyID=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Dashboard/GetReportAccount" -d "currencyID=1" | python3 -m json.tool || echo "Failed"

# 7. Game
echo "👉 Testing GameController..."
curl -s -X POST "$BASE_URL/Game/UpdateProfitConfig" -d "gameId=1&profitPercent=5" | python3 -m json.tool || echo "Failed"

# 8. BotAPI
echo "👉 Testing BotAPIController..."
curl -s -X POST "$BASE_URL/BotAPI/GetBotList" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"

# 9. Bot
echo "👉 Testing BotController..."
curl -s -X POST "$BASE_URL/Bot/AddBot" -d "username=bot" | python3 -m json.tool || echo "Failed"

# 10. Agency
echo "👉 Testing AgencyController..."
curl -s -X POST "$BASE_URL/Agency/GetList" -d "pageNumber=1" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/Agency/Create" -d "name=Agency" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/Agency/GetListCurrency" | python3 -m json.tool || echo "Failed"

# 11. Home
echo "👉 Testing HomeController..."
curl -s -X POST "$BASE_URL/Home/ConfirmLogin" -d "username=admin&password=123" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/Home/Logout" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/Home/GetSessionInfo" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/Home/GetSidebarMenu" | python3 -m json.tool || echo "Failed"

# 12. System
echo "👉 Testing SystemController..."
curl -s "$BASE_URL/System/AccountHistory" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/LogUserAction" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/ListMaintenance" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/CardConfigList" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/PatnerCard" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/PartnerConfigList" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/HistoryAddCard" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/HistoryBuyCard" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/StoreCardList" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/System/StoreCardInsert" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/StoreCardDetail" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/StoreCardTempList" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/System/StoreCardTempInsert" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/MomoHistoryLog" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/System/LogUpdateGameBank" | python3 -m json.tool || echo "Failed"

# 13. EventManager
echo "👉 Testing EventManagerController..."
curl -s "$BASE_URL/EventManager/LuckeySpinConfig" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/EventManager/LuckeySpinconfigUpdate" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/LuckeySpinList" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/LuckeySpinStatistic" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/DepositConfig" | python3 -m json.tool || echo "Failed"
curl -s -X POST "$BASE_URL/EventManager/DepositConfigUpdate" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/DepositHistory" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/DepositStatistic" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/FootballEventExchangeGift" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/FootballReportEventByUser" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/SummonDragon" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/SearchDragon" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/GodOfWealthStatistic" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/CardEventDoubleCharge" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/PlayGameConfig" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/ShareProfitConfigList" | python3 -m json.tool || echo "Failed"
curl -s "$BASE_URL/EventManager/XWealthSlotConfig" | python3 -m json.tool || echo "Failed"

echo ""
echo "========================================="
echo "Test Complete"
echo "========================================="
