using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServerCore.Utilities.Interfaces;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System.Dynamic;

namespace ServerCore.PortalAPI.Presentation.Controllers
{
    [Route("api/blockchain")]
    [ApiController]
    public class BlockChainController : ControllerBase
    {
        private readonly AccountSession _accountSession;
        private readonly IDataService _dataService;
        private readonly AppSettings _appSettings;
        public BlockChainController(AccountSession accountSession, IDataService dataService, IOptions<AppSettings> option)
        {
            _accountSession = accountSession;
            _dataService = dataService;
            _appSettings = option.Value;
        }
        [HttpGet("GetWallet")]
        public ActionResult<ResponseBuilder> GetWallet(string networkName, decimal amount, long refId)
        {
            var token = _accountSession.Token;
            var accountID = _accountSession.AccountID;
            var callback = string.Format("https://api.hoabet.com/api/blockchain/callback?accountId={0}&amount={1}&refId={2}", accountID, amount, refId);
            var uri = string.Format("{0}/getwallet?accountId={1}&callback={2}&networkName={3}", _appSettings.BlockchainApiUrl, accountID, callback, networkName);
            var res = _dataService.GetApiAsync<dynamic>(uri, token, true, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }
        [HttpPost("Withdraw")]
        public ActionResult<ResponseBuilder> Withdraw(string transId, decimal amount, string address)
        {
            var token = _accountSession.Token;
            var accountID = _accountSession.AccountID;
            var uri = string.Format("{0}/withdraw", _appSettings.BlockchainApiUrl);
            dynamic data = new ExpandoObject();
            data.transId = transId;
            data.accountId = _accountSession.AccountID;
            data.address = address;
            data.amount = amount;
            var res = _dataService.PostApiAsync(uri, data, token, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }
        [HttpGet("CheckWithdrawStatus")]
        public ActionResult<ResponseBuilder> CheckWithdrawStatus(string transId)
        {
            var token = _accountSession.Token;
            var uri = string.Format("{0}/CheckWithdrawStatus?transId={1}", _appSettings.BlockchainApiUrl, transId);
            var res = _dataService.GetApiAsync<dynamic>(uri, token, true, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }
        [HttpGet("Callback")]
        public ActionResult<ResponseBuilder> Callback(long accountId, decimal amount, long refId)
        {
            var token = _accountSession.Token;

            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
        }

        [HttpGet("GetListMerchantWallet")]
        public ActionResult<ResponseBuilder> GetListMerchantWallet(int merchantId)
        {
            var token = _accountSession.Token;
            var uri = string.Format("{0}/GetListMerchantWallet?merchantId={1}", _appSettings.BlockchainApiUrl, merchantId);
            var res = _dataService.GetApiAsync<dynamic>(uri, token, true, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }

        [HttpPost("TransferFromMerchant")]
        public ActionResult<ResponseBuilder> TransferFromMerchant(int merchantId, int type, string address, decimal amount)
        {
            var token = _accountSession.Token;
            var accountID = _accountSession.AccountID;
            var uri = string.Format("{0}/TransferFromMerchant", _appSettings.BlockchainApiUrl);
            dynamic data = new ExpandoObject();
            data.merchantId = merchantId;
            data.type = type;
            data.address = address;
            data.amount = amount;
            var res = _dataService.PostApiAsync(uri, data, token, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }

        [HttpGet("GetListContracts")]
        public ActionResult<ResponseBuilder> GetListContracts()
        {
            var token = _accountSession.Token;
            var uri = string.Format("{0}/GetListContracts", _appSettings.BlockchainApiUrl);
            var res = _dataService.GetApiAsync<dynamic>(uri, token, true, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }


        [HttpGet("GetListNetworks")]
        public ActionResult<ResponseBuilder> GetListNetworks()
        {
            var token = _accountSession.Token;
            var uri = string.Format("{0}/GetListNetworks", _appSettings.BlockchainApiUrl);
            var res = _dataService.GetApiAsync<dynamic>(uri, token, true, true);
            return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, res);
        }
    }
}
