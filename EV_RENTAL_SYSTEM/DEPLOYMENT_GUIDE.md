# Hướng dẫn Deploy EV Rental System API

## 🚀 **Cách 1: Deploy với Docker (Khuyến nghị)**

### **Bước 1: Cài đặt Docker**
```bash
# Windows
# Tải Docker Desktop từ https://www.docker.com/products/docker-desktop

# Linux
sudo apt-get update
sudo apt-get install docker.io docker-compose
```

### **Bước 2: Deploy API**
```bash
# Vào thư mục project
cd EV_RENTAL_SYSTEM

# Chạy với Docker Compose
docker-compose up -d

# Kiểm tra logs
docker-compose logs -f
```

### **Bước 3: Truy cập API**
- **API URL**: `http://localhost:8080`
- **Swagger UI**: `http://localhost:8080/swagger`
- **Database**: `localhost:1433`

---

## ☁️ **Cách 2: Deploy lên Azure (Dễ nhất)**

### **Bước 1: Tạo Azure App Service**
1. Vào [Azure Portal](https://portal.azure.com)
2. Tạo "App Service" mới
3. Chọn:
   - **Runtime**: .NET 8
   - **Operating System**: Windows/Linux
   - **Pricing Plan**: Free F1 (để test)

### **Bước 2: Tạo Azure SQL Database**
1. Tạo "SQL Database" mới
2. Chọn:
   - **Server**: Tạo server mới
   - **Pricing**: Basic (để test)
   - **Database name**: EV_Rental_System

### **Bước 3: Deploy code**
```bash
# Publish code
dotnet publish -c Release -o ./publish

# Zip code
Compress-Archive -Path ./publish/* -DestinationPath ./api.zip

# Upload lên Azure App Service
# (Dùng Azure Portal hoặc Azure CLI)
```

### **Bước 4: Cấu hình**
1. **Connection String** trong App Settings:
   ```
   DefaultConnection=Server=tcp:your-server.database.windows.net,1433;Initial Catalog=EV_Rental_System;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

2. **JWT Settings**:
   ```
   JwtSettings__SecretKey=YourSuperSecretKeyForProduction123!
   JwtSettings__Issuer=EV_Rental_System
   JwtSettings__Audience=EV_Rental_System_Users
   JwtSettings__ExpiryInMinutes=60
   ```

---

## 🖥️ **Cách 3: Deploy lên VPS/Server riêng**

### **Bước 1: Chuẩn bị server**
```bash
# Cài đặt .NET 8 Runtime
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-runtime-8.0

# Cài đặt Nginx (reverse proxy)
sudo apt-get install nginx

# Cài đặt SQL Server
# (Theo hướng dẫn của Microsoft)
```

### **Bước 2: Upload và chạy code**
```bash
# Upload code lên server
scp -r ./publish user@your-server:/var/www/ev-rental-api

# Chạy API
cd /var/www/ev-rental-api
dotnet EV_RENTAL_SYSTEM.dll --urls="http://0.0.0.0:5000"
```

### **Bước 3: Cấu hình Nginx**
```nginx
# /etc/nginx/sites-available/ev-rental-api
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

## 🔧 **Cấu hình cho Frontend**

### **API Endpoints cho Frontend:**
```javascript
// Base URL
const API_BASE_URL = 'https://your-api-domain.com/api';

// Authentication APIs
const authAPI = {
  login: `${API_BASE_URL}/auth/login`,
  register: `${API_BASE_URL}/auth/register`,
  logout: `${API_BASE_URL}/auth/logout`,
  validate: `${API_BASE_URL}/auth/validate`
};

// Example usage
const login = async (email, password) => {
  const response = await fetch(authAPI.login, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ email, password })
  });
  return response.json();
};
```

### **CORS Configuration:**
API đã được cấu hình CORS để cho phép frontend gọi từ bất kỳ domain nào.

---

## 📋 **Checklist trước khi deploy:**

- [ ] Code đã build thành công
- [ ] Database connection string đúng
- [ ] JWT settings được cấu hình
- [ ] CORS đã enable
- [ ] Logging đã cấu hình
- [ ] SSL certificate (nếu cần)

---

## 🆘 **Troubleshooting:**

### **Lỗi thường gặp:**
1. **Database connection failed**
   - Kiểm tra connection string
   - Kiểm tra firewall/network

2. **CORS error**
   - Kiểm tra CORS configuration
   - Kiểm tra frontend URL

3. **JWT token invalid**
   - Kiểm tra JWT settings
   - Kiểm tra token format

### **Logs:**
```bash
# Docker
docker-compose logs -f ev-rental-api

# Azure
# Xem logs trong Azure Portal

# VPS
journalctl -u your-service-name -f
```

---

## 🎯 **Kết quả sau khi deploy:**

- ✅ API chạy ổn định trên server
- ✅ Frontend có thể gọi API
- ✅ Database kết nối thành công
- ✅ JWT authentication hoạt động
- ✅ Swagger UI accessible

