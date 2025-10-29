# Hướng dẫn Deploy EV Rental System lên Render

## Bước 1: Chuẩn bị Repository

1. **Push code lên GitHub:**
   ```bash
   git add .
   git commit -m "Prepare for deployment"
   git push origin main
   ```

2. **Đảm bảo có các file cần thiết:**
   - ✅ `Dockerfile`
   - ✅ `.dockerignore`
   - ✅ `render.yaml` (tùy chọn)
   - ✅ `appsettings.json`
   - ✅ `appsettings.Production.json`

## Bước 2: Tạo Web Service trên Render

### Cách 1: Sử dụng Docker (Khuyến nghị)

1. **Truy cập Render Dashboard:**
   - Đăng nhập vào [render.com](https://render.com)
   - Click "New +" → "Web Service"

2. **Cấu hình Web Service:**
   - **Connect Repository:** Chọn GitHub repo của bạn
   - **Name:** `ev-rental-system`
   - **Language:** `Docker`
   - **Branch:** `main`
   - **Region:** `Oregon (US West)` (hoặc region gần nhất)
   - **Root Directory:** Để trống
   - **Dockerfile Path:** `./Dockerfile`

3. **Cấu hình Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:10000
   ```

4. **Cấu hình Database:**
   - Click "Add Database"
   - Chọn "PostgreSQL"
   - Name: `ev-rental-db`
   - Plan: `Free`
   - Region: Cùng region với web service

5. **Cấu hình Connection String:**
   - Trong Environment Variables, thêm:
   ```
   ConnectionStrings__DefaultConnection=[Database URL từ Render]
   ```

### Cách 2: Sử dụng .NET Core trực tiếp

1. **Cấu hình Web Service:**
   - **Language:** `.NET Core`
   - **Build Command:** `dotnet restore && dotnet publish -c Release -o ./publish`
   - **Start Command:** `dotnet ./publish/EV_RENTAL_SYSTEM.dll`

## Bước 3: Cấu hình Database

1. **Tạo PostgreSQL Database:**
   - Trong Render Dashboard, click "New +" → "PostgreSQL"
   - Name: `ev-rental-db`
   - Plan: `Free`
   - Region: Cùng region với web service

2. **Lấy Connection String:**
   - Vào database dashboard
   - Copy "External Database URL"
   - Thêm vào Environment Variables của web service

## Bước 4: Cấu hình Environment Variables

Thêm các biến môi trường sau vào web service:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
ConnectionStrings__DefaultConnection=[Database URL]
```

## Bước 5: Deploy

1. **Click "Create Web Service"**
2. **Chờ build và deploy** (5-10 phút)
3. **Kiểm tra logs** nếu có lỗi

## Bước 6: Chạy Migration

Sau khi deploy thành công, cần chạy migration để tạo database schema:

1. **Vào web service dashboard**
2. **Click "Shell"**
3. **Chạy lệnh:**
   ```bash
   dotnet ef database update
   ```

## Bước 7: Test API

1. **Lấy URL của web service** (ví dụ: `https://ev-rental-system.onrender.com`)
2. **Test API:**
   ```bash
   curl https://ev-rental-system.onrender.com/api/vehicle
   ```

## Troubleshooting

### Lỗi thường gặp:

1. **Build failed:**
   - Kiểm tra Dockerfile
   - Kiểm tra .dockerignore
   - Kiểm tra dependencies

2. **Database connection failed:**
   - Kiểm tra Connection String
   - Kiểm tra database đã được tạo chưa
   - Kiểm tra network access

3. **Migration failed:**
   - Chạy migration thủ công trong Shell
   - Kiểm tra database permissions

### Logs và Debug:

1. **Xem logs:** Web Service → Logs
2. **Shell access:** Web Service → Shell
3. **Environment variables:** Web Service → Environment

## Cấu hình Production

### Security:
- Sử dụng HTTPS
- Cấu hình CORS
- Sử dụng secrets cho sensitive data

### Performance:
- Cấu hình caching
- Optimize database queries
- Sử dụng CDN cho static files

### Monitoring:
- Cấu hình health checks
- Set up alerts
- Monitor performance metrics

## Cost Optimization

- Sử dụng Free tier cho development
- Upgrade lên Starter plan cho production
- Monitor usage và optimize resources

## Backup và Recovery

- Enable automatic backups cho database
- Test restore process
- Document recovery procedures
