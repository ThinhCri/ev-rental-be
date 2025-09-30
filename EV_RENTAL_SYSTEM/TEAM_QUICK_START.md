# 🚀 TEAM QUICK START GUIDE

## ⚡ Setup Nhanh (30 giây)

```powershell
# 1. Clone project
git clone <your-repo-url>
cd EV_RENTAL_SYSTEM

# 2. Chạy setup script (tự động làm mọi thứ)
.\setup.ps1

# 3. Chạy ứng dụng
dotnet run
```

## 🌐 Truy Cập

- **Swagger UI:** http://localhost:5228/swagger
- **API:** http://localhost:5228/api

## 🔧 Nếu Gặp Lỗi

### Lỗi Database
```powershell
dotnet ef database drop --force
dotnet ef database update
```

### Lỗi PowerShell
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Lỗi API "Failed to fetch"
- Sử dụng HTTP: `http://localhost:5228` (không phải HTTPS)
- Test trên Swagger UI thay vì Postman

## 📝 Test API

1. Mở http://localhost:5228/swagger
2. Test `/api/Auth/register` với form-data
3. Upload file ảnh bằng lái xe
4. Test `/api/Auth/login`

## 🎯 Lưu Ý Quan Trọng

- ✅ Luôn dùng `.\setup.ps1` khi clone project mới
- ✅ Sử dụng HTTP (port 5228) thay vì HTTPS
- ✅ Test API trên Swagger UI
- ✅ API register cần upload file ảnh

## 🆘 Cần Giúp Đỡ?

Nếu vẫn gặp lỗi, hãy:
1. Chạy `.\setup.ps1` lại
2. Kiểm tra SQL Server đang chạy
3. Kiểm tra connection string trong `appsettings.json`