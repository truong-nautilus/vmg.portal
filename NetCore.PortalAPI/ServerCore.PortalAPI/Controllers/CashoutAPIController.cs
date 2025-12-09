using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServerCore.DataAccess.DAO;
using ServerCore.PortalAPI.Models.Payment;
using ServerCore.Utilities.Interfaces;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Controllers
{
    [Route("CashoutAPI")]
    [ApiController]
    public class CashoutAPIController : ControllerBase
    {
        private readonly AccountSession _accountSession;
        private readonly IDataService _dataService;
        private readonly AppSettings _appSettings;
        private readonly IPaymentDAO _paymentDAO;

        public CashoutAPIController(AccountSession accountSession, IDataService dataService, IOptions<AppSettings> options, IPaymentDAO paymentDAO)
        {
            _accountSession = accountSession;
            _dataService = dataService;
            _appSettings = options.Value;
            _paymentDAO = paymentDAO;
        }

        [HttpPost("GetListBank")]
        public async Task<ActionResult<ResponseBuilder>> GetListBank([FromBody] dynamic request)
        {
            try
            {
                int type = 0;
                // Attempt to read type from request if possible, though body is empty in the user example
                try { type = (int)request?.type; } catch { }

                string uri;
                if (type == 0)
                {
                    uri = string.Format("{0}listBank?api_key={1}", _appSettings.PaymentService.ApiUrl, _appSettings.PaymentService.ApiKey);
                }
                else
                {
                    uri = string.Format("{0}list?api_key={1}", _appSettings.PaymentService.ApiUrl, _appSettings.PaymentService.ApiKey);
                }

                var res = await _dataService.GetApiAsync<List<Bank>>(uri, "");
                
                if (res == null)
                {
                     // Return empty list instead of error if null, or system error
                     // Following PaymentController pattern
                     // But if null, select will fail.
                     return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
                }

                var data = res.Select(x => new
                {
                    bankID = x.id,
                    bankCode = x.code,
                    bankName = x.bank_name
                });
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
    }
}
