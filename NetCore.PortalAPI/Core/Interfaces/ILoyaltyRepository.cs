using ServerCore.PortalAPI.Core.Domain.Models;
using System;
using System.Collections.Generic;

namespace NetCore.PortalAPI.Core.Interfaces
{
	public interface ILoyaltyRepository
	{
		List<VpLevel> GetVpLevel(int? type);
		VipPointByAccountId FindVipPointInfo(long accountID);
		List<AccountVpLevelExt> FindTopAccountVpTransaction(long accountID, int top);
		List<AccountVpLevelInfo> FindTopRankVipPointLevelInfo(long accountID, int top, DateTime date, int type);
	}
}
