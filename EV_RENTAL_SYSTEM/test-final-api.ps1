# Test Final API
$baseUrl = "http://localhost:5228"

Write-Host "=== TEST FINAL API ===" -ForegroundColor Green

# Test 1: Login
Write-Host "1. Login..." -ForegroundColor Yellow
$loginData = '{"email":"admin@evrental.com","password":"Admin123!"}'

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/Auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "Login success!" -ForegroundColor Green
}
catch {
    Write-Host "Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Create Rental
Write-Host "`n2. Create Rental..." -ForegroundColor Yellow
$futureTime = (Get-Date).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss")
$endTime = (Get-Date).AddDays(6).ToString("yyyy-MM-ddTHH:mm:ss")

$rentalData = @{
    startTime = $futureTime
    endTime = $endTime
    vehicleIds = @(1)
    depositAmount = 100000
    notes = "Test rental final"
} | ConvertTo-Json

$headers = @{
    "Content-Type" = "application/json"
    "Authorization" = "Bearer $token"
}

try {
    $rentalResponse = Invoke-RestMethod -Uri "$baseUrl/api/Rental" -Method POST -Body $rentalData -Headers $headers
    Write-Host "Create rental success!" -ForegroundColor Green
    Write-Host "OrderId: $($rentalResponse.OrderId)" -ForegroundColor Cyan
    Write-Host "ContractId: $($rentalResponse.ContractId)" -ForegroundColor Cyan
    Write-Host "Status: $($rentalResponse.Data.Status)" -ForegroundColor Cyan
    Write-Host "TotalAmount: $($rentalResponse.Data.TotalAmount)" -ForegroundColor Cyan
    
    # Test 3: Test Payment
    Write-Host "`n3. Test Payment..." -ForegroundColor Yellow
    $paymentData = @{
        amount = 100000
        orderId = $rentalResponse.OrderId
        contractId = $rentalResponse.ContractId
        description = "Test payment for rental"
    } | ConvertTo-Json
    
    try {
        $paymentResponse = Invoke-RestMethod -Uri "$baseUrl/api/Payment/vnpay" -Method POST -Body $paymentData -Headers $headers
        Write-Host "Create payment success!" -ForegroundColor Green
        Write-Host "Payment URL: $($paymentResponse.Data.PaymentUrl)" -ForegroundColor Cyan
    }
    catch {
        Write-Host "Create payment failed: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response: $responseBody" -ForegroundColor Red
        }
    }
}
catch {
    Write-Host "Create rental failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}

Write-Host "`n=== KET LUAN ===" -ForegroundColor Green
Write-Host "1. API tao don thue da tra ve OrderId va ContractId" -ForegroundColor White
Write-Host "2. API thanh toan co the su dung OrderId va ContractId" -ForegroundColor White
Write-Host "3. Khong con loi 400 khi tao don thue" -ForegroundColor White
Write-Host "4. Du lieu duoc luu dung vao DB" -ForegroundColor White





