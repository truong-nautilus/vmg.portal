#!/bin/bash

echo "Starting ServerCore.PortalAPI in Development Local mode..."

# Sử dụng cấu hình Development
export ASPNETCORE_ENVIRONMENT=Development

# Override Redis Host về localhost
export AppSettings__RedisHost="217.216.36.189:6379,password=cdP7mSFrujsy8cYy5XK9Wep"
export AppSettings__IsRedisCache=true

# Override Connection Strings về localhost (dev database)
# Mặc định appsettings.json có một số IP public, ta ép về localhost để dev an toàn
export AppSettings__CardGameBettingAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Vmg.BillingDB;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__BillingGifcodeAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Billing.GifCode;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__BillingAgencyAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Billing.Agency;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__BillingGuildAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Billing.Guild;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__BillingAuthenticationAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Vmg.BillingDB;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__BillingDatabaseAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Vmg.BillingDB;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__EventAPIConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Billing.Event;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__InboxConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=PTCN.CrossPlatform.BettingGame.Notifications;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__ReportConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=PTCN.TT.Billing.Report;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__LoyaltyConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Billing.Loyalty;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"
export AppSettings__CryptoConnectionString="Data Source=217.216.36.189,1433;Initial Catalog=Cexsea.BettingGameCore;Persist Security Info=True;User Id=sa;Password=cdP7mSFrujsy8cYy5XK9Wep;TrustServerCertificate=true"

# Di chuyển vào thư mục project chính
cd NetCore.PortalAPI/ServerCore.PortalAPI

# Chạy lệnh run
echo "Running dotnet run..."
# Bạn có thể dùng 'dotnet watch run' để hot-reload nếu muốn
dotnet run
