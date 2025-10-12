-- Thêm cột Contract_Code vào bảng Contract
USE EV_Rental_System;

-- Kiểm tra xem cột đã tồn tại chưa
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Contract]') AND name = 'Contract_Code')
BEGIN
    ALTER TABLE [Contract] 
    ADD [Contract_Code] nvarchar(50) NULL;
    
    PRINT 'Đã thêm cột Contract_Code vào bảng Contract';
END
ELSE
BEGIN
    PRINT 'Cột Contract_Code đã tồn tại trong bảng Contract';
END

-- Cập nhật Contract_Code cho các bản ghi hiện có (nếu có)
UPDATE [Contract] 
SET [Contract_Code] = 'EV' + FORMAT(GETDATE(), 'yyyyMMddHHmmss') + CAST(Contract_Id AS VARCHAR(10))
WHERE [Contract_Code] IS NULL;

PRINT 'Đã cập nhật Contract_Code cho các bản ghi hiện có';
