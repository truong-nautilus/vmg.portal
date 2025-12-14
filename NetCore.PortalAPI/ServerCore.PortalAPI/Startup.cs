using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Cors.Internal; // Not available in .NET 8
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetCore.Utils.Cache;
using NetCore.Utils.Interfaces;
using PortalAPI.Services;
using ServerCore.DataAccess.DAO;
using ServerCore.DataAccess.DAOImpl;
using ServerCore.PortalAPI.DataAccess.DAO;
using ServerCore.PortalAPI.DataAccess.DAOImpl;
using ServerCore.PortalAPI.Handlers;
using ServerCore.PortalAPI.OTP;
using ServerCore.PortalAPI.Services;
using ServerCore.Utilities;
using ServerCore.Utilities.Captcha;
using ServerCore.Utilities.Database;
using ServerCore.Utilities.Facebook;
using ServerCore.Utilities.Interfaces;
using ServerCore.Utilities.Sessions;
using ServerCore.Utilities.Utils;
using System.Text;

namespace ServerCore.PortalAPI
{
    public class Startup
    {
        public static AppSettings settings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            NLogManager.Info("------------ Server Startup ------------");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            ////configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            settings = appSettings;
            var key = Encoding.ASCII.GetBytes(appSettings.JwtKey);

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed((host) => true)
                       .AllowCredentials();
            }));
            // services.Configure<MvcOptions>(options =>
            // {
            //     options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            // });
            services.AddControllers(options => options.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // Sử dụng Google cho xác thực khi cần OAuth
                x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            })
            // .AddGoogle(googleOptions =>
            // {
            //     googleOptions.ClientId = appSettings.GoogleClientID;
            //     googleOptions.ClientSecret = appSettings.GoogleClientSecret;
            //     googleOptions.CallbackPath = "/signin-google";
            // })
            .AddCookie();

            NLogManager.Info("appSettings.IsRedisCache: " + appSettings.IsRedisCache);
            services.AddHttpContextAccessor();
            if (appSettings.IsRedisCache)
            {
                NLogManager.Info("Config RedisCache");
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = appSettings.RedisHost;
                });
            }
            else
            {
                NLogManager.Info("Config MemoryCache");
                services.AddDistributedMemoryCache();
            }
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax; // Đảm bảo cookie được gửi cùng với yêu cầu chuyển hướng
                options.Secure = CookieSecurePolicy.Always; // Sử dụng cookie bảo mật (HTTPS)
            });
            services.AddMemoryCache();
            services.AddSingleton<CacheHandler>();
            services.AddSingleton<IDBHelper, DBHelper>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddSingleton<IEventDAO, EventDAOImpl>();
            services.AddSingleton<IAccountDAO, AccountDAOImpl>();
            services.AddSingleton<IMobileDAO, MobileDAOIplm>();
            services.AddSingleton<IOTPDAO, OTPDAOImpl>();
            services.AddSingleton<IAgencyDAO, AgencyDAOImpl>();
            services.AddSingleton<ILoyaltyDAO, LoyaltyDAOImpl>();
            services.AddSingleton<IGuildDAO, GuildDAOImpl>();
            services.AddSingleton<OTPSecurity>();
            services.AddSingleton<AccountSession>();
            services.AddSingleton<OTPSecurity>();
            services.AddSingleton<CoreAPI>();
            services.AddSingleton<IReportDAO, ReportDAOImpl>();
            services.AddTransient<IGameTransactionDAO, GameTransactionDAOImpl>();
            services.AddHttpClient<IDataService, DataService>();
            services.AddSingleton<FacebookUtil>();
            services.AddSingleton<Captcha>();
            services.AddScoped<ISpinHubService, SpinHubService>();
            services.AddSingleton<ISpinHubDAO, SpinHubDAOImpl>();
            services.AddSingleton<IPaymentDAO, PaymentDAOImpl>();
            services.AddSingleton<ICryptoChargeDAO, CryptoChargeDAOImpl>();
            services.AddSingleton<WalletService>();
            services.AddSingleton<TatumService>();
            services.AddSingleton<IVMGDAO, VMGDAOImpl>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseCors(x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCors("CorsPolicy");
            app.UseCookiePolicy();
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
