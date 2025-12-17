using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NetCore.PortalAPI.Core.Interfaces;
using ServerCore.Utilities.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.PortalAPI.Presentation.Handlers
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IAccountRepository _accountRepository;

        public TokenValidationMiddleware(RequestDelegate next, IOptions<AppSettings> options, IAccountRepository accountRepository)
        {
            _next = next;
            _appSettings = options.Value;
            _accountRepository = accountRepository;
        }

        public async Task Invoke(HttpContext context)
        {

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtKey);

            try
            {
                // Xác thực token và kiểm tra thời hạn
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Không cho phép thời gian chênh lệch
                }, out SecurityToken validatedToken);
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var tokenId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                // Kiểm tra thêm trong cơ sở dữ liệu nếu cần (VD: kiểm tra xem token đã bị thu hồi chưa)
                var isTokenRevoked = _accountRepository.CheckLoginSession(tokenId);
                if (isTokenRevoked < 0)
                {
                    await WriteJsonResponse(context, 401, "Token has been revoked");
                    return;
                }
                await _next(context); // Nếu token hợp lệ, tiếp tục request
            }
            catch (SecurityTokenExpiredException)
            {
                await WriteJsonResponse(context, 401, "Token has expired");
                return;
            }
            catch (Exception)
            {
                // Token không hợp lệ
                await WriteJsonResponse(context, 401, "Invalid token");
                return;
            }
        }
        private async Task WriteJsonResponse(HttpContext context, int code, string description)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;

            var response = new
            {
                code = code,
                description = description
            };

            var jsonResponse = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
