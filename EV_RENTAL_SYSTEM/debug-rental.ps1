# Debug Rental API Script
# Sử dụng: .\debug-rental.ps1

$baseUrl = "http://localhost:5228"
$debugUrl = "$baseUrl/api/Rental/debug"
$createUrl = "$baseUrl/api/Rental"

Write-Host "=== DEBUG RENTAL API ===" -ForegroundColor Green

# Test 1: Debug endpoint (không cần JWT)
Write-Host "`n1. Testing debug endpoint..." -ForegroundColor Yellow
$testJson = @{
    startTime = "2025-01-15T08:00:00"
    endTime = "2025-01-20T18:00:00"
    vehicleIds = @(1)
    depositAmount = 100000
    notes = "Test rental"
} | ConvertTo-Json

Write-Host "JSON: $testJson" -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri $debugUrl -Method POST -Body $testJson -ContentType "application/json"
    Write-Host "✅ Debug Success!" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
}
catch {
    Write-Host "❌ Debug Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}

# Test 2: Tạo đơn thuê (cần JWT)
Write-Host "`n2. Testing create rental (cần JWT token)..." -ForegroundColor Yellow
Write-Host "Để test tạo đơn thuê, cần đăng nhập trước để lấy JWT token" -ForegroundColor White
Write-Host "Sau đó thay YOUR_JWT_TOKEN vào script này" -ForegroundColor White

# Uncomment để test với JWT token
# $headers = @{
#     "Content-Type" = "application/json"
#     "Authorization" = "Bearer YOUR_JWT_TOKEN"
# }
# 
# try {
#     $response = Invoke-RestMethod -Uri $createUrl -Method POST -Body $testJson -Headers $headers
#     Write-Host "✅ Create Success!" -ForegroundColor Green
#     $response | ConvertTo-Json -Depth 3
# }
# catch {
#     Write-Host "❌ Create Error: $($_.Exception.Message)" -ForegroundColor Red
#     if ($_.Exception.Response) {
#         $stream = $_.Exception.Response.GetResponseStream()
#         $reader = New-Object System.IO.StreamReader($stream)
#         $responseBody = $reader.ReadToEnd()
#         Write-Host "Response: $responseBody" -ForegroundColor Red
#     }
# }

Write-Host "`n=== Kết luận ===" -ForegroundColor Green
Write-Host "1. Chạy debug endpoint trước để kiểm tra JSON parsing" -ForegroundColor White
Write-Host "2. Nếu debug OK, thì vấn đề là ở JWT token hoặc business logic" -ForegroundColor White
Write-Host "3. Kiểm tra logs trong console để xem chi tiết lỗi" -ForegroundColor White






