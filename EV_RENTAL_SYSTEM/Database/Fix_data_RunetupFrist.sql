-- Thêm cột vehicle_img
ALTER TABLE Vehicle
ADD vehicle_img VARCHAR(255);

-- Thêm cột price_per_day
ALTER TABLE Vehicle
ADD price_per_day INT;

ALTER TABLE Vehicle
ADD Battery varchar(10);

ALTER TABLE Vehicle
drop column Vehicle_type;

-- Xóa cột daily_rate
ALTER TABLE Vehicle
DROP COLUMN Daily_rate;



AlTER TABLE LicensePlate
drop column Province;
 
AlTER TABLE LicensePlate
drop column Registration_date;

alter table LicensePlate
add Kilometers_driven decimal(10,2);

alter table LicensePlate
add Plate_number varchar(50);

alter table Station
add Total_vehicle int;

alter table Station
add Ward nvarchar(50);

alter table ProcessStep
add Step_order int;

alter table Contract
add Contract_code varchar(50);

alter table Contract
drop column Status ;

alter table Payment
add Method varchar(50);

alter table Payment
add Note varchar(50);

alter table [Transaction]
drop column Amount;

alter table [Transaction]
add Status varchar(50);
