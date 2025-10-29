# 🗄️ Hướng dẫn sử dụng SQL Server trên Render

## 📋 **Cách tạo SQL Server Database trên Render:**

### **Bước 1: Tạo SQL Server Database**
1. **Vào Render Dashboard** → "New +" → "PostgreSQL" (Render không có SQL Server trực tiếp)
2. **Hoặc sử dụng External Database** (khuyến nghị)

### **Bước 2: Sử dụng External SQL Server (Khuyến nghị)**

#### **Option 1: Azure SQL Database (Miễn phí)**
1. **Tạo Azure Account:** [portal.azure.com](https://portal.azure.com)
2. **Tạo SQL Database:**
   - Vào "Create a resource" → "SQL Database"
   - **Database name:** `ev-rental-system`
   - **Server:** Tạo server mới
   - **Pricing tier:** Basic (miễn phí)
   - **Authentication:** SQL authentication
3. **Lấy Connection String:**
   ```
   Server=tcp:your-server.database.windows.net,1433;Initial Catalog=ev-rental-system;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

#### **Option 2: Railway (Miễn phí)**
1. **Tạo account:** [railway.app](https://railway.app)
2. **Tạo SQL Server project:**
   - "New Project" → "Database" → "SQL Server"
3. **Lấy Connection String:**
   ```
   Server=sql.railway.app,1433;Database=railway;User Id=sa;Password=your-password;TrustServerCertificate=true;
   ```

#### **Option 3: PlanetScale (MySQL - tương thích)**
1. **Tạo account:** [planetscale.com](https://planetscale.com)
2. **Tạo database:** MySQL
3. **Connection String:**
   ```
   Server=aws.connect.psdb.cloud;Database=your-db;User Id=your-user;Password=your-password;SslMode=Required;
   ```

### **Bước 3: Cấu hình trên Render**

1. **Vào Web Service** → Environment Variables
2. **Thêm:**
   ```
   ConnectionStrings__DefaultConnection=[SQL Server Connection String]
   ```

## 🔧 **Cấu hình Connection String:**

### **Format SQL Server:**
```
Server=server-address,port;Database=database-name;User Id=username;Password=password;TrustServerCertificate=true;
```

### **Ví dụ cụ thể:**

#### **Azure SQL:**
```
Server=tcp:ev-rental-server.database.windows.net,1433;Initial Catalog=ev-rental-system;Persist Security Info=False;User ID=admin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

#### **Railway SQL Server:**
```
Server=sql.railway.app,1433;Database=railway;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
```

## 🚀 **Deploy với SQL Server:**

### **Bước 1: Cập nhật code**
```bash
git add .
git commit -m "Configure for SQL Server"
git push origin NewBranchName
```

### **Bước 2: Cấu hình Render**
1. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:10000
   ConnectionStrings__DefaultConnection=[Your SQL Server URL]
   ```

### **Bước 3: Deploy**
1. **Manual Deploy** → "Deploy latest commit"
2. **Chờ build và deploy**

## 🧪 **Test Connection:**

### **Test 1: Health Check**
```bash
curl https://ev-rental-be-2.onrender.com/
# Kết quả: "EV Rental System API is running!"
```

### **Test 2: Database Connection**
```bash
curl https://ev-rental-be-2.onrender.com/api/vehicle
# Nếu thành công: JSON response
# Nếu lỗi: Kiểm tra connection string
```

## 🔍 **Troubleshooting:**

### **Lỗi Connection String:**
```
Format of the initialization string does not conform to specification
```
**Giải pháp:**
- Kiểm tra format connection string
- Đảm bảo có `TrustServerCertificate=true`
- Kiểm tra username/password

### **Lỗi Database không tồn tại:**
```
Cannot open database "database-name"
```
**Giải pháp:**
- Tạo database trước
- Kiểm tra tên database trong connection string

### **Lỗi Authentication:**
```
Login failed for user
```
**Giải pháp:**
- Kiểm tra username/password
- Kiểm tra SQL Server authentication mode

## 💡 **Khuyến nghị:**

### **Cho Development:**
- **Railway** - Dễ setup, miễn phí
- **Azure SQL** - Ổn định, có free tier

### **Cho Production:**
- **Azure SQL** - Professional, có support
- **AWS RDS** - Scalable, reliable

## 📋 **Checklist:**

- [ ] ✅ Tạo SQL Server database (Azure/Railway)
- [ ] ✅ Lấy connection string
- [ ] ✅ Cập nhật Environment Variables trên Render
- [ ] ✅ Push code mới lên GitHub
- [ ] ✅ Manual Deploy trên Render
- [ ] ✅ Test API endpoints
- [ ] ✅ Kiểm tra database connection

---

**🎯 Sau khi setup xong, API sẽ hoạt động với SQL Server!**
