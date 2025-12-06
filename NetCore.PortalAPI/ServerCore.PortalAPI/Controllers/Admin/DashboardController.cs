using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using ServerCore.PortalAPI.Services;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Controllers.Admin
{
    /// <summary>
    /// Admin Dashboard APIs
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Dashboard statistics and reports")]
    public class DashboardController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public DashboardController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Get daily total statistics
        /// </summary>
        [HttpPost("GetReportTotalDaily")]
        [SwaggerOperation(
            Summary = "Get daily total statistics",
            Description = "Returns daily statistics including total registrations, deposits, and non-deposits"
        )]
        [SwaggerResponse(200, "Success", typeof(DailyReportResponse))]
        public async Task<IActionResult> GetReportTotalDaily([FromForm] CurrencyRequest request)
        {
            try
            {
                var today = DateTime.Today;
                var startOfDay = today;
                var endOfDay = today.AddDays(1);

                // Count registers today
                var registerSql = "SELECT COUNT(1) FROM Accounts WHERE CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalRegister = await _dbService.ExecuteScalarAsync<int>(registerSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                // Count deposits (TransactionType = 1)
                var depositSql = @"
                    SELECT COUNT(DISTINCT AccountID) 
                    FROM TransactionLogs 
                    WHERE TransactionType = 1 AND CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalDeposit = await _dbService.ExecuteScalarAsync<int>(depositSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                // Revenue (Total Deposit Amount)
                var revenueSql = @"
                    SELECT ISNULL(SUM(Amount), 0) 
                    FROM TransactionLogs 
                    WHERE TransactionType = 1 AND CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalRevenue = await _dbService.ExecuteScalarAsync<long>(revenueSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                // Non-depositors (Total Registers - Depositors)
                // Note: accurate logic would be checking if new registers made a deposit
                int totalNonDeposit = Math.Max(0, totalRegister - totalDeposit);

                var response = new DailyReportResponse
                {
                    Response = 1,
                    Data = new DailyReportData
                    {
                        totalRegister = totalRegister,
                        totalDeposit = totalDeposit,
                        totalNonDeposit = totalNonDeposit,
                        totalRevenue = totalRevenue,
                        totalProfit = (long)(totalRevenue * 0.2) // Mock profit as 20% of revenue
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
        /// Get concurrent users (CCU) by game
        /// </summary>
        [HttpPost("GetReportCCU")]
        [SwaggerOperation(
            Summary = "Get CCU by game",
            Description = "Returns concurrent users statistics grouped by game (Using Platform as game group for now)"
        )]
        [SwaggerResponse(200, "Success", typeof(CCUReportResponse))]
        public async Task<IActionResult> GetReportCCU([FromForm] CurrencyRequest request)
        {
            try
            {
                // Count active sessions grouped by PlatformId (via Join with Accounts)
                // Assuming IsActive=1 and ExpiredDate > Now means online
                var sql = @"
                    SELECT a.PlatformId, COUNT(DISTINCT s.AccountID) as CCU
                    FROM LoginSessions s
                    JOIN Accounts a ON s.AccountID = a.AccountID
                    WHERE s.IsActive = 1 AND s.ExpiredDate > GETDATE()
                    GROUP BY a.PlatformId";

                var ccuList = await _dbService.QueryAsync<dynamic>(sql);

                var names = new List<string>();
                var colors = new List<string>();
                var ccus = new List<int>();

                // Map PlatformID to names
                foreach (var item in ccuList)
                {
                    int platformId = (int)item.PlatformId;
                    int count = (int)item.CCU;

                    names.Add($"Platform {platformId}");
                    ccus.Add(count);
                    
                    // Generate random color or fixed based on ID
                    colors.Add(GetColor(platformId));
                }

                // If no data, return empty or mock structure
                if (names.Count == 0)
                {
                    names.Add("Portal");
                    colors.Add("#4BC0C0");
                    ccus.Add(0);
                }

                var response = new CCUReportResponse
                {
                    Response = 1,
                    Data = new CCUReportData
                    {
                        names = names,
                        colors = colors,
                        ccus = ccus
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

        private string GetColor(int id)
        {
            var colors = new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF", "#FF9F40" };
            return colors[id % colors.Length];
        }

        /// <summary>
        /// Get account statistics
        /// </summary>
        [HttpPost("GetReportAccount")]
        [SwaggerOperation(
            Summary = "Get account statistics",
            Description = "Returns account-related statistics including balance, deposits, and withdrawals"
        )]
        [SwaggerResponse(200, "Success", typeof(AccountReportResponse))]
        public async Task<IActionResult> GetReportAccount([FromForm] CurrencyRequest request)
        {
            try
            {
                var today = DateTime.Today;
                var startOfDay = today;
                var endOfDay = today.AddDays(1);

                // Total Balance of all accounts
                var balanceSql = "SELECT ISNULL(SUM(Balance), 0) FROM Accounts";
                var totalBalance = await _dbService.ExecuteScalarAsync<long>(balanceSql);

                // Total Active Users
                var activeUsersSql = "SELECT COUNT(1) FROM Accounts WHERE Status = 1";
                var totalActiveUsers = await _dbService.ExecuteScalarAsync<int>(activeUsersSql);

                // New Users Today
                var newUsersSql = "SELECT COUNT(1) FROM Accounts WHERE CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalNewUsersToday = await _dbService.ExecuteScalarAsync<int>(newUsersSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                // Withdrawals Today (TransactionType = 2)
                var withdrawSql = @"
                    SELECT ISNULL(SUM(Amount), 0) 
                    FROM TransactionLogs 
                    WHERE TransactionType = 2 AND CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalWithdrawToday = await _dbService.ExecuteScalarAsync<long>(withdrawSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                // Deposits Today (TransactionType = 1)
                var depositSql = @"
                    SELECT ISNULL(SUM(Amount), 0) 
                    FROM TransactionLogs 
                    WHERE TransactionType = 1 AND CreatedDate >= @StartOfDay AND CreatedDate < @EndOfDay";
                var totalDepositToday = await _dbService.ExecuteScalarAsync<long>(depositSql, new { StartOfDay = startOfDay, EndOfDay = endOfDay });

                var response = new AccountReportResponse
                {
                    Response = 1,
                    Data = new AccountReportData
                    {
                        totalBalance = totalBalance,
                        totalPendingFund = 0, // Mock pending
                        totalDepositToday = totalDepositToday,
                        totalWithdrawToday = totalWithdrawToday,
                        totalActiveUsers = totalActiveUsers,
                        totalNewUsersToday = totalNewUsersToday
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
    }

    #region Request Models

    public class CurrencyRequest
    {
        public int currencyID { get; set; } = 1;
    }

    #endregion

    #region Response Models

    public class DailyReportResponse
    {
        public int Response { get; set; }
        public DailyReportData Data { get; set; }
        public string message { get; set; }
    }

    public class DailyReportData
    {
        public int totalRegister { get; set; }
        public int totalDeposit { get; set; }
        public int totalNonDeposit { get; set; }
        public long totalRevenue { get; set; }
        public long totalProfit { get; set; }
    }

    public class CCUReportResponse
    {
        public int Response { get; set; }
        public CCUReportData Data { get; set; }
        public string message { get; set; }
    }

    public class CCUReportData
    {
        public List<string> names { get; set; }
        public List<string> colors { get; set; }
        public List<int> ccus { get; set; }
    }

    public class AccountReportResponse
    {
        public int Response { get; set; }
        public AccountReportData Data { get; set; }
        public string message { get; set; }
    }

    public class AccountReportData
    {
        public long totalBalance { get; set; }
        public long totalPendingFund { get; set; }
        public long totalDepositToday { get; set; }
        public long totalWithdrawToday { get; set; }
        public int totalActiveUsers { get; set; }
        public int totalNewUsersToday { get; set; }
    }

    #endregion
}
