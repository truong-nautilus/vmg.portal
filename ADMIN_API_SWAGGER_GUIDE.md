# Admin API Endpoints - Swagger Documentation Guide

## 📋 Overview

This document lists all Admin API endpoints that should be added to the Swagger documentation at `http://localhost:64327/swagger/index.html`.

Currently, Swagger only documents Portal APIs (client-facing). This guide helps add Admin/CMS APIs to Swagger.

---

## 🔧 How to Add to Swagger

### Step 1: Add Swagger Attributes to Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("[controller]")]
[SwaggerTag("Admin APIs for user management")]
public class UserAPIController : Controller
{
    [HttpPost("getUsers")]
    [SwaggerOperation(
        Summary = "Get list of users",
        Description = "Returns paginated list of users with filters"
    )]
    [SwaggerResponse(200, "Success", typeof(UserResponse))]
    public JsonResult GetUsers([FromForm] UserListRequest request)
    {
        // Implementation
    }
}
```

---

## 📚 Complete API List

### 1. **UserAPI Controller**

#### GET/POST `/UserAPI/getUsers`
**Summary**: Get list of users with pagination and filters
**Request Body** (FormData):
```json
{
  "pageNumber": 1,
  "pageSize": 50,
  "nickName": "string",
  "userName": "string",
  "status": 0,
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31"
}
```
**Response**:
```json
{
  "Response": 1,
  "Data": {
    "users": [
      {
        "AccountID": 123,
        "UserName": "user123",
        "NickName": "Player1",
        "Balance": 1000000,
        "Status": 1
      }
    ],
    "totalCount": 100
  }
}
```

#### POST `/UserAPI/getUserDetail`
**Summary**: Get detailed information of a specific user
**Request Body**:
```json
{
  "accountId": 123
}
```

#### POST `/UserAPI/lockUser`
**Summary**: Lock/Unlock user account
**Request Body**:
```json
{
  "accountId": 123,
  "reason": "Violation of terms"
}
```

---

### 2. **Group Controller**

#### GET `/Group/Index`
**Summary**: Get all user groups
**Response**: List of groups

#### POST `/Group/FunGroup_Add`
**Summary**: Add new user group
**Request Body**:
```json
{
  "txtName": "New Group Name"
}
```

#### POST `/Group/FunGroup_Edit`
**Summary**: Edit existing group
**Request Body**:
```json
{
  "txtGroupID": "1",
  "txtName": "Updated Group Name"
}
```

#### POST `/Group/Group_Delete`
**Summary**: Toggle group status (soft delete)
**Request Body**:
```json
{
  "GroupID": 1
}
```

---

### 3. **Roles Controller**

#### GET `/Roles/Index`
**Summary**: Get roles management page

#### POST `/Roles/Roles_GetPart`
**Summary**: Get permission tree for a specific group
**Request Body**:
```json
{
  "groupID": 1
}
```

#### POST `/Roles/UpdatePermission`
**Summary**: Update permissions for a group
**Request Body**:
```json
{
  "groupID": 1,
  "functionID": 10,
  "isView": true,
  "isInsert": true,
  "isUpdate": true,
  "isDelete": false,
  "isDisplay": true
}
```

#### POST `/Roles/FunRoles_Add`
**Summary**: Add new role/function
**Request Body**:
```json
{
  "txtName": "New Function",
  "txtParentID": 0,
  "txtUrl": "/admin/function",
  "txtIcon": "fa-cog"
}
```

#### POST `/Roles/FunRoles_Edit`
**Summary**: Edit existing role/function
**Request Body**:
```json
{
  "txtFunctionID": 10,
  "txtName": "Updated Function",
  "txtParentID": 0,
  "txtUrl": "/admin/function",
  "txtIcon": "fa-cog"
}
```

#### POST `/Roles/FunRoles_Delete`
**Summary**: Delete role/function
**Request Body**:
```json
{
  "functionID": 10
}
```

---

### 4. **CashoutAPI Controller**

#### POST `/CashoutAPI/getCashoutList`
**Summary**: Get list of cashout requests
**Request Body**:
```json
{
  "pageNumber": 1,
  "pageSize": 50,
  "status": 0,
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31"
}
```

#### POST `/CashoutAPI/approveCashout`
**Summary**: Approve cashout request
**Request Body**:
```json
{
  "cashoutId": 123,
  "note": "Approved"
}
```

#### POST `/CashoutAPI/rejectCashout`
**Summary**: Reject cashout request
**Request Body**:
```json
{
  "cashoutId": 123,
  "reason": "Insufficient verification"
}
```

---

### 5. **ReportAPI Controller**

#### POST `/ReportAPI/GetTopWin`
**Summary**: Get top winners report
**Request Body**:
```json
{
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31",
  "pageNumber": 1,
  "pageSize": 50
}
```

#### POST `/ReportAPI/GetTopLose`
**Summary**: Get top losers report

#### POST `/ReportAPI/SourceCampainGetList`
**Summary**: Get marketing campaign sources
**Request Body**:
```json
{
  "source": "facebook",
  "pageNumber": 1,
  "pageSize": 50
}
```

#### POST `/ReportAPI/GameStatistic`
**Summary**: Get game statistics
**Request Body**:
```json
{
  "gameId": 1,
  "platform": 1,
  "channel": 1,
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31"
}
```

---

### 6. **Dashboard Controller**

#### POST `/Dashboard/GetReportTotalDaily`
**Summary**: Get daily total statistics
**Request Body**:
```json
{
  "currencyID": 1
}
```
**Response**:
```json
{
  "Response": 1,
  "Data": {
    "totalRegister": 1000,
    "totalDeposit": 500,
    "totalNonDeposit": 500
  }
}
```

#### POST `/Dashboard/GetReportCCU`
**Summary**: Get concurrent users (CCU) by game
**Request Body**:
```json
{
  "currencyID": 1
}
```
**Response**:
```json
{
  "Response": 1,
  "Data": {
    "names": ["Game1", "Game2", "Game3"],
    "colors": ["#FF6384", "#36A2EB", "#FFCE56"],
    "ccus": [150, 200, 100]
  }
}
```

#### POST `/Dashboard/GetReportAccount`
**Summary**: Get account statistics
**Request Body**:
```json
{
  "currencyID": 1
}
```
**Response**:
```json
{
  "Response": 1,
  "Data": {
    "totalBalance": 10000000,
    "totalPendingFund": 500000,
    "totalDepositToday": 1000000,
    "totalWithdrawToday": 500000
  }
}
```

---

### 7. **Game Controller**

#### POST `/Game/UpdateProfitConfig`
**Summary**: Update game profit configuration
**Request Body**:
```json
{
  "gameId": 1,
  "profitPercent": 5.5
}
```

---

### 8. **BotAPI Controller**

#### POST `/botapi/GetBotList`
**Summary**: Get list of bots
**Request Body**:
```json
{
  "pageNumber": 1,
  "pageSize": 50
}
```

---

### 9. **Bot Controller**

#### POST `/bot/AddBot`
**Summary**: Add new bot
**Request Body**:
```json
{
  "username": "bot123",
  "nickname": "Bot Player",
  "gameId": 1
}
```

---

### 10. **Agency Controller**

#### POST `/Agency/GetList`
**Summary**: Get list of agencies
**Request Body**:
```json
{
  "pageNumber": 1,
  "pageSize": 50
}
```

#### POST `/Agency/Create`
**Summary**: Create new agency
**Request Body**:
```json
{
  "name": "New Agency",
  "parentId": 0,
  "commission": 10.5
}
```

#### GET `/Agency/GetListCurrency`
**Summary**: Get list of currencies

---

### 11. **Home Controller**

#### POST `/Home/ConfirmLogin`
**Summary**: Admin login
**Request Body**:
```json
{
  "username": "admin",
  "password": "password123"
}
```

#### GET `/Home/Logout`
**Summary**: Admin logout

#### GET `/Home/GetSessionInfo`
**Summary**: Get current session information
**Response**:
```json
{
  "Response": 1,
  "Data": {
    "AccountID": 1,
    "Username": "admin",
    "GroupID": 1,
    "Mobile": "0123456789"
  }
}
```

#### GET `/Home/GetSidebarMenu`
**Summary**: Get dynamic sidebar menu based on permissions
**Response**:
```json
{
  "Response": 1,
  "Data": [
    {
      "id": 1,
      "name": "Dashboard",
      "url": "/",
      "icon": "fa-dashboard",
      "parentId": 0,
      "children": []
    }
  ]
}
```

---

### 12. **System Controller**

#### GET `/System/AccountHistory`
**Summary**: Get account history

#### GET `/System/LogUserAction`
**Summary**: Get user action logs

#### GET `/System/ListMaintenance`
**Summary**: Get maintenance schedule list

#### GET `/System/CardConfigList`
**Summary**: Get card configuration list

#### GET `/System/PatnerCard`
**Summary**: Get partner cards

#### GET `/System/PartnerConfigList`
**Summary**: Get partner configuration list

#### GET `/System/HistoryAddCard`
**Summary**: Get card addition history

#### GET `/System/HistoryBuyCard`
**Summary**: Get card purchase history

#### GET `/System/StoreCardList`
**Summary**: Get store card list

#### POST `/System/StoreCardInsert`
**Summary**: Insert new store card

#### GET `/System/StoreCardDetail`
**Summary**: Get store card details

#### GET `/System/StoreCardTempList`
**Summary**: Get temporary store card list

#### POST `/System/StoreCardTempInsert`
**Summary**: Insert temporary store card

#### GET `/System/MomoHistoryLog`
**Summary**: Get Momo transaction history

#### GET `/System/LogUpdateGameBank`
**Summary**: Get game bank update logs

---

### 13. **EventManager Controller**

#### GET `/EventManager/LuckeySpinConfig`
**Summary**: Get lucky spin configuration

#### POST `/EventManager/LuckeySpinconfigUpdate`
**Summary**: Update lucky spin configuration

#### GET `/EventManager/LuckeySpinList`
**Summary**: Get lucky spin history

#### GET `/EventManager/LuckeySpinStatistic`
**Summary**: Get lucky spin statistics

#### GET `/EventManager/DepositConfig`
**Summary**: Get deposit event configuration

#### POST `/EventManager/DepositConfigUpdate`
**Summary**: Update deposit event configuration

#### GET `/EventManager/DepositHistory`
**Summary**: Get deposit event history

#### GET `/EventManager/DepositStatistic`
**Summary**: Get deposit event statistics

#### GET `/EventManager/FootballEventExchangeGift`
**Summary**: Get football gift exchange

#### GET `/EventManager/FootballReportEventByUser`
**Summary**: Get football event report by user

#### GET `/EventManager/SummonDragon`
**Summary**: Get summon dragon event

#### GET `/EventManager/SearchDragon`
**Summary**: Search dragon sessions

#### GET `/EventManager/GodOfWealthStatistic`
**Summary**: Get God of Wealth statistics

#### GET `/EventManager/CardEventDoubleCharge`
**Summary**: Get double charge event

#### GET `/EventManager/PlayGameConfig`
**Summary**: Get play game event configuration

#### GET `/EventManager/ShareProfitConfigList`
**Summary**: Get share profit configuration

#### GET `/EventManager/XWealthSlotConfig`
**Summary**: Get X Wealth Slot configuration

---

## 🔐 Common Response Format

All Admin APIs return this standard format:

```json
{
  "Response": 1,
  "Data": {},
  "message": "Success",
  "Description": "Additional info"
}
```

### Response Codes:
- `1`: Success
- `0`: Success (no data)
- `-1`: General error
- `-2`: Authentication error
- `-3`: Permission denied
- `-99`: System error

---

## 🛠️ Implementation Steps

### 1. Install Swashbuckle (if not installed)

```bash
Install-Package Swashbuckle.AspNetCore
```

### 2. Configure Swagger in Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "VMG Portal API", 
            Version = "v1" 
        });
        
        // Add Admin APIs
        c.SwaggerDoc("admin", new OpenApiInfo 
        { 
            Title = "VMG Admin API", 
            Version = "v1",
            Description = "Admin/CMS APIs for VMG Portal management"
        });
        
        c.EnableAnnotations();
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VMG Portal API V1");
        c.SwaggerEndpoint("/swagger/admin/swagger.json", "VMG Admin API V1");
    });
}
```

