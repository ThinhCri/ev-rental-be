# 📋 HƯỚNG DẪN THÊM 2 CỘT MỚI VÀO BẢNG CONTRACT

## ⚠️ QUAN TRỌNG: Cần chạy SQL script này trước khi test API mới

### Cách 1: Chạy trong SQL Server Management Studio (SSMS)

1. Mở **SQL Server Management Studio (SSMS)**
2. Connect vào database của bạn
3. Chọn database `EV_RENTAL_SYSTEM` (hoặc tên database của bạn)
4. Mở file `Database/AddVehicleImagesToContract.sql`
5. Copy và paste vào query window
6. Nhấn **F5** để execute

### Cách 2: Chạy trong Azure Data Studio

1. Mở **Azure Data Studio**
2. Connect vào database
3. Tạo query mới
4. Copy script dưới đây vào và chạy

### Cách 3: Chạy trực tiếp

```sql
-- Open SSMS hoặc Azure Data Studio
-- Connect vào database của bạn
-- Chạy script dưới đây:
```

```sql
-- Kiểm tra và thêm cột Handover_Image
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contract') AND name = 'Handover_Image')
BEGIN
    ALTER TABLE [Contract]
    ADD [Handover_Image] nvarchar(500) NULL;
    PRINT '✓ Added column Handover_Image to Contract table';
END
ELSE
BEGIN
    PRINT '⚠ Column Handover_Image already exists';
END
GO

-- Kiểm tra và thêm cột Return_Image
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contract') AND name = 'Return_Image')
BEGIN
    ALTER TABLE [Contract]
    ADD [Return_Image] nvarchar(500) NULL;
    PRINT '✓ Added column Return_Image to Contract table';
END
ELSE
BEGIN
    PRINT '⚠ Column Return_Image already exists';
END
GO

PRINT '✅ Migration completed successfully!';
```

---

## ✅ KIỂM TRA SAU KHI CHẠY

Sau khi chạy script, kiểm tra bằng câu lệnh sau:

```sql
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Contract'
AND COLUMN_NAME IN ('Handover_Image', 'Return_Image')
```

**Kết quả mong đợi:**
```
COLUMN_NAME         DATA_TYPE    CHARACTER_MAXIMUM_LENGTH  IS_NULLABLE
Handover_Image      nvarchar     500                       YES
Return_Image        nvarchar     500                       YES
```

---

## 🎯 SAU KHI CHẠY SCRIPT

1. ✅ Build lại project:
   ```bash
   dotnet build
   ```

2. ✅ Test API:
   ```bash
   # Giao xe
   PUT /api/Rental/{orderId}/handover-details
   
   # Trả xe
   PUT /api/Rental/{orderId}/return
   
   # Xem ảnh
   GET /api/Rental/{orderId}
   ```

---

## 📌 LƯU Ý

- **KHÔNG** xóa dữ liệu hiện có
- **KHÔNG** ảnh hưởng đến các bảng khác
- Chỉ thêm 2 cột mới vào bảng Contract
- Các cột mới là **NULL** được phép

---

## 🚀 NEXT STEPS

Sau khi chạy script, bạn có thể:
1. Test upload ảnh khi giao xe
2. Test upload ảnh khi trả xe  
3. Xem ảnh để đối chiếu qua API GET /api/Rental/{orderId}

**File được tạo:** `Database/AddVehicleImagesToContract.sql`

