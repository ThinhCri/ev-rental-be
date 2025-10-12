USE EV_Rental_System;

-- Kiểm tra có LicensePlate nào không
SELECT COUNT(*) as LicensePlateCount FROM LicensePlate;

-- Kiểm tra có LicensePlate Available không
SELECT COUNT(*) as AvailableLicensePlateCount FROM LicensePlate WHERE Status = 'Available';

-- Kiểm tra có Vehicle nào không
SELECT COUNT(*) as VehicleCount FROM Vehicle;

-- Kiểm tra có Contract_Code column không
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Contract' AND COLUMN_NAME = 'Contract_Code';

-- Xem một vài LicensePlate mẫu
SELECT TOP 5 License_plate_Id, Plate_Number, Status, Vehicle_Id FROM LicensePlate;
