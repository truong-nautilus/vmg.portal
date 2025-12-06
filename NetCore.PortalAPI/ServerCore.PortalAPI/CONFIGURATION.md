# Hướng dẫn cấu hình appsettings.Development.json

File `appsettings.Development.json` chứa các thông tin nhạy cảm và cấu hình môi trường development. File này **KHÔNG** được commit lên Git.

## 📋 Các bước cấu hình

### 1. Security Keys (Bắt buộc)
```json
"JwtKey": "YOUR_JWT_SECRET_KEY_HERE"
"FishingKey": "YOUR_FISHING_KEY_HERE"  
"EncryptKey": "YOUR_ENCRYPT_KEY_HERE"
```

### 2. Database Connection Strings (Bắt buộc)
Cập nhật tất cả connection strings với thông tin database của bạn:
- Server address
- Database name
- Username & Password

### 3. External Services (Tùy chọn)
- **OTP Service**: URL của dịch vụ OTP
- **Report Service**: URL của dịch vụ báo cáo
- **Loyalty Service**: URL của dịch vụ loyalty
- **Payment Service**: Thông tin API payment gateway

### 4. Google OAuth (Nếu sử dụng)
```json
"GoogleClientID": "your-client-id.apps.googleusercontent.com"
"GoogleClientSecret": "your-client-secret"
"ClientGoogleCallback": "https://yourdomain.com/account/loginOauth/google"
```

### 5. Blockchain & Crypto (Nếu sử dụng)
- BlockchainApiUrl
- BlockchainUsername & Password
- Crypto wallet URLs

### 6. Redis Cache
```json
"RedisHost": "localhost:6379"
"IsRedisCache": true  // Set false để dùng Memory Cache
```

## ⚠️ Lưu ý bảo mật

1. **KHÔNG BAO GIỜ** commit file này lên Git
2. File đã được thêm vào `.gitignore`
3. Mỗi môi trường (dev, staging, production) nên có file riêng
4. Backup file này ở nơi an toàn

## 🔄 Restore từ backup

Nếu bạn mất file này, có thể restore từ:
1. Backup cá nhân
2. Hỏi team lead
3. Tham khảo `appsettings.json` (chỉ có placeholders)
