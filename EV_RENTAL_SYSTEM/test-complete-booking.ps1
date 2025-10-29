# Script test luồng booking hoàn chỉnh
# Chạy: .\test-complete-booking.ps1

Write-Host "=== TEST LUỒNG BOOKING HOÀN CHỈNH ===" -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Function để lấy token
function Get-Token {
    Write-Host "Đang lấy token..." -ForegroundColor Yellow
    
    # Thử các tài khoản khác nhau
    $accounts = @(
        @{ email = "admin@example.com"; password = "admin123" },
        @{ email = "staff@example.com"; password = "staff123" },
        @{ email = "user@example.com"; password = "user123" }
    )
    
    foreach ($account in $accounts) {
        try {
            $loginData = @{
                email = $account.email
                password = $account.password
            } | ConvertTo-Json
            
            Write-Host "Thử đăng nhập với: $($account.email)" -ForegroundColor Cyan
            
            $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
            
            Write-Host "✅ Đăng nhập thành công với: $($account.email)" -ForegroundColor Green
            return $response.data.token
            
        } catch {
            Write-Host "❌ Không thể đăng nhập với: $($account.email)" -ForegroundColor Red
            continue
        }
    }
    
    Write-Host "❌ Không thể đăng nhập với bất kỳ tài khoản nào" -ForegroundColor Red
    return $null
}

# Function để test request
function Test-Request {
    param(
        [string]$TestName,
        [string]$Url,
        [string]$Method = "GET",
        [string]$Body = $null,
        [string]$Token
    )
    
    Write-Host "`n--- $TestName ---" -ForegroundColor Yellow
    
    try {
        $headers = @{"Authorization" = "Bearer $Token"}
        
        if ($Body) {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Body $Body -ContentType "application/json" -Headers $headers
        } else {
            $response = Invoke-RestMethod -Uri $Url -Method $Method -Headers $headers
        }
        
        Write-Host "✅ Thành công: $($response.message)" -ForegroundColor Green
        return $response
        
    } catch {
        Write-Host "❌ Lỗi: $($_.Exception.Message)" -ForegroundColor Red
        
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response: $responseBody" -ForegroundColor Red
        }
        
        return $null
    }
}

# Bước 1: Kiểm tra server
Write-Host "`n1. Kiểm tra server..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/vehicle" -Method GET
    Write-Host "✅ Server đang chạy - Số lượng xe: $($response.data.Count)" -ForegroundColor Green
} catch {
    Write-Host "❌ Server không chạy" -ForegroundColor Red
    exit 1
}

# Bước 2: Lấy token
$token = Get-Token
if (-not $token) {
    Write-Host "❌ Không thể lấy token, dừng test" -ForegroundColor Red
    exit 1
}

# Bước 3: Test tạo đơn thuê
$requestBody = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking complete"
    isBookingForOthers = $false
} | ConvertTo-Json -Depth 3

$createResponse = Test-Request -TestName "Tạo đơn thuê" -Url "$baseUrl/api/rental" -Method "POST" -Body $requestBody -Token $token

if (-not $createResponse) {
    Write-Host "❌ Không thể tạo đơn thuê, dừng test" -ForegroundColor Red
    exit 1
}

$orderId = $createResponse.orderId
Write-Host "Order ID: $orderId" -ForegroundColor Cyan

# Bước 4: Lấy thông tin hợp đồng
$contractResponse = Test-Request -TestName "Lấy thông tin hợp đồng" -Url "$baseUrl/api/rental/$orderId/contract-summary" -Token $token

if ($contractResponse) {
    Write-Host "Contract Code: $($contractResponse.data.contractCode)" -ForegroundColor Cyan
    Write-Host "Tổng tiền: $($contractResponse.data.totalAmount)" -ForegroundColor Cyan
}

# Bước 5: Xác nhận hợp đồng
$confirmResponse = Test-Request -TestName "Xác nhận hợp đồng" -Url "$baseUrl/api/rental/$orderId/confirm-contract" -Method "POST" -Token $token

if ($confirmResponse) {
    Write-Host "QR Code URL: $($confirmResponse.data.qrCodeUrl)" -ForegroundColor Cyan
    Write-Host "Payment URL: $($confirmResponse.data.paymentUrl)" -ForegroundColor Cyan
}

# Bước 6: Test staff confirmation (nếu có quyền staff)
$staffConfirmBody = @{
    isConfirmed = $true
    notes = "Test staff confirmation"
    action = "Handover"
} | ConvertTo-Json

$staffResponse = Test-Request -TestName "Staff xác nhận bàn giao" -Url "$baseUrl/api/rental/$orderId/staff-confirm" -Method "POST" -Body $staffConfirmBody -Token $token

if ($staffResponse) {
    Write-Host "✅ Staff confirmation thành công!" -ForegroundColor Green
}

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green

if ($createResponse -and $contractResponse -and $confirmResponse) {
    Write-Host "🎉 LUỒNG BOOKING HOẠT ĐỘNG CHẮC CHẮN!" -ForegroundColor Green
    Write-Host "✅ Tạo đơn thuê: OK" -ForegroundColor Green
    Write-Host "✅ Lấy thông tin hợp đồng: OK" -ForegroundColor Green
    Write-Host "✅ Xác nhận hợp đồng: OK" -ForegroundColor Green
    if ($staffResponse) {
        Write-Host "✅ Staff confirmation: OK" -ForegroundColor Green
    }
} else {
    Write-Host "⚠️  Có một số bước chưa hoạt động hoàn hảo" -ForegroundColor Yellow
}
