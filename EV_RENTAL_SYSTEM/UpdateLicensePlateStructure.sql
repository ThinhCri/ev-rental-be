-- Script để cập nhật cấu trúc bảng LicensePlate
USE EV_Rental_System;
GO

-- Bước 1: Tạo bảng tạm để lưu dữ liệu hiện tại
CREATE TABLE LicensePlate_Temp (
    License_plate_Id NVARCHAR(50),
    Status NVARCHAR(50),
    Vehicle_Id INT,
    Province NVARCHAR(50),
    Registration_date DATE,
    Condition NVARCHAR(50),
    Station_Id INT
);

-- Bước 2: Copy dữ liệu từ bảng cũ sang bảng tạm
INSERT INTO LicensePlate_Temp 
SELECT * FROM LicensePlate;

-- Bước 3: Xóa bảng cũ
DROP TABLE Order_LicensePlate;
DROP TABLE Maintenance;
DROP TABLE LicensePlate;

-- Bước 4: Tạo lại bảng LicensePlate với cấu trúc mới
CREATE TABLE LicensePlate (
    License_plate_Id INT PRIMARY KEY IDENTITY(1,1),
    Plate_Number NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50),
    Vehicle_Id INT FOREIGN KEY REFERENCES Vehicle(Vehicle_Id),
    Province NVARCHAR(50),
    Registration_date DATE,
    Condition NVARCHAR(50),
    Station_Id INT FOREIGN KEY REFERENCES Station(Station_Id)
);

-- Bước 5: Insert dữ liệu từ bảng tạm vào bảng mới
INSERT INTO LicensePlate (Plate_Number, Status, Vehicle_Id, Province, Registration_date, Condition, Station_Id)
SELECT License_plate_Id, Status, Vehicle_Id, Province, Registration_date, Condition, Station_Id
FROM LicensePlate_Temp;

-- Bước 6: Tạo lại bảng Order_LicensePlate với cấu trúc mới
CREATE TABLE Order_LicensePlate (
    Order_Id INT FOREIGN KEY REFERENCES [Order](Order_Id),
    License_plate_Id INT FOREIGN KEY REFERENCES LicensePlate(License_plate_Id),
    PRIMARY KEY (Order_Id, License_plate_Id)
);

-- Bước 7: Tạo lại bảng Maintenance với cấu trúc mới
CREATE TABLE Maintenance (
    Maintenance_Id INT PRIMARY KEY IDENTITY,
    Description NVARCHAR(255),
    Cost DECIMAL(10,2),
    Maintenance_date DATE,
    Status NVARCHAR(50),
    License_plate_Id INT FOREIGN KEY REFERENCES LicensePlate(License_plate_Id)
);

-- Bước 8: Xóa bảng tạm
DROP TABLE LicensePlate_Temp;

PRINT 'LicensePlate structure updated successfully!';
