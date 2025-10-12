# Script test tạo đơn thuê đơn giản
# Chạy: .\test-simple-rental.ps1

Write-Host "=== TEST TẠO ĐƠN THUÊ XE ĐƠN GIẢN ===" -ForegroundColor Green

# Thay đổi token và URL theo môi trường của bạn
$token = "YOUR_TOKEN_HERE"
$baseUrl = "http://localhost:5000"

# JSON request đơn giản và hợp lệ
$requestBody = @{
    startTime = "2024-12-25T08:00:00"
    endTime = "2024-12-27T18:00:00"
    vehicleIds = @(1)
    depositAmount = 500000
    notes = "Test booking"
    isBookingForOthers = $false
} | ConvertTo-Json -Depth 3

Write-Host "Request Body:"
Write-Host $requestBody
Write-Host ""

Write-Host "Gửi request..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $requestBody -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    
    Write-Host "✅ THÀNH CÔNG!" -ForegroundColor Green
    Write-Host "Message: $($response.message)"
    Write-Host "Order ID: $($response.orderId)"
    Write-Host "Contract ID: $($response.contractId)"
    
    if ($response.data) {
        Write-Host "Data: $($response.data | ConvertTo-Json -Depth 2)"
    }
    
} catch {
    Write-Host "❌ LỖI!" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
    Write-Host "Error Message: $($_.Exception.Message)"
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:"
        Write-Host $responseBody -ForegroundColor Red
        
        # Thử parse JSON response để hiển thị đẹp hơn
        try {
            $errorObj = $responseBody | ConvertFrom-Json
            Write-Host "`nChi tiết lỗi:" -ForegroundColor Yellow
            Write-Host "Success: $($errorObj.success)"
            Write-Host "Message: $($errorObj.message)"
            
            if ($errorObj.errors) {
                Write-Host "Errors:"
                $errorObj.errors | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
            }
            
            if ($errorObj.fieldErrors) {
                Write-Host "Field Errors:"
                $errorObj.fieldErrors.PSObject.Properties | ForEach-Object {
                    Write-Host "  $($_.Name):" -ForegroundColor Yellow
                    $_.Value | ForEach-Object { Write-Host "    - $_" -ForegroundColor Red }
                }
            }
            
            if ($errorObj.receivedData) {
                Write-Host "Received Data:"
                Write-Host ($errorObj.receivedData | ConvertTo-Json -Depth 2) -ForegroundColor Cyan
            }
        } catch {
            Write-Host "Không thể parse JSON response" -ForegroundColor Yellow
        }
    }
}

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green
