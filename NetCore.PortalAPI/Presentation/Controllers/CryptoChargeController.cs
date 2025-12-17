using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetCore.Utils.Cache;
using Nethereum.Util;
using Newtonsoft.Json;
using NetCore.PortalAPI.Core.Interfaces;
using ServerCore.PortalAPI.Core.Domain.Models;
using ServerCore.PortalAPI.Core.Domain.Models.Crypto;
using ServerCore.PortalAPI.Core.Application.Services;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ServerCore.PortalAPI.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoChargeController : ControllerBase
    {
        private readonly string CALLBACK_SECRET_KEY;
        private static readonly string KEY_CRYPTO_CHARGE = "CryptoCharge";
        private readonly AccountSession _accountSession;
        private readonly CacheHandler _cacheHandler;
        private readonly ICryptoChargeRepository _cryptoChargeRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly WalletService _walletService;
        private readonly TatumService _tatumService;
        public CryptoChargeController(AccountSession accountSession, CacheHandler cacheHandler, ICryptoChargeRepository cryptoChargeRepository, IConfiguration configuration, WalletService walletService, TatumService tatumService)
        {
            _accountSession = accountSession;
            _cacheHandler = cacheHandler;
            _cryptoChargeRepository = cryptoChargeRepository;
            CALLBACK_SECRET_KEY = configuration.GetSection("AppSettings:SPIDER_MAN_CRYPTO_APIS_CALLBACK_SECRET_KEY").Value;
            _walletService = walletService;
            _tatumService = tatumService;
        }
        [HttpGet("GetListCurrency")]
        public dynamic GetCryptoCharge()
        {
            try
            {
                var data = GetCurrencyProfiles().Where(t => t.IsActive).ToList();

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        private List<CurrencyProfile> GetCurrencyProfiles()
        {
            string keyHu = _cacheHandler.GeneralRedisKey(KEY_CRYPTO_CHARGE, "ListCurrency");
            List<CurrencyProfile> data;

            if (!_cacheHandler.MemTryGet(keyHu, out data))
            {
                data = _cryptoChargeRepository.GetListCurrency();
                _cacheHandler.MemSet(1800, keyHu, data);
            }

            return data;
        }
        [HttpGet("GetListChainByCurrencyId")]
        public dynamic GetListChainByCurrencyId(int currencyId)
        {
            try
            {
                var currencyProfiles = GetCurrencyProfiles().Where(t => t.IsActive).ToList();

                var currency = currencyProfiles.Where(t => t.CurrencyId == currencyId).FirstOrDefault();

                if (currency == null)
                {
                    return new ResponseBuilder(ErrorCodes.CURRENCY_NOT_FOUND, _accountSession.Language);
                }

                string keyHu = _cacheHandler.GeneralRedisKey(KEY_CRYPTO_CHARGE, $"GetListChainByCurrencyId:{currencyId}");
                List<ChainProfile> data;
                if (!_cacheHandler.MemTryGet(keyHu, out data))
                {
                    data = _cryptoChargeRepository.GetListChain(currencyId);
                    _cacheHandler.MemSet(1800, keyHu, data);
                }
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpGet("GetWallet")]
        public dynamic GetWallet(int chainId)
        {
            try
            {
                var accountId = _accountSession.AccountID;
                var displayName = _accountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_NOT_EXIST, _accountSession.Language);
                }

                string keyHu = _cacheHandler.GeneralRedisKey(KEY_CRYPTO_CHARGE, $"UserAddress:{accountId}:{chainId}");
                string address;
                if (!_cacheHandler.MemTryGet(keyHu, out address))
                {

                    address = _cryptoChargeRepository.GetAddress(accountId, chainId);
                    _cacheHandler.MemSet(keyHu, address);
                }

                if (address != string.Empty)
                {
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_EXIST, _accountSession.Language, address);
                }

                var chainCreateName = _cryptoChargeRepository.GetChainCreateName(chainId);
                if (String.IsNullOrEmpty(chainCreateName))
                {
                    return new ResponseBuilder(ErrorCodes.CHAIN_NOT_FOUND, _accountSession.Language);
                }

                var wallet = _walletService.Create(chainCreateName);
                NLogManager.Info($"_walletService1: {JsonConvert.SerializeObject(wallet)}");
                if (wallet == null)
                {
                    NLogManager.Info("_walletService1");
                    return new ResponseBuilder(ErrorCodes.WALLET_CREATE_FAIL, _accountSession.Language);
                }

                int resp = _cryptoChargeRepository.CreateAddress(accountId, chainId, wallet.Address, wallet.PrivateKey);

                if (resp != 1)
                {
                    NLogManager.Info("_walletService2");
                    return new ResponseBuilder(ErrorCodes.WALLET_CREATE_FAIL, _accountSession.Language);
                }

                if (chainId == 1)
                {
                    _cryptoChargeRepository.CreateAddress(accountId, 2, wallet.Address, wallet.PrivateKey);
                    AddTaTum(accountId, 2, "BSC", wallet.Address);


                }
                else if (chainId == 2)
                {
                    _cryptoChargeRepository.CreateAddress(accountId, 1, wallet.Address, wallet.PrivateKey);
                    AddTaTum(accountId, 1, "ETH", wallet.Address);

                }

                AddTaTum(accountId, chainId, chainCreateName, wallet.Address);

                _cacheHandler.MemSet(keyHu, wallet.Address);
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, wallet.Address);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        private void AddTaTum(long userId, int chainId, string chainCreateName, string address)
        {
            string tatumtokenId = _tatumService.CreateASubscriptionToken(chainCreateName, address);
            string tatumNativeId = _tatumService.CreateASubscriptionNative(chainCreateName, address); ;

            if (String.IsNullOrEmpty(tatumtokenId) || String.IsNullOrEmpty(tatumNativeId))
            {
                return;
            }

            _cryptoChargeRepository.UpdateTaTum(userId, chainId, tatumtokenId, tatumNativeId);
        }
        [HttpPost("CallbackDeposit")]
        public dynamic CallbackDeposit(TatumDepositReq req)
        {
            try
            {
                NLogManager.Info(JsonConvert.SerializeObject(req));
                if (req.SubscriptionType != "INCOMING_FUNGIBLE_TX" && req.SubscriptionType != "INCOMING_NATIVE_TX")
                {
                    return new ResponseBuilder(ErrorCodes.SUBSCRIPTION_TYPE_INVALID, _accountSession.Language);
                }

                string secretkey = _contextAccessor.HttpContext.Request.Query["secretkey"];
                string ip = _contextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = _contextAccessor.HttpContext.Request.Headers["REMOTE_ADDR"];
                }

                if (secretkey != CALLBACK_SECRET_KEY)
                {
                    return new ResponseBuilder(ErrorCodes.SIGN_FAIL, _accountSession.Language);
                }

                if (req.SubscriptionType == "INCOMING_NATIVE_TX")
                {
                    req.ContractAddress = req.Currency;
                }

                string symbol;
                decimal currencyPrice;
                long rateUsdtToVnd;
                long amountVnd;

                var resp = _cryptoChargeRepository.Deposit(req.Currency, req.Chain, req.Amount, req.CounterAddress, req.Address, req.SubscriptionType, req.BlockNumber, req.TxId, req.ContractAddress, out symbol, out currencyPrice, out rateUsdtToVnd, out amountVnd);
                string msg;

                long userId;
                _cryptoChargeRepository.GetUserIdByAddress(req.Address, out userId);

                string userName = "Không xác định";
                string nickName = "Không xác định";

                if (userId != 0)
                {
                    var account = _cryptoChargeRepository.GetAccountInfo(userId, null, null, 1);

                    if (account != null)
                    {
                        userName = account.AccountUserName;
                        nickName = account.AccountName;
                    }
                }

                if (resp == 1)
                {
                    msg = $"<b>[THÔNG BÁO NẠP {symbol}]</b>\r\n";
                    msg += $"Mạng lưới: {req.Chain}\r\n";
                    msg += $"Địa chỉ ví nạp: {req.CounterAddress}\r\n";
                    msg += $"Địa chỉ ví nhận: {req.Address}\r\n";
                    msg += $"----------------------------------------\r\n";
                    msg += $"Số lượng: {req.Amount.ToString("N8")} ({req.Amount}) {symbol}\r\n";
                    msg += $"Giá hiện tại: {currencyPrice.ToString("N8")}  ({currencyPrice}) USDT\r\n";
                    msg += $"Tỷ giá USDT/VNĐ: {rateUsdtToVnd.ToString("N0")}\r\n";
                    msg += $"Số tiền nhận: {amountVnd.ToString("N0")} VNĐ\r\n";
                    msg += $"----------------------------------------\r\n";
                    msg += $"AccountId: {userId}\r\n";
                    msg += $"UserName: {userName}\r\n";
                    msg += $"NickName: {nickName}\r\n";
                    msg += $"----------------------------------------\r\n";
                    msg += $"TxId: {req.TxId}\r\n";
                    msg += $"Trạng thái: <b>Thành công</b>\r\n";

                    ThreadPool.QueueUserWorkItem(o => { _walletService.SendTelePush(msg, 998); });

                    return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
                }

                msg = $"<b>[THÔNG BÁO NẠP]</b>\r\n";
                msg += $"Mạng lưới: {req.Chain}\r\n";
                msg += $"Địa chỉ ví nạp: {req.CounterAddress}\r\n";
                msg += $"Địa chỉ ví nhận: {req.Address}\r\n";
                msg += $"Giá trị: {req.Amount}\r\n";
                msg += $"TxId: {req.TxId}\r\n";
                msg += $"----------------------------------------\r\n";
                msg += $"AccountId: {userId}\r\n";
                msg += $"UserName: {userName}\r\n";
                msg += $"NickName: {nickName}\r\n";
                msg += $"----------------------------------------\r\n";
                msg += $"Trạng thái: <b>Thất bại - Resp: {resp}</b>\r\n";

                ThreadPool.QueueUserWorkItem(o => { _walletService.SendTelePush(msg, 998); });

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpGet("GetHistory")]
        public dynamic GetHistory(int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                var accountId = _accountSession.AccountID;
                var displayName = _accountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new ResponseBuilder(ErrorCodes.NOT_AUTHEN, _accountSession.Language);
                }

                int totalRecord;
                var list = _cryptoChargeRepository.GetHistoryByUserId(accountId, pageIndex, pageSize, out totalRecord);
                var paging = new
                {
                    List = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPage = (totalRecord % pageSize) != 0 ? (totalRecord / pageSize) + 1 : totalRecord / pageSize,
                    TotalRecord = totalRecord
                };
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, paging);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        [HttpGet("GetConfig")]
        public dynamic GetConfig()
        {
            try
            {
                var accountId = _accountSession.AccountID;
                var displayName = _accountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new ResponseBuilder(ErrorCodes.NOT_AUTHEN, _accountSession.Language);
                }

                var data = new
                {
                    RateDepositUsdtToVnd = GetDepositRateToVnd(),
                    RateWithdrawUsdtToVnd = GetWithdrawRateToVnd(),
                    MinUsdtDeposit = GetMinUsdtToDeposit(),
                    MinUsdtWithdraw = GetMinUsdtToWithdraw(),
                };
                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, data);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        private long GetDepositRateToVnd()
        {
            string keyHu = _cacheHandler.GeneralRedisKey("Config", $"CRYPTO_CHARGE_DEPOSIT_RATE_TO_VND");
            long rate;
            if (_cacheHandler.MemTryGet(keyHu, out rate))
            {
                return rate;
            }
            string value;
            _cryptoChargeRepository.GetCoreConfig("VALUE", "CRYPTO_CHARGE_DEPOSIT_RATE_TO_VND", out value);
            long min = long.Parse(value);
            _cacheHandler.MemSet(keyHu, min);

            return min;
        }
        private long GetWithdrawRateToVnd()
        {
            string keyHu = _cacheHandler.GeneralRedisKey("Config", $"CRYPTO_CHARGE_WITHDRAW_RATE_TO_VND");
            long rate;
            if (_cacheHandler.MemTryGet(keyHu, out rate))
            {
                return rate;
            }

            string value;
            _cryptoChargeRepository.GetCoreConfig("VALUE", "CRYPTO_CHARGE_WITHDRAW_RATE_TO_VND", out value);
            long min = long.Parse(value);
            _cacheHandler.MemSet(1800, keyHu, min);

            return min;
        }
        private decimal GetMinUsdtToDeposit()
        {
            string keyHu = _cacheHandler.GeneralRedisKey("Config", $"CRYPTO_CHARGE_DEPOSIT_MIN_USDT");
            long rate;

            if (_cacheHandler.MemTryGet(keyHu, out rate))
            {
                return rate;
            }

            string value;
            _cryptoChargeRepository.GetCoreConfig("VALUE", "CRYPTO_CHARGE_DEPOSIT_MIN_USDT", out value);
            decimal num = decimal.Parse(value);
            _cacheHandler.MemSet(keyHu, num);

            return num;
        }

        private decimal GetMinUsdtToWithdraw()
        {
            string keyHu = _cacheHandler.GeneralRedisKey("Config", $"CRYPTO_CHARGE_WITHDRAW_MIN_USDT");
            long rate;

            if (_cacheHandler.MemTryGet(keyHu, out rate))
            {
                return rate;
            }

            string value;
            _cryptoChargeRepository.GetCoreConfig("VALUE", "CRYPTO_CHARGE_WITHDRAW_MIN_USDT", out value);
            decimal num = decimal.Parse(value);
            _cacheHandler.MemSet(keyHu, num);
            return num;
        }

        [HttpPost("Withdraw")]
        public dynamic Withdraw(CurrencyWithdrawReq req)
        {
            try
            {
                var accountId = _accountSession.AccountID;
                var displayName = _accountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new ResponseBuilder(ErrorCodes.NOT_AUTHEN, _accountSession.Language);
                }

                decimal min = GetMinUsdtToWithdraw();

                if (req.Amount < min)
                {
                    return new ResponseBuilder(ErrorCodes.WITHDRAW_AMOUNT_LESS_THAN_MIN, _accountSession.Language);
                }

                if (!IsValidAddress(req.Address))
                {
                    return new ResponseBuilder(ErrorCodes.WALLET_ADDRESS_INVALID, _accountSession.Language);
                }

                var account = _cryptoChargeRepository.GetAccountInfo(accountId, null, null, 1);

                if (account.Status != 1)
                {
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_BLOCKED, _accountSession.Language);
                }

                decimal amountMaxAutoWithdraw = GetMaxUsdtAutoToWithdraw();

                int status = 2;
                if (req.Amount <= amountMaxAutoWithdraw)
                {
                    status = 3;
                }

                long withdraRateToVnd = GetWithdrawRateToVnd();
                long amountVnd = (long)(withdraRateToVnd * req.Amount);
                long balance;
                var resp = _cryptoChargeRepository.Withdraw(accountId, req.CurrencyId, req.ChainId, req.Amount, withdraRateToVnd, amountVnd, req.Address, status, out balance);

                if (resp == 1)
                {
                    string msg = $"<b>[THÔNG BÁO RÚT USDT {(status == 3 ? "- Duyệt Auto" : "")}]</b>\r\n";
                    msg += $"Tên nhân vật: {displayName}\r\n";
                    msg += $"Địa chỉ ví rút: {req.Address}\r\n";
                    msg += $"Số tiền: ${req.Amount}\r\n";
                    msg += $"Trạng thái: <b>{(status == 3 ? "- Đã duyệt auto" : "Chờ xác nhận")}</b>\r\n";

                    ThreadPool.QueueUserWorkItem(o => { _walletService.SendTelePush(msg, 999); });

                    return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language, balance);

                }
                else if (resp == -502)
                {
                    return new ResponseBuilder(ErrorCodes.WITHDRAW_CONTRACT_ADDRESS_NOT_FOUND, _accountSession.Language);
                }
                else if (resp == -503)
                {
                    return new ResponseBuilder(ErrorCodes.NOT_AUTHEN, _accountSession.Language);
                }
                else if (resp == -504)
                {
                    return new ResponseBuilder(ErrorCodes.ACCOUNT_BALANCE_NOT_ENOUGH, _accountSession.Language);
                }
                else if (resp == -505)
                {
                    return new ResponseBuilder(ErrorCodes.WITHDRAW_AMOUNT_LESS_THAN_MIN, _accountSession.Language);
                }
                else if (resp == -511)
                {
                    return new ResponseBuilder(ErrorCodes.MAX_WITHDRAW_PER_DAY, _accountSession.Language);
                }

                return new ResponseBuilder(ErrorCodes.SUCCESS, _accountSession.Language);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);

                return new ResponseBuilder(ErrorCodes.SYSTEM_ERROR, _accountSession.Language);
            }
        }
        private bool IsValidAddress(string address)
        {
            var addressUtil = new AddressUtil();
            return addressUtil.IsValidEthereumAddressHexFormat(address);
        }
        private decimal GetMaxUsdtAutoToWithdraw()
        {
            string keyHu = _cacheHandler.GeneralRedisKey("Config", $"CRYPTO_CHARGE_WITHDRAW_AUTO_MAX_USDT");
            long rate;

            if (_cacheHandler.MemTryGet(keyHu, out rate))
            {
                return rate;
            }

            string value;
            _cryptoChargeRepository.GetCoreConfig("VALUE", "CRYPTO_CHARGE_WITHDRAW_AUTO_MAX_USDT", out value);
            decimal num = decimal.Parse(value);
            _cacheHandler.MemSet(keyHu, num);

            return num;
        }
    }
}
