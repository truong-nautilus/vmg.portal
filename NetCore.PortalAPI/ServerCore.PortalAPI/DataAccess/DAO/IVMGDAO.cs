using ServerCore.PortalAPI.Models.VMG;
using System.Collections.Generic;

namespace ServerCore.PortalAPI.DataAccess.DAO
{
    public interface IVMGDAO
    {
        Agency GetAgentById(int id);
        int DepositUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance);
        int CashoutUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance);
        List<BetHistory> GetAccountBetHistory(long fromDate, long toDate, int merchantID, out int totalPage, int pageNumber = 1, int pageSize = 50);
        ChipValue GetChipByCurrencyID(int currencyID);
    }
}
