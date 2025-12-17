using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore.Utilities.Models;

namespace NetCore.PortalAPI.Core.Interfaces {
    public interface ISuportRepository {
        List<TopupHistory> GetTopupHistory(string permitKey, string userName, string dateFrom, string dateTo, string nickName);
        List<UserTransactionHistory> GetUserTransactionHistory(string permitKey, string userName, string dateFrom, string dateTo, string nickName, int gameType);
    }
}
