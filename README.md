# VMG Portal API

Dự án cung cấp API cho VMG Portal, được xây dựng trên nền tảng .NET Core 8.0.

## Hướng dẫn start dự án

### Yêu cầu
*   .NET SDK 8.0 trở lên.

### Các bước khởi chạy
1.  Mở terminal tại thư mục gốc của repository.
2.  Di chuyển vào thư mục chứa project API:
    ```bash
    cd NetCore.PortalAPI/ServerCore.PortalAPI
    ```
3.  Chạy lệnh restore các dependencies (nếu cần):
    ```bash
    dotnet restore
    ```
4.  Khởi chạy dự án:
    ```bash
    dotnet run
    ```
    *   Hoặc mở file `ServerCore.PortalAPI.sln` bằng Visual Studio / Rider và nhấn **Start**.

## Tài liệu Swagger API

Sau khi dự án khởi chạy thành công, bạn có thể truy cập tài liệu API đầy đủ qua Swagger UI:

👉 **Link Swagger**: [http://localhost:64327/swagger/index.html](http://localhost:64327/swagger/index.html)

*(Lưu ý: Nếu bạn chạy trên cổng khác, hãy thay `64327` bằng cổng tương ứng hiển thị trong terminal)*

### Tính năng Swagger

Swagger UI đã được cấu hình đầy đủ với các tính năng sau:

#### 📝 Tài liệu chi tiết
- **Mô tả đầy đủ** cho mỗi endpoint (mục đích, tham số, response)
- **XML Comments** hiển thị ngay trong Swagger UI
- **Example values** cho tất cả các model properties
- **Response codes** và ý nghĩa của từng code

#### 🔐 Authentication
- Hỗ trợ **JWT Bearer Token** authentication
- Nhấn nút **"Authorize"** ở góc trên bên phải
- Nhập: `Bearer <your_access_token>` 
- Tất cả request sau đó sẽ tự động gửi kèm token

#### 🧪 Test API trực tiếp
- Click vào bất kỳ endpoint nào
- Nhấn **"Try it out"**
- Dữ liệu mẫu sẽ tự động điền sẵn
- Nhấn **"Execute"** để gọi API và xem kết quả

### Example Data (Admin)

Hệ thống đã được cấu hình sẵn dữ liệu mẫu cho endpoint đăng nhập (`/Authen/Login`).
Khi test trên Swagger, bạn có thể sử dụng thông tin sau:

*   **Endpoint**: `POST /Authen/Login`
*   **UserName**: `admin`
*   **Password**: `admin`
*   **PlatformId**: `1` (1: Android, 2: iOS, 3: Web)
*   **MerchantId**: `1`
*   **Uiid**: `device-12345-abcdef`

Dữ liệu này sẽ tự động hiển thị trong phần **Example Value** của request body trên giao diện Swagger.

### Các Endpoints chính

#### Authentication (`/Authen`)
- `POST /Authen/Login` - Đăng nhập vào hệ thống
- `POST /Authen/Register` - Đăng ký tài khoản mới
- `POST /Authen/LoginFacebook` - Đăng nhập bằng Facebook
- `GET /Authen/RefreshToken` - Làm mới access token

#### Account Management
- Quản lý thông tin tài khoản
- Đổi mật khẩu
- Cập nhật thông tin bảo mật

#### Payment & Transactions
- Nạp tiền
- Rút tiền
- Lịch sử giao dịch

## Cấu hình

*   **Database**: Cấu hình chuỗi kết nối trong `appsettings.json` (phần ConnectionStrings).
*   **Authentication**: API sử dụng JWT. Sau khi đăng nhập thành công, copy `accessToken` và nhấn nút **Authorize** ở góc trên bên phải Swagger UI -> nhập `Bearer <your_token>` để xác thực cho các request bảo mật.
