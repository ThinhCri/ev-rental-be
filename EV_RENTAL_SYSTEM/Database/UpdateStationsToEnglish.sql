-- Update station names to English
USE EV_Rental_System;
GO

-- Update Ho Chi Minh City stations
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

-- Update Binh Duong stations
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

-- Update Da Nang stations
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

PRINT 'Station names updated to English successfully!';




