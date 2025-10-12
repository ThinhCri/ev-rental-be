# Script test luồng booking cuối cùng
# Chạy: .\test-booking-final.ps1

Write-Host "=== TEST LUỒNG BOOKING CUỐI CÙNG ===" -ForegroundColor Green

# Cấu hình
$baseUrl = "http://localhost:5000"

# Bước 1: Kiểm tra server có chạy không
Write-Host "`n1. Kiểm tra server..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/vehicle" -Method GET
    Write-Host "✅ Server đang chạy" -ForegroundColor Green
    Write-Host "Số lượng xe: $($response.data.Count)"
} catch {
    Write-Host "❌ Server không chạy hoặc không accessible" -ForegroundColor Red
    Write-Host "Lỗi: $($_.Exception.Message)"
    exit 1
}

# Bước 2: Lấy token (cần thay bằng token thực tế)
Write-Host "`n2. Lấy token..." -ForegroundColor Yellow
$token = "YOUR_TOKEN_HERE"  # Thay bằng token thực tế từ login

if ($token -eq "YOUR_TOKEN_HERE") {
    Write-Host "⚠️  Cần thay token thực tế vào biến `$token" -ForegroundColor Yellow
    Write-Host "Token có thể lấy từ API login hoặc từ database" -ForegroundColor Cyan
}

# Bước 3: Test tạo đơn thuê
Write-Host "`n3. Test tạo đơn thuê..." -ForegroundColor Yellow
$requestBody = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking final"
    isBookingForOthers = $false
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $requestBody

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $requestBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    
    Write-Host "✅ Tạo đơn thuê thành công!" -ForegroundColor Green
    Write-Host "Message: $($response.message)"
    Write-Host "Order ID: $($response.orderId)"
    Write-Host "Contract ID: $($response.contractId)"
    
    $orderId = $response.orderId
    
    # Bước 4: Lấy thông tin hợp đồng
    Write-Host "`n4. Lấy thông tin hợp đồng..." -ForegroundColor Yellow
    try {
        $contractResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/$orderId/contract-summary" -Method GET -Headers @{"Authorization" = "Bearer $token"}
        
        Write-Host "✅ Lấy thông tin hợp đồng thành công!" -ForegroundColor Green
        Write-Host "Contract Code: $($contractResponse.data.contractCode)"
        Write-Host "Tổng tiền: $($contractResponse.data.totalAmount)"
        Write-Host "Trạng thái: $($contractResponse.data.status)"
        
    } catch {
        Write-Host "❌ Lỗi lấy thông tin hợp đồng: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Bước 5: Xác nhận hợp đồng
    Write-Host "`n5. Xác nhận hợp đồng..." -ForegroundColor Yellow
    try {
        $confirmResponse = Invoke-RestMethod -Uri "$baseUrl/api/rental/$orderId/confirm-contract" -Method POST -Headers @{"Authorization" = "Bearer $token"}
        
        Write-Host "✅ Xác nhận hợp đồng thành công!" -ForegroundColor Green
        Write-Host "QR Code URL: $($confirmResponse.data.qrCodeUrl)"
        Write-Host "Payment URL: $($confirmResponse.data.paymentUrl)"
        Write-Host "Hết hạn: $($confirmResponse.data.expiryDate)"
        
    } catch {
        Write-Host "❌ Lỗi xác nhận hợp đồng: $($_.Exception.Message)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "❌ Lỗi tạo đơn thuê: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:"
        Write-Host $responseBody -ForegroundColor Red
        
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
}

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green
Write-Host "Nếu tất cả bước đều thành công, luồng booking đã hoạt động chắc chắn!" -ForegroundColor Cyan
