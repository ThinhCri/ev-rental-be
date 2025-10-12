# LUỒNG BOOKING XE MỚI

## Tổng quan
Luồng booking xe đã được cập nhật để hỗ trợ đặt hộ, hiển thị bảng hợp đồng với các loại phí, và quy trình xác nhận của staff.

## Các bước trong luồng booking

### 1. Tạo đơn thuê xe
**Endpoint:** `POST /api/rental`

**Request Body:**
```json
{
  "startTime": "2024-12-20T08:00:00",
  "endTime": "2024-12-22T18:00:00",
  "vehicleIds": [1, 2],
  "depositAmount": 500000,
  "notes": "Ghi chú đơn thuê",
  "isBookingForOthers": false,
  "renterLicenseImageUrl": "URL ảnh GPLX (nếu đặt hộ)"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Tạo đơn thuê thành công",
  "orderId": 123,
  "contractId": 456
}
```

### 2. Lấy thông tin bảng hợp đồng
**Endpoint:** `GET /api/rental/{orderId}/contract-summary`

**Response:**
```json
{
  "success": true,
  "message": "Lấy thông tin hợp đồng thành công",
  "data": {
    "contractCode": "EV20241220123456789",
    "rentalFee": 1000000,
    "deposit": 500000,
    "overKmFee": 0,
    "electricityFee": 0,
    "totalAmount": 1500000,
    "status": "Pending",
    "createdDate": "2024-12-20T10:00:00",
    "expiryDate": "2024-12-20T10:02:00",
    "feeDetails": [
      {
        "feeType": "Rental",
        "feeName": "Phí thuê xe",
        "amount": 1000000,
        "description": "Thuê xe từ 20/12/2024 08:00 đến 22/12/2024 18:00"
      },
      {
        "feeType": "Deposit",
        "feeName": "Phí cọc",
        "amount": 500000,
        "description": "Phí cọc xe (sẽ hoàn trả khi trả xe)"
      }
    ]
  }
}
```

### 3. Xác nhận hợp đồng và tạo QR code thanh toán
**Endpoint:** `POST /api/rental/{orderId}/confirm-contract`

**Response:**
```json
{
  "success": true,
  "message": "Xác nhận hợp đồng thành công",
  "data": {
    "qrCodeUrl": "https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=EV123_EV20241220123456789",
    "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?orderId=123&contractCode=EV20241220123456789",
    "expiryDate": "2024-12-20T10:02:00",
    "contractCode": "EV20241220123456789",
    "totalAmount": 1500000
  }
}
```

**Lưu ý:** QR code sẽ hết hạn sau 2 phút.

### 4. Staff xác nhận GPLX và bàn giao xe
**Endpoint:** `POST /api/rental/{orderId}/staff-confirm`

**Request Body:**
```json
{
  "isConfirmed": true,
  "notes": "Xác nhận GPLX đúng người",
  "action": "Handover"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Xác nhận thành công",
  "data": {
    "success": true,
    "message": "Xác nhận thành công",
    "data": {
      "orderId": 123,
      "status": "Active",
      "contractId": 456
    }
  }
}
```

### 5. Staff xác nhận trả xe
**Endpoint:** `POST /api/rental/{orderId}/staff-confirm`

**Request Body:**
```json
{
  "isConfirmed": true,
  "notes": "Xe đã trả, kiểm tra tình trạng tốt",
  "action": "Return"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Xác nhận thành công",
  "data": {
    "success": true,
    "message": "Xác nhận thành công",
    "data": {
      "orderId": 123,
      "status": "Completed",
      "contractId": 456
    }
  }
}
```

## Các trạng thái Order

- **Pending**: Chờ xác nhận hợp đồng (mới tạo)
- **Confirmed**: Đã xác nhận hợp đồng, chờ thanh toán (sau khi confirm-contract)
- **Active**: Đang thuê xe (sau khi staff xác nhận bàn giao)
- **Completed**: Hoàn thành thuê xe (sau khi staff xác nhận trả xe)
- **Rejected**: Bị từ chối bởi staff
- **Cancelled**: Đã hủy

## Các trạng thái Contract

- **Pending**: Chờ xác nhận (mới tạo)
- **Confirmed**: Đã xác nhận, chờ thanh toán (sau khi confirm-contract)
- **Active**: Đang hoạt động (xe đang được thuê)
- **Completed**: Hoàn thành
- **Rejected**: Bị từ chối

## Các trạng thái LicensePlate

- **Available**: Có sẵn để thuê
- **Reserved**: Đã được đặt (khi tạo order)
- **Rented**: Đang được thuê (sau khi staff xác nhận bàn giao)
- **Maintenance**: Đang bảo trì

## Lưu ý quan trọng

1. **Đặt hộ**: Khi `isBookingForOthers = true`, chỉ cần upload ảnh GPLX của người thuê thực tế.

2. **QR Code timeout**: QR code thanh toán sẽ hết hạn sau 2 phút.

3. **Staff confirmation**: Chỉ staff và admin mới có thể xác nhận GPLX và cập nhật trạng thái xe.

4. **Contract Code**: Mỗi hợp đồng có một mã duy nhất để dễ dàng tra cứu.

5. **Fee calculation**: Các loại phí (phí vượt KM, phí điện) sẽ được tính sau khi có thông tin thực tế.

6. **GPLX verification**: Staff sẽ xác nhận GPLX bằng cách so sánh ảnh upload với hồ sơ trên hệ thống.

7. **Validation cải tiến**: 
   - Kiểm tra trạng thái Order/Contract trước khi thực hiện các action
   - Chỉ cho phép bàn giao xe khi trạng thái là "Confirmed"
   - Chỉ cho phép trả xe khi trạng thái là "Active"
   - Tự động cập nhật trạng thái biển số khi bị từ chối

8. **Luồng trạng thái**: Pending → Confirmed → Active → Completed (hoặc Rejected/Cancelled)
