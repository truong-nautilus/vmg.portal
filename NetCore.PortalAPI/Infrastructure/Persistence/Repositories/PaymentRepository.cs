using Microsoft.Extensions.Options;
using NetCore.Utils.Interfaces;
using NetCore.PortalAPI.Core.Interfaces;
using NetCore.PortalAPI.Core.DTO;
using ServerCore.PortalAPI.Core.Domain.Models.Payment;
using ServerCore.Utilities.Utils;
using System;
using System.Data;
using System.Data.SqlClient;

namespace NetCore.PortalAPI.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppSettings appSettings;
        private readonly IDBHelper _dbHepler;
        public PaymentRepository(IOptions<AppSettings> options)
        {
            appSettings = options.Value;
        }
        /// <summary>
        /// // nạp Vcoin qua thẻ cào Vcoin
        /// cardInfo.Error: 
        /// >=0: nạp thành công
        /// -1: Thẻ đã dùng rồi
        /// -2: thẻ đã bị khóa
        /// -3: thẻ quá hạn
        /// -10: seri ko tồn tại
        /// -11: mã thẻ ko đúng
        /// -12: Thẻ ko tồn tại
        /// </summary>
        /// <param name="vcoin"></param>
        /// <param name="gift"></param>
        /// <param name="totalVcoin"></param>
        /// <param name="cardInfo"></param>
        [Obsolete("Old version of billing Vcoin - use method TopupStarByVTCCard", false)]
        public int TopupVcoinByVTCCard(ref int vcoin, ref int gift, ref int totalVcoin, VTOCard cardInfo)
        {
            NLogManager.Info(string.Format("TopupVcoinByVTCCard:{0}_CardID: {1}_oCardCode:{2}_CardCode:{3}", cardInfo.AccountName, cardInfo.CardID, cardInfo.OriginalCode, cardInfo.CardCode));
            return -1;
            //var pars = new SqlParameter[13];
            //pars[0] = new SqlParameter("@_ServiceID", cardInfo.ServiceID);
            //pars[1] = new SqlParameter("@_ServiceKey", cardInfo.ServiceKey);
            //pars[2] = new SqlParameter("@_OrderID", SqlDbType.Int) { Value = 0 };
            //pars[3] = new SqlParameter("@_AccountName", cardInfo.AccountName);
            //pars[4] = new SqlParameter("@_CardID", cardInfo.CardID);
            //pars[5] = new SqlParameter("@_oCardCode", cardInfo.OriginalCode);
            //pars[6] = new SqlParameter("@_CardCode", cardInfo.CardCode);
            //pars[7] = new SqlParameter("@_AccountIP", cardInfo.AccountIP);
            //pars[8] = new SqlParameter("@_CardValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[9] = new SqlParameter("@_Vcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[10] = new SqlParameter("@_Gift", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[11] = new SqlParameter("@_TotalVCoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

            //try
            //{
            //    var db = new DBHelper(AppSettings.IntecomApiConnectionString);
            //    if (cardInfo.Prefix.Equals("PM", StringComparison.OrdinalIgnoreCase))
            //    {
            //        db.ExecuteNonQuerySP("SP_Account_InputCard", pars);
            //    }
            //    else if (cardInfo.Prefix.Equals("ID", StringComparison.OrdinalIgnoreCase))
            //    {
            //        db.ExecuteNonQuerySP("SP_Account_InputCard_ID", pars);
            //    }
            //    else throw new Exception("Invalid card prefix");

            //    var result = (int)pars[12].Value;
            //    if (result == 0)
            //    {
            //        cardInfo.CardValue = (int)pars[8].Value;
            //        vcoin = (int)pars[9].Value;
            //        gift = (int)pars[10].Value;
            //        totalVcoin = (int)pars[11].Value;
            //    }
            //    NLogManager.Info(string.Format("TopupVcoinByVTCCard:{0}_CardID: {1}_oCardCode:{2}_CardCode:{3}_Result:{4}", cardInfo.AccountName, cardInfo.CardID, cardInfo.OriginalCode, cardInfo.CardCode, result));

            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    NLogManager.Exception(ex);
            //    return -99;
            //}
        }

        /// <summary>
        /// // nạp SAO qua thẻ cào Vcoin
        /// cardInfo.Error: 
        /// >=0: nạp thành công
        /// -1: Thẻ đã dùng rồi
        /// -2: thẻ đã bị khóa
        /// -3: thẻ quá hạn
        /// -10: seri ko tồn tại
        /// -11: mã thẻ ko đúng
        /// -12: Thẻ ko tồn tại
        /// </summary>
        /// <param name="vcoin"></param>
        /// <param name="gift"></param>
        /// <param name="totalVcoin"></param>
        /// <param name="cardInfo"></param>
        public int TopupStarByVTCCard(ref int vcoin, ref int gift, ref int totalVcoin, ref long totalStar, VTOCard cardInfo)
        {
            NLogManager.Info(string.Format("TopupStarByVTCCard:{0}_CardID: {1}_oCardCode:{2}_CardCode:{3}", cardInfo.AccountName, cardInfo.CardID, cardInfo.OriginalCode, cardInfo.CardCode));
            return -1;
            //var pars = new SqlParameter[15];
            //pars[0] = new SqlParameter("@_ServiceID", cardInfo.ServiceID);
            //pars[1] = new SqlParameter("@_ServiceKey", cardInfo.ServiceKey);
            //pars[2] = new SqlParameter("@_OrderID", SqlDbType.Int) { Value = 0 };
            //pars[3] = new SqlParameter("@_AccountName", cardInfo.AccountName);
            //pars[4] = new SqlParameter("@_CardID", cardInfo.CardID);
            //pars[5] = new SqlParameter("@_oCardCode", cardInfo.OriginalCode);
            //pars[6] = new SqlParameter("@_CardCode", cardInfo.CardCode);
            //pars[7] = new SqlParameter("@_AccountIP", cardInfo.AccountIP);
            //pars[8] = new SqlParameter("@_CardValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[9] = new SqlParameter("@_Vcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[10] = new SqlParameter("@_Gift", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[11] = new SqlParameter("@_TotalVCoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //pars[13] = new SqlParameter("@_IsTopupStar", true);
            //pars[14] = new SqlParameter("@_GameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };//OUTPUT -- Số dư Sao (nếu nạp Sao)

            //try
            //{
            //    var db = new DBHelper(AppSettings.IntecomApiConnectionString);
            //    if (cardInfo.Prefix.Equals("PM", StringComparison.OrdinalIgnoreCase))
            //    {
            //        db.ExecuteNonQuerySP("SP_Account_InputCard", pars);
            //    }
            //    else if (cardInfo.Prefix.Equals("ID", StringComparison.OrdinalIgnoreCase))
            //    {
            //        db.ExecuteNonQuerySP("SP_Account_InputCard_ID", pars);
            //    }
            //    else throw new Exception("Invalid card prefix");

            //    var result = (int)pars[12].Value;
            //    if (result == 0)
            //    {
            //        cardInfo.CardValue = (int)pars[8].Value;
            //        vcoin = (int)pars[9].Value;
            //        gift = (int)pars[10].Value;
            //        totalVcoin = (int)pars[11].Value;
            //        totalStar = (long)pars[14].Value;
            //    }
            //    NLogManager.Info(string.Format("TopupStarByVTCCard:{0}_CardID: {1}_oCardCode:{2}_CardCode:{3}_Result:{4}", cardInfo.AccountName, cardInfo.CardID, cardInfo.OriginalCode, cardInfo.CardCode, result));

            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    NLogManager.Exception(ex);
            //    return -99;
            //}
        }
        public long DepositMomoRequestInsert(MomoLogs obj)
        {
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@partnerId", obj.partnerId);
                pars[1] = new SqlParameter("@orgRequestId", obj.orgRequestId);
                pars[2] = new SqlParameter("@accountId", obj.accountId);
                pars[3] = new SqlParameter("@accountName", obj.accountName);
                pars[4] = new SqlParameter("@bankCode", obj.bankCode);
                pars[5] = new SqlParameter("@amount", obj.amount);
                pars[6] = new SqlParameter("@description", obj.description);
                pars[7] = new SqlParameter("@urlCallback", obj.amount);
                pars[8] = new SqlParameter("@customContent", obj.description);
                pars[9] = new SqlParameter("@id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
                _dbHepler.ExecuteNonQuerySP("sp_MomoLogs_Insert", pars);
                return Convert.ToInt64((pars[10].Value.ToString()));
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return -99;
            }
        }
        public void DepositMomoRequestUpdate(long id, string bankName, string bankAccount, string bankAccountName, string reqCode, string transId, int status, string redirectLink, string deepLink)
        {
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@id", id);
                pars[1] = new SqlParameter("@bankName", bankName);
                pars[2] = new SqlParameter("@bankAccount", bankAccount);
                pars[3] = new SqlParameter("@bankAccountName", bankAccountName);
                pars[4] = new SqlParameter("@reqCode", reqCode);
                pars[5] = new SqlParameter("@transId", transId);
                pars[6] = new SqlParameter("@status", status);
                pars[7] = new SqlParameter("@redirectLink", redirectLink);
                pars[8] = new SqlParameter("@deepLink", deepLink);
                _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
                _dbHepler.ExecuteNonQuerySP("sp_MomoLogs_UpdatePartner", pars);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
        }
        public BalanceInfo TopupAccount(int serviceID, string serrviceKey, int accountID, string userName, int amount, string description, long referenceID, int merchantID, int sourceID, string clientIP)
        {
            try
            {
                var pars = new SqlParameter[14];
                pars[0] = new SqlParameter("@_ServiceID", serviceID);
                pars[1] = new SqlParameter("@_ServiceKey", serrviceKey);
                pars[2] = new SqlParameter("@_AccountID", accountID);
                pars[3] = new SqlParameter("@_Username", userName);
                pars[4] = new SqlParameter("@_Amount", amount);
                pars[5] = new SqlParameter("@_Description", description);
                pars[6] = new SqlParameter("@_ReferenceID", referenceID);
                pars[7] = new SqlParameter("@_MerchantID", merchantID);
                pars[8] = new SqlParameter("@_SourceID", sourceID);
                pars[9] = new SqlParameter("@_ClientIP", clientIP);
                pars[10] = new SqlParameter("@_TotalGameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[11] = new SqlParameter("@_GameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_VcoinBalance", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[13] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(appSettings.BillingDatabaseAPIConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_Transaction_TopupAccount", pars);
                return new BalanceInfo
                {
                    Balance = Convert.ToInt64(pars[11].Value),
                    ResponseCode = Convert.ToInt64(pars[13].Value)
                };
            }
            catch
            {
                return new BalanceInfo
                {
                    Balance = 0,
                    ResponseCode = -1
                };
            }
        }
        public MomoLogs GetMomoLogById(string id)
        {
            var objParam = new SqlParameter[]
                                    {
                                        new SqlParameter("@orgRequestId", id)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            var lRet = _dbHepler.GetListSP<MomoLogs>("sp_MomoLogs_GetByorgRequestId", objParam);
            if (lRet == null || lRet.Count == 0)
                return new MomoLogs();
            return lRet[0];
        }
        public void UpdateCallback(long id, long realAmount, long virtualAmount, string transCode, int status)
        {

            var objParam = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id),
                                        new SqlParameter("@realAmount", realAmount),
                                        new SqlParameter("@virtualAmount", virtualAmount),
                                        new SqlParameter("@transCode", transCode),
                                        new SqlParameter("@status", status)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_MomoLogs_UpdateCallback", objParam);
        }
        public long CashOutMomoRequestInsert(MomoOutLogs obj)
        {
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@requestId", obj.requestId);
                pars[1] = new SqlParameter("@bankAccount", obj.bankAccount);
                pars[2] = new SqlParameter("@accountId", obj.accountId);
                pars[3] = new SqlParameter("@accountName", obj.accountName);
                pars[4] = new SqlParameter("@amount", obj.amount);
                pars[5] = new SqlParameter("@urlCallback", obj.urlCallback);
                pars[6] = new SqlParameter("@signature", obj.signature);
                pars[7] = new SqlParameter("@id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
                _dbHepler.ExecuteNonQuerySP("sp_MomoOutLogs_Insert", pars);
                return Convert.ToInt64((pars[7].Value.ToString()));
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return -99;
            }
        }
        public void CashOutMomoRequestUpdate(long id, int status, long amount)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@id", id);
                pars[1] = new SqlParameter("@status", status);
                pars[2] = new SqlParameter("@amount", amount);
                _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
                _dbHepler.ExecuteNonQuerySP("sp_MomoOutLogs_Update", pars);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
        }
        public MomoOutLogs GetMomoOutLogByRequestId(string id)
        {
            var objParam = new SqlParameter[]
                                    {
                                        new SqlParameter("@requestId", id)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            var lRet = _dbHepler.GetListSP<MomoOutLogs>("sp_MomoOutLogs_GetByRequestId", objParam);
            if (lRet == null || lRet.Count == 0)
                return new MomoOutLogs();
            return lRet[0];
        }
        public BalanceInfo DeductAccount(int serviceID, string serrviceKey, long accountID, string userName, long amount, long referenceID, string description, string merchantCode,
            int merchantID, int sourceID, string clientIP)
        {
            try
            {
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_ServiceID", serviceID);
                pars[1] = new SqlParameter("@_ServiceKey", serrviceKey);
                pars[2] = new SqlParameter("@_AccountID", accountID);
                pars[3] = new SqlParameter("@_Username", userName);
                pars[4] = new SqlParameter("@_Amount", amount);
                pars[5] = new SqlParameter("@_ReferenceID", referenceID);
                pars[6] = new SqlParameter("@_Description", description);
                pars[7] = new SqlParameter("@_MerchantCode", merchantCode);
                pars[8] = new SqlParameter("@_MerchantID", merchantID);
                pars[9] = new SqlParameter("@_SourceID", sourceID);
                pars[10] = new SqlParameter("@_ClientIP", clientIP);
                pars[11] = new SqlParameter("@_GameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(appSettings.BillingDatabaseAPIConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_Transaction_Bit_Deduct", pars);

                //long ResponseCode = Convert.ToInt64(pars[11].Value);
                //return ResponseCode;
                return new BalanceInfo
                {
                    Balance = Convert.ToInt64(pars[11].Value),
                    ResponseCode = Convert.ToInt64(pars[12].Value)
                };
            }
            catch
            {
                return new BalanceInfo
                {
                    Balance = 0,
                    ResponseCode = -1
                };
            }
        }
        public long DepositCardRequestInsert(CardLogs obj)
        {
            var pars = new SqlParameter[10];
            pars[0] = new SqlParameter("@partnerId", obj.partnerId);
            pars[1] = new SqlParameter("@orgRequestId", obj.orgRequestId);
            pars[2] = new SqlParameter("@accountId", obj.accountId);
            pars[3] = new SqlParameter("@accountName", obj.accountName);
            pars[4] = new SqlParameter("@carrier", obj.carrier);
            pars[5] = new SqlParameter("@pin", obj.pin);
            pars[6] = new SqlParameter("@serial", obj.serial);
            pars[7] = new SqlParameter("@amount", obj.amount);
            pars[8] = new SqlParameter("@description", obj.description);
            pars[9] = new SqlParameter("@id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CardLogs_Insert", pars);
            return (long)pars[9].Value;
        }
        public void DepositCardRequestUpdate(long id, string transId, string message, int status)
        {
            var pars = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id),
                                        new SqlParameter("@transId", transId),
                                        new SqlParameter("@message", message),
                                        new SqlParameter("@status", status)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CardLogs_UpdatePartner", pars);
        }
        public CardLogs GetCardLogById(string id)
        {
            var objParam = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            var lRet = _dbHepler.GetListSP<CardLogs>("sp_CardLogs_GetById", objParam);
            if (lRet == null || lRet.Count == 0)
                return new CardLogs();
            return lRet[0];
        }
        public void CardCallbackUpdate(long id, int realAmount, int virtualAmount, string message, int status)
        {

            var pars = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id),
                                        new SqlParameter("@realAmount", realAmount),
                                        new SqlParameter("@virtualAmount", virtualAmount),
                                        new SqlParameter("@message", message),
                                        new SqlParameter("@status", status)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CardLogs_UpdateCallback", pars);
        }
        public long DepositBankRequest(BankLogs obj)
        {
            var pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@partnerId", obj.partnerId);
            pars[1] = new SqlParameter("@orgRequestId", obj.orgRequestId);
            pars[2] = new SqlParameter("@accountId", obj.accountId);
            pars[3] = new SqlParameter("@accountName", obj.accountName);
            pars[4] = new SqlParameter("@bankCode", obj.bankCode);
            pars[5] = new SqlParameter("@amount", obj.amount);
            pars[6] = new SqlParameter("@description", obj.description);
            pars[7] = new SqlParameter("@id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_BankLogs_Insert", pars);
            return (long)pars[7].Value;
        }
        public void DepositBankRequestUpdate(long id, string bankName, string bankAccount, string bankAccountName, string reqCode, string transId, int status)
        {
            try
            {
                var pars = new SqlParameter[]
                {
                    new SqlParameter("@id", id),
                    new SqlParameter("@bankName", bankName),
                    new SqlParameter("@bankAccount", bankAccount),
                    new SqlParameter("@bankAccountName", bankAccountName),
                    new SqlParameter("@reqCode", reqCode),
                    new SqlParameter("@transId", transId),
                    new SqlParameter("@status", status)
                };
                _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
                _dbHepler.ExecuteNonQuerySP("sp_BankLogs_UpdatePartner", pars);
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
            }
        }
        public BankLogs GetBankLogById(long id)
        {
            var objParam = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            var lRet = _dbHepler.GetListSP<BankLogs>("sp_BankLogs_GetById", objParam);
            if (lRet == null || lRet.Count == 0)
                return new BankLogs();
            return lRet[0];
        }
        public void BankCallbackUpdate(long id, long realAmount, long virtualAmount, string transCode, int status)
        {

            var pars = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id),
                                        new SqlParameter("@realAmount", realAmount),
                                        new SqlParameter("@virtualAmount", virtualAmount),
                                        new SqlParameter("@transCode", transCode),
                                        new SqlParameter("@status", status)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_BankLogs_UpdateCallback", pars);
        }
        public long GetBalance(long accountID)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_AccountID", accountID);
                pars[1] = new SqlParameter("@_TotalGameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                _dbHepler.SetConnectionString(appSettings.BillingDatabaseAPIConnectionString);
                _dbHepler.ExecuteNonQuerySP("SP_Accounts_GetBalance", pars);

                long ResponseCode = Convert.ToInt64(pars[1].Value);

                return ResponseCode;
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return -99;
            }
        }
        public long CashoutBankInsert(CashoutLogs obj)
        {
            var pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@orgRequestId", obj.orgRequestId);
            pars[1] = new SqlParameter("@accountId", obj.accountId);
            pars[2] = new SqlParameter("@accountName", obj.accountName);
            pars[3] = new SqlParameter("@bankCode", obj.bankCode);
            pars[4] = new SqlParameter("@bankAccount", obj.bankAccount);
            pars[5] = new SqlParameter("@bankAccountName", obj.bankAccountName);
            pars[6] = new SqlParameter("@amount", obj.amount);
            pars[7] = new SqlParameter("@description", obj.description);
            pars[8] = new SqlParameter("@id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CashoutLogs_Insert", pars);
            return (long)pars[8].Value;
        }
        public CashoutLogs GetCashoutBankById(long id)
        {
            var pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@id", id);
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            var lRet = _dbHepler.GetListSP<CashoutLogs>("sp_CashoutLogs_GetById", pars);
            if (lRet == null || lRet.Count == 0)
                return new CashoutLogs();
            return lRet[0];
        }
        public void CashoutBankCallbackUpdate(long id, long realAmount, string transCode, int status)
        {
            var pars = new SqlParameter[]
                                    {
                                        new SqlParameter("@id", id),
                                        new SqlParameter("@realAmount", realAmount),
                                        new SqlParameter("@transCode", transCode),
                                        new SqlParameter("@status", status)
                                    };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CashoutLogs_UpdateCallback", pars);
        }
        public long CashoutUpdateStatus(long id, int status)
        {
            var pars = new SqlParameter[3];

            pars[0] = new SqlParameter("@_id", id);
            pars[1] = new SqlParameter("@_status", status);
            pars[2] = new SqlParameter("@_responseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            _dbHepler.SetConnectionString(appSettings.PaymentDBConnectionString);
            _dbHepler.ExecuteNonQuerySP("sp_CashoutLogs_UpdateStatus", pars);
            return (long)pars[2].Value;
        }
    }
}