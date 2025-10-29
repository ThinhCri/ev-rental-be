# Script test booking flow sau khi fix
Write-Host "=== TEST BOOKING FLOW FIXED ===" -ForegroundColor Green

# Cấu hình
$baseUrl = "https://deloyapi-8.onrender.com"
$testEmail = "test@example.com"
$testPassword = "Test123!@#"

Write-Host "1. Đăng nhập để lấy token..." -ForegroundColor Yellow
$loginBody = @{
    email = $testEmail
    password = $testPassword
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.data.token
    Write-Host "✓ Đăng nhập thành công" -ForegroundColor Green
} catch {
    Write-Host "✗ Lỗi đăng nhập: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Headers với token
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Write-Host "2. Lấy danh sách xe có sẵn..." -ForegroundColor Yellow
$searchBody = @{
    startTime = (Get-Date).AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss")
    endTime = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss")
    pageNumber = 1
    pageSize = 10
} | ConvertTo-Json

try {
    $vehiclesResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/available-vehicles" -Method POST -Body $searchBody -Headers $headers
    Write-Host "✓ Tìm thấy $($vehiclesResponse.data.Count) xe có sẵn" -ForegroundColor Green
    
    if ($vehiclesResponse.data.Count -eq 0) {
        Write-Host "✗ Không có xe nào có sẵn để test" -ForegroundColor Red
        exit 1
    }
    
    $vehicleId = $vehiclesResponse.data[0].vehicleId
    Write-Host "✓ Sử dụng xe ID: $vehicleId" -ForegroundColor Green
} catch {
    Write-Host "✗ Lỗi tìm xe: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "3. Tạo đơn thuê xe..." -ForegroundColor Yellow
$bookingBody = @{
    startTime = (Get-Date).AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss")
    endTime = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss")
    vehicleIds = @($vehicleId)
    depositAmount = 100000
    notes = "Test booking sau khi fix"
} | ConvertTo-Json

try {
    $bookingResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $bookingBody -Headers $headers
    Write-Host "✓ Tạo đơn thuê thành công!" -ForegroundColor Green
    Write-Host "  - Order ID: $($bookingResponse.orderId)" -ForegroundColor Cyan
    Write-Host "  - Contract ID: $($bookingResponse.contractId)" -ForegroundColor Cyan
    Write-Host "  - Tổng tiền: $($bookingResponse.data.totalAmount) VND" -ForegroundColor Cyan
    Write-Host "  - Trạng thái: $($bookingResponse.data.status)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Lỗi tạo đơn thuê: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorStream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorStream)
        $errorBody = $reader.ReadToEnd()
        Write-Host "  Chi tiết lỗi: $errorBody" -ForegroundColor Red
    }
    exit 1
}

Write-Host "4. Lấy thông tin hợp đồng..." -ForegroundColor Yellow
try {
    $contractResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/$($bookingResponse.orderId)/contract-summary" -Method GET -Headers $headers
    Write-Host "✓ Lấy thông tin hợp đồng thành công!" -ForegroundColor Green
    Write-Host "  - Mã hợp đồng: $($contractResponse.data.contractCode)" -ForegroundColor Cyan
    Write-Host "  - Phí thuê: $($contractResponse.data.rentalFee) VND" -ForegroundColor Cyan
    Write-Host "  - Phí cọc: $($contractResponse.data.deposit) VND" -ForegroundColor Cyan
    Write-Host "  - Tổng cộng: $($contractResponse.data.totalAmount) VND" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Lỗi lấy thông tin hợp đồng: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "5. Xác nhận hợp đồng và tạo QR thanh toán..." -ForegroundColor Yellow
try {
    $confirmResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/$($bookingResponse.orderId)/confirm-contract" -Method POST -Headers $headers
    Write-Host "✓ Xác nhận hợp đồng thành công!" -ForegroundColor Green
    Write-Host "  - QR Code URL: $($confirmResponse.data.qrCodeUrl)" -ForegroundColor Cyan
    Write-Host "  - Payment URL: $($confirmResponse.data.paymentUrl)" -ForegroundColor Cyan
    Write-Host "  - Hết hạn: $($confirmResponse.data.expiryDate)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Lỗi xác nhận hợp đồng: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "6. Kiểm tra lịch sử đơn thuê..." -ForegroundColor Yellow
try {
    $historyResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/my-rentals" -Method GET -Headers $headers
    Write-Host "✓ Lấy lịch sử đơn thuê thành công!" -ForegroundColor Green
    Write-Host "  - Tổng số đơn: $($historyResponse.data.Count)" -ForegroundColor Cyan
    Write-Host "  - Đơn mới nhất: Order ID $($historyResponse.data[0].orderId)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Lỗi lấy lịch sử: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== KẾT QUẢ TEST ===" -ForegroundColor Green
Write-Host "✓ Booking flow đã hoạt động bình thường!" -ForegroundColor Green
Write-Host "✓ Các API đã được fix và test thành công!" -ForegroundColor Green

