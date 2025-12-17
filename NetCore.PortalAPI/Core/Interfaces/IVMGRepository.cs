using ServerCore.PortalAPI.Core.Domain.Models.VMG;
using System.Collections.Generic;

namespace NetCore.PortalAPI.Core.Interfaces
{
    public interface IVMGRepository
    {
        Agency GetAgentById(int id);
        int DepositUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance);
        int CashoutUser(long accountID, string username, int merchantID, decimal amount, string transID, out decimal balance);
        List<BetHistory> GetAccountBetHistory(long fromDate, long toDate, int merchantID, out int totalPage, int pageNumber = 1, int pageSize = 50);
        ChipValue GetChipByCurrencyID(int currencyID);
    }
}
