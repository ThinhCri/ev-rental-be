-- Add more electric vehicles with English names
USE EV_Rental_System;
GO

-- Insert additional electric vehicle brands
INSERT INTO Brand (Brand_name)
SELECT v.Brand_name
FROM (VALUES
    ('VinFast'),
    ('BYD'),
    ('NIO'),
    ('Rivian'),
    ('Polestar')
) AS v(Brand_name)
WHERE NOT EXISTS (
    SELECT 1 FROM Brand b WHERE b.Brand_name = v.Brand_name
);


-- Get station IDs
DECLARE @HCMStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%HCM Central%');
DECLARE @HCMStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%HCM Business%');
DECLARE @BDStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%BD Industrial%');
DECLARE @BDStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%BD Gateway%');
DECLARE @DNStation1 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%DN City Center%');
DECLARE @DNStation2 INT = (SELECT Station_Id FROM Station WHERE Station_name LIKE '%DN Airport%');

-- Get brand IDs
DECLARE @TeslaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Tesla');
DECLARE @VinFastBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'VinFast');
DECLARE @BYDBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'BYD');
DECLARE @BMWBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'BMW');
DECLARE @MercedesBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Mercedes');
DECLARE @HondaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Honda');
DECLARE @YamahaBrand INT = (SELECT Brand_Id FROM Brand WHERE Brand_name = 'Yamaha');

-- Insert additional electric vehicles
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

PRINT 'Additional electric vehicles added successfully!';




