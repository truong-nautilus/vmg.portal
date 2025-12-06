using ServerCore.PortalAPI.DataAccess.DAO;
using ServerCore.Utilities.Models;
using System.Data.SqlClient;
using System.Data;
using System;
using ServerCore.Utilities.Utils;
using Microsoft.Extensions.Options;
using NetCore.Utils.Interfaces;
using ServerCore.PortalAPI.Models.SpinHub;

namespace ServerCore.PortalAPI.DataAccess.DAOImpl
{
	public class SpinHubDAOImpl: ISpinHubDAO
	{
		private readonly AppSettings appSettings;
		private readonly IDBHelper _dbHepler;

		public SpinHubDAOImpl(IDBHelper dbHepler, IOptions<AppSettings> options)
		{
			appSettings = options.Value;
			_dbHepler = dbHepler;
		}
		public UserBalance GetUserBalance(int accountID)
		{
			try
			{
				var pars = new SqlParameter[1];
				pars[0] = new SqlParameter("@_AccountID", accountID);
				_dbHepler.SetConnectionString(appSettings.BillingAuthenticationAPIConnectionString);
				var account = _dbHepler.GetListSP<UserBalance>("SP_SpinHub_GetUserBalance", pars);
				if(account != null && account.Count>0)
				{
					return account[0];
				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
			}

			return null;
		}
		public PlayerBet PlayerBet(string txId, int token, string currency, long totalBet, string timeStamp, string ip, string gameName, string platform, string gameId, string betDetails, string matchID)
		{
			try
			{
				var pars = new SqlParameter[14];
				pars[0] = new SqlParameter("@_TxId", txId);
				pars[1] = new SqlParameter("@_Token", token);
				pars[2] = new SqlParameter("@_Currency", currency);
				pars[3] = new SqlParameter("@_TotalBet", totalBet);
				pars[4] = new SqlParameter("@_TimeStamp", timeStamp);
				pars[5] = new SqlParameter("@_Ip", ip);
				pars[6] = new SqlParameter("@_GameName", gameName);
				pars[7] = new SqlParameter("@_Platform", platform);
				pars[8] = new SqlParameter("@_GameId", gameId);
				pars[9] = new SqlParameter("@_BetDetails", betDetails);
				pars[10] = new SqlParameter("@_MatchID", matchID);
				pars[11] = new SqlParameter("@_TotalGameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output};
				pars[12] = new SqlParameter("@_Username", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
				pars[13] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				_dbHepler.SetConnectionString(appSettings.BillingAuthenticationAPIConnectionString);
				_dbHepler.ExecuteNonQuerySP("SP_SpinHub_Deduct_PlayerBet", pars);
				if (!DBNull.Value.Equals(pars[13].Value))
				{
					if (Convert.ToInt64(pars[13].Value) >= 0)
					{
						return new PlayerBet
						{
							TxId = txId,
							Coin = Convert.ToInt64(pars[11].Value),
							Currency = currency,
							UserName = pars[12].Value.ToString(),
							RTPType = (int)SPINHUB_RTP_TYPE.NONE
						};
					}
					
				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
			}

			return null;
		}

		public PlayerWin PlayerWin(string txId, int token, string currency, long winCoin, string timeStamp, string ip, string gameName, string platform, string gameId, bool isFreeTurn, string matchID, long totalBet)
		{
			try
			{
				var pars = new SqlParameter[15];
				pars[0] = new SqlParameter("@_TxId", txId);
				pars[1] = new SqlParameter("@_Token", token);
				pars[2] = new SqlParameter("@_Currency", currency);
				pars[3] = new SqlParameter("@_WinCoin", winCoin);
				pars[4] = new SqlParameter("@_TimeStamp", timeStamp);
				pars[5] = new SqlParameter("@_Ip", ip);
				pars[6] = new SqlParameter("@_GameName", gameName);
				pars[7] = new SqlParameter("@_Platform", platform);
				pars[8] = new SqlParameter("@_GameId", gameId);
				pars[9] = new SqlParameter("@_IsFreeTurn", isFreeTurn);
				pars[10] = new SqlParameter("@_MatchID", matchID);
				pars[11] = new SqlParameter("@_TotalBet", totalBet);
				pars[12] = new SqlParameter("@_TotalGameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				pars[13] = new SqlParameter("@_Username", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
				pars[14] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				_dbHepler.SetConnectionString(appSettings.BillingAuthenticationAPIConnectionString);
				_dbHepler.ExecuteNonQuerySP("SP_SpinHub_Topup_PlayerWin", pars);
				NLogManager.Info(string.Format("@_ResponseStatus: {0}", pars[14].Value));
				if (!DBNull.Value.Equals(pars[14].Value))
				{
					if (Convert.ToInt64(pars[14].Value) >= 0)
					{
						return new PlayerWin
						{
							TxId = txId,
							Coin = Convert.ToInt64(pars[12].Value),
							Currency = currency,
							UserName = pars[13].Value.ToString()
						};
					}
						
				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
			}

			return null;
		}
		public PlayerCancelBet PlayerCancelBet(string txId, int token, string currency, long totalBet, string timeStamp, string gameName, string platform, string gameId, string matchID, string revTxId)
		{
			try
			{
				var pars = new SqlParameter[13];
				pars[0] = new SqlParameter("@_TxId", txId);
				pars[1] = new SqlParameter("@_Token", token);
				pars[2] = new SqlParameter("@_Currency", currency);
				pars[3] = new SqlParameter("@_TotalBet", totalBet);
				pars[4] = new SqlParameter("@_TimeStamp", timeStamp);
				pars[5] = new SqlParameter("@_GameName", gameName);
				pars[6] = new SqlParameter("@_Platform", platform);
				pars[7] = new SqlParameter("@_GameId", gameId);
				pars[8] = new SqlParameter("@_MatchID", matchID);
				pars[9] = new SqlParameter("@_RevTxId", revTxId);
				pars[10] = new SqlParameter("@_TotalGameBalance", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				pars[11] = new SqlParameter("@_Username", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
				pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				_dbHepler.SetConnectionString(appSettings.BillingAuthenticationAPIConnectionString);
				_dbHepler.ExecuteNonQuerySP("SP_SpinHub_Refund_PlayerCancelBet", pars);
				if (!DBNull.Value.Equals(pars[12].Value))
				{
					if (Convert.ToInt64(pars[12].Value) >= 0)
					{
						return new PlayerCancelBet
						{
							TxId = txId,
							Coin = Convert.ToInt64(pars[10].Value),
							Currency = currency,
							UserName = pars[11].Value.ToString()
						};
					}

				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
			}

			return null;
		}
		public long PlayerLeave(int token, string gameName, string gameId, string time)
		{
			try
			{
				var pars = new SqlParameter[5];
				pars[0] = new SqlParameter("@_Token", token);
				pars[1] = new SqlParameter("@_GameName", gameName);
				pars[2] = new SqlParameter("@_GameId", gameId);
				pars[3] = new SqlParameter("@_Time", time);
				pars[4] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
				_dbHepler.SetConnectionString(appSettings.BillingAuthenticationAPIConnectionString);
				_dbHepler.ExecuteNonQuerySP("SP_SpinHub_PlayerLeave", pars);
				if (!DBNull.Value.Equals(pars[4].Value))
				{
					return Convert.ToInt64(pars[4].Value);
				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
			}

			return -99;
		}
	}
}
