using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServerCore.PortalAPI.Services;

namespace ServerCore.PortalAPI.Controllers.Admin
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Admin Home & Authentication APIs")]
    public class HomeController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public HomeController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("ConfirmLogin")]
        [SwaggerOperation(Summary = "Admin login")]
        public async Task<IActionResult> ConfirmLogin([FromForm] LoginRequest request)
        {
            try
            {
                // Placeholder
                return Ok(new { Response = 1, Data = new { Token = "mock-token" }, message = "Login successful" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        [HttpGet("Logout")]
        [SwaggerOperation(Summary = "Admin logout")]
        public IActionResult Logout()
        {
            return Ok(new { Response = 1, message = "Logout successful" });
        }

        [HttpGet("GetSessionInfo")]
        [SwaggerOperation(Summary = "Get current session information")]
        public IActionResult GetSessionInfo()
        {
            return Ok(new 
            { 
                Response = 1, 
                Data = new 
                { 
                    AccountID = 1, 
                    Username = "admin", 
                    GroupID = 1, 
                    Mobile = "0123456789" 
                }, 
                message = "Success" 
            });
        }

        [HttpGet("GetSidebarMenu")]
        [SwaggerOperation(Summary = "Get dynamic sidebar menu based on permissions")]
        public IActionResult GetSidebarMenu()
        {
            var menu = new List<dynamic>
            {
                new { id = 1, name = "Dashboard", url = "/", icon = "fa-dashboard", parentId = 0, children = new List<dynamic>() },
                new { id = 2, name = "Users", url = "/users", icon = "fa-users", parentId = 0, children = new List<dynamic>() }
            };

            return Ok(new { Response = 1, Data = menu, message = "Success" });
        }
    }

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("System Configuration Management")]
    public class SystemController : ControllerBase
    {
        private readonly IAdminDbService _dbService;
        public SystemController(IAdminDbService dbService) { _dbService = dbService; }

        [HttpGet("AccountHistory")]
        public IActionResult AccountHistory() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("LogUserAction")]
        public IActionResult LogUserAction() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("ListMaintenance")]
        public IActionResult ListMaintenance() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("CardConfigList")]
        public IActionResult CardConfigList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("PatnerCard")]
        public IActionResult PatnerCard() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("PartnerConfigList")]
        public IActionResult PartnerConfigList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("HistoryAddCard")]
        public IActionResult HistoryAddCard() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("HistoryBuyCard")]
        public IActionResult HistoryBuyCard() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("StoreCardList")]
        public IActionResult StoreCardList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpPost("StoreCardInsert")]
        public IActionResult StoreCardInsert() => Ok(new { Response = 1, message = "Success" });

        [HttpGet("StoreCardDetail")]
        public IActionResult StoreCardDetail() => Ok(new { Response = 1, Data = new {}, message = "Success" });

        [HttpGet("StoreCardTempList")]
        public IActionResult StoreCardTempList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpPost("StoreCardTempInsert")]
        public IActionResult StoreCardTempInsert() => Ok(new { Response = 1, message = "Success" });

        [HttpGet("MomoHistoryLog")]
        public IActionResult MomoHistoryLog() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("LogUpdateGameBank")]
        public IActionResult LogUpdateGameBank() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });
    }

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Event Management APIs")]
    public class EventManagerController : ControllerBase
    {
        private readonly IAdminDbService _dbService;
        public EventManagerController(IAdminDbService dbService) { _dbService = dbService; }

        [HttpGet("LuckeySpinConfig")]
        public IActionResult LuckeySpinConfig() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpPost("LuckeySpinconfigUpdate")]
        public IActionResult LuckeySpinconfigUpdate() => Ok(new { Response = 1, message = "Success" });

        [HttpGet("LuckeySpinList")]
        public IActionResult LuckeySpinList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("LuckeySpinStatistic")]
        public IActionResult LuckeySpinStatistic() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("DepositConfig")]
        public IActionResult DepositConfig() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpPost("DepositConfigUpdate")]
        public IActionResult DepositConfigUpdate() => Ok(new { Response = 1, message = "Success" });

        [HttpGet("DepositHistory")]
        public IActionResult DepositHistory() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("DepositStatistic")]
        public IActionResult DepositStatistic() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("FootballEventExchangeGift")]
        public IActionResult FootballEventExchangeGift() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("FootballReportEventByUser")]
        public IActionResult FootballReportEventByUser() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("SummonDragon")]
        public IActionResult SummonDragon() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("SearchDragon")]
        public IActionResult SearchDragon() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("GodOfWealthStatistic")]
        public IActionResult GodOfWealthStatistic() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("CardEventDoubleCharge")]
        public IActionResult CardEventDoubleCharge() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("PlayGameConfig")]
        public IActionResult PlayGameConfig() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("ShareProfitConfigList")]
        public IActionResult ShareProfitConfigList() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });

        [HttpGet("XWealthSlotConfig")]
        public IActionResult XWealthSlotConfig() => Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });
    }

    #region Models
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    #endregion
}
