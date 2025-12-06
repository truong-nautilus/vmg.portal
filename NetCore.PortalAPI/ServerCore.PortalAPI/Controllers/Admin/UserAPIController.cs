using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using ServerCore.PortalAPI.Services;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Controllers.Admin
{
    /// <summary>
    /// Admin User Management APIs
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("User management APIs for admin panel")]
    public class UserAPIController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public UserAPIController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Get list of users with pagination and filters
        /// </summary>
        /// <param name="request">User list request parameters</param>
        /// <returns>Paginated list of users</returns>
        [HttpPost("getUsers")]
        [SwaggerOperation(
            Summary = "Get list of users",
            Description = "Returns paginated list of users with optional filters for nickname, username, status, and date range",
            OperationId = "GetUsers"
        )]
        [SwaggerResponse(200, "Success", typeof(UserListResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(500, "Internal Server Error")]
        public async Task<IActionResult> GetUsers([FromForm] UserListRequest request)
        {
            try
            {
                var sql = "SELECT AccountID, UserName, NickName, Balance, Status, CreatedDate, LastLoginDate FROM Accounts WHERE 1=1";
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(request.userName))
                {
                    sql += " AND UserName LIKE @UserName";
                    parameters.Add("UserName", $"%{request.userName}%");
                }

                if (!string.IsNullOrEmpty(request.nickName))
                {
                    sql += " AND NickName LIKE @NickName";
                    parameters.Add("NickName", $"%{request.nickName}%");
                }

                if (request.status > 0)
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", request.status);
                }

                if (!string.IsNullOrEmpty(request.fromDate) && DateTime.TryParse(request.fromDate, out DateTime from))
                {
                    sql += " AND CreatedDate >= @FromDate";
                    parameters.Add("FromDate", from);
                }

                if (!string.IsNullOrEmpty(request.toDate) && DateTime.TryParse(request.toDate, out DateTime to))
                {
                    sql += " AND CreatedDate <= @ToDate";
                    parameters.Add("ToDate", to.AddDays(1)); // Include the end date
                }

                var (users, totalCount) = await _dbService.GetPaginatedListAsync<UserInfo>(
                    sql, 
                    sql,
                    parameters.Count > 0 ? parameters : null, 
                    request.pageNumber, 
                    request.pageSize, 
                    "CreatedDate DESC"
                );

                var response = new UserListResponse
                {
                    Response = 1,
                    Data = new UserListData
                    {
                        users = new List<UserInfo>(users),
                        totalCount = totalCount
                    },
                    message = "Success"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed information of a specific user
        /// </summary>
        [HttpPost("getUserDetail")]
        [SwaggerOperation(Summary = "Get user details", Description = "Returns detailed information of a specific user")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetUserDetail([FromForm] UserDetailRequest request)
        {
            try
            {
                var sql = @"
                    SELECT 
                        AccountID, UserName, NickName, Email, Mobile, 
                        Balance, VipLevel, Status, CreatedDate, LastLoginDate,
                        PlatformId, MerchantId, IsOTP, IsAgency, VipPoint
                    FROM Accounts 
                    WHERE AccountID = @AccountID";

                var user = await _dbService.QueryFirstOrDefaultAsync<dynamic>(sql, new { AccountID = request.accountId });

                if (user == null)
                {
                    return Ok(new { Response = -1, message = "User not found" });
                }

                return Ok(new
                {
                    Response = 1,
                    Data = user,
                    message = "Success"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        /// <summary>
        /// Lock or unlock user account
        /// </summary>
        [HttpPost("lockUser")]
        [SwaggerOperation(Summary = "Lock/Unlock user", Description = "Lock or unlock a user account with reason")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> LockUser([FromForm] LockUserRequest request)
        {
            try
            {
                // Check if user exists
                var checkSql = "SELECT COUNT(1) FROM Accounts WHERE AccountID = @AccountID";
                var exists = await _dbService.ExecuteScalarAsync<int>(checkSql, new { AccountID = request.accountId });

                if (exists == 0)
                {
                    return Ok(new { Response = -1, message = "User not found" });
                }
                
                // Toggle status (for simplify, assuming we just query current status and flip it, or use a specific status from request if we had it)
                // Since the request doesn't specify status, let's assume this is a LOCK action (Status = -1) if user is active, or UNLOCK (Status = 1) if locked.
                // But better to be explicit. The request only has reason. 
                // Let's implement as: Get current status -> If -1 then set 1, else set -1.
                
                var statusSql = "SELECT Status FROM Accounts WHERE AccountID = @AccountID";
                var currentStatus = await _dbService.ExecuteScalarAsync<int>(statusSql, new { AccountID = request.accountId });
                
                int newStatus = (currentStatus == -1) ? 1 : -1;
                string action = (newStatus == -1) ? "locked" : "unlocked";

                var updateSql = "UPDATE Accounts SET Status = @Status WHERE AccountID = @AccountID";
                await _dbService.ExecuteAsync(updateSql, new { Status = newStatus, AccountID = request.accountId });

                return Ok(new
                {
                    Response = 1,
                    message = $"User {action} successfully",
                    Data = new { NewStatus = newStatus }
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    #region Request Models

    public class UserListRequest
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
        public string nickName { get; set; }
        public string userName { get; set; }
        public int status { get; set; } = 0;
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }

    public class UserDetailRequest
    {
        public long accountId { get; set; }
    }

    public class LockUserRequest
    {
        public long accountId { get; set; }
        public string reason { get; set; }
    }

    #endregion

    #region Response Models

    public class UserListResponse
    {
        public int Response { get; set; }
        public UserListData Data { get; set; }
        public string message { get; set; }
    }

    public class UserListData
    {
        public List<UserInfo> users { get; set; }
        public int totalCount { get; set; }
    }

    public class UserInfo
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public long Balance { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    #endregion
}
