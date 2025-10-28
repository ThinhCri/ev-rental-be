-- Tạo database EV_Rental_System
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EV_Rental_System')
BEGIN
    CREATE DATABASE EV_Rental_System;
END
GO

USE EV_Rental_System;
GO

-- Bảng Role
CREATE TABLE Role (
    Role_Id INT PRIMARY KEY IDENTITY,
    Role_name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng User
CREATE TABLE [User] (
    User_Id INT PRIMARY KEY IDENTITY,
    Full_name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Birthday DATE,
    Created_at DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50),
<<<<<<< HEAD
    Role_Id INT FOREIGN KEY REFERENCES Role(Role_Id),
    Phone NVARCHAR(15)
=======
    Role_Id INT FOREIGN KEY REFERENCES Role(Role_Id)
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
);

-- Bảng LicenseType
CREATE TABLE LicenseType (
<<<<<<< HEAD
    License_type_Id NVARCHAR PRIMARY KEY ,
=======
    License_type_Id INT PRIMARY KEY IDENTITY,
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
    Type_name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng License
CREATE TABLE License (
    License_Id INT PRIMARY KEY IDENTITY,
    License_number NVARCHAR(50) UNIQUE NOT NULL,
    Expiry_date DATE NOT NULL,
    User_Id INT FOREIGN KEY REFERENCES [User](User_Id),
    License_type_Id INT FOREIGN KEY REFERENCES LicenseType(License_type_Id),
    License_ImageUrl NVARCHAR(255) -- ảnh bằng lái
);

-- Bảng Brand
CREATE TABLE Brand (
    Brand_Id INT PRIMARY KEY IDENTITY,
    Brand_name NVARCHAR(100) NOT NULL
);

-- Bảng Vehicle
CREATE TABLE Vehicle (
    Vehicle_Id INT PRIMARY KEY IDENTITY,
    Model NVARCHAR(100) NOT NULL,
    Model_year INT,
    Brand_Id INT FOREIGN KEY REFERENCES Brand(Brand_Id),
    Vehicle_type NVARCHAR(50),
    Description NVARCHAR(255),
    Daily_rate DECIMAL(10,2),
    Seat_number INT
);

-- Bảng Station
CREATE TABLE Station (
    Station_Id INT PRIMARY KEY IDENTITY,
    Station_name NVARCHAR(100),
    Street NVARCHAR(100),
    District NVARCHAR(50),
    Province NVARCHAR(50),
    Country NVARCHAR(50)
);

-- Bảng LicensePlate (fix: thêm Station_Id để tạo quan hệ 1–N)
CREATE TABLE LicensePlate (
    License_plate_Id NVARCHAR(50) PRIMARY KEY,
    Status NVARCHAR(50),
    Vehicle_Id INT FOREIGN KEY REFERENCES Vehicle(Vehicle_Id),
    Province NVARCHAR(50),
    Registration_date DATE,
    Condition NVARCHAR(50),
    Station_Id INT FOREIGN KEY REFERENCES Station(Station_Id) -- Quan hệ 1–N
);

-- Bảng Order
CREATE TABLE [Order] (
    Order_Id INT PRIMARY KEY IDENTITY,
    Order_date DATETIME DEFAULT GETDATE(),
    Start_time DATETIME,
    End_time DATETIME,
    Total_amount DECIMAL(10,2),
    Status NVARCHAR(50),
    User_Id INT FOREIGN KEY REFERENCES [User](User_Id)
);

-- Bảng Order_LicensePlate (N–N giữa Order và LicensePlate)
CREATE TABLE Order_LicensePlate (
    Order_Id INT FOREIGN KEY REFERENCES [Order](Order_Id),
    License_plate_Id NVARCHAR(50) FOREIGN KEY REFERENCES LicensePlate(License_plate_Id),
    PRIMARY KEY (Order_Id, License_plate_Id)
);

-- Bảng Complaint
CREATE TABLE Complaint (
    Complaint_Id INT PRIMARY KEY IDENTITY,
    Complaint_date DATETIME DEFAULT GETDATE(),
    Description NVARCHAR(255),
    Status NVARCHAR(50),
    User_Id INT FOREIGN KEY REFERENCES [User](User_Id),
    Order_Id INT FOREIGN KEY REFERENCES [Order](Order_Id)
);

-- Bảng Maintenance
CREATE TABLE Maintenance (
    Maintenance_Id INT PRIMARY KEY IDENTITY,
    Description NVARCHAR(255),
    Cost DECIMAL(10,2),
    Maintenance_date DATE,
    Status NVARCHAR(50),
    License_plate_Id NVARCHAR(50) FOREIGN KEY REFERENCES LicensePlate(License_plate_Id)
);

-- Bảng ProcessStep
CREATE TABLE ProcessStep (
    Step_Id INT PRIMARY KEY IDENTITY,
    Step_name NVARCHAR(50),
    Terms NVARCHAR(255)
);

-- Bảng Contract
CREATE TABLE Contract (
    Contract_Id INT PRIMARY KEY IDENTITY,
    Order_Id INT FOREIGN KEY REFERENCES [Order](Order_Id),
    Created_date DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50),
    Deposit DECIMAL(10,2),
    Rental_fee DECIMAL(10,2),
    Extra_fee DECIMAL(10,2)
);

