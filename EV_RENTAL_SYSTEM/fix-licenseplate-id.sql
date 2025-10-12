-- Script để sửa LicensePlateId từ string sang int
-- Bước 1: Tạo bảng tạm
CREATE TABLE LicensePlate_Temp (
    License_plate_Id INT IDENTITY(1,1) PRIMARY KEY,
    Plate_Number NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50),
    Vehicle_Id INT NOT NULL,
    Registration_date DATE,
    Station_Id INT NOT NULL
);

-- Bước 2: Copy dữ liệu từ bảng cũ sang bảng mới
INSERT INTO LicensePlate_Temp (Plate_Number, Status, Vehicle_Id, Registration_date, Station_Id)
SELECT Plate_Number, Status, Vehicle_Id, Registration_date, Station_Id
FROM LicensePlate;

-- Bước 3: Xóa bảng cũ
DROP TABLE LicensePlate;

-- Bước 4: Đổi tên bảng tạm thành bảng chính
EXEC sp_rename 'LicensePlate_Temp', 'LicensePlate';

-- Bước 5: Tạo lại foreign keys
ALTER TABLE LicensePlate
ADD CONSTRAINT FK_LicensePlate_Vehicle_Vehicle_Id
FOREIGN KEY (Vehicle_Id) REFERENCES Vehicle(Vehicle_Id);

ALTER TABLE LicensePlate
ADD CONSTRAINT FK_LicensePlate_Station_Station_Id
FOREIGN KEY (Station_Id) REFERENCES Station(Station_Id);

-- Bước 6: Tạo lại bảng Order_LicensePlate với License_plate_Id là int
-- Trước tiên xóa bảng cũ
DROP TABLE Order_LicensePlate;

-- Tạo lại bảng với kiểu dữ liệu đúng
CREATE TABLE Order_LicensePlate (
    Order_Id INT NOT NULL,
    License_plate_Id INT NOT NULL,
    PRIMARY KEY (Order_Id, License_plate_Id),
    FOREIGN KEY (Order_Id) REFERENCES [Order](Order_Id),
    FOREIGN KEY (License_plate_Id) REFERENCES LicensePlate(License_plate_Id)
);

-- Bước 7: Tạo lại bảng Maintenance với License_plate_Id là int
-- Trước tiên xóa bảng cũ
DROP TABLE Maintenance;

-- Tạo lại bảng với kiểu dữ liệu đúng
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

