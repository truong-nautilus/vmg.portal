# 🗄️ Database Setup Guide

## Quick Start (Recommended)

Chạy script tự động để setup toàn bộ:

```bash
chmod +x setup-database.sh
./setup-database.sh
```

Script này sẽ:
1. ✅ Tạo SQL Server container trong Docker
2. ✅ Tạo database `Vmg.BillingDB`
3. ✅ Tạo tất cả tables cần thiết
4. ✅ Insert dữ liệu mẫu (bao gồm tài khoản admin)
5. ✅ Cập nhật `appsettings.Development.json`

## Manual Setup

### Bước 1: Setup SQL Server

```bash
chmod +x setup-sqlserver.sh
./setup-sqlserver.sh
```

### Bước 2: Tạo Schema

```bash
docker exec -i vmg-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Passw0rd" -C \
    -i /dev/stdin < database/create-schema.sql
```

### Bước 3: Seed Data

```bash
docker exec -i vmg-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Passw0rd" -C \
    -i /dev/stdin < database/seed-data.sql
```

### Bước 4: Update Connection String

Cập nhật file `NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json`:

```json
{
  "AppSettings": {
    "BillingAuthenticationAPIConnectionString": "Server=localhost,1433;Database=Vmg.BillingDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=false",
    "BillingDatabaseAPIConnectionString": "Server=localhost,1433;Database=Vmg.BillingDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=false",
    "CardGameBettingAPIConnectionString": "Server=localhost,1433;Database=Vmg.BillingDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;Encrypt=false"
  }
}
```

## Database Information

- **Server**: localhost,1433
- **Database**: Vmg.BillingDB
- **Username**: sa
- **Password**: YourStrong@Passw0rd

## Test Accounts

| Username | Password | Balance | VIP Level | Description |
|----------|----------|---------|-----------|-------------|
| admin | admin | 10,000,000 | 10 | Administrator account |
| testuser | admin | 1,000,000 | 1 | Test user |
| player001 | admin | 500,000 | 2 | Player 1 |
| player002 | admin | 750,000 | 3 | Player 2 |

## Database Schema

### Tables Created

1. **Accounts** - User accounts
   - AccountID, UserName, Password, NickName, Email, Mobile
   - PlatformId, MerchantId, Status, Balance
   - IsOTP, IsAgency, VipLevel, VipPoint
   - CreatedDate, LastLoginDate, LastLoginIP

2. **LoginSessions** - Active login sessions
   - SessionID, AccountID, AccessToken, RefreshToken
   - DeviceName, IPAddress, Uiid
   - CreatedDate, ExpiredDate, IsActive

3. **TransactionLogs** - Transaction history
   - TransactionID, AccountID, TransactionType
   - Amount, BalanceBefore, BalanceAfter
   - Description, ReferenceID, Status

4. **Locations** - Available locations
   - LocationID, LocationName, CountryCode

5. **MobileCodes** - Country dial codes
   - MobileCodeID, CountryCode, CountryName, DialCode

## Docker Commands

### Start SQL Server
```bash
docker start vmg-sqlserver
```

### Stop SQL Server
```bash
docker stop vmg-sqlserver
```

### View Logs
```bash
docker logs vmg-sqlserver
```

### Connect to SQL Server
```bash
docker exec -it vmg-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

### Remove SQL Server (Warning: Deletes all data!)
```bash
docker rm -f vmg-sqlserver
```

## Testing

After setup, restart your API server and test:

```bash
# In terminal running dotnet run, press Ctrl+C
# Then restart:
cd NetCore.PortalAPI/ServerCore.PortalAPI
dotnet run

# In another terminal, test the API:
./test-api.sh
```

Or use Swagger UI:
http://localhost:64327/swagger/index.html

## Troubleshooting

### Docker not running
```
Error: Cannot connect to the Docker daemon
Solution: Start Docker Desktop
```

### Port 1433 already in use
```
Error: port is already allocated
Solution: Stop other SQL Server instances or change port in setup script
```

### Connection refused
```
Error: Cannot connect to SQL Server
Solution: Wait 30 seconds after starting container, SQL Server needs time to initialize
```

### Password authentication failed
```
Error: Login failed for user 'sa'
Solution: Check password in appsettings.Development.json matches SA_PASSWORD in script
```

## Production Notes

⚠️ **Important**: This setup is for DEVELOPMENT ONLY!

For production:
1. Use strong passwords (not "admin")
2. Use proper password hashing (bcrypt, not MD5)
3. Setup SSL/TLS encryption
4. Configure firewall rules
5. Regular backups
6. Use managed database service (Azure SQL, AWS RDS, etc.)

## Additional Resources

- SQL Server Docker: https://hub.docker.com/_/microsoft-mssql-server
- .NET Data Access: https://docs.microsoft.com/en-us/ef/core/
- Connection Strings: https://www.connectionstrings.com/sql-server/
