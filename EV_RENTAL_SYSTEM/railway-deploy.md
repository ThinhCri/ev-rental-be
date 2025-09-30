# Deploy EV Rental API lên Railway

## 🚀 **Bước 1: Chuẩn bị**

### **1.1. Tạo tài khoản Railway**
- Vào [railway.app](https://railway.app)
- Đăng ký bằng GitHub

### **1.2. Cài đặt Railway CLI**
```bash
# Windows (PowerShell)
npm install -g @railway/cli

# Mac/Linux
sudo npm install -g @railway/cli
```

## 🚀 **Bước 2: Deploy API**

### **2.1. Login Railway**
```bash
railway login
```

### **2.2. Tạo project mới**
```bash
# Vào thư mục project
cd EV_RENTAL_SYSTEM

# Khởi tạo Railway project
railway init
```

### **2.3. Cấu hình environment variables**
```bash
# Thêm connection string
railway variables set ConnectionStrings__DefaultConnection="Server=localhost;Database=EV_Rental_System;User Id=sa;Password=12345;TrustServerCertificate=true;"

# Thêm JWT settings
railway variables set JwtSettings__SecretKey="YourSuperSecretKeyForProduction123!"
railway variables set JwtSettings__Issuer="EV_Rental_System"
railway variables set JwtSettings__Audience="EV_Rental_System_Users"
railway variables set JwtSettings__ExpiryInMinutes="60"

# Set environment
railway variables set ASPNETCORE_ENVIRONMENT="Production"
```

### **2.4. Deploy**
```bash
# Deploy lên Railway
railway up
```

## 🚀 **Bước 3: Cấu hình Database**

### **3.1. Tạo PostgreSQL database**
```bash
# Tạo database service
railway add postgresql
```

### **3.2. Cập nhật connection string**
```bash
# Lấy connection string từ Railway dashboard
# Cập nhật trong Railway variables
railway variables set ConnectionStrings__DefaultConnection="postgresql://..."
```

### **3.3. Chạy migration**
```bash
# Chạy migration để tạo tables
railway run dotnet ef database update
```

## 🚀 **Bước 4: Test API**

### **4.1. Lấy URL API**
```bash
# Xem URL của API
railway status
```

### **4.2. Test endpoints**
```bash
# Test đăng ký
curl -X POST "https://your-app.railway.app/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"fullName":"Test User","email":"test@example.com","password":"password123","confirmPassword":"password123"}'

# Test đăng nhập
curl -X POST "https://your-app.railway.app/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}'
```

## 🚀 **Bước 5: Cấu hình cho Frontend**

### **5.1. API Base URL**
```javascript
// Sử dụng URL từ Railway
const API_BASE_URL = 'https://your-app.railway.app/api';

// Test connection
fetch(`${API_BASE_URL}/auth/validate`)
  .then(response => response.json())
  .then(data => console.log(data));
```

### **5.2. CORS Configuration**
API đã được cấu hình CORS để cho phép frontend gọi từ bất kỳ domain nào.

## 🔧 **Troubleshooting**

### **Lỗi thường gặp:**

1. **Build failed**
   ```bash
   # Kiểm tra .NET version
   dotnet --version
   
   # Clean và rebuild
   dotnet clean
   dotnet build
   ```

2. **Database connection failed**
   ```bash
   # Kiểm tra connection string
   railway variables
   
   # Test connection
   railway run dotnet ef database update
   ```

3. **CORS error**
   - Kiểm tra CORS configuration trong Program.cs
   - Kiểm tra frontend URL

### **Logs:**
```bash
# Xem logs
railway logs

# Xem logs real-time
railway logs --follow
```

## 📋 **Checklist**

- [ ] Railway CLI đã cài đặt
- [ ] Đã login Railway
- [ ] Environment variables đã set
- [ ] Database đã tạo
- [ ] Migration đã chạy
- [ ] API đã deploy thành công
- [ ] Test endpoints hoạt động

## 🎯 **Kết quả**

Sau khi deploy thành công:
- ✅ **API URL**: `https://your-app.railway.app`
- ✅ **Swagger UI**: `https://your-app.railway.app/swagger`
- ✅ **Database**: PostgreSQL trên Railway
- ✅ **Frontend** có thể gọi API
- ✅ **Free tier** đủ dùng cho development

## 💰 **Chi phí**

- **Free tier**: $5 credit/tháng
- **Pro**: $20/tháng
- **Database**: Bao gồm trong plan