-- Bảng ContractProcessing (fix: Contract_Id FK tới Contract)
CREATE TABLE ContractProcessing (
    ContractProcessing_Id INT PRIMARY KEY IDENTITY,
    Contract_Id INT FOREIGN KEY REFERENCES Contract(Contract_Id),
    Step_Id INT FOREIGN KEY REFERENCES ProcessStep(Step_Id),
    Status NVARCHAR(50)
);

-- Bảng Payment
CREATE TABLE Payment (
    Payment_Id INT PRIMARY KEY IDENTITY,
    Payment_date DATETIME DEFAULT GETDATE(),
    Amount DECIMAL(10,2),
    Status NVARCHAR(50),
    Contract_Id INT FOREIGN KEY REFERENCES Contract(Contract_Id)
);

-- Bảng Transaction
CREATE TABLE [Transaction] (
    Transaction_Id INT PRIMARY KEY IDENTITY,
    Amount DECIMAL(10,2),
    Transaction_date DATETIME DEFAULT GETDATE(),
    Payment_Id INT FOREIGN KEY REFERENCES Payment(Payment_Id),
    User_Id INT FOREIGN KEY REFERENCES [User](User_Id)
);

-- Insert dữ liệu mẫu
-- Insert Roles
<<<<<<< HEAD
=======
INSERT INTO Role (Role_name, Description) VALUES 
('Admin', 'System Administrator'),
('Station Staff', 'Station Staff Member'),
('EV Renter', 'Electric Vehicle Renter');
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88

-- Insert License Types
INSERT INTO LicenseType (Type_name, Description) VALUES 
('A1', 'Motorcycle up to 125cc'),
('A2', 'Motorcycle up to 175cc'),
<<<<<<< HEAD
('A', 'All types of two-wheeled motorcycles'),
('B1', 'Car up to 9 seats'),
('B2', 'Unlimited car (commercial use allowed)');

-- Insert sample brands
INSERT INTO Brand (Brand_name) VALUES
('Tesla'),
('BYD'),
('VinFast'),
('NIO'),
('Hyundai');


-- Insert sample stations
=======
('A', 'Unlimited motorcycle'),
('B1', 'Car up to 9 seats'),
('B2', 'Unlimited car');

-- Insert sample brands
INSERT INTO Brand (Brand_name) VALUES 
('Honda'),
('Yamaha'),
('Suzuki'),
('Kawasaki'),
('Ducati');

