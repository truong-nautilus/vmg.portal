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
    [SwaggerTag("Cashout Management APIs")]
    public class CashoutAPIController : ControllerBase
    {
        private readonly IAdminDbService _dbService;

        public CashoutAPIController(IAdminDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("getCashoutList")]
        [SwaggerOperation(Summary = "Get list of cashout requests")]
        public async Task<IActionResult> GetCashoutList([FromForm] CashoutListRequest request)
        {
            try
            {
                var response = new
                {
                    Response = 1,
                    Data = new
                    {
                        cashouts = new List<dynamic>
                        {
                            new { CashoutID = 1, AccountID = 1, Username = "user1", Amount = 500000, Status = 0, CreatedDate = DateTime.Now }
                        },
                        totalCount = 1
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

        [HttpPost("approveCashout")]
        [SwaggerOperation(Summary = "Approve cashout request")]
        public async Task<IActionResult> ApproveCashout([FromForm] ApproveCashoutRequest request)
        {
            try
            {
                return Ok(new { Response = 1, message = "Cashout approved successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }

        [HttpPost("rejectCashout")]
        [SwaggerOperation(Summary = "Reject cashout request")]
        public async Task<IActionResult> RejectCashout([FromForm] RejectCashoutRequest request)
        {
            try
            {
                return Ok(new { Response = 1, message = "Cashout rejected successfully" });
            }
            catch (Exception ex)
            {
                return Ok(new { Response = -1, message = ex.Message });
            }
        }
    }

    #region Models
    public class CashoutListRequest
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 50;
        public int status { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }

    public class ApproveCashoutRequest
    {
        public long cashoutId { get; set; }
        public string note { get; set; }
    }

    public class RejectCashoutRequest
    {
        public long cashoutId { get; set; }
        public string reason { get; set; }
    }
    #endregion
}
