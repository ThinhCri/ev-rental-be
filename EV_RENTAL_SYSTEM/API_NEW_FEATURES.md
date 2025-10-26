# 🚀 API MỚI - TỔNG HỢP

## 📋 Mục lục
1. [API Bàn Giao Xe (Handover Vehicle)](#1-api-bàn-giao-xe-handover-vehicle)
2. [API Trả Xe (Return Vehicle)](#2-api-trả-xe-return-vehicle)
3. [API Bảo Trì Xe (Maintenance)](#3-api-bảo-trì-xe-maintenance)
4. [API Update Order với Upload Ảnh](#4-api-update-order-với-upload-ảnh)
5. [API Xem Xe Theo Trạng Thái](#5-api-xem-xe-theo-trạng-thái)
6. [API Thống Kê Lợi Nhuận (Updated)](#6-api-thống-kê-lợi-nhuận-updated)

---

## 1. API Bàn Giao Xe (Handover Vehicle)

### 1.1 Bàn giao xe cơ bản

**Endpoint:**
```
PUT /api/Rental/{orderId}/handover
```

**Method:** PUT

**Authorization:** Staff/Admin

**Description:** Bàn giao xe cho khách hàng (không cần chi tiết)

**Tác động:**
- Đổi trạng thái Order: `Confirmed` → `Active`
- Đổi trạng thái LicensePlate: `Reserved` → `Rented`

---

### 1.2 Bàn giao xe với chi tiết (upload ảnh, odometer, battery)

**Endpoint:**
```
PUT /api/Rental/{orderId}/handover-details
```

**Method:** PUT
**Content-Type:** multipart/form-data
**Authorization:** Staff/Admin

**Body (Form-data):**
- `VehicleImage` (file, required) - Ảnh xe trước khi giao
- `Notes` (string, optional, max 1000 ký tự) - Ghi chú tình trạng xe
- `Odometer` (int, required, 0-999999) - Số km hiện tại của xe
- `Battery` (decimal, required, 0-100) - % pin hiện tại

**Tác động:**
- Upload ảnh xe → Lưu vào `HandoverImage` trong bảng Contract
- Lưu `Odometer` vào `RangeKm` của Vehicle
- Lưu `Battery` vào Vehicle
- Đổi trạng thái Order: `Confirmed` → `Active`
- Đổi trạng thái LicensePlate: `Reserved` → `Rented`

**Response:**
```json
{
  "success": true,
  "message": "Bàn giao xe thành công",
  "data": {
    "orderId": 123,
    "status": "Active",
    "vehicles": [...]
  }
}
```

---

## 2. API Trả Xe (Return Vehicle)

### 2.1 Trả xe với upload ảnh và tính phí phát sinh

**Endpoint:**
```
PUT /api/Rental/{orderId}/return
```

**Method:** PUT
**Content-Type:** multipart/form-data
**Authorization:** Staff/Admin

**Body (Form-data):**
- `VehicleImage` (file, required) - Ảnh xe sau khi trả
- `Notes` (string, optional, max 1000 ký tự) - Ghi chú
- `Odometer` (int, required, 0-999999) - Số km khi trả xe
- `Battery` (decimal, required, 0-100) - % pin khi trả xe

**Tính phí phát sinh:**
- **60 km đầu tiên:** Free
- **Mỗi km vượt quá 60:** $1/km
- **Pin dưới 50%:** +$10

**Tác động:**
- Upload ảnh xe → Lưu vào `ReturnImage` trong bảng Contract
- Cập nhật thông tin xe: `RangeKm`, `Battery`
- Tính Extra Fee → Cập nhật `ExtraFee` trong Contract
- Đổi trạng thái Order: `Active` → `Completed`
- **Tự động đổi trạng thái LicensePlate:** `Rented` → `Maintenance`

**Response:**
```json
{
  "success": true,
  "message": "Trả xe thành công",
  "data": {
    "orderId": 123,
    "status": "Completed",
    "extraFee": 25,
    "breakdown": {
      "kmOverLimit": 15,
      "kmFee": 15,
      "lowBatteryFee": 10
    }
  }
}
```

---

## 3. API Bảo Trì Xe (Maintenance)

### 3.1 Lấy tất cả bảo trì

**Endpoint:**
```
GET /api/Maintenance
```

**Authorization:** Staff/Admin

**Response:**
```json
{
  "success": true,
  "data": [...],
  "count": 10
}
```

---

### 3.2 Lấy bảo trì theo ID

**Endpoint:**
```
GET /api/Maintenance/{id}
```

**Authorization:** Staff/Admin

---

### 3.3 Lấy bảo trì theo biển số

**Endpoint:**
```
GET /api/Maintenance/license-plate/{licensePlateId}
```

**Authorization:** Staff/Admin

---

### 3.4 Tạo bảo trì mới

**Endpoint:**
```
POST /api/Maintenance
```

**Method:** POST
**Content-Type:** application/json
**Authorization:** Staff/Admin

**Body:**
```json
{
  "description": "Thay pin xe",
  "cost": 1000000,
  "maintenanceDate": "2025-10-26T08:00:00",
  "licensePlateId": 1,
  "status": "Scheduled"
}
```

**Tác động:**
- Tạo maintenance mới
- **Tự động chuyển LicensePlate sang `Maintenance`**

---

### 3.5 Cập nhật bảo trì

**Endpoint:**
```
PUT /api/Maintenance/{id}
```

**Method:** PUT
**Content-Type:** application/json
**Authorization:** Staff/Admin

**Body:**
```json
{
  "description": "Sửa chữa phanh",
  "cost": 500000,
  "maintenanceDate": "2025-10-27T08:00:00",
  "status": "In Progress"
}
```

**Tác động khi status = "Completed":**
- **Tự động set Battery = 100%**
- **Tự động chuyển LicensePlate sang `Available`**

---

### 3.6 Xóa bảo trì

**Endpoint:**
```
DELETE /api/Maintenance/{id}
```

**Authorization:** Staff/Admin

**Lưu ý:** Không cho phép xóa bảo trì đã hoàn thành

**Tác động:**
- Nếu bảo trì chưa hoàn thành → **Tự động chuyển LicensePlate sang `Available`**

---

### 3.7 Tìm kiếm bảo trì

**Endpoint:**
```
POST /api/Maintenance/search
```

**Method:** POST
**Content-Type:** application/json
**Authorization:** Staff/Admin

**Body:**
```json
{
  "licensePlateId": 1,
  "status": "Scheduled",
  "startDate": "2025-10-01",
  "endDate": "2025-10-31",
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## 4. API Update Order với Upload Ảnh

### 4.1 Cập nhật đơn thuê (hỗ trợ upload ảnh bằng lái xe)

**Endpoint:**
```
PUT /api/Rental/{id}
```

**Method:** PUT
**Content-Type:** multipart/form-data
**Authorization:** Staff/Admin

**Body (Form-data):**
- `StartTime` (DateTime, optional)
- `EndTime` (DateTime, optional)
- `VehicleIds` (List<int>, optional)
- `DepositAmount` (decimal, optional)
- `Status` (string, optional)
- `Notes` (string, optional)
- `RenterLicenseImage` (file, optional) - **Ảnh bằng lái xe (đặt hộ)**

**Tác động:**
- Upload ảnh BLX → Lưu vào `User.Notes`
- Cập nhật các thông tin khác
- Trả về thông tin đơn thuê đầy đủ

---

## 5. API Xem Xe Theo Trạng Thái

### 5.1 Lấy danh sách xe theo trạng thái

**Endpoint:**
```
GET /api/Vehicle/license-plates/status/{status}
```

**Authorization:** Staff/Admin

**Status values:**
- `Available` - Xe sẵn sàng cho thuê
- `Rented` - Xe đang được thuê
- `Maintenance` - Xe đang bảo trì
- `Reserved` - Xe đã đặt trước

**Response:**
```json
{
  "success": true,
  "message": "Lấy danh sách biển số xe trạng thái 'Maintenance' thành công",
  "data": [
    {
      "licensePlateId": 5,
      "plateNumber": "29A-12345",
      "status": "Maintenance",
      "vehicle": {
        "vehicleId": 10,
        "model": "VinFast VF e34",
        "modelYear": 2024,
        "description": "Xe điện sedan",
        "rangeKm": 25000,
        "battery": 45,
        "vehicleImage": "https://cloudinary.../vehicle.jpg",
        "brand": {
          "brandId": 1,
          "brandName": "VinFast"
        }
      },
      "station": {
        "stationId": 1,
        "stationName": "Chi nhánh Quận 1",
        "street": "123 Lê Lợi",
        "district": "Quận 1",
        "province": "TP.HCM",
        "country": "Vietnam"
      }
    }
  ],
  "count": 5
}
```

---

## 6. API Thống Kê Lợi Nhuận (Updated)

### 6.1 Overview (đã cập nhật để bao gồm Extra Fee)

**Endpoint:**
```
GET /api/Revenue/overview
```

**Authorization:** Admin

**Cập nhật:** Đã thêm Extra Fee vào tính toán

**Công thức:** 
```
Lợi nhuận = Tổng tiền thuê + Extra fee
```

---

## 🔄 FLOW HOẠT ĐỘNG

### Flow 1: Giao Xe

```
1. Order status: Confirmed
   ↓
2. Call: PUT /api/Rental/{orderId}/handover-details
   - Upload ảnh xe
   - Nhập Odometer (km)
   - Nhập Battery (%)
   ↓
3. → Order: Active, LicensePlate: Rented
```

---

### Flow 2: Trả Xe

```
1. Order status: Active
   ↓
2. Call: PUT /api/Rental/{orderId}/return
   - Upload ảnh xe
   - Nhập km (Odometer)
   - Nhập pin (Battery)
   ↓
3. Tính phí:
   - km vượt quá 60 → $1/km
   - Pin < 50% → +$10
   ↓
4. → Order: Completed, LicensePlate: Maintenance
   → Cập nhật ExtraFee trong Contract
```

---

### Flow 3: Bảo Trì

```
1. LicensePlate status: Maintenance
   ↓
2. POST /api/Maintenance (tạo bảo trì)
   ↓
3. PUT /api/Maintenance/{id} (thực hiện bảo trì)
   Status: In Progress
   ↓
4. PUT /api/Maintenance/{id} (hoàn thành)
   Status: Completed
   ↓
5. → Battery = 100%, LicensePlate: Available
```

---

### Flow 4: Thống Kê

```
GET /api/Revenue/overview
↓
Returns: Total Revenue (bao gồm Extra Fee)
```

---

## 📝 LƯU Ý QUAN TRỌNG

### ✅ Tính năng đã triển khai

1. ✅ Upload ảnh xe trước/sau khi sử dụng
2. ✅ Upload ảnh bằng lái xe (đặt hộ)
3. ✅ Ghi nhận odometer và battery
4. ✅ Tính phí phát sinh tự động
5. ✅ Tự động cập nhật trạng thái xe
6. ✅ API bảo trì xe đầy đủ
7. ✅ Tự động reset battery 100% khi hoàn thành bảo trì
8. ✅ Tính toán revenue bao gồm extra fee
9. ✅ API xem xe theo trạng thái

### ⚠️ Cần chạy SQL script

**Chạy script SQL để thêm 2 cột mới vào bảng Contract:**
```sql
File: Database/AddVehicleImagesToContract.sql
```
Script sẽ thêm:
- `Handover_Image` - Ảnh xe khi khách nhận
- `Return_Image` - Ảnh xe khi khách trả

### Cách xem ảnh sau khi lưu:

**API để lấy ảnh:**
```
GET /api/Rental/{orderId}
```

Response sẽ có:
```json
{
  "success": true,
  "data": {
    "orderId": 123,
    "contract": {
      "contractId": 456,
      "handoverImage": "https://cloudinary.../handover.jpg",
      "returnImage": "https://cloudinary.../return.jpg",
      "extraFee": 25
    },
    "vehicles": [...]
  }
}
```

---

## 🎯 CÁCH SỬ DỤNG CHO STAFF

### Xem xe nào đang bảo trì:

```bash
GET /api/Vehicle/license-plates/status/Maintenance
Authorization: Bearer {token}
```

### Xem bảo trì chi tiết:

```bash
GET /api/Maintenance/license-plate/{licensePlateId}
```

### Hoàn thành bảo trì:

```bash
PUT /api/Maintenance/{id}
Body: { "status": "Completed" }
```

---

## 📊 Ví Dụ Thực Tế

### Scenario 1: Khách trả xe (pin yếu)

**Request:**
```
PUT /api/Rental/123/return

VehicleImage: [ảnh xe khi trả]
Odometer: 25000 km
Battery: 45% (thấp)
Notes: "Pin yếu, cần sạc"
```

**Response:**
```json
{
  "success": true,
  "extraFee": 10,
  "breakdown": {
    "kmUsed": 50,
    "kmFee": 0,
    "lowBatteryFee": 10
  }
}
```

**Tác động:**
- Order: `Completed`
- LicensePlate: `Maintenance` (auto)
- Contract.ExtraFee: 10

---

### Scenario 2: Staff bảo trì và hoàn thành

**Step 1: Tạo bảo trì**
```
POST /api/Maintenance

{
  "description": "Sạc pin",
  "licensePlateId": 5,
  "status": "Scheduled"
}
```

**Step 2: Sạc pin**
```
PUT /api/Maintenance/10

{
  "status": "Completed"
}
```

**Kết quả:**
- Battery: 100%
- LicensePlate: `Available`
- Maintenance: `Completed`

---

## 🚀 Summary

Tất cả các API mới đã được implement và test thành công. Build không có lỗi. Code sẵn sàng để push lên GitHub!

### Files được thêm/sửa:
- ✅ `Controllers/RentalController.cs` - Thêm API handover và return
- ✅ `Controllers/MaintenanceController.cs` - Mới tạo
- ✅ `Controllers/VehicleController.cs` - Thêm API xem theo status
- ✅ `Services/Implementations/RentalService.cs` - Implement logic mới
- ✅ `Services/Implementations/MaintenanceService.cs` - Mới tạo
- ✅ `Services/Implementations/RevenueService.cs` - Cập nhật tính Extra Fee
- ✅ `Repositories/Implementations/MaintenanceRepository.cs` - Mới tạo
- ✅ `Repositories/Implementations/LicensePlateRepository.cs` - Thêm method GetLicensePlatesByStatusAsync
- ✅ `Models/DTOs/RentalDto.cs` - Thêm HandoverVehicleDto, ReturnVehicleDto
- ✅ `Models/DTOs/MaintenanceDto.cs` - Mới tạo
- ✅ `Models/Contract.cs` - Thêm HandoverImage, ReturnImage
- ✅ `Database/AddVehicleImagesToContract.sql` - Script thêm 2 cột mới vào Contract

---

**Version:** 1.0.0  
**Last Updated:** 2025-10-26  
**Status:** ✅ Production Ready

