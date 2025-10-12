# Script test với JSON thô để debug lỗi 400
# Chạy: .\test-raw-json.ps1

Write-Host "=== TEST VỚI JSON THÔ ===" -ForegroundColor Green

# Cấu hình
$token = "YOUR_TOKEN_HERE"  # Thay bằng token thực tế
$baseUrl = "http://localhost:5000"

# JSON thô hợp lệ
$validJson = '{
    "startTime": "2024-12-25T08:00:00",
    "endTime": "2024-12-27T18:00:00",
    "vehicleIds": [1],
    "depositAmount": 500000,
    "notes": "Test booking",
    "isBookingForOthers": false
}'

Write-Host "JSON Request:"
Write-Host $validJson
Write-Host ""

Write-Host "Gửi request..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/rental" -Method POST -Body $validJson -ContentType "application/json" -Headers @{"Authorization" = "Bearer $token"}
    
    Write-Host "✅ THÀNH CÔNG!" -ForegroundColor Green
    Write-Host "Message: $($response.message)"
    Write-Host "Order ID: $($response.orderId)"
    Write-Host "Contract ID: $($response.contractId)"
    
} catch {
    Write-Host "❌ LỖI!" -ForegroundColor Red
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
    Write-Host "Error Message: $($_.Exception.Message)"
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body:"
        Write-Host $responseBody -ForegroundColor Red
    }
}

Write-Host "`n=== KẾT THÚC TEST ===" -ForegroundColor Green