### 3. Add Attributes to Controllers

```csharp
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "admin")] // Add to Admin group
public class UserAPIController : Controller
{
    [HttpPost("getUsers")]
    [SwaggerOperation(Summary = "Get list of users")]
    [SwaggerResponse(200, "Success", typeof(UserResponse))]
    public JsonResult GetUsers([FromForm] UserListRequest request)
    {
        // Implementation
    }
}
```

### 4. Define Request/Response Models

```csharp
public class UserListRequest
{
    [SwaggerParameter("Page number", Required = false)]
    public int pageNumber { get; set; } = 1;
    
    [SwaggerParameter("Page size", Required = false)]
    public int pageSize { get; set; } = 50;
    
    [SwaggerParameter("Filter by nickname", Required = false)]
    public string nickName { get; set; }
    
    [SwaggerParameter("Filter by username", Required = false)]
    public string userName { get; set; }
}

public class UserResponse
{
    public int Response { get; set; }
    public UserData Data { get; set; }
    public string message { get; set; }
}

public class UserData
{
    public List<User> users { get; set; }
    public int totalCount { get; set; }
}
```

---

## 📝 Example Controller with Full Swagger

```csharp
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "admin")]
[SwaggerTag("User management APIs")]
public class UserAPIController : Controller
{
    [HttpPost("getUsers")]
    [SwaggerOperation(
        Summary = "Get list of users",
        Description = "Returns paginated list of users with optional filters for nickname, username, status, and date range",
        OperationId = "GetUsers",
        Tags = new[] { "UserAPI" }
    )]
    [SwaggerResponse(200, "Success", typeof(UserResponse))]
    [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(500, "Internal Server Error")]
    public JsonResult GetUsers(
        [FromForm, SwaggerParameter("User list request parameters")] UserListRequest request)
    {
        try
        {
            // Implementation
            var users = GetUsersFromDatabase(request);
            
            return Json(new UserResponse
            {
                Response = 1,
                Data = users,
                message = "Success"
            });
        }
        catch (Exception ex)
        {
            return Json(new UserResponse
            {
                Response = -1,
                message = ex.Message
            });
        }
    }
}
```

---

## 🎯 Benefits of Adding to Swagger

1. ✅ **Auto-generated documentation**
2. ✅ **Interactive API testing**
3. ✅ **Type-safe client generation**
4. ✅ **Better team collaboration**
5. ✅ **Easier frontend development**

---

## 📚 Additional Resources

- Swashbuckle Documentation: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- OpenAPI Specification: https://swagger.io/specification/
- ASP.NET Core Swagger: https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger

---

*Generated: 2025-12-06*
*Purpose: Add Admin APIs to Swagger documentation*
*Total Endpoints: 50+*
