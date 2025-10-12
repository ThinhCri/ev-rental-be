# Script debug lỗi 400 khi tạo đơn thuê
# Chạy: .\debug-rental-400.ps1

Write-Host "=== DEBUG LỖI 400 KHI TẠO ĐƠN THUÊ ===" -ForegroundColor Green

# Cấu hình
$token = "YOUR_TOKEN_HERE"  # Thay bằng token thực tế
$baseUrl = "http://localhost:5000"

# Function để test request
function Test-RentalRequest {
    param(
        [string]$TestName,
        [object]$RequestBody,
        [string]$Token = $token
    )
    
    Write-Host "`n--- $TestName ---" -ForegroundColor Yellow
    Write-Host "Request Body:"
    $jsonBody = $RequestBody | ConvertTo-Json -Depth 3
    Write-Host $jsonBody
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $jsonBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer $Token"}
        Write-Host "✅ THÀNH CÔNG: $($response.message)" -ForegroundColor Green
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
            } catch {
                Write-Host "Không thể parse error response" -ForegroundColor Yellow
            }
        }
        return $null
    }
}

# Test 1: Request hợp lệ cơ bản
$validRequest = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
    isBookingForOthers = $false
}
Test-RentalRequest -TestName "Test 1: Request hợp lệ" -RequestBody $validRequest

# Test 2: Thiếu StartTime
$missingStartTime = @{
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
}
Test-RentalRequest -TestName "Test 2: Thiếu StartTime" -RequestBody $missingStartTime

# Test 3: Thiếu EndTime
$missingEndTime = @{
    startTime = "2024-12-25T08:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
}
Test-RentalRequest -TestName "Test 3: Thiếu EndTime" -RequestBody $missingEndTime

# Test 4: Thiếu VehicleIds
$missingVehicleIds = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    depositAmount = 500000
    notes = "Test booking"
}
Test-RentalRequest -TestName "Test 4: Thiếu VehicleIds" -RequestBody $missingVehicleIds

# Test 5: VehicleIds rỗng
$emptyVehicleIds = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @()
    depositAmount = 500000
    notes = "Test booking"
}
Test-RentalRequest -TestName "Test 5: VehicleIds rỗng" -RequestBody $emptyVehicleIds

# Test 6: DepositAmount âm
$negativeDeposit = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = -100000
    notes = "Test booking"
}
Test-RentalRequest -TestName "Test 6: DepositAmount âm" -RequestBody $negativeDeposit

# Test 7: Notes quá dài
$longNotes = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "A" * 501  # 501 ký tự
}
Test-RentalRequest -TestName "Test 7: Notes quá dài" -RequestBody $longNotes

# Test 8: Đặt hộ nhưng thiếu thông tin
$bookingForOthersMissing = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
    isBookingForOthers = $true
    # Thiếu RenterName, RenterPhone, RenterLicenseImageUrl
}
Test-RentalRequest -TestName "Test 8: Đặt hộ thiếu thông tin" -RequestBody $bookingForOthersMissing

# Test 9: Không có token
Test-RentalRequest -TestName "Test 9: Không có token" -RequestBody $validRequest -Token ""

# Test 10: Token không hợp lệ
Test-RentalRequest -TestName "Test 10: Token không hợp lệ" -RequestBody $validRequest -Token "invalid_token"

Write-Host "`n=== KẾT THÚC DEBUG ===" -ForegroundColor Green
Write-Host "Hãy kiểm tra các lỗi trên để xác định nguyên nhân gây lỗi 400" -ForegroundColor Cyan
