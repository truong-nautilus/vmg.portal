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
    [SwaggerTag("Statistics and Analysis Reports")]
    public class ReportAPIController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public ReportAPIController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("GetTopWin")]
        [SwaggerOperation(Summary = "Get top winners report")]
        public async Task<IActionResult> GetTopWin([FromForm] ReportRequest request)
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

        [HttpPost("GetTopLose")]
        [SwaggerOperation(Summary = "Get top losers report")]
        public async Task<IActionResult> GetTopLose([FromForm] ReportRequest request)
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

        [HttpPost("SourceCampainGetList")]
        [SwaggerOperation(Summary = "Get marketing campaign sources")]
        public async Task<IActionResult> SourceCampainGetList([FromForm] CampaignReportRequest request)
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

        [HttpPost("GameStatistic")]
        [SwaggerOperation(Summary = "Get game statistics")]
        public async Task<IActionResult> GameStatistic([FromForm] GameStatisticRequest request)
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
    public class ReportRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }

    public class CampaignReportRequest
    {
        public string source { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
    }

    public class GameStatisticRequest
    {
        public int gameId { get; set; }
        public int platform { get; set; }
        public int channel { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
    #endregion
}
