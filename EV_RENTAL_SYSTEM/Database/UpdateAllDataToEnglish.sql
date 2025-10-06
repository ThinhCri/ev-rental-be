-- Update all data to English
USE EV_Rental_System;
GO

-- Update station names to English
UPDATE Station 
SET Station_name = 'HCM Central Station',
    Street = '123 Nguyen Hue Boulevard'
WHERE Station_name LIKE '%HCM Station 1%';

UPDATE Station 
SET Station_name = 'HCM Business District Station',
    Street = '456 Le Loi Street'
WHERE Station_name LIKE '%HCM Station 2%';

UPDATE Station 
SET Station_name = 'HCM Binh Thanh Station',
    Street = '789 Cach Mang Thang 8 Street'
WHERE Station_name LIKE '%HCM Station 3%';

UPDATE Station 
SET Station_name = 'HCM New Urban Station',
    Street = '321 Nguyen Van Linh Boulevard'
WHERE Station_name LIKE '%HCM Station 4%';

UPDATE Station 
SET Station_name = 'BD Industrial Station',
    Street = '123 Nguyen Thi Minh Khai Street'
WHERE Station_name LIKE '%BD Station 1%';

UPDATE Station 
SET Station_name = 'BD Gateway Station',
    Street = '456 National Highway 1A'
WHERE Station_name LIKE '%BD Station 2%';

UPDATE Station 
SET Station_name = 'BD Tech Park Station',
    Street = '789 Provincial Road 743'
WHERE Station_name LIKE '%BD Station 3%';

UPDATE Station 
SET Station_name = 'BD Manufacturing Station',
    Street = '321 National Highway 13'
WHERE Station_name LIKE '%BD Station 4%';

UPDATE Station 
SET Station_name = 'DN City Center Station',
    Street = '123 Le Duan Street'
WHERE Station_name LIKE '%DN Station 1%';

UPDATE Station 
SET Station_name = 'DN Airport Station',
    Street = '456 Dien Bien Phu Street'
WHERE Station_name LIKE '%DN Station 2%';

UPDATE Station 
SET Station_name = 'DN Beach Station',
    Street = '789 Vo Nguyen Giap Street'
WHERE Station_name LIKE '%DN Station 3%';

UPDATE Station 
SET Station_name = 'DN University Station',
    Street = '321 Nguyen Van Linh Street'
WHERE Station_name LIKE '%DN Station 4%';

-- Add more electric vehicle brands
INSERT INTO Brand (Brand_name) VALUES 
('VinFast'),
('BYD'),
('NIO'),
('Rivian'),
('Polestar')
WHERE NOT EXISTS (SELECT 1 FROM Brand WHERE Brand_name IN ('VinFast', 'BYD', 'NIO', 'Rivian', 'Polestar'));

-- Get station IDs
DECLARE @HCMStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'HCM Central Station');
DECLARE @HCMStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'HCM Business District Station');
DECLARE @BDStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'BD Industrial Station');
DECLARE @BDStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'BD Gateway Station');
DECLARE @DNStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'DN City Center Station');
DECLARE @DNStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name = 'DN Airport Station');

-- Get brand IDs
DECLARE @TeslaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Tesla');
DECLARE @VinFastBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'VinFast');
DECLARE @BYDBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'BYD');
DECLARE @BMWBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'BMW');
DECLARE @MercedesBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Mercedes');
DECLARE @HondaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Honda');
DECLARE @YamahaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Yamaha');

