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
    [SwaggerTag("Game Management APIs")]
    public class GameController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public GameController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("UpdateProfitConfig")]
        [SwaggerOperation(Summary = "Update game profit configuration")]
        public async Task<IActionResult> UpdateProfitConfig([FromForm] GameProfitConfigRequest request)
        {
             try
            {
                return Ok(new { Response = 1, message = "Profit config updated successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Bot Management APIs (Internal)")]
    public class BotAPIController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public BotAPIController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("GetBotList")]
        [SwaggerOperation(Summary = "Get list of bots")]
        public async Task<IActionResult> GetBotList([FromForm] BotListRequest request)
        {
             try
            {
                return Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }
    
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Bot Management APIs (Public)")]
    public class BotController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public BotController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("AddBot")]
        [SwaggerOperation(Summary = "Add new bot")]
        public async Task<IActionResult> AddBot([FromForm] AddBotRequest request)
        {
             try
            {
                return Ok(new { Response = 1, message = "Bot added successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "admin")]
    [SwaggerTag("Agency Management APIs")]
    public class AgencyController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public AgencyController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("GetList")]
        [SwaggerOperation(Summary = "Get list of agencies")]
        public async Task<IActionResult> GetList([FromForm] AgencyListRequest request)
        {
             try
            {
                return Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Create new agency")]
        public async Task<IActionResult> Create([FromForm] AgencyCreateRequest request)
        {
             try
            {
                return Ok(new { Response = 1, message = "Agency created successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
        
        [HttpGet("GetListCurrency")]
        [SwaggerOperation(Summary = "Get list of currencies")]
        public async Task<IActionResult> GetListCurrency()
        {
             try
            {
                return Ok(new { Response = 1, Data = new List<dynamic>(), message = "Success" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    #region Models
    public class GameProfitConfigRequest
    {
        public int gameId { get; set; }
        public double profitPercent { get; set; }
    }

    public class BotListRequest
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }

    public class AddBotRequest
    {
        public string username { get; set; }
        public string nickname { get; set; }
        public int gameId { get; set; }
    }

    public class AgencyListRequest
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }

    public class AgencyCreateRequest
    {
        public string name { get; set; }
        public int parentId { get; set; } = 0;
        public double commission { get; set; }
    }
    #endregion
}
