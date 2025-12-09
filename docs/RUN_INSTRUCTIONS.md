# Hướng dẫn chạy dự án với Docker Compose

Tài liệu này hướng dẫn cách build và chạy toàn bộ hệ thống (Infrastructure + API) sử dụng Docker Compose.

## 1. Yêu cầu
- Docker & Docker Compose (hoặc Docker Plugin `docker compose`)
- Git bash hoặc Terminal (Mac/Linux)

## 2. Các bước thực hiện

### Bước 1: Khởi động Infrastructure
Chạy các service nền tảng (SQL Server, MongoDB, Redis) trước để đảm bảo database sẵn sàng.

```bash
docker-compose up -d sqlserver mongodb redis
```

### Bước 2: Khởi tạo Database
Chạy script để tạo schema và dữ liệu mẫu cho SQL Server. Script này sẽ kết nối vào container `gaming-sqlserver` đang chạy.

```bash
chmod +x setup-database.sh setup-sqlserver.sh
./setup-database.sh
```

> **Lưu ý:** Script sẽ tự động bỏ qua việc tạo container nếu phát hiện container `gaming-sqlserver` đã chạy từ bước 1.

### Bước 3: Build và chạy Portal API
Sau khi database đã sẵn sàng, tiến hành build image cho API và khởi chạy.

```bash
docker-compose up -d --build portal-api
```

### Bước 4: Kiểm tra
- **Swagger UI**: [http://localhost:5001/swagger](http://localhost:5001/swagger)
- **Logs**: `docker-compose logs -f portal-api`

## 3. Cấu hình
File `docker-compose.yml` đã được cập nhật để ghi đè các cấu hình trong `appsettings.json` thông qua biến môi trường:
- **Database**: Kết nối trực tiếp tới container `gaming-sqlserver`.
- **Redis**: Kết nối tới `gaming-redis`.
- **Environment**: `Development`.

## 4. Troubleshooting
Nếu gặp lỗi `docker-compose not found`, hãy thử dùng `docker compose` (v2):
```bash
docker compose up -d ...
```
