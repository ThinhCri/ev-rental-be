# 🔧 Sửa lỗi 404 trên Render

## ❌ **Vấn đề:**
- URL `https://ev-rental-be-2.onrender.com` trả về lỗi 404
- API endpoints không hoạt động

## ✅ **Đã sửa:**

### 1. **Chuyển từ SQL Server sang PostgreSQL:**
- ✅ Thêm package `Npgsql.EntityFrameworkCore.PostgreSQL`
- ✅ Sửa `Program.cs` để sử dụng `UseNpgsql()`
- ✅ Cập nhật connection string format

### 2. **Thêm health check endpoints:**
- ✅ `GET /` - Trả về "EV Rental System API is running!"
- ✅ `GET /health` - Trả về "OK"

### 3. **Cập nhật code:**
- ✅ Push code mới lên GitHub
- ✅ Render sẽ tự động redeploy

## 🚀 **Cách sửa trên Render:**

### **Bước 1: Cập nhật Environment Variables**
1. Vào Render Dashboard → Web Service → Environment
2. Thêm/sửa:
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
ConnectionStrings__DefaultConnection=[PostgreSQL URL từ Render]
```

### **Bước 2: Lấy PostgreSQL URL**
1. Vào PostgreSQL Database → Settings
2. Copy "External Database URL"
3. Format: `postgresql://username:password@host:port/database`

### **Bước 3: Manual Deploy**
1. Vào Web Service → Manual Deploy
2. Click "Deploy latest commit"
3. Chờ build và deploy

## 🧪 **Test sau khi sửa:**

### **Test 1: Health Check**
```bash
curl https://ev-rental-be-2.onrender.com/
# Kết quả mong đợi: "EV Rental System API is running!"

curl https://ev-rental-be-2.onrender.com/health
# Kết quả mong đợi: "OK"
```

### **Test 2: API Endpoints**
```bash
# Test vehicle API
curl https://ev-rental-be-2.onrender.com/api/vehicle

# Test auth API
curl -X POST https://ev-rental-be-2.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "admin123"
  }'
```

### **Test 3: Swagger UI**
- Truy cập: `https://ev-rental-be-2.onrender.com/swagger`
- Kiểm tra API documentation

## 🔍 **Troubleshooting:**

### **Nếu vẫn lỗi 404:**
1. **Kiểm tra logs:** Web Service → Logs
2. **Kiểm tra build:** Có lỗi build không?
3. **Kiểm tra database:** Connection string đúng chưa?

### **Nếu lỗi database:**
1. **Kiểm tra PostgreSQL URL:** Format đúng chưa?
2. **Kiểm tra database:** Đã tạo chưa?
3. **Kiểm tra migration:** Chạy migration trong Shell

### **Nếu lỗi build:**
1. **Kiểm tra packages:** PostgreSQL package đã thêm chưa?
2. **Kiểm tra code:** Có lỗi syntax không?
3. **Kiểm tra logs:** Chi tiết lỗi build

## 📋 **Checklist sửa lỗi:**

- [ ] ✅ Code đã được push lên GitHub
- [ ] ✅ Environment Variables đã cập nhật
- [ ] ✅ PostgreSQL URL đã được thêm
- [ ] ✅ Manual Deploy đã chạy
- [ ] ✅ Health check endpoints hoạt động
- [ ] ✅ API endpoints hoạt động
- [ ] ✅ Database connection thành công

## 🎯 **Kết quả mong đợi:**

Sau khi sửa:
- ✅ `https://ev-rental-be-2.onrender.com/` → "EV Rental System API is running!"
- ✅ `https://ev-rental-be-2.onrender.com/health` → "OK"
- ✅ `https://ev-rental-be-2.onrender.com/api/vehicle` → JSON response
- ✅ `https://ev-rental-be-2.onrender.com/swagger` → Swagger UI

---

**🎉 Sau khi sửa, API sẽ hoạt động bình thường!**
