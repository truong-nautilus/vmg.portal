using NetCore.PortalAPI.Core.DTO;
using ServerCore.PortalAPI.Core.Domain.Models.Payment;
using System;

namespace NetCore.PortalAPI.Core.Interfaces
{
    public interface IPaymentRepository
    {
        [Obsolete("Old version of billing Vcoin - use method TopupStarByVTCCard", false)]
        int TopupVcoinByVTCCard(ref int vcoin, ref int gift, ref int totalVcoin, VTOCard cardInfo);
        int TopupStarByVTCCard(ref int vcoin, ref int gift, ref int totalVcoin, ref long totalStar, VTOCard cardInfo);
        long DepositMomoRequestInsert(MomoLogs obj);
        void DepositMomoRequestUpdate(long id, string bankName, string bankAccount, string bankAccountName, string reqCode, string transId, int status, string redirectLink, string deepLink);
        BalanceInfo TopupAccount(int serviceID, string serrviceKey, int accountID, string userName, int amount, string description, long referenceID, int merchantID, int sourceID, string clientIP);
        MomoLogs GetMomoLogById(string id);
        void UpdateCallback(long id, long realAmount, long virtualAmount, string transCode, int status);
        long CashOutMomoRequestInsert(MomoOutLogs obj);
        void CashOutMomoRequestUpdate(long id, int status, long amount);
        MomoOutLogs GetMomoOutLogByRequestId(string id);
        BalanceInfo DeductAccount(int serviceID, string serrviceKey, long accountID, string userName, long amount, long referenceID, string description, string merchantCode,
            int merchantID, int sourceID, string clientIP);
        long DepositCardRequestInsert(CardLogs obj);
        void DepositCardRequestUpdate(long id, string transId, string message, int status);
        CardLogs GetCardLogById(string id);
        void CardCallbackUpdate(long id, int realAmount, int virtualAmount, string message, int status);
        long DepositBankRequest(BankLogs obj);
        void DepositBankRequestUpdate(long id, string bankName, string bankAccount, string bankAccountName, string reqCode, string transId, int status);
        BankLogs GetBankLogById(long id);
        void BankCallbackUpdate(long id, long realAmount, long virtualAmount, string transCode, int status);
        long GetBalance(long accountID);
        long CashoutBankInsert(CashoutLogs obj);
        CashoutLogs GetCashoutBankById(long id);
        void CashoutBankCallbackUpdate(long id, long realAmount, string transCode, int status);
        long CashoutUpdateStatus(long id, int status);
    }
}