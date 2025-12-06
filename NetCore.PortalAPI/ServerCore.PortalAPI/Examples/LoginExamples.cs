using Swashbuckle.AspNetCore.Filters;
using ServerCore.PortalAPI.Models;

namespace ServerCore.PortalAPI.Examples
{
    /// <summary>
    /// Ví dụ request đăng nhập
    /// </summary>
    public class LoginRequestExample : IExamplesProvider<LoginAccount>
    {
        public LoginAccount GetExamples()
        {
            return new LoginAccount
            {
                UserName = "admin",
                Password = "admin",
                PlatformId = 1,
                MerchantId = 1,
                Uiid = "device-12345-abcdef",
                DeviceName = "Chrome 120.0",
                CaptchaText = "",
                CaptchaToken = ""
            };
        }
    }

    /// <summary>
    /// Ví dụ request đăng ký
    /// </summary>
    public class RegisterRequestExample : IExamplesProvider<LoginAccount>
    {
        public LoginAccount GetExamples()
        {
            return new LoginAccount
            {
                UserName = "newuser123",
                Password = "SecurePass123!",
                NickName = "Player123",
                PlatformId = 1,
                MerchantId = 1,
                Uiid = "device-unique-id-xyz",
                DeviceName = "Chrome 120.0",
                CaptchaText = "ABC123",
                CaptchaToken = "captcha-token-from-api",
                Email = "user@example.com"
            };
        }
    }
}
