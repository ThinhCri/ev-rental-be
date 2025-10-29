# Test API booking
Write-Host "=== TEST API BOOKING ===" -ForegroundColor Green

# Test 1: Tạo đơn thuê xe
Write-Host "`n1. TEST TAO DON THUE XE..." -ForegroundColor Yellow
$createRentalBody = @{
    startTime = "2025-10-15T08:00:00"
    endTime = "2025-10-17T18:00:00"
    vehicleIds = @(1)
    depositAmount = 300000
    notes = "Test API"
    isBookingForOthers = $false
    renterName = "Nguyen Van A"
    renterPhone = "0123456789"
    renterLicenseImageUrl = "https://example.com/license.jpg"
} | ConvertTo-Json

try {
    $createResponse = Invoke-RestMethod -Uri "https://localhost:7181/api/rental" -Method POST -Body $createRentalBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer YOUR_TOKEN_HERE"} -SkipCertificateCheck
    Write-Host "SUCCESS: Tao don thue thanh cong!" -ForegroundColor Green
    Write-Host "Order ID: $($createResponse.orderId)"
    Write-Host "Contract ID: $($createResponse.contractId)"
    Write-Host "Message: $($createResponse.message)"
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== KET THUC TEST ===" -ForegroundColor Green
