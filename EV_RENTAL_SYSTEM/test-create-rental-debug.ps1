# Script test tạo đơn thuê với các trường hợp khác nhau để debug lỗi 400
# Chạy: .\test-create-rental-debug.ps1

Write-Host "=== TEST TẠO ĐƠN THUÊ XE - DEBUG LỖI 400 ===" -ForegroundColor Green

# Lấy token (thay YOUR_TOKEN_HERE bằng token thực tế)
$token = "YOUR_TOKEN_HERE"
$baseUrl = "http://localhost:5000"

# Test case 1: JSON hợp lệ cơ bản
Write-Host "`n1. Test JSON hợp lệ cơ bản..." -ForegroundColor Yellow
$validBody = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
    isBookingForOthers = $false
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $validBody

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $validBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
    Write-Host "Order ID: $($response.orderId)"
    Write-Host "Contract ID: $($response.contractId)"
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 2: Thiếu vehicleIds
Write-Host "`n2. Test thiếu vehicleIds..." -ForegroundColor Yellow
$missingVehicleIds = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    depositAmount = 500000
    notes = "Test booking"
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $missingVehicleIds

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $missingVehicleIds -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 3: vehicleIds rỗng
Write-Host "`n3. Test vehicleIds rỗng..." -ForegroundColor Yellow
$emptyVehicleIds = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @()
    depositAmount = 500000
    notes = "Test booking"
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $emptyVehicleIds

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $emptyVehicleIds -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 4: Thời gian không hợp lệ
Write-Host "`n4. Test thời gian không hợp lệ..." -ForegroundColor Yellow
$invalidTime = @{
    startTime = "2024-12-27T18:00:00"
    endTime = "2024-12-25T08:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $invalidTime

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $invalidTime -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 5: Thời gian trong quá khứ
Write-Host "`n5. Test thời gian trong quá khứ..." -ForegroundColor Yellow
$pastTime = @{
    startTime = "2023-12-25T08:00:00"
    endTime = "2023-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $pastTime

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $pastTime -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 6: JSON không hợp lệ
Write-Host "`n6. Test JSON không hợp lệ..." -ForegroundColor Yellow
$invalidJson = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking"
    // Thiếu dấu phẩy và ngoặc đóng
}'

Write-Host "Request Body:"
Write-Host $invalidJson

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $invalidJson -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

# Test case 7: Không có token
Write-Host "`n7. Test không có token..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $validBody -ContentType "application/json"
    Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green