>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
-- Insert sample stations
INSERT INTO Station (Station_name, Street, District, Province, Country) VALUES 
('Station 1', '123 Main Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Station 2', '456 Le Loi', 'District 3', 'Ho Chi Minh City', 'Vietnam'),
<<<<<<< HEAD
('Station 3', '789 Nguyen Hue', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Station 4', '12 Binh Duong Avenue', 'Thu Dau Mot', 'Binh Duong', 'Vietnam'),
('Station 5', '88 Vo Nguyen Giap', 'Bien Hoa', 'Dong Nai', 'Vietnam'),
('Station 6', '45 Le Hong Phong', 'Vung Tau City', 'Ba Ria - Vung Tau', 'Vietnam'),
('Station 7', '101 Tran Hung Dao', 'My Tho', 'Tien Giang', 'Vietnam'),
('Station 8', '202 Cach Mang Thang 8', 'Ninh Kieu', 'Can Tho', 'Vietnam');

-- Tesla
INSERT INTO Vehicle (Model, Model_year, Brand_Id, Description, Seat_number, vehicle_img, price_per_day, Battery) VALUES
('Model 3', 2023, 1, 'Tesla Model 3, 5-seat electric sedan with Level 2 autopilot', 5, '/uploads/vehicles/tesla_model3.jpg', 1200000, 100),
('Model Y', 2023, 1, 'Tesla Model Y, 5-seat electric crossover with long-range battery', 5, '/uploads/vehicles/tesla_modely.jpg', 1300000, 100),
('BYD Dolphin', 2023, 2, 'Compact electric hatchback, economical and city-friendly', 5, '/uploads/vehicles/byd_dolphin.jpg', 800000, 100),
('BYD Atto 3', 2023, 2, 'Mid-size electric SUV with smart technology', 5, '/uploads/vehicles/byd_atto3.jpg', 1000000, 100),
('VF8', 2023, 3, 'VinFast VF8, 5-seat electric SUV with built-in virtual assistant', 5, '/uploads/vehicles/vf8.jpg', 1000000, 100),
('VF9', 2023, 3, 'VinFast VF9, 7-seat electric SUV with large battery and extended range', 7, '/uploads/vehicles/vf9.jpg', 1400000, 100),
('NIO ES6', 2022, 4, 'Luxury electric SUV with premium interior', 5, '/uploads/vehicles/nio_es6.jpg', 1500000, 100),
('NIO ET7', 2023, 4, 'Luxury electric sedan with Level 3 autonomous driving', 5, '/uploads/vehicles/nio_et7.jpg', 1700000, 100),
('Ioniq 5', 2023, 5, 'Hyundai Ioniq 5, 5-seat crossover EV with 350kW ultra-fast charging', 5, '/uploads/vehicles/ioniq5.jpg', 1100000, 100),
('Kona Electric', 2022, 5, 'Compact electric SUV, practical for urban driving', 5, '/uploads/vehicles/kona_electric.jpg', 900000, 100);


INSERT INTO LicensePlate (License_plate_Id, Status, Vehicle_Id, Condition, Station_Id, Kilometers_driven, Plate_number) VALUES
(1, 'Active', 1, 'New', 1, 1200.5, '51H-123.45'),
(2, 'Rent', 2, 'Good', 2, 3500.0, '51K-456.78'),
(3, 'Repair', 3, 'Needs Maintenance', 3, 5200.3, '61A-789.12'),
(4, 'Active', 4, 'Good', 4, 2100.0, '61B-234.56'),
(5, 'Rent', 5, 'Good', 5, 7800.0, '60A-111.22'),
(6, 'Active', 6, 'New', 6, 950.0, '72B-333.44'),
(7, 'Repair', 7, 'Needs Maintenance', 7, 10400.8, '63A-555.66'),
(8, 'Active', 8, 'Good', 8, 3300.0, '65B-777.88'),
(9, 'Rent', 9, 'Good', 1, 4700.6, '51F-999.00'),
(10, 'Active', 10, 'New', 2, 600.0, '51G-888.11');



=======
('Station 3', '789 Nguyen Hue', 'District 1', 'Ho Chi Minh City', 'Vietnam');

PRINT 'Database setup completed successfully!';
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88


