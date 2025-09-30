# 🚀 Team Quick Start Guide

## 📋 Cách setup project lần đầu

### **Bước 1: Clone repository**
```bash
git clone <repository-url>
cd EV_RENTAL_SYSTEM
```

### **Bước 2: Chạy migration**
```bash
dotnet ef database update
```

### **Bước 3: Chạy app**
```bash
dotnet run
```

### **Bước 4: Data tự động xuất hiện! 🎉**

## 📊 Data sẽ được tạo tự động

Khi chạy `dotnet run`, console sẽ hiển thị:
```
✓ Database migration completed successfully.
Starting data seeding...
✓ Roles seeded successfully
✓ License Types seeded successfully  
✓ Brands seeded successfully
✓ Stations seeded successfully
✓ Process Steps seeded successfully
✓ Sample Vehicles seeded successfully
✓ Sample License Plates seeded successfully
✓ Data seeding completed successfully.
```

## 🔍 Cách kiểm tra data

### **1. Qua Swagger UI:**
- Mở: `https://localhost:7000/swagger`
- Test các API endpoints
- Sẽ thấy data đã được tạo

### **2. Qua Database:**
```sql
-- Kiểm tra trong SQL Server Management Studio
SELECT * FROM Role;
SELECT * FROM LicenseType; 
SELECT * FROM Brand;
SELECT * FROM Station;
SELECT * FROM Vehicle;
SELECT * FROM LicensePlate;
```

## 🔄 Cách pull code mới

### **Khi có code mới:**
```bash
git pull origin main
dotnet ef database update  # Chạy migration mới (nếu có)
dotnet run                # Chạy app + data seeding
```

## ⚠️ Troubleshooting

### **Nếu data không xuất hiện:**
1. Kiểm tra console log có lỗi gì không
2. Kiểm tra database connection
3. Reset database: `dotnet ef database drop && dotnet ef database update`

### **Nếu migration lỗi:**
1. Xem chi tiết: `dotnet ef database update --verbose`
2. Rollback: `dotnet ef database update 0`
3. Chạy lại: `dotnet ef database update`

## 📝 Lưu ý quan trọng

- ✅ **KHÔNG cần xóa migration cũ**
- ✅ **Data seeding an toàn** - chỉ thêm data chưa có
- ✅ **Tự động** - không cần làm gì thêm
- ✅ **Nhất quán** - mọi người có data giống nhau

## 🎯 Kết luận

**Chỉ cần 3 lệnh là có data ngay!**
```bash
git clone <repo>
dotnet ef database update
dotnet run
```

**Data sẽ tự động xuất hiện!** 🎉
