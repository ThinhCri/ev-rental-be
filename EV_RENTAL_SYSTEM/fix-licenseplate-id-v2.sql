-- Script để sửa LicensePlateId từ string sang int (phiên bản 2)
-- Bước 1: Xóa tất cả foreign key constraints trước
ALTER TABLE Order_LicensePlate DROP CONSTRAINT IF EXISTS FK_Order_LicensePlate_LicensePlate_License_plate_Id;
ALTER TABLE Maintenance DROP CONSTRAINT IF EXISTS FK_Maintenance_LicensePlate_License_plate_Id;
ALTER TABLE LicensePlate DROP CONSTRAINT IF EXISTS FK_LicensePlate_Vehicle_Vehicle_Id;
ALTER TABLE LicensePlate DROP CONSTRAINT IF EXISTS FK_LicensePlate_Station_Station_Id;

-- Bước 2: Xóa các bảng phụ thuộc
DROP TABLE IF EXISTS Order_LicensePlate;
DROP TABLE IF EXISTS Maintenance;

-- Bước 3: Tạo bảng tạm với kiểu dữ liệu mới
CREATE TABLE LicensePlate_New (
    License_plate_Id INT IDENTITY(1,1) PRIMARY KEY,
    Plate_Number NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50),
    Vehicle_Id INT NOT NULL,
    Registration_date DATE,
    Station_Id INT NOT NULL
);

-- Bước 4: Copy dữ liệu từ bảng cũ sang bảng mới
INSERT INTO LicensePlate_New (Plate_Number, Status, Vehicle_Id, Registration_date, Station_Id)
SELECT Plate_Number, Status, Vehicle_Id, Registration_date, Station_Id
FROM LicensePlate;

-- Bước 5: Xóa bảng cũ
DROP TABLE LicensePlate;

-- Bước 6: Đổi tên bảng mới thành bảng chính
EXEC sp_rename 'LicensePlate_New', 'LicensePlate';

-- Bước 7: Tạo lại foreign keys
ALTER TABLE LicensePlate
ADD CONSTRAINT FK_LicensePlate_Vehicle_Vehicle_Id
FOREIGN KEY (Vehicle_Id) REFERENCES Vehicle(Vehicle_Id);

ALTER TABLE LicensePlate
ADD CONSTRAINT FK_LicensePlate_Station_Station_Id
FOREIGN KEY (Station_Id) REFERENCES Station(Station_Id);

-- Bước 8: Tạo lại bảng Order_LicensePlate
CREATE TABLE Order_LicensePlate (
    Order_Id INT NOT NULL,
    License_plate_Id INT NOT NULL,
    PRIMARY KEY (Order_Id, License_plate_Id),
    FOREIGN KEY (Order_Id) REFERENCES [Order](Order_Id),
    FOREIGN KEY (License_plate_Id) REFERENCES LicensePlate(License_plate_Id)
);

-- Bước 9: Tạo lại bảng Maintenance
CREATE TABLE Maintenance (
    Maintenance_Id INT IDENTITY(1,1) PRIMARY KEY,
    Description NVARCHAR(255),
    Cost DECIMAL(10,2),
    Maintenance_date DATE,
    Status NVARCHAR(50),
    License_plate_Id INT NOT NULL,
    FOREIGN KEY (License_plate_Id) REFERENCES LicensePlate(License_plate_Id)
);

PRINT 'Đã sửa xong LicensePlateId từ string sang int';

