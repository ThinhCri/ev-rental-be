# Script test luồng booking xe mới
# Chạy: .\test-booking-flow.ps1

Write-Host "=== TEST LUỒNG BOOKING XE MỚI ===" -ForegroundColor Green

# 1. Tạo đơn thuê xe
Write-Host "`n1. Tạo đơn thuê xe..." -ForegroundColor Yellow
$createRentalBody = @{
    startTime = "2024-12-20T08:00:00"
    endTime = "2024-12-22T18:00:00"
    vehicleIds = @(1, 2)
    depositAmount = 500000
    notes = "Test booking flow"
    isBookingForOthers = $false
    renterLicenseImageUrl = "https://example.com/license.jpg"
} | ConvertTo-Json

$createResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/rental" -Method POST -Body $createRentalBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer YOUR_TOKEN_HERE"}
Write-Host "Tạo đơn thuê: $($createResponse.message)"
Write-Host "Order ID: $($createResponse.orderId)"
Write-Host "Contract ID: $($createResponse.contractId)"

$orderId = $createResponse.orderId

# 2. Lấy thông tin bảng hợp đồng
Write-Host "`n2. Lấy thông tin bảng hợp đồng..." -ForegroundColor Yellow
$contractResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/rental/$orderId/contract-summary" -Method GET -Headers @{"Authorization" = "Bearer YOUR_TOKEN_HERE"}
Write-Host "Contract Code: $($contractResponse.data.contractCode)"
Write-Host "Tổng tiền: $($contractResponse.data.totalAmount)"
Write-Host "Trạng thái: $($contractResponse.data.status)"

# 3. Xác nhận hợp đồng và tạo QR code
Write-Host "`n3. Xác nhận hợp đồng và tạo QR code..." -ForegroundColor Yellow
$confirmResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/rental/$orderId/confirm-contract" -Method POST -Headers @{"Authorization" = "Bearer YOUR_TOKEN_HERE"}
Write-Host "QR Code URL: $($confirmResponse.data.qrCodeUrl)"
Write-Host "Payment URL: $($confirmResponse.data.paymentUrl)"
Write-Host "Hết hạn: $($confirmResponse.data.expiryDate)"

# 4. Staff xác nhận GPLX và bàn giao xe
Write-Host "`n4. Staff xác nhận GPLX và bàn giao xe..." -ForegroundColor Yellow
$staffConfirmBody = @{
    isConfirmed = $true
    notes = "Xác nhận GPLX đúng người"
    action = "Handover"
} | ConvertTo-Json

$staffResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/rental/$orderId/staff-confirm" -Method POST -Body $staffConfirmBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer STAFF_TOKEN_HERE"}
Write-Host "Staff xác nhận: $($staffResponse.message)"
Write-Host "Trạng thái mới: $($staffResponse.data.data.status)"

# 5. Staff xác nhận trả xe
Write-Host "`n5. Staff xác nhận trả xe..." -ForegroundColor Yellow
$returnBody = @{
    isConfirmed = $true
    notes = "Xe đã trả, kiểm tra tình trạng tốt"
    action = "Return"
} | ConvertTo-Json

$returnResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/rental/$orderId/staff-confirm" -Method POST -Body $returnBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer STAFF_TOKEN_HERE"}
Write-Host "Trả xe: $($returnResponse.message)"
Write-Host "Trạng thái cuối: $($returnResponse.data.data.status)"

Write-Host "`n=== HOÀN THÀNH TEST LUỒNG BOOKING ===" -ForegroundColor Green
