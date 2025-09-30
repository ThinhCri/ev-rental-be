-- Insert dữ liệu mẫu cần thiết cho EV Rental System
USE EV_Rental_System;
GO

-- Insert Roles nếu chưa có
IF NOT EXISTS (SELECT 1 FROM Role WHERE Role_name = 'Admin')
BEGIN
    INSERT INTO Role (Role_name, Description) VALUES 
    ('Admin', 'System Administrator'),
    ('Station Staff', 'Station Staff Member'),
    ('EV Renter', 'Electric Vehicle Renter');
END

-- Insert License Types nếu chưa có
IF NOT EXISTS (SELECT 1 FROM LicenseType WHERE Type_name = 'A1')
BEGIN
    INSERT INTO LicenseType (Type_name, Description) VALUES 
    ('A1', 'Motorcycle up to 125cc'),
    ('A2', 'Motorcycle up to 175cc'),
    ('A', 'Unlimited motorcycle'),
    ('B1', 'Car up to 9 seats'),
    ('B2', 'Unlimited car');
END

-- Insert sample brands nếu chưa có
IF NOT EXISTS (SELECT 1 FROM Brand WHERE Brand_name = 'Honda')
BEGIN
    INSERT INTO Brand (Brand_name) VALUES 
    ('Honda'),
    ('Yamaha'),
    ('Suzuki'),
    ('Kawasaki'),
    ('Ducati');
END

-- Insert sample stations nếu chưa có
IF NOT EXISTS (SELECT 1 FROM Station WHERE Station_name = 'Station 1')
BEGIN
    INSERT INTO Station (Station_name, Street, District, Province, Country) VALUES 
    ('Station 1', '123 Main Street', 'District 1', 'Ho Chi Minh City', 'Vietnam'),
    ('Station 2', '456 Le Loi', 'District 3', 'Ho Chi Minh City', 'Vietnam'),
    ('Station 3', '789 Nguyen Hue', 'District 1', 'Ho Chi Minh City', 'Vietnam');
END

PRINT 'Sample data inserted successfully!';

