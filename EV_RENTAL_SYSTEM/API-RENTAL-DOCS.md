# API Documentation - Rental Endpoints

## 📋 POST `/api/rental` - Tạo đơn thuê xe

### Endpoint
```
POST /api/rental
Content-Type: application/json
Authorization: Bearer {token}
```

---

## 🔹 Case 1: Tự đặt xe (isBookingForOthers = false)

### Request Body

```json
{
  "startTime": "2025-10-15T08:00:00",
  "endTime": "2025-10-17T18:00:00",
  "vehicleId": 1,
  "depositAmount": 300000,
  "notes": "Test self",
  "isBookingForOthers": false
}
```

### Required Fields
- ✅ `startTime` (DateTime với time): Thời gian bắt đầu
- ✅ `endTime` (DateTime với time): Thời gian kết thúc
- ✅ `vehicleId` (int): ID của xe cần thuê (chỉ 1 xe)
- ❌ `depositAmount` (decimal): Phí cọc (optional, default = 20% rental fee)
- ❌ `notes` (string): Ghi chú (optional)
- ❌ `isBookingForOthers` (bool): Mặc định = false

### Validation
- User phải có GPLX trong hệ thống
- `startTime` < `endTime`
- `startTime` >= hiện tại (cho phép -5 phút)
- Xe phải có sẵn trong khoảng thời gian đó
- Phải có biển số Available

### Response
```json
{
  "success": true,
  "message": "Tạo đơn thuê thành công",
  "data": {
    "orderId": 1,
    "orderDate": "2025-10-14T10:00:00",
    "userId": 123,
    "userName": "Nguyễn Văn A",
    "userEmail": "user@example.com",
    "status": "Pending",
    "vehicles": [
      {
        "vehicleId": 1,
        "model": "VinFast VF8",
        "brandName": "VinFast"
      }
    ]
  },
  "orderId": 1,
  "contractId": 100
}
```

---

## 🔹 Case 2: Đặt hộ (isBookingForOthers = true)

### Request Body

```json
{
  "startTime": "2025-10-15T08:00:00",
  "endTime": "2025-10-17T18:00:00",
  "vehicleId": 1,
  "depositAmount": 300000,
  "notes": "Test others",
  "isBookingForOthers": true,
  "renterName": "Nguyen Van A",
  "renterPhone": "0123456789",
  "renterLicenseImageUrl": "https://example.com/license.jpg"
}
```

### Required Fields (khi isBookingForOthers = true)
- ✅ `startTime` (DateTime): Thời gian bắt đầu
- ✅ `endTime` (DateTime): Thời gian kết thúc
- ✅ `vehicleId` (int): ID của xe
- ✅ `isBookingForOthers` (bool): **true**
- ✅ `renterName` (string): **Tên người thuê (bắt buộc khi đặt hộ)**
- ✅ `renterPhone` (string): **SĐT người thuê (bắt buộc khi đặt hộ)**
- ✅ `renterLicenseImageUrl` (string): **URL ảnh GPLX (bắt buộc khi đặt hộ)**
- ❌ `depositAmount` (decimal): Phí cọc (optional)
- ❌ `notes` (string): Ghi chú (optional)

### Validation
- **KHÔNG** kiểm tra GPLX của người đặt hộ
- Phải có đầy đủ: `renterName`, `renterPhone`, `renterLicenseImageUrl`
- `startTime` < `endTime`
- Xe phải có sẵn

### Response
```json
{
  "success": true,
  "message": "Tạo đơn thuê thành công",
  "data": {
    "orderId": 2,
    "orderDate": "2025-10-14T10:30:00",
    "userId": 123,
    "userName": "Nguyen Van A",
    "userEmail": "0123456789@temp.com",
    "status": "Pending",
    "vehicles": [...]
  },
  "orderId": 2,
  "contractId": 101
}
```

---

## 📝 Field Descriptions

### DateTime Format
```
"2025-10-15T08:00:00"  ✅ Có time
"2025-10-15"           ❌ Chỉ có date (sẽ bị lỗi validation)
```

### vehicleId
```json
"vehicleId": 1          ✅ Single integer
"vehicleIds": [1]       ❌ Không còn dùng mảng
```

### isBookingForOthers
```json
false  → Tự đặt (mặc định)
        - Cần có GPLX
        - Không cần renterName, renterPhone, renterLicenseImageUrl
        
true   → Đặt hộ
        - KHÔNG cần GPLX của người đặt
        - BẮT BUỘC: renterName, renterPhone, renterLicenseImageUrl
```

---

## 🎯 Flow Logic

### Tự đặt (isBookingForOthers = false):
```
User A login → Check GPLX của A → Tạo Order (UserId = A)
```