-- Insert additional electric vehicles with English names
INSERT INTO Vehicle (Model, Model_year, Brand_Id, Vehicle_type, Description, Price_per_day, Seat_number, Battery, Charging_time, Range_km, Status, Station_Id) VALUES
-- HCM Electric Cars
('Tesla Model S Plaid', 2024, @TeslaBrand, 'Electric Car', 'High-performance electric sedan with 1000+ horsepower', 5000000, 5, 100.0, 15.0, 600, 'Good', @HCMStation1),
('VinFast VF8', 2024, @VinFastBrand, 'Electric SUV', 'Vietnamese electric SUV with modern design', 3500000, 7, 87.0, 12.0, 450, 'Good', @HCMStation1),
('BYD Atto 3', 2024, @BYDBrand, 'Electric SUV', 'Chinese electric SUV with advanced technology', 2800000, 5, 60.0, 8.0, 400, 'Good', @HCMStation2),

-- BD Electric Cars
('BMW i4 M50', 2024, @BMWBrand, 'Electric Car', 'High-performance electric sedan from BMW', 4500000, 5, 83.0, 10.0, 500, 'Good', @BDStation1),
('Mercedes EQS 580', 2024, @MercedesBrand, 'Electric Car', 'Luxury electric sedan with premium features', 6000000, 5, 108.0, 15.0, 700, 'Good', @BDStation1),
('VinFast VF9', 2024, @VinFastBrand, 'Electric SUV', 'Large electric SUV for families', 4000000, 7, 92.0, 14.0, 500, 'Good', @BDStation2),

-- DN Electric Cars
('Tesla Model X', 2024, @TeslaBrand, 'Electric SUV', 'Premium electric SUV with falcon doors', 5500000, 7, 100.0, 16.0, 550, 'Good', @DNStation1),
('BYD Tang EV', 2024, @BYDBrand, 'Electric SUV', 'Chinese electric SUV with 7 seats', 3200000, 7, 86.0, 11.0, 500, 'Good', @DNStation1),
('BMW iX xDrive50', 2024, @BMWBrand, 'Electric SUV', 'Luxury electric SUV with advanced tech', 5000000, 5, 111.0, 18.0, 600, 'Good', @DNStation2),

-- Electric Motorcycles
('Honda PCX Electric Pro', 2024, @HondaBrand, 'Electric Motorcycle', 'Premium electric scooter with extended range', 200000, 2, 4.0, 6.0, 80, 'Good', @HCMStation1),
('Yamaha E-Vino Deluxe', 2024, @YamahaBrand, 'Electric Motorcycle', 'Luxury electric scooter with smart features', 180000, 2, 3.5, 5.0, 70, 'Good', @HCMStation2),
('VinFast Theon', 2024, @VinFastBrand, 'Electric Motorcycle', 'Vietnamese electric motorcycle with sporty design', 250000, 2, 5.0, 7.0, 100, 'Good', @BDStation1),
('Yamaha E-Vino Sport', 2024, @YamahaBrand, 'Electric Motorcycle', 'Sport electric scooter for city commuting', 160000, 2, 3.0, 4.5, 60, 'Good', @BDStation2),
('Honda PCX Electric City', 2024, @HondaBrand, 'Electric Motorcycle', 'City-focused electric scooter', 170000, 2, 3.2, 4.8, 65, 'Good', @DNStation1),
('Yamaha E-Vino Touring', 2024, @YamahaBrand, 'Electric Motorcycle', 'Touring electric scooter for long distances', 220000, 2, 4.5, 6.5, 90, 'Good', @DNStation2);

-- Get vehicle IDs for new vehicles
DECLARE @TeslaModelS INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Tesla Model S Plaid');
DECLARE @VinFastVF8 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'VinFast VF8');
DECLARE @BYDAtto3 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BYD Atto 3');
DECLARE @BMWi4 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BMW i4 M50');
DECLARE @MercedesEQS INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Mercedes EQS 580');
DECLARE @VinFastVF9 INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'VinFast VF9');
DECLARE @TeslaModelX INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'Tesla Model X');
DECLARE @BYDTang INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BYD Tang EV');
DECLARE @BMWiX INT = (SELECT Vehicle_Id FROM Vehicle WHERE Model = 'BMW iX xDrive50');

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

PRINT 'All data updated to English successfully!';




