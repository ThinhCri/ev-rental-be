-- Add more license plates for the new vehicles
USE EV_Rental_System;
GO

-- Get vehicle IDs for new vehicles
DECLARE @TeslaModelS INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Tesla Model S Plaid');
DECLARE @VinFastVF8 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'VinFast VF8');
DECLARE @BYDAtto3 INT = (SELECT Vehicle_ID FROM Vehicle WHERE Model = 'BYD Atto 3');
DECLARE @BMWi4 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BMW i4 M50');
DECLARE @MercedesEQS INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Mercedes EQS 580');
DECLARE @VinFastVF9 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'VinFast VF9');
DECLARE @TeslaModelX INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Tesla Model X');
DECLARE @BYDTang INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BYD Tang EV');
DECLARE @BMWiX INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BMW iX xDrive50');

-- Get station IDs
DECLARE @HCMStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%HCM Central%');
DECLARE @HCMStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%HCM Business%');
DECLARE @BDStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%BD Industrial%');
DECLARE @BDStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%BD Gateway%');
DECLARE @DNStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%DN City Center%');
DECLARE @DNStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%DN Airport%');

-- Insert license plates for new vehicles
INSERT INTO LicensePlate (License_plate_Id, Status, Vehicle_Id, Province, Registration_date, Condition, Station_Id) VALUES
-- HCM License Plates
('51A-20001', 'Available', @TeslaModelS, 'Ho Chi Minh City', '2024-01-15', 'Excellent', @HCMStation1),
('51A-20002', 'Available', @VinFastVF8, 'Ho Chi Minh City', '2024-01-20', 'Good', @HCMStation1),
('51A-20003', 'Available', @BYDAtto3, 'Ho Chi Minh City', '2024-01-25', 'Good', @HCMStation2),

-- BD License Plates
('61A-20001', 'Available', @BMWi4, 'Binh Duong', '2024-02-01', 'Excellent', @BDStation1),
('61A-20002', 'Available', @MercedesEQS, 'Binh Duong', '2024-02-05', 'Excellent', @BDStation1),
('61A-20003', 'Available', @VinFastVF9, 'Binh Duong', '2024-02-10', 'Good', @BDStation2),

-- DN License Plates
('43A-20001', 'Available', @TeslaModelX, 'Da Nang', '2024-02-15', 'Excellent', @DNStation1),
('43A-20002', 'Available', @BYDTang, 'Da Nang', '2024-02-20', 'Good', @DNStation1),
('43A-20003', 'Available', @BMWiX, 'Da Nang', '2024-02-25', 'Excellent', @DNStation2);

PRINT 'Additional license plates added successfully!';




