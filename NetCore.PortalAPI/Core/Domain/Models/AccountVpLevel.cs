using NLog.Fluent;
using System;

namespace ServerCore.PortalAPI.Core.Domain.Models
{
	public class AccountVpLevel
	{
		public long id { get; set; }
		public long accountId { get; set; }
		public int vpLevelId { get; set; }
		public long accountVp { get; set; }
		public long totalAmount { get; set; }
		public long vpLevelTime { get; set; }
		public long vpExchangeValue { get; set; }
		public int vpType { get; set; }
		public DateTime lastUpdate { get; set; }
	}
}
