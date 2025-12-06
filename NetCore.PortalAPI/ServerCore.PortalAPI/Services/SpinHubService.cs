using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerCore.PortalAPI.DataAccess.DAO;
using ServerCore.PortalAPI.Models.SpinHub;
using ServerCore.Utilities.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServerCore.PortalAPI.Services
{
	public interface ISpinHubService
	{
		UserBalance GetUserBalance(int userID);
		string DecryptData(byte[] encryptedData, string encryptionKey);
		byte[] EncryptData(string data, string encryptionKey);
		long PlayerLeave(int token, string gameName, string gameId, string time);
		string ComputeChecksum(string queryString, string sha256Key, string time, string secretKey);
		PlayerCancelBet PlayerCancelBet(string txId, int token, string currency, long totalBet, string timeStamp, string gameName, string platform, string gameId, string matchID, string revTxId);
		PlayerWin PlayerWin(string txId, int token, string currency, long winCoin, string timeStamp, string ip, string gameName, string platform, string gameId, bool isFreeTurn, string matchID, long totalBet);
		PlayerBet PlayerBet(string txId, int token, string currency, long totalBet, string timeStamp, string ip, string gameName, string platform, string gameId, string betDetails, string matchID);
	}
	public class SpinHubService: ISpinHubService
	{
		private readonly AppSettings _appSettings;
		private readonly ISpinHubDAO _spinHubDAO;
		public SpinHubService(IOptions<AppSettings> options, ISpinHubDAO spinHubDAO) {
			_appSettings =  options.Value;
			_spinHubDAO = spinHubDAO;
		}
		// Mã hóa byte theo encryptionKey
		public byte[] EncryptData(string data, string encryptionKey)
		{
			using (AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider())
			{
				aesCryptoProvider.KeySize = 256;
				aesCryptoProvider.BlockSize = 128;
				aesCryptoProvider.Mode = CipherMode.CBC;
				aesCryptoProvider.Padding = PaddingMode.PKCS7;
				byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0,32));
				byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0,16));
				aesCryptoProvider.Key = keyBytes;
				aesCryptoProvider.IV = ivBytes;
				ICryptoTransform encryptor =
				aesCryptoProvider.CreateEncryptor(aesCryptoProvider.Key, aesCryptoProvider.IV);
				byte[] dataBytes = Encoding.UTF8.GetBytes(data);
				byte[] encryptedDataBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
				return encryptedDataBytes;
			}
		}
		// Hàm giải mã byte
		public string DecryptData(byte[] encryptedData, string encryptionKey)
		{
			using (AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider())
			{
				aesCryptoProvider.KeySize = 256;
				aesCryptoProvider.BlockSize = 128;
				aesCryptoProvider.Mode = CipherMode.CBC;
				aesCryptoProvider.Padding = PaddingMode.PKCS7;
				byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0,32));
				byte[] ivBytes = Encoding.UTF8.GetBytes(encryptionKey.PadRight(16).Substring(0,16));
				aesCryptoProvider.Key = keyBytes;
				aesCryptoProvider.IV = ivBytes;
				ICryptoTransform decryptor = aesCryptoProvider.CreateDecryptor(aesCryptoProvider.Key, aesCryptoProvider.IV);
				byte[] decryptedDataBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
				string decryptedData = Encoding.UTF8.GetString(decryptedDataBytes);
				return decryptedData;
			}
		}
		// Hàm tính mã băm SHA256
		public static string ComputeSHA256Hash(string input)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = sha256.ComputeHash(inputBytes);
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					builder.Append(hashBytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
		public string ComputeChecksum(string queryString, string sha256Key, string time,string secretKey)
		{
			string dataToHash = queryString + sha256Key + time + secretKey;
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					builder.Append(hashBytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}

		public UserBalance GetUserBalance(int userID)
		{
			var userBalance = _spinHubDAO.GetUserBalance(userID);
			if (userBalance != null)
			{
				userBalance.AgentId = _appSettings.SpinHubAgentID;
			}
			return userBalance;
		}
		public PlayerBet PlayerBet(string txId, int token, string currency, long totalBet, string timeStamp, string ip, string gameName, string platform, string gameId, string betDetails, string matchID)
		{
			var playerbet = _spinHubDAO.PlayerBet(txId,token,currency,totalBet,timeStamp,ip,gameName,platform,gameId,betDetails,matchID);
			
			return playerbet;
		}
		public PlayerWin PlayerWin(string txId, int token, string currency, long winCoin, string timeStamp, string ip, string gameName, string platform, string gameId, bool isFreeTurn, string matchID, long totalBet)
		{
			var playerWin = _spinHubDAO.PlayerWin(txId, token, currency, winCoin, timeStamp, ip, gameName, platform, gameId, isFreeTurn, matchID, totalBet);

			return playerWin;
		}
		public PlayerCancelBet PlayerCancelBet(string txId, int token, string currency, long totalBet, string timeStamp, string gameName, string platform, string gameId, string matchID, string revTxId)
		{
			var playerCancelBet = _spinHubDAO.PlayerCancelBet(txId, token, currency, totalBet, timeStamp, gameName, platform, gameId, matchID, revTxId);

			return playerCancelBet;
		}
		public long PlayerLeave(int token, string gameName, string gameId, string time)
		{
			return _spinHubDAO.PlayerLeave(token, gameName, gameId, time);
		}
	}
}
