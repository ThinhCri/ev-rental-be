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
    Role_Id INT FOREIGN KEY REFERENCES Role(Role_Id),
    Phone NVARCHAR(15)
);

-- Bảng LicenseType
CREATE TABLE LicenseType (
    License_type_Id NVARCHAR PRIMARY KEY ,
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

-- Insert License Types
INSERT INTO LicenseType (Type_name, Description) VALUES 
('A1', 'Motorcycle up to 125cc'),
('A2', 'Motorcycle up to 175cc'),
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
-- Insert sample stations
INSERT INTO Station (Station_name, Street, District, Province, Country) VALUES 
('Station 1', '123 Main Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Station 2', '456 Le Loi', 'District 3', 'Ho Chi Minh City', 'Vietnam'),
('Station 3', '789 Nguyen Hue', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
('Station 4', '12 Binh Duong Avenue', 'Thu Dau Mot', 'Binh Duong', 'Vietnam'),
('Station 5', '88 Vo Nguyen Giap', 'Bien Hoa', 'Dong Nai', 'Vietnam'),
('Station 6', '45 Le Hong Phong', 'Vung Tau City', 'Ba Ria - Vung Tau', 'Vietnam'),
('Station 7', '101 Tran Hung Dao', 'My Tho', 'Tien Giang', 'Vietnam'),
('Station 8', '202 Cach Mang Thang 8', 'Ninh Kieu', 'Can Tho', 'Vietnam');




