using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ServerCore.PortalAPI.Models
{
    /// <summary>
    /// Model đăng nhập/đăng ký tài khoản
    /// </summary>
    public class LoginAccount
    {
        /// <summary>
        /// ID nền tảng (1: Android, 2: iOS, 3: Web)
        /// </summary>
        [SwaggerSchema("Platform ID - 1: Android, 2: iOS, 3: Web")]
        [DefaultValue(1)]
        public int PlatformId { get; set; }
        
        /// <summary>
        /// ID merchant
        /// </summary>
        [SwaggerSchema("Merchant ID")]
        [DefaultValue(1)]
        public int MerchantId { get; set; }
        
        /// <summary>
        /// Địa chỉ IP của client
        /// </summary>
        [SwaggerSchema("Client IP Address")]
        public string IpAddress { get; set; }
        
        /// <summary>
        /// Tên đăng nhập (6-20 ký tự, chỉ chữ và số)
        /// </summary>
        [SwaggerSchema("Username (6-20 characters, alphanumeric only)")]
        [DefaultValue("admin")]
        [Required]
        public string UserName { get; set; }
        
        /// <summary>
        /// Mật khẩu (tối thiểu 6 ký tự)
        /// </summary>
        [SwaggerSchema("Password (minimum 6 characters)")]
        [DefaultValue("admin")]
        [Required]
        public string Password { get; set; }
        
        /// <summary>
        /// ID thiết bị duy nhất
        /// </summary>
        [SwaggerSchema("Unique device identifier")]
        [DefaultValue("device-12345")]
        public string Uiid { get; set; }
        
        /// <summary>
        /// Mã captcha
        /// </summary>
        [SwaggerSchema("Captcha text")]
        public string CaptchaText { get; set; }
        
        /// <summary>
        /// Token captcha
        /// </summary>
        [SwaggerSchema("Captcha token")]
        public string CaptchaToken { get; set; }
        
        /// <summary>
        /// Tên hiển thị trong game
        /// </summary>
        [SwaggerSchema("Display name in game")]
        public string NickName { get; set; }
        
        /// <summary>
        /// Facebook access token
        /// </summary>
        [SwaggerSchema("Facebook access token for social login")]
        public string FacebookToken { get; set; }
        
        /// <summary>
        /// Facebook user ID
        /// </summary>
        [SwaggerSchema("Facebook user ID")]
        public string FacebookId { get; set; }
        
        /// <summary>
        /// Mã VIP
        /// </summary>
        [SwaggerSchema("VIP code")]
        public string VipCode { get; set; }
        
        /// <summary>
        /// Nguồn chiến dịch marketing
        /// </summary>
        [SwaggerSchema("Marketing campaign source")]
        public string CampaignSource { get; set; }
        
        /// <summary>
        /// Apple ID token
        /// </summary>
        [SwaggerSchema("Apple ID token for social login")]
        public string AppleToken { get; set; }
        
        /// <summary>
        /// Apple user ID
        /// </summary>
        [SwaggerSchema("Apple user ID")]
        public string AppleId { get; set; }
        
        /// <summary>
        /// Email từ Apple ID
        /// </summary>
        [SwaggerSchema("Email from Apple ID")]
        public string AppleEmail { get; set; }
        
        /// <summary>
        /// Tên thiết bị
        /// </summary>
        [SwaggerSchema("Device name")]
        [DefaultValue("Chrome 120.0")]
        public string DeviceName { get; set; }
        
        /// <summary>
        /// Đăng nhập bằng UID (0: không, 1: có)
        /// </summary>
        [SwaggerSchema("Login with UID flag")]
        public int IsLoginUID { get; set; }
        
        /// <summary>
        /// ID vị trí
        /// </summary>
        [SwaggerSchema("Location ID")]
        public int LocationID { get; set; }
        
        /// <summary>
        /// Tiền tố tài khoản
        /// </summary>
        [SwaggerSchema("Account prefix")]
        public string PreFix { get; set; }
        
        /// <summary>
        /// Địa chỉ email
        /// </summary>
        [SwaggerSchema("Email address")]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// Địa chỉ ví
        /// </summary>
        [SwaggerSchema("Wallet address")]
        public string WAddress { get; set; }
        
        /// <summary>
        /// ID người dùng từ đối tác
        /// </summary>
        [SwaggerSchema("Partner user ID")]
        public string PartnerUserID { get; set; }

    }
}
