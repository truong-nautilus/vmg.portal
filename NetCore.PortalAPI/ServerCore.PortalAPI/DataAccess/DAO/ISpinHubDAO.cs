using ServerCore.PortalAPI.Models.SpinHub;

namespace ServerCore.PortalAPI.DataAccess.DAO
{
	public interface ISpinHubDAO
	{
		UserBalance GetUserBalance(int accountID);
		long PlayerLeave(int token, string gameName, string gameId, string time);
		PlayerCancelBet PlayerCancelBet(string txId, int token, string currency, long totalBet, string timeStamp, string gameName, string platform, string gameId, string matchID, string revTxId);
		PlayerWin PlayerWin(string txId, int token, string currency, long winCoin, string timeStamp, string ip, string gameName, string platform, string gameId, bool isFreeTurn, string matchID, long totalBet);
		PlayerBet PlayerBet(string txId, int token, string currency, long totalBet, string timeStamp, string ip, string gameName, string platform, string gameId, string betDetails, string matchID);
	}
}
