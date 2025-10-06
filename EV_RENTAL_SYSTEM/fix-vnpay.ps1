# Script tự động fix lỗi VNPay
Write-Host "=== FIX VNPAY ERROR 72 ===" -ForegroundColor Red

# Kiểm tra ngrok
$ngrokProcess = Get-Process -Name "ngrok" -ErrorAction SilentlyContinue
if (-not $ngrokProcess) {
    Write-Host "ngrok chưa chạy. Đang khởi động ngrok..." -ForegroundColor Yellow
    
    # Tìm ngrok
    $ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
    if (-not $ngrokPath) {
        Write-Host "ngrok chưa được cài đặt!" -ForegroundColor Red
        Write-Host "Vui lòng cài đặt ngrok từ: https://ngrok.com/download" -ForegroundColor Yellow
        Write-Host "Hoặc chạy: choco install ngrok" -ForegroundColor Yellow
        exit 1
    }
    
    # Chạy ngrok
    Write-Host "Đang khởi động ngrok trên port 7000..." -ForegroundColor Cyan
    Start-Process -FilePath "ngrok" -ArgumentList "http", "7000" -WindowStyle Minimized
    Start-Sleep -Seconds 3
}

# Lấy URL ngrok
Write-Host "Đang lấy URL ngrok..." -ForegroundColor Cyan
try {
    $ngrokApi = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels" -Method Get
    $ngrokUrl = $ngrokApi.tunnels[0].public_url
    
    if ($ngrokUrl) {
        Write-Host "URL ngrok: $ngrokUrl" -ForegroundColor Green
        
        # Cập nhật appsettings.json
        $appsettingsPath = "appsettings.json"
        if (Test-Path $appsettingsPath) {
            $json = Get-Content $appsettingsPath | ConvertFrom-Json
            $json.VnPaySettings | Add-Member -Name "ReturnUrl" -Value "$ngrokUrl/api/payment/vnpay-callback" -Force
            $json | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath
            
            Write-Host "Đã cập nhật appsettings.json với URL ngrok!" -ForegroundColor Green
            Write-Host "ReturnUrl: $ngrokUrl/api/payment/vnpay-callback" -ForegroundColor Cyan
        }
    } else {
        Write-Host "Không thể lấy URL ngrok. Vui lòng kiểm tra ngrok có chạy không." -ForegroundColor Red
    }
} catch {
    Write-Host "Lỗi khi lấy URL ngrok: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Vui lòng chạy ngrok thủ công: ngrok http 7000" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== HƯỚNG DẪN TIẾP THEO ===" -ForegroundColor Green
Write-Host "1. Restart ứng dụng: dotnet run" -ForegroundColor Yellow
Write-Host "2. Test VNPay với URL ngrok" -ForegroundColor Yellow
Write-Host "3. VNPay sẽ redirect về đúng callback URL" -ForegroundColor Yellow




