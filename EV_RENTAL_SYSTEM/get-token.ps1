# Script lấy token để test
# Chạy: .\get-token.ps1

Write-Host "=== LẤY TOKEN ĐỂ TEST ===" -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Thông tin đăng nhập (thay đổi theo user thực tế)
$loginData = @{
    email = "admin@example.com"  # Thay bằng email thực tế
    password = "admin123"        # Thay bằng password thực tế
} | ConvertTo-Json

Write-Host "Đang đăng nhập với email: $($loginData | ConvertFrom-Json | Select-Object -ExpandProperty email)" -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    
    Write-Host "✅ Đăng nhập thành công!" -ForegroundColor Green
    Write-Host "Token: $($response.data.token)" -ForegroundColor Cyan
    
    # Lưu token vào file để sử dụng
    $token = $response.data.token
    $token | Out-File -FilePath "token.txt" -Encoding UTF8
    Write-Host "Token đã được lưu vào file token.txt" -ForegroundColor Green
    
    return $token
    
} catch {
    Write-Host "❌ Lỗi đăng nhập: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
    
    Write-Host "`nCó thể thử với các tài khoản khác:" -ForegroundColor Yellow
    Write-Host "1. staff@example.com / staff123" -ForegroundColor Yellow
    Write-Host "2. user@example.com / user123" -ForegroundColor Yellow
    Write-Host "3. Hoặc tạo tài khoản mới qua API register" -ForegroundColor Yellow
}
