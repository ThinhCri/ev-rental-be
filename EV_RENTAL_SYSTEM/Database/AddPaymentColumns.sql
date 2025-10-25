-- Script để thêm các cột thiếu vào bảng Payment
-- Chạy script này để sửa lỗi "Invalid column name"

-- Thêm cột Method nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payment') AND name = 'Method')
BEGIN
    ALTER TABLE Payment ADD Method NVARCHAR(50) NULL;
    PRINT 'Added Method column to Payment table';
END
ELSE
BEGIN
    PRINT 'Method column already exists in Payment table';
END

-- Thêm cột Note nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payment') AND name = 'Note')
BEGIN
    ALTER TABLE Payment ADD Note NVARCHAR(500) NULL;
    PRINT 'Added Note column to Payment table';
END
ELSE
BEGIN
    PRINT 'Note column already exists in Payment table';
END

-- Thêm cột Status nếu chưa có (có thể đã có nhưng kiểm tra lại)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payment') AND name = 'Status')
BEGIN
    ALTER TABLE Payment ADD Status NVARCHAR(50) NULL;
    PRINT 'Added Status column to Payment table';
END
ELSE
BEGIN
    PRINT 'Status column already exists in Payment table';
END

-- Cập nhật giá trị mặc định cho các cột mới (chỉ sau khi đã thêm cột)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payment') AND name = 'Method')
BEGIN
    UPDATE Payment SET Method = 'VNPay' WHERE Method IS NULL;
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payment') AND name = 'Status')
BEGIN
    UPDATE Payment SET Status = 'Pending' WHERE Status IS NULL;
END

PRINT 'Payment table schema updated successfully!';
