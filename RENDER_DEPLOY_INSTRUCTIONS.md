# 🚀 Hướng dẫn Deploy lên Render - Đã sửa lỗi Dockerfile

## ✅ **Đã sửa lỗi:**
- ✅ Di chuyển `Dockerfile` lên root repository
- ✅ Sửa đường dẫn trong Dockerfile để build từ thư mục `EV_RENTAL_SYSTEM`
- ✅ Push code lên GitHub branch `NewBranchName`

## 📋 **Bước deploy trên Render:**

### 1. **Tạo Web Service:**
- Vào [render.com](https://render.com) → Dashboard
- Click "New +" → "Web Service"
- Connect repository: `ThinhCri/ev-rental-be`
- **Branch:** `NewBranchName` (không phải main!)
- **Language:** `Docker`
- **Region:** `Oregon (US West)`
- **Root Directory:** Để trống
- **Dockerfile Path:** Để trống (sẽ tự động tìm `./Dockerfile`)

### 2. **Cấu hình Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
```

### 3. **Tạo Database:**
- Click "New +" → "PostgreSQL"
- Name: `ev-rental-db`
- Plan: `Free`
- Region: Cùng region với web service

### 4. **Kết nối Database:**
- Lấy Database URL từ PostgreSQL service
- Thêm vào Environment Variables của web service:
```
ConnectionStrings__DefaultConnection=[Database URL từ Render]
```

### 5. **Deploy:**
- Click "Create Web Service"
- Chờ build (5-10 phút)
- Kiểm tra logs nếu có lỗi

### 6. **Chạy Migration:**
- Vào Web Service → Shell
- Chạy: `dotnet ef database update`

## 🔧 **Cấu hình Render:**

### **Web Service Settings:**
- **Name:** `ev-rental-system`
- **Language:** `Docker`
- **Branch:** `NewBranchName`
- **Region:** `Oregon (US West)`
- **Plan:** `Free`

### **Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
ConnectionStrings__DefaultConnection=[Database URL]
```

### **Database Settings:**
- **Type:** PostgreSQL
- **Name:** `ev-rental-db`
- **Plan:** Free
- **Region:** Oregon (US West)

## 🎯 **Kết quả mong đợi:**

Sau khi deploy thành công:
- ✅ Web service chạy trên `https://ev-rental-system.onrender.com`
- ✅ Database PostgreSQL được tạo
- ✅ API endpoints hoạt động
- ✅ Có thể test booking flow

## 🚨 **Lưu ý quan trọng:**

1. **Branch:** Sử dụng `NewBranchName`, không phải `main`
2. **Dockerfile:** Đã được di chuyển lên root và sửa đường dẫn
3. **Database:** Cần tạo PostgreSQL trước khi deploy web service
4. **Migration:** Chạy migration sau khi deploy thành công

## 🔍 **Troubleshooting:**

### Nếu vẫn lỗi Dockerfile:
- Kiểm tra branch `NewBranchName` có file `Dockerfile` ở root
- Kiểm tra Dockerfile có đường dẫn đúng: `EV_RENTAL_SYSTEM/EV_RENTAL_SYSTEM.csproj`

### Nếu lỗi database:
- Kiểm tra Connection String
- Kiểm tra database đã được tạo
- Chạy migration trong Shell

### Nếu lỗi build:
- Kiểm tra logs trong Render dashboard
- Kiểm tra dependencies trong `.csproj`

## 📞 **Support:**

Nếu gặp vấn đề:
1. Kiểm tra logs trong Render dashboard
2. Kiểm tra Environment Variables
3. Test API endpoints sau khi deploy
4. Chạy migration nếu cần

---

**🎉 Chúc bạn deploy thành công!**
