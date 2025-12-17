using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NetCore.PortalAPI.Core.Interfaces;
using ServerCore.PortalAPI.Core.Domain.Models.Payment;
using ServerCore.Utilities.Interfaces;
using ServerCore.Utilities.Security;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AccountSession _accountSession;
        private readonly IDataService _dataService;
        private readonly AppSettings _appSettings;
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(AccountSession accountSession, IDataService dataService, IOptions<AppSettings> options, IPaymentRepository paymentRepository)
        {
            _accountSession = accountSession;
            _dataService = dataService;
            _appSettings = options.Value;
            _paymentRepository = paymentRepository;
        }
        [HttpGet("GetListBankAccount")]
        public async Task<ActionResult<ResponseBuilder>> GetListBank(int type)
        {
            try
            {
                if (type == 0)
                {
                    var uri = string.Format("{0}listBank?api_key={1}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey);
                    var res = await _dataService.GetApiAsync<List<Bank>>(uri, "");
                    var data = res.Select(x => new
                    {
                        bankID = x.id,
                        bankCode = x.code,
                        bankName = x.bank_name
                    });
                    return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
                }
                else
                {
                    var uri = string.Format("{0}list?api_key={1}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey);
                    var res = await _dataService.GetApiAsync<List<Bank>>(uri, "");
                    var data = res.Select(x => new
                    {
                        bankID = x.id,
                        bankCode = x.code,
                        bankName = x.bank_name
                    });
                    return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpPost("RequestTopup")]
        public async Task<ActionResult<ResponseBuilder>> DepositBankRequest(BankChargeRequest bankChargeRequest)
        {
            NLogManager.Info(string.Format("[DepositBankChargeRequest] ClientIP: {1}, Request: {0}", JsonConvert.SerializeObject(bankChargeRequest), IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true)));

            var _bankLogsObj = new BankLogs
            {
                partnerId = 4,
                orgRequestId = string.Empty,
                accountId = _accountSession.AccountID,
                accountName = _accountSession.AccountName,
                bankCode = bankChargeRequest.bankCode,
                amount = bankChargeRequest.amount,
                description = string.Empty
            };
            var _id = _paymentRepository.DepositBankRequest(_bankLogsObj);
            if (_id <= 0)
            {
                return Ok(new { code = 1, description = "Lỗi hệ thống" });
            }

            var requestObj = new DepositBankRequest { amount = bankChargeRequest.amount, requestType = 1, bankCode = bankChargeRequest.bankCode, requestID = _id.ToString() };

            var uri = string.Format("{0}requestPayment?api_key={1}&request_id={2}&amount={3}&custom_content={4}&bank_code={5}&url_callback={6}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey
                    , _id
                    , bankChargeRequest.amount
                    , 1
                    , bankChargeRequest.bankCode
                    , string.Empty);
            var res = await _dataService.GetApiAsync<DepositBankResponse>(uri, "");

            _paymentRepository.DepositBankRequestUpdate(_id, res.bank ?? "", res.account ?? "", res.bank_name ?? "", res.content ?? "", "", res.status == "1" ? 1 : -1);
            if (res.status != "1" || res == null)
            {
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }


            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, new { bankName = res.bank, bankAccount = res.account, bankAccountName = res.bank_name, reqCode = res.content, amount = bankChargeRequest.amount });
        }
        [HttpGet("WithdrawBankRequest")]
        public async Task<ActionResult<ResponseBuilder>> CashoutBankRequest(WithdrawBankRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.bankAccount) || request.amount > 0 || string.IsNullOrEmpty(request.bankCode) || string.IsNullOrEmpty(request.bankAccountName))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var requestBalance = request.amount;
                if (requestBalance < 500000)
                {
                    return Ok(new { code = 1, description = "Số tiền không hợp lệ, tối thiểu là 500,000." });
                }
                long balanceBefore = _paymentRepository.GetBalance(_accountSession.AccountID);

                if (balanceBefore < requestBalance)
                {
                    return Ok(new { code = 1, description = "Số dư tài khoản không đủ." });
                }
                var _deductResult = _paymentRepository.DeductAccount(1000071, "0c659d039705f071385f7e287470fce7", _accountSession.AccountID, _accountSession.AccountName ?? "", request.amount, 0, "Rút tiền", 0 + "", 0, 0, IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true));
                if (_deductResult.ResponseCode <= 0)
                {
                    return Ok(new { code = 1, description = "Số dư tài khoản không đủ." });
                }

                var _cashoutLogsObj = new CashoutLogs
                {
                    orgRequestId = "",
                    accountId = _accountSession.AccountID,
                    accountName = _accountSession.AccountName ?? "",
                    bankCode = request.bankCode ?? "",
                    bankAccount = request.bankAccount ?? "",
                    bankAccountName = request.bankAccountName ?? "",
                    amount = request.amount,
                    description = ""
                };
                var _id = _paymentRepository.CashoutBankInsert(_cashoutLogsObj);
                if (_id <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }
                var signatureStr = string.Concat(_appSettings.PaymentService.ApiPin, _id, request.bankAccount, request.amount, _appSettings.PaymentService.ApiKey);
                var signature = Security.MD5Encrypt(signatureStr);

                var uri = string.Format("{0}outBank?api_key={1}&request_id={2}&bank_code={3}&bank_account={4}&bank_fullname={5}&amount={6}&url_callback={7}&signature={8}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey
                    , _id
                    , request.bankCode
                    , request.bankAccount
                    , request.bankAccountName
                    , request.amount
                    , _appSettings.PaymentService.MomoOutCallbackUrl
                    , signature);
                var res = await _dataService.GetApiAsync<dynamic>(uri, "");

                var _logObj = _paymentRepository.GetCashoutBankById(_id);
                if (res.status == 1)
                {
                    _paymentRepository.CashoutUpdateStatus(_logObj.id, 1);
                    return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
                }
                else
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }

                //if (_logObj != null && _logObj.id > 0 && _logObj.status == 0)
                //{
                //    var _notifyResponse = NotifyServices.Notify(_logObj.accountId, _logObj.accountName ?? "", _logObj.nickName ?? "", _logObj.amount, _logObj.createdTime);
                //    _logger.LogInformation(string.Format("[NotifyResponse] Response: {0}", JsonConvert.SerializeObject(_notifyResponse)));
                //}

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_BUSY, _accountSession.Language);
        }
        [HttpGet("DepositMomoRequest")]
        public async Task<ActionResult<ResponseBuilder>> DepositMomoRequest(string amount, string urlCallback)
        {
            try
            {
                if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(urlCallback))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var _momoLogsObj = new MomoLogs
                {
                    partnerId = 4,
                    orgRequestId = string.Empty,
                    accountId = _accountSession.AccountID,
                    accountName = _accountSession.AccountName,
                    bankCode = "MOMO",
                    amount = Convert.ToInt64(amount),
                    description = string.Empty,
                    urlCallback = urlCallback,
                    customContent = 1
                };
                var _id = _paymentRepository.DepositMomoRequestInsert(_momoLogsObj);
                if (_id <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }
                var uri = string.Format("{0}requestPayment?api_key={1}&request_id={2}&amount={3}&custom_content={4}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey
                    , _id
                    , amount
                    , 1);
                var res = await _dataService.GetApiAsync<MomoDepositResponse>(uri, "");

                if (res.status != 1 || res == null)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }

                _paymentRepository.DepositMomoRequestUpdate(_id, "MOMO", res.phone, res.name, res.content, string.Empty, res.status, res.redirectLink, res.deepLink);

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, new { bankName = "MOMO", bankAccount = res.phone, bankAccountName = res.name, reqCode = res.content, amount = res.amount, redirectLink = res.redirectLink, deeplink = res.deepLink });
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_BUSY, _accountSession.Language);
        }
        [HttpGet("MomoCallback")]
        public ActionResult<ResponseBuilder> MomoCallback(string request_id, string status, long amount, string message, string signature)
        {
            try
            {
                if (string.IsNullOrEmpty(request_id) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(message))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                if (amount <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var conStr = string.Concat(request_id, message, amount, _appSettings.PaymentService.ApiKey);
                var md5Str = Security.MD5Encrypt(conStr);
                if (signature != md5Str)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var _log = _paymentRepository.GetMomoLogById(request_id);
                if (_log == null || _log.id <= 0)
                {
                    return Ok(new { code = 404, description = "Giao dịch không hợp lệ" });
                }
                if (_log.status == 2)
                {
                    return Ok(new { code = 405, description = "Giao dịch đã xử lý" });
                }
                var _realAmount = amount;
                var _virtualAmount = _realAmount;

                _paymentRepository.UpdateCallback(_log.id, _realAmount, _virtualAmount, "", status == "1" ? 2 : -2);

                var _topupResult = _paymentRepository.TopupAccount(1000146, "218312118fc3b91578616b0d746e6be4", (int)_log.accountId, _log.accountName ?? "", (int)_virtualAmount, message, 0, 0, 0, IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true));

                if (_topupResult.ResponseCode <= 0)
                {
                    return Ok(new { code = 500, description = "UnknownError" });
                }
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpGet("CashOutMomoRequest")]
        public async Task<ActionResult<ResponseBuilder>> CashoutMomoRequest(string requestId, string bankAccount, string amount)
        {
            try
            {
                if (string.IsNullOrEmpty(requestId) || string.IsNullOrEmpty(bankAccount) || string.IsNullOrEmpty(amount))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var signatureStr = string.Concat(_appSettings.PaymentService.ApiPin, requestId, bankAccount, amount, _appSettings.PaymentService.ApiKey);
                var signature = Security.MD5Encrypt(signatureStr);
                var _momoLogsObj = new MomoOutLogs
                {
                    requestId = requestId,
                    accountId = _accountSession.AccountID,
                    accountName = _accountSession.AccountName,
                    amount = amount,
                    bankAccount = bankAccount,
                    signature = signature,
                    urlCallback = _appSettings.PaymentService.MomoOutCallbackUrl

                };
                var _id = _paymentRepository.CashOutMomoRequestInsert(_momoLogsObj);
                if (_id <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }
                var uri = string.Format("{0}outMomo?api_key={1}&request_id={2}&account={3}&amount={4}&url_callback={5}&signature={6}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey
                    , requestId
                    , bankAccount
                    , amount
                    , _appSettings.PaymentService.MomoOutCallbackUrl
                    , signature);
                var res = await _dataService.GetApiAsync<dynamic>(uri, "");

                if (res.status != 1 || res == null)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }

                _paymentRepository.CashOutMomoRequestUpdate(_id, res.status, Convert.ToInt64(amount));

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_BUSY, _accountSession.Language);
        }
        [HttpGet("CashoutMomoCallback")]
        public ActionResult<ResponseBuilder> CashoutMomoCallback(string request_id, string status, long amount, string signature)
        {
            try
            {
                if (string.IsNullOrEmpty(request_id) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(status))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                if (amount <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var conStr = string.Concat(request_id, status, amount.ToString(), _appSettings.PaymentService.ApiKey);
                var md5Str = Security.MD5Encrypt(conStr);
                if (signature != md5Str)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var _log = _paymentRepository.GetMomoOutLogByRequestId(request_id);
                if (_log == null || _log.Id <= 0)
                {
                    return Ok(new { code = 404, description = "Giao dịch không hợp lệ" });
                }
                if (_log.status == 2)
                {
                    return Ok(new { code = 405, description = "Giao dịch đã xử lý" });
                }
                var _realAmount = amount;
                var _virtualAmount = _realAmount;

                _paymentRepository.UpdateCallback(_log.Id, _realAmount, _virtualAmount, "", status == "1" ? 2 : -2);

                //var _topupResult = _paymentRepository.TopupAccount(1000146, "218312118fc3b91578616b0d746e6be4", (int)_log.accountId, _log.accountName ?? "", (int)_virtualAmount, string.Empty, 0, 0, 0, IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true));

                //if (_topupResult.ResponseCode <= 0)
                //{
                //    return Ok(new { code = 500, description = "UnknownError" });
                //}
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpGet("DepositCardRequest")]
        public async Task<ActionResult<ResponseBuilder>> DepositCardRequest(string cardAmount, string cardSeri, string cardCode, string cardType)
        {
            try
            {
                if (string.IsNullOrEmpty(cardAmount) || string.IsNullOrEmpty(cardSeri) || string.IsNullOrEmpty(cardCode) || string.IsNullOrEmpty(cardType))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }

                var _cardLogsObj = new CardLogs { partnerId = 4, orgRequestId = string.Empty, accountId = _accountSession.AccountID, accountName = _accountSession.AccountName, carrier = cardType, pin = cardCode, serial = cardSeri, amount = Convert.ToInt32(cardAmount), description = string.Empty };

                var _id = _paymentRepository.DepositCardRequestInsert(_cardLogsObj);
                if (_id <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }
                var conStr = string.Concat(_appSettings.PaymentService.ApiKey, cardAmount, cardCode, cardSeri);
                var md5Str = Security.MD5Encrypt(conStr);
                var uri = string.Format("{0}cardcharging2?api_key={1}&request_id={2}&card_amount ={3}&card_seri ={4}&card_code={5}&card_type={6}&signature={7}"
                    , _appSettings.PaymentService.ApiUrl
                    , _appSettings.PaymentService.ApiKey
                    , _id
                    , cardAmount
                    , cardSeri
                    , cardCode
                    , cardType
                    , md5Str);
                var res = await _dataService.GetApiAsync<CardDepositResponse>(uri, "");

                if (res.status != 0 || res == null)
                {
                    return new ResponseBuilder(ErrorCodes.SERVER_ERROR, _accountSession.Language);
                }

                _paymentRepository.DepositCardRequestUpdate(_id, res.trans_code.ToString(), res.message, res.status == 0 ? 1 : -1);

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
            return new ResponseBuilder(ErrorCodes.SYSTEM_BUSY, _accountSession.Language);
        }
        [HttpGet("CardCallback")]
        public ActionResult<ResponseBuilder> CardCallback(string status, long amount, long real_amount, string tran_id, string card_seri, string card_code, string request_id, string message, string signature)
        {
            try
            {
                if (string.IsNullOrEmpty(request_id) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(message))
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                if (amount <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var conStr = string.Concat(request_id, message, amount, _appSettings.PaymentService.ApiKey);
                var md5Str = Security.MD5Encrypt(conStr);
                if (signature != md5Str)
                {
                    return new ResponseBuilder(ErrorCodes.INPUT_PARAM_ERROR, _accountSession.Language);
                }
                var _log = _paymentRepository.GetCardLogById(request_id);
                if (_log == null || _log.id <= 0)
                {
                    return Ok(new { code = 404, description = "Giao dịch không hợp lệ" });
                }
                if (_log.status == 2)
                {
                    return Ok(new { code = 405, description = "Giao dịch đã xử lý" });
                }
                var _realAmount = Convert.ToInt32(real_amount);
                var _virtualAmount = Convert.ToInt32(amount);

                _paymentRepository.CardCallbackUpdate(_log.id, _realAmount, _virtualAmount, message ?? "", status == "0" ? 2 : -2);

                var _topupResult = _paymentRepository.TopupAccount(1000146, "218312118fc3b91578616b0d746e6be4", (int)_log.accountId, _log.accountName ?? "", _realAmount, message, 0, 0, 0, IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true));

                if (_topupResult.ResponseCode <= 0)
                {
                    return Ok(new { code = 500, description = "UnknownError" });
                }
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }


        [HttpGet("BankCallback")]
        public ActionResult<ResponseBuilder> BankCallback(string request_id, string bank, int status, long amount, string message, string signature)
        {
            //_logger.LogInformation(string.Format("[CallBack] ClientIP: {1}, Request: {0}", JsonConvert.SerializeObject(request), _httpContextExtensions.GetClientIP()));


            //if (request.status != 1)
            //{
            //    return Ok(new { statusCode = 200, message = "Successful" });
            //}

            long _logId = 0;
            Int64.TryParse(request_id, out _logId);
            if (_logId <= 0)
                return Ok(new { statusCode = 404, message = "Giao dịch không hợp lệ" });

            var _log = _paymentRepository.GetBankLogById(_logId);
            if (_log == null)
            {
                return Ok(new { statusCode = 404, message = "Giao dịch không hợp lệ" });
            }
            if (_log.status == 2)
            {
                return Ok(new { statusCode = 405, message = "Giao dịch đã xử lý" });
            }
            var _realAmount = amount;
            var _virtualAmount = _realAmount;
            _paymentRepository.BankCallbackUpdate(_log.id, _realAmount, _virtualAmount, message ?? "", status == 1 ? 2 : -2);
            if (status == 1)
            {
                var _topupResult = _paymentRepository.TopupAccount(1000146, "218312118fc3b91578616b0d746e6be4", (int)_log.accountId, _log.accountName ?? "", (int)_virtualAmount, message, 0, 0, 0, IPAddressHelper.GetRemoteIPAddress(Request.HttpContext, true));
                if (_topupResult.ResponseCode <= 0)
                {
                    return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
                }
            }
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
        }

        [HttpGet("CashoutBankCallback")]
        public IActionResult WithdrawCallback(string request_id, int status, long amount, string signature, string message)
        {
            //_logger.LogInformation(string.Format("[WithdrawCallback] ClientIP: {1}, Request: {0}", JsonConvert.SerializeObject(request), _httpContextExtensions.GetClientIP()));


            //if (request.status != 1)
            //{
            //    return Ok(new { statusCode = 200, message = "Successful" });
            //}
            long _id = 0;
            Int64.TryParse(request_id, out _id);
            if (_id <= 0)
                return Ok(new { statusCode = 404, message = "Giao dịch không hợp lệ" });

            var _log = _paymentRepository.GetCashoutBankById(_id);
            if (_log == null)
            {
                return Ok(new { statusCode = 404, message = "Giao dịch không hợp lệ" });
            }
            if (_log.status == 2)
            {
                return Ok(new { statusCode = 405, message = "Giao dịch đã xử lý" });
            }
            _paymentRepository.CashoutBankCallbackUpdate(_log.id, amount, message, status == 1 ? 2 : -2);
            //if (request.status == 3)
            //{
            //    var _topupResult = _transactionLogs.TopupAccount(1000149, "fed76feefee55743610e3f322ad333ba", (int)_log.accountId, _log.accountName ?? "", (int)_log.amount, "Hoàn tiền", _log.id, 0, 0, _httpContextExtensions.GetClientIP());
            //    if (_topupResult.ResponseCode <= 0)
            //    {
            //        return Ok(new { code = 500, description = "UnknownError" });
            //    }
            //}
            return Ok(new { statusCode = 200, message = "Successful" });
        }
    }
}
