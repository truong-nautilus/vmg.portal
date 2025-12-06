using System.Threading.Tasks;

namespace ServerCore.Utilities.Interfaces
{
    public interface IDataService
    {
        Task<T> PostAsync<T>(string uri, dynamic data, bool isLog = true, int timeout = 30000);

        Task<string> PostAsync(string uri, dynamic data, bool isLog = true, int timeout = 30000);

        /// <summary>
        ///  GetAsync with Partner-Key in config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string uri, bool isLog = true, int timeout = 30000);

        Task<string> GetAsync(string uri, bool isLog = true, int timeout = 30000);
        Task<T> GetApiAsync<T>(string uri, dynamic token, bool isLog = true, bool isAuthen = false, int timeout = 30000);
        Task<T> PostApiAsync<T>(string uri, dynamic data, dynamic token, bool isLog = true, int timeout = 30000);


    }
}
