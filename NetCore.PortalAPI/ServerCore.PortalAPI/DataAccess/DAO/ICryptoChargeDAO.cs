using ServerCore.PortalAPI.Models;
using ServerCore.PortalAPI.Models.Crypto;
using System.Collections.Generic;

namespace ServerCore.PortalAPI.DataAccess.DAO
{
    public interface ICryptoChargeDAO
    {
        List<CurrencyProfile> GetListCurrency();
        string GetAddress(long userId, int chainId);
        string GetChainCreateName(int chainId);
        int CreateAddress(long userId, int chainId, string address, string privateKey);
        int UpdateTaTum(long userId, int chainId, string tatumtokenId, string tatumNativeId);
        int Deposit(string currrency, string chain, decimal amount, string senderAddress, string receiverAddress, string subscriptionType, long blockNumber, string txId, string contractAddress, out string currencySymbol, out decimal currencyPrice, out long rateUsdtToVnd, out long amountVnd);
        void GetUserIdByAddress(string address, out long userId);
        Account GetAccountInfo(long accountId, string username, string phone, int ServiceID);
        List<CurrencyTransactionHistory> GetHistoryByUserId(long userId, int pageIndex, int pageSize, out int totalRecord);
        void GetCoreConfig(string ParamType, string Code, out string Value);
        int Withdraw(long userId, int currencyId, int chainId, decimal amount, long rateToVnd, long amountVnd, string receiverAddress, int status, out long balance);
        List<ChainProfile> GetListChain(int currencyId);
    }
}
