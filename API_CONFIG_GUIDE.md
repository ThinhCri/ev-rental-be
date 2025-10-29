# API Configuration Guide

## Cách cấu hình API Base URL

### 1. Tạo file .env.local
Tạo file `.env.local` trong thư mục `ev-rental-fe-dev/` với nội dung:

```bash
# Production API (mặc định)
VITE_API_BASE_URL=https://deloyapi-8.onrender.com

# Development API (để test local)
# VITE_API_BASE_URL=http://localhost:7057

# Ngrok API (để test với ngrok)
# VITE_API_BASE_URL=https://your-ngrok-url.ngrok.io
```

### 2. Các API endpoints có sẵn

#### Production API
- URL: `https://deloyapi-8.onrender.com`
- Trạng thái: Đang hoạt động
- Sử dụng: Mặc định

#### Local Development API
- URL: `http://localhost:7057`
- Trạng thái: Cần chạy backend local
- Sử dụng: Để test và phát triển

#### Ngrok API
- URL: `https://your-ngrok-url.ngrok.io`
- Trạng thái: Cần setup ngrok
- Sử dụng: Để test với external services

### 3. Cách thay đổi API

1. **Tạo file .env.local** trong thư mục `ev-rental-fe-dev/`
2. **Thêm dòng cấu hình**:
   ```bash
   VITE_API_BASE_URL=your-api-url-here
   ```
3. **Restart development server**:
   ```bash
   npm run dev
   ```

### 4. Kiểm tra cấu hình

Sau khi cấu hình, bạn có thể kiểm tra bằng cách:
- Mở Developer Tools (F12)
- Vào tab Console
- Gõ: `console.log(import.meta.env.VITE_API_BASE_URL)`

### 5. Troubleshooting

#### Lỗi: API không kết nối được
- Kiểm tra URL có đúng không
- Kiểm tra backend có đang chạy không
- Kiểm tra CORS settings

#### Lỗi: Environment variable không load
- Đảm bảo file `.env.local` ở đúng thư mục
- Restart development server
- Kiểm tra tên biến có đúng `VITE_API_BASE_URL` không

### 6. Các trang admin sử dụng API

Các trang admin đã được cập nhật để sử dụng cấu hình từ file .env:
- AdminDashboard: Revenue overview
- ManageStaff: Staff management
- ManageUser: User management
- StationRevenueTable: Revenue by station
- OverviewStats: Dashboard statistics
