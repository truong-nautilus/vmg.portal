using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
// using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging; // Not available in .NET 8
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerCore.PortalAPI.Core.Domain.Models.SpinHub;
using ServerCore.PortalAPI.Core.Application.Services;
using ServerCore.Utilities.Interfaces;
using ServerCore.Utilities.Models;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace ServerCore.PortalAPI.Presentation.Controllers
{
	[Route("SpinHub")]
	[ApiController]
	[AllowAnonymous]
	public class SpinHubController : ControllerBase
	{
		private readonly ISpinHubService _spinHubService;
		private readonly AccountSession _accountSession;
		private readonly AppSettings _appSettings;
        private readonly IDataService _dataService;
        public SpinHubController(ISpinHubService spinHubService, IOptions<AppSettings> options, IDataService dataService) { 
			_spinHubService = spinHubService;
			_appSettings = options.Value;
			_dataService = dataService;
		}

		[HttpGet("Test")]
		public ActionResult<ResponseBuilder> Test()
		{
			return new ObjectResult(new
			{
				KMData = "Hello",
			});
		}

		[HttpGet("GetEncyptedData")]
		public ActionResult<ResponseBuilder> GetEncyptedData([FromQuery] string KMData, [FromQuery] string timeString)
		{
			byte[] encryptedData = _spinHubService.EncryptData(KMData, _appSettings.SpinHubEncryptKey);
			var encryptedDataString = Convert.ToBase64String(encryptedData);
			var checksum = _spinHubService.ComputeChecksum(KMData, _appSettings.SpinHubSHA256Key, timeString, _appSettings.SpinHubSecretKey);
			return new ObjectResult(new
			{
				KMData = HttpUtility.UrlEncode(encryptedDataString),
				KMS = HttpUtility.UrlEncode(checksum)
			});
		}

		[HttpPost("Player")]
		public ActionResult<ResponseBuilder> GetBalanceUser([FromQuery] int methodId, [FromQuery] string KMData, [FromQuery] string KMS)
		{
			try
			{
				NLogManager.Info(String.Format("tham số đầu vào: methodId={0}, KMData={1}, KMS={2}", methodId, KMData, KMS));
				// giải mã data
				var encryptedDataString = HttpUtility.UrlDecode(KMData);
				var encryptedData = Convert.FromBase64String(KMData);
				var queryString = _spinHubService.DecryptData(encryptedData, _appSettings.SpinHubEncryptKey);

				var dict = HttpUtility.ParseQueryString(queryString);
				string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
				var queryStringObj = JsonConvert.DeserializeObject<dynamic>(json);
				NLogManager.Info(json);
				var time = (string)queryStringObj.TimeStamp == null ? (string)queryStringObj.Time : (string)queryStringObj.TimeStamp;

				var checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
				var checksumEncode = HttpUtility.UrlEncode(checksum);
				if(KMS != checksumEncode)
				{
					return new ObjectResult(new
					{
						ErrorCode = 1007,
						Result = false,
						Message = "kmdata không khớp với kms",
						Data = ""
					});
				}

				switch (methodId)
				{
					case 1:
						var userBalance = _spinHubService.GetUserBalance((int)queryStringObj.Token);
						if (userBalance != null)
						{
							return new ObjectResult(new SpinHubResponse
							{
								ErrorCode = 0,
								Result = true,
								Message = "",
								Data = JsonConvert.SerializeObject(userBalance)
							});
						}
						return new ObjectResult(new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Not found",
							Data = ""
						});

					case 2:
						var playerBet = _spinHubService.PlayerBet(	
								(string)queryStringObj.TxId,
								(int)queryStringObj.Token,
								(string)queryStringObj.Currency,
								(long)queryStringObj.TotalBet,
								(string)queryStringObj.TimeStamp,
								(string)queryStringObj.Ip,
								(string)queryStringObj.GameName,
								(string)queryStringObj.Platform,
								(string)queryStringObj.GameId,
								(string)queryStringObj.BetDetails,
								(string)queryStringObj.MatchID
							);
						if (playerBet != null)
						{
							var queryStringRes = string.Format("TxId={0}&UserName={1}&Currency={2}&Coin={3}&RTPType={4}", playerBet.TxId, playerBet.UserName, playerBet.Currency, playerBet.Coin, playerBet.RTPType);
							var encryptRes = _spinHubService.EncryptData(queryStringRes, _appSettings.SpinHubEncryptKey);
							var encryptResStr = Convert.ToBase64String(encryptRes);
							return new ObjectResult(new SpinHubResponse
							{
								ErrorCode = 0,
								Result = true,
								Message = "",
								Data = encryptResStr
							}) ;
						}
						return new ObjectResult(new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Fail playerbet",
							Data = ""
						});

					case 3:
						var playerWin = _spinHubService.PlayerWin(
								(string)queryStringObj.TxId,
								(int)queryStringObj.UserName,
								(string)queryStringObj.Currency,
								(long)queryStringObj.WinCoin,
								(string)queryStringObj.TimeStamp,
								(string)queryStringObj.Ip,
								(string)queryStringObj.GameName,
								(string)queryStringObj.Platform,
								(string)queryStringObj.GameId,
								(bool)queryStringObj.IsFreeTurn,
								(string)queryStringObj.MatchID,
								(long)queryStringObj.TotalBet
							);
						if (playerWin != null)
						{
							var queryStringRes = string.Format("TxId={0}&UserName={1}&Currency={2}&Coin={3}", playerWin.TxId, playerWin.UserName, playerWin.Currency, playerWin.Coin);
							var encryptRes = _spinHubService.EncryptData(queryStringRes, _appSettings.SpinHubEncryptKey);
							var encryptResStr = Convert.ToBase64String(encryptRes);
							return new ObjectResult(new SpinHubResponse
							{
								ErrorCode = 0,
								Result = true,
								Message = "",
								Data = encryptResStr
							});
						}
						return new ObjectResult(new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Fail playerwin",
							Data = ""
						});
					case 4:
						var playerCancelBet = _spinHubService.PlayerCancelBet(
								(string)queryStringObj.TxId,
								(int)queryStringObj.Token,
								(string)queryStringObj.Currency,
								(long)queryStringObj.TotalBet,
								(string)queryStringObj.TimeStamp,
								(string)queryStringObj.GameName,
								(string)queryStringObj.Platform,
								(string)queryStringObj.GameId,
								(string)queryStringObj.MatchID,
								(string)queryStringObj.RevTxId
							);
						if (playerCancelBet != null)
						{
							var queryStringRes = string.Format("TxId={0}&UserName={1}&Currency={2}&Coin={3}", playerCancelBet.TxId, playerCancelBet.UserName, playerCancelBet.Currency, playerCancelBet.Coin);
							var encryptRes = _spinHubService.EncryptData(queryStringRes, _appSettings.SpinHubEncryptKey);
							var encryptResStr = Convert.ToBase64String(encryptRes);
							return new ObjectResult(new SpinHubResponse
							{
								ErrorCode = 0,
								Result = true,
								Message = "",
								Data = encryptResStr
							});
						}
						return new ObjectResult(new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Fail playerCancelBet",
							Data = ""
						});
					case 6:
						var responseCode = _spinHubService.PlayerLeave(
								(int)queryStringObj.Token,
								(string)queryStringObj.GameName,
								(string)queryStringObj.GameId,
								(string)queryStringObj.Time
                        );
						var jsonData = JsonConvert.SerializeObject(new
						{
							AccountID = (int)queryStringObj.Token,
							GameName = (string)queryStringObj.GameName,
                            GameId = (string)queryStringObj.GameId,
                            Time = (string)queryStringObj.Time
                        });
                        NLogManager.Info(string.Format("Telegram: {0} - {1}", jsonData, _appSettings.SpinNotifyURL + "api/event/playerleave"));
                        string res = HttpUtil.SendPost(jsonData, _appSettings.SpinNotifyURL + "api/event/playerleave");
                        NLogManager.Info(string.Format("Telegram Result: {0}", res));
                        if (responseCode > 0)
						{
							return new ObjectResult(new SpinHubResponse
							{
								ErrorCode = 0,
								Result = true,
								Message = ""
							});
						}
						return new ObjectResult(new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Fail PlayerLeave"
						});
					case 7:
                        return new ObjectResult(new SpinHubResponse
                        {
                            ErrorCode = 1007,
                            Result = false,
                            Message = "Fail playerCancelBet",
                            Data = ""
                        });
                    default:
						var data = new SpinHubResponse
						{
							ErrorCode = 1007,
							Result = false,
							Message = "Not found",
							Data = string.Empty
						};
						return new ObjectResult(data);
				}
			}
			catch (Exception ex)
			{
				NLogManager.Exception(ex);
				return new ObjectResult(new SpinHubResponse
				{
										ErrorCode = 1007,
										Result = false,
										Message = "This is error message",
										Data = ""
									});
			}
		}
		[HttpGet("Reconcile")]
        public ActionResult<ResponseBuilder> Reconcile(string method, string time, string gameType, string fromTime, string toTime, int offset, int row, int userId, int room)
        {
            try
            {
                NLogManager.Info(String.Format("tham số đầu vào: method={0}, time={1}, gameType={2}", method, time, gameType));
				string queryString, encryptedDataString, encryptedDataStringEncode, checksum, checksumEncode, url, response;
				byte[] encryptedData;
                switch (method)
				{
					case "GetJackpotLobby":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&GameType={3}", method, _appSettings.SpinHubSecretKey, time, gameType);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
					case "GetJackpotLobbyByRoomId":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&GameType={3}&Room={4}", method, _appSettings.SpinHubSecretKey, time, gameType, room);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetBigWinLog":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}", method, _appSettings.SpinHubSecretKey, time);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetHistoryJackpot":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&GameType={3}", method, _appSettings.SpinHubSecretKey, time, gameType);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetAgentTotalWinLossByTimeLog":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&FromTime={3}&ToTime={4}&OffSet={5}&row={6}", method, _appSettings.SpinHubSecretKey, time, fromTime, toTime, offset, row);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetAgentTotalWinLossByTime":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&FromTime={3}&ToTime={4}", method, _appSettings.SpinHubSecretKey, time, fromTime, toTime);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetUserTotalWinLossByTimeLog":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&User={3}&FromTime={4}&ToTime={5}&OffSet={6}&row={7}", method, _appSettings.SpinHubSecretKey, time, userId, fromTime, toTime, offset, row);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    case "GetUserTotalWinLossByTime":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&User={3}&FromTime={4}&ToTime={5}", method, _appSettings.SpinHubSecretKey, time, userId, fromTime, toTime);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));

                    case "GetHistoryPlayGame":
                        queryString = string.Format("Method={0}&Key={1}&Time={2}&User={3}&FromTime={4}&ToTime={5}&OffSet={6}&row={7}", method, _appSettings.SpinHubSecretKey, time, userId, fromTime, toTime, offset, row);
                        encryptedData = _spinHubService.EncryptData(queryString, _appSettings.SpinHubEncryptKey);
                        encryptedDataString = Convert.ToBase64String(encryptedData);
                        encryptedDataStringEncode = HttpUtility.UrlEncode(encryptedDataString);
                        checksum = _spinHubService.ComputeChecksum(queryString, _appSettings.SpinHubSHA256Key, time, _appSettings.SpinHubSecretKey);
                        checksumEncode = HttpUtility.UrlEncode(checksum);
                        url = string.Format("{0}{1}?agentId={2}&encryptedData={3}&checksum={4}", _appSettings.SpinHubURLApi, method, _appSettings.SpinHubAgentID, encryptedDataStringEncode, checksumEncode);
                        response = _dataService.GetAsync(url).Result;
                        return new ObjectResult(JsonConvert.DeserializeObject(response));
                    default:
                        return new ObjectResult(new
                        {
                            Status = -1,
                            Messenger = "Thất bại",
                            DataResponse = ""
                        });
                }
            }
            catch (Exception ex)
            {
                NLogManager.Exception(ex);
                return new ObjectResult(new
                {
                   Status= -1,
				   Messenger = "Thất bại",
				   DataResponse = ""
                });
            }
        }
    }
}
