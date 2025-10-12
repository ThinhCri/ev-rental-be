USE EV_Rental_System;
ALTER TABLE [Contract] ADD [Contract_Code] nvarchar(50) NULL;
PRINT 'Đã thêm cột Contract_Code';
