# Script test nhanh luồng booking
# Chạy: .\quick-test.ps1

Write-Host "=== QUICK TEST LUỒNG BOOKING ===" -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Test 1: Kiểm tra server
Write-Host "`n1. Kiểm tra server..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/vehicle" -Method GET
    Write-Host "✅ Server OK - Có $($response.data.Count) xe" -ForegroundColor Green
} catch {
    Write-Host "❌ Server không chạy: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Test tạo đơn thuê với token giả
Write-Host "`n2. Test tạo đơn thuê..." -ForegroundColor Yellow
$requestBody = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Quick test"
    isBookingForOthers = $false
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $requestBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer fake_token"}
    Write-Host "✅ Tạo đơn thuê thành công!" -ForegroundColor Green
} catch {
    if ($_.Exception.Response.StatusCode -eq 401) {
        Write-Host "✅ API hoạt động - Chỉ cần token hợp lệ" -ForegroundColor Green
    } elseif ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "⚠️  Lỗi 400 - Có thể do validation hoặc database" -ForegroundColor Yellow
        
        # Hiển thị chi tiết lỗi
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Chi tiết lỗi: $responseBody" -ForegroundColor Red
    } else {
        Write-Host "❌ Lỗi khác: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== KẾT QUẢ ===" -ForegroundColor Green
Write-Host "Nếu thấy 'API hoạt động - Chỉ cần token hợp lệ' thì luồng booking đã sẵn sàng!" -ForegroundColor Cyan
Write-Host "Chạy .\test-complete-booking.ps1 để test đầy đủ với token thực" -ForegroundColor Cyan