### Đặt hộ (isBookingForOthers = true):
```
User A login → Không check GPLX → Tạo Order (UserId = A, ghi chú đặt hộ cho B)
```

---

## ⚠️ Common Errors

### 1. Thiếu thời gian trong DateTime
```json
❌ "startTime": "2025-10-15"
✅ "startTime": "2025-10-15T08:00:00"
```

### 2. Dùng mảng cho vehicleId
```json
❌ "vehicleIds": [1, 2]
✅ "vehicleId": 1
```

### 3. Đặt hộ thiếu thông tin
```json
{
  "isBookingForOthers": true,
  "renterName": "A"      ← Có
  // ❌ Thiếu renterPhone và renterLicenseImageUrl
}

→ Error: "Số điện thoại người thuê là bắt buộc khi đặt hộ"
```

### 4. Tự đặt nhưng không có GPLX
```json
{
  "isBookingForOthers": false
}

→ Error: "Tài khoản của bạn chưa có GPLX. Vui lòng cập nhật GPLX trước khi thuê xe."
```

---

## 🔄 Status Flow

```
Tạo đơn → Status "Pending"
    ↓
Thanh toán → LicensePlate status "Available" → "Reserved"
    ↓
Staff bàn giao → Order "Active", LicensePlate "Rented"
    ↓
Trả xe → Order "Completed", LicensePlate "Available"
```

---

## 📊 Example Requests (Postman/Swagger)

### Self Booking
```bash
curl -X POST "http://localhost:5228/api/rental" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2025-10-15T08:00:00",
    "endTime": "2025-10-17T18:00:00",
    "vehicleId": 1,
    "depositAmount": 300000,
    "notes": "Self booking",
    "isBookingForOthers": false
  }'
```

### Booking For Others
```bash
curl -X POST "http://localhost:5228/api/rental" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "startTime": "2025-10-15T08:00:00",
    "endTime": "2025-10-17T18:00:00",
    "vehicleId": 1,
    "depositAmount": 300000,
    "notes": "Booking for friend",
    "isBookingForOthers": true,
    "renterName": "Nguyen Van B",
    "renterPhone": "0987654321",
    "renterLicenseImageUrl": "https://cloudinary.com/license.jpg"
  }'
```

---

## 🎨 Frontend Form Design

### HTML Structure

```html
<!-- Basic Fields (Always show) -->
<input type="datetime-local" name="startTime" required />
<input type="datetime-local" name="endTime" required />
<input type="number" name="vehicleId" required />
<input type="number" name="depositAmount" />
<textarea name="notes"></textarea>

<!-- Toggle Checkbox -->
<input type="checkbox" id="isBookingForOthers" name="isBookingForOthers" />
<label for="isBookingForOthers">Đặt hộ</label>

<!-- Conditional Fields (Show only when checkbox is checked) -->
<div id="bookingForOthersFields" style="display: none;">
  <input type="text" name="renterName" placeholder="Tên người thuê" />
  <input type="tel" name="renterPhone" placeholder="Số điện thoại" />
  <input type="url" name="renterLicenseImageUrl" placeholder="URL ảnh GPLX" />
</div>

<script>
  document.getElementById('isBookingForOthers').addEventListener('change', (e) => {
    document.getElementById('bookingForOthersFields').style.display = 
      e.target.checked ? 'block' : 'none';
  });
</script>
```

### JavaScript/React Example

```javascript
const [isBookingForOthers, setIsBookingForOthers] = useState(false);

<Checkbox 
  checked={isBookingForOthers}
  onChange={(e) => setIsBookingForOthers(e.target.checked)}
>
  Đặt hộ
</Checkbox>

{isBookingForOthers && (
  <>
    <Input name="renterName" placeholder="Tên người thuê" required />
    <Input name="renterPhone" placeholder="SĐT" required />
    <Input name="renterLicenseImageUrl" placeholder="URL ảnh GPLX" required />
  </>
)}
```

---

## ✅ Summary

| Field | Type | Self Booking | Booking For Others |
|-------|------|--------------|-------------------|
| `startTime` | DateTime | ✅ Required | ✅ Required |
| `endTime` | DateTime | ✅ Required | ✅ Required |
| `vehicleId` | int | ✅ Required | ✅ Required |
| `depositAmount` | decimal | ❌ Optional | ❌ Optional |
| `notes` | string | ❌ Optional | ❌ Optional |
| `isBookingForOthers` | bool | false (default) | **true** |
| `renterName` | string | ❌ Hide | ✅ **Required** |
| `renterPhone` | string | ❌ Hide | ✅ **Required** |
| `renterLicenseImageUrl` | string | ❌ Hide | ✅ **Required** |

**GPLX Check**: Required for Self | Not required for Booking For Others

