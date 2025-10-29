# Script test VNPay Sandbox local
# Chạy: .\test-vnpay-local.ps1

Write-Host "=== TEST VNPAY SANDBOX LOCAL ===" -ForegroundColor Green

$baseUrl = "http://localhost:5000"

Write-Host "`n🧪 Test 1: Health Check" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/" -Method Get
    Write-Host "✅ Health Check: $response" -ForegroundColor Green
} catch {
    Write-Host "❌ Health Check failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🧪 Test 2: Swagger UI" -ForegroundColor Yellow
Write-Host "Mở browser: $baseUrl/swagger" -ForegroundColor Cyan

Write-Host "`n🧪 Test 3: Login để lấy token" -ForegroundColor Yellow
try {
    $loginData = @{
        email = "admin@example.com"
        password = "admin123"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginData -ContentType "application/json"
    $token = $response.token
    Write-Host "✅ Login thành công, token: $($token.Substring(0, 20))..." -ForegroundColor Green
} catch {
    Write-Host "❌ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Có thể cần tạo user admin trước" -ForegroundColor Yellow
}

Write-Host "`n🧪 Test 4: Tạo đơn thuê xe" -ForegroundColor Yellow
if ($token) {
    try {
        $rentalData = @{
            startTime = "2024-12-25T08:00:00"
            endTime = "2024-12-27T18:00:00"
            vehicleIds = @(1)
            depositAmount = 500000
            notes = "Test booking với VNPay Sandbox"
        } | ConvertTo-Json

        $headers = @{
            "Authorization" = $token
            "Content-Type" = "application/json"
        }

        $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method Post -Body $rentalData -Headers $headers
        Write-Host "✅ Tạo đơn thuê thành công" -ForegroundColor Green
        Write-Host "Order ID: $($response.orderId)" -ForegroundColor Cyan
        Write-Host "Contract ID: $($response.contractId)" -ForegroundColor Cyan
    } catch {
        Write-Host "❌ Tạo đơn thuê failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n🧪 Test 5: Xác nhận hợp đồng" -ForegroundColor Yellow
if ($token -and $response.contractId) {
    try {
        $confirmData = @{
            contractId = $response.contractId
        } | ConvertTo-Json

        $confirmResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/confirm-contract" -Method Post -Body $confirmData -Headers $headers
        Write-Host "✅ Xác nhận hợp đồng thành công" -ForegroundColor Green
        Write-Host "Payment URL: $($confirmResponse.paymentUrl)" -ForegroundColor Cyan
    } catch {
        Write-Host "❌ Xác nhận hợp đồng failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n🧪 Test 6: VNPay Sandbox" -ForegroundColor Yellow
Write-Host "1. Mở Payment URL trong browser" -ForegroundColor Cyan
Write-Host "2. Sẽ redirect đến VNPay Sandbox" -ForegroundColor Cyan
Write-Host "3. Sử dụng thẻ test để thanh toán" -ForegroundColor Cyan
Write-Host "4. Sau khi thanh toán, sẽ redirect về localhost:5000" -ForegroundColor Cyan

Write-Host "`n💡 Thẻ test VNPay Sandbox:" -ForegroundColor Yellow
Write-Host "Số thẻ: 9704198526191432198" -ForegroundColor Cyan
Write-Host "Tên chủ thẻ: NGUYEN VAN A" -ForegroundColor Cyan
Write-Host "Ngày hết hạn: 07/15" -ForegroundColor Cyan
Write-Host "CVV: 123" -ForegroundColor Cyan

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Green
Write-Host "🎉 Test VNPay Sandbox local hoàn thành!" -ForegroundColor Green
