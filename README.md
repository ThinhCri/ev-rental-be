# EV Rental System Backend

Backend service for the EV Rental System, built with .NET Web API. Provides authentication, vehicle management, booking, rental processing, payment handling, and reporting features. Exposes RESTful APIs for the frontend applications (Renter App, Station Staff App, and Admin Dashboard).

## 🚀 Quick Start (1 câu lệnh)

### Windows:
```powershell
cd EV_RENTAL_SYSTEM
.\setup-database.ps1
```

### Mac/Linux:
```bash
cd EV_RENTAL_SYSTEM
rm -rf Migrations
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
dotnet run
```

## 📋 Yêu cầu hệ thống

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- PowerShell (Windows) hoặc Terminal (Mac/Linux)

## 🔧 Cài đặt chi tiết

1. **Clone repository**
2. **Cấu hình connection string** trong `EV_RENTAL_SYSTEM/appsettings.json`
3. **Chạy setup script** (xem Quick Start ở trên)
4. **Truy cập API** tại: http://localhost:5228/swagger

## 📁 Cấu trúc project

```
EV_RENTAL_SYSTEM/
├── Controllers/          # API Controllers
├── Data/                # DbContext và cấu hình database
├── Models/              # Entity models và DTOs
├── Repositories/        # Data access layer
├── Services/            # Business logic layer
├── Mappings/            # AutoMapper profiles
├── Migrations/          # Entity Framework migrations
├── setup-database.ps1   # Script setup cho Windows
├── setup-database.bat   # Script setup cho Windows (batch)
└── SETUP.md            # Hướng dẫn setup chi tiết
```

## ✨ Tính năng

- Xác thực và phân quyền người dùng với JWT
- Quản lý giấy phép lái xe
- Quản lý xe và trạm
- Xử lý đơn hàng
- Xử lý thanh toán và giao dịch
- Hệ thống khiếu nại
- Theo dõi bảo trì

## 🐛 Troubleshooting

### Lỗi thường gặp:

1. **Lỗi `ArgumentNullException: Value cannot be null. (Parameter 'path1')`**
   - **Nguyên nhân**: `IWebHostEnvironment.WebRootPath` null
   - **Giải pháp**: Script setup sẽ tự động tạo thư mục `wwwroot`

2. **Lỗi kết nối database**
   - Kiểm tra SQL Server có đang chạy không
   - Kiểm tra connection string trong `appsettings.json`

3. **Lỗi migration**
   - Xóa thư mục `Migrations` và chạy lại script setup

Xem file `EV_RENTAL_SYSTEM/SETUP.md` để biết thêm chi tiết.

## 📄 License

This project is licensed under the MIT License.
