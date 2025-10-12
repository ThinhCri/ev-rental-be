# Script test cuối cùng để debug lỗi 400
# Chạy: .\test-final-rental.ps1

Write-Host "=== TEST CUỐI CÙNG - DEBUG LỖI 400 ===" -ForegroundColor Green

# Cấu hình
$token = "YOUR_TOKEN_HERE"  # Thay bằng token thực tế
$baseUrl = "http://localhost:5000"

# Function để test request
function Test-RentalRequest {
    param(
        [string]$TestName,
        [string]$JsonBody,
        [string]$Token = $token
    )
    
    Write-Host "`n--- $TestName ---" -ForegroundColor Yellow
    Write-Host "Request Body:"
    Write-Host $JsonBody
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $JsonBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer $Token"}
        Write-Host "✅ THÀNH CÔNG: $($response.message)" -ForegroundColor Green
        Write-Host "Order ID: $($response.orderId)"
        Write-Host "Contract ID: $($response.contractId)"
        return $response
    } catch {
        Write-Host "❌ LỖI: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response Body:" -ForegroundColor Red
            Write-Host $responseBody
            
            # Parse error response
            try {
                $errorObj = $responseBody | ConvertFrom-Json
                if ($errorObj.fieldErrors) {
                    Write-Host "`nField Errors:" -ForegroundColor Yellow
                    $errorObj.fieldErrors.PSObject.Properties | ForEach-Object {
                        Write-Host "  $($_.Name): $($_.Value -join ', ')" -ForegroundColor Red
                    }
                }
                if ($errorObj.errors) {
                    Write-Host "`nErrors:" -ForegroundColor Yellow
                    $errorObj.errors | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
                }
            } catch {
                Write-Host "Không thể parse error response" -ForegroundColor Yellow
            }
        }
        return $null
    }
}

# Test 1: JSON hợp lệ cơ bản
$validJson = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking",
    "isBookingForOthers": false
}'
Test-RentalRequest -TestName "Test 1: JSON hợp lệ cơ bản" -JsonBody $validJson

# Test 2: Thiếu StartTime
$missingStartTime = '{
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking"
}'
Test-RentalRequest -TestName "Test 2: Thiếu StartTime" -JsonBody $missingStartTime

# Test 3: Thiếu EndTime
$missingEndTime = '{
    "startTime": "2024-12-25T08:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking"
}'
Test-RentalRequest -TestName "Test 3: Thiếu EndTime" -JsonBody $missingEndTime

# Test 4: Thiếu VehicleIds
$missingVehicleIds = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "depositAmount": 500000,
    "notes": "Test booking"
}'
Test-RentalRequest -TestName "Test 4: Thiếu VehicleIds" -JsonBody $missingVehicleIds

# Test 5: VehicleIds rỗng
$emptyVehicleIds = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [],
    "depositAmount": 500000,
    "notes": "Test booking"
}'
Test-RentalRequest -TestName "Test 5: VehicleIds rỗng" -JsonBody $emptyVehicleIds

# Test 6: DepositAmount âm
$negativeDeposit = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": -100000,
    "notes": "Test booking"
}'
Test-RentalRequest -TestName "Test 6: DepositAmount âm" -JsonBody $negativeDeposit

# Test 7: Notes quá dài
$longNotes = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "' + ("A" * 501) + '"
}'
Test-RentalRequest -TestName "Test 7: Notes quá dài" -JsonBody $longNotes

# Test 8: Đặt hộ thiếu thông tin
$bookingForOthersMissing = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking",
    "isBookingForOthers": true
}'
Test-RentalRequest -TestName "Test 8: Đặt hộ thiếu thông tin" -JsonBody $bookingForOthersMissing

# Test 9: Đặt hộ đầy đủ thông tin
$bookingForOthersComplete = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking",
    "isBookingForOthers": true,
    "renterName": "Nguyễn Văn A",
    "renterPhone": "0123456789",
    "renterLicenseImageUrl": "https://example.com/license.jpg"
}'
Test-RentalRequest -TestName "Test 9: Đặt hộ đầy đủ thông tin" -JsonBody $bookingForOthersComplete

# Test 10: Không có token
Test-RentalRequest -TestName "Test 10: Không có token" -JsonBody $validJson -Token ""

# Test 11: Token không hợp lệ
Test-RentalRequest -TestName "Test 11: Token không hợp lệ" -JsonBody $validJson -Token "invalid_token"

# Test 12: JSON không hợp lệ
$invalidJson = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking"
    // Thiếu dấu phẩy và ngoặc đóng
}'
Test-RentalRequest -TestName "Test 12: JSON không hợp lệ" -JsonBody $invalidJson

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green
Write-Host "Hãy kiểm tra các lỗi trên để xác định nguyên nhân gây lỗi 400" -ForegroundColor Cyan
Write-Host "Nếu tất cả test đều lỗi, có thể do:" -ForegroundColor Yellow
Write-Host "1. Token không hợp lệ hoặc hết hạn" -ForegroundColor Yellow
Write-Host "2. Server chưa chạy hoặc không accessible" -ForegroundColor Yellow
Write-Host "3. Database không có dữ liệu xe (vehicleIds không tồn tại)" -ForegroundColor Yellow
Write-Host "4. Có lỗi trong service layer" -ForegroundColor Yellow
