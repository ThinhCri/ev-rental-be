# Script deploy với VNPay Production
# Chạy: .\deploy-with-vnpay.ps1

Write-Host "=== DEPLOY VỚI VNPAY PRODUCTION ===" -ForegroundColor Green

Write-Host "`n📋 Checklist trước khi deploy:" -ForegroundColor Yellow
Write-Host "1. ✅ Có tài khoản merchant VNPay" -ForegroundColor Cyan
Write-Host "2. ✅ Lấy Terminal ID (TmnCode)" -ForegroundColor Cyan
Write-Host "3. ✅ Lấy Hash Secret" -ForegroundColor Cyan
Write-Host "4. ✅ Có SQL Server database" -ForegroundColor Cyan

Write-Host "`n🔧 Cấu hình cần thiết:" -ForegroundColor Yellow

# Environment Variables cần thiết
$envVars = @{
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ASPNETCORE_URLS" = "http://0.0.0.0:10000"
    "ConnectionStrings__DefaultConnection" = "[SQL Server Connection String]"
    "VnPay__TmnCode" = "[YOUR_TERMINAL_ID]"
    "VnPay__HashSecret" = "[YOUR_HASH_SECRET]"
    "VnPay__Url" = "https://payment.vnpayment.vn/vpcpay.html"
    "VnPay__ReturnUrl" = "https://ev-rental-be-2.onrender.com/payment/return"
    "VnPay__Command" = "pay"
    "VnPay__CurrCode" = "VND"
    "VnPay__Version" = "2.1.0"
    "VnPay__Locale" = "vn"
}

Write-Host "`nEnvironment Variables cần thêm vào Render:" -ForegroundColor Green
foreach ($key in $envVars.Keys) {
    Write-Host "$key = $($envVars[$key])" -ForegroundColor White
}

Write-Host "`n🚀 Bước deploy:" -ForegroundColor Yellow
Write-Host "1. Cập nhật Environment Variables trên Render" -ForegroundColor Cyan
Write-Host "2. Push code lên GitHub" -ForegroundColor Cyan
Write-Host "3. Manual Deploy trên Render" -ForegroundColor Cyan

# Push code
Write-Host "`n📤 Push code lên GitHub..." -ForegroundColor Yellow
try {
    git add .
    git commit -m "Configure VNPay production settings"
    git push origin NewBranchName
    Write-Host "✅ Code đã được push thành công!" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi push code: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎯 Bước tiếp theo:" -ForegroundColor Yellow
Write-Host "1. Vào Render Dashboard → Web Service → Environment" -ForegroundColor Cyan
Write-Host "2. Thêm các Environment Variables ở trên" -ForegroundColor Cyan
Write-Host "3. Manual Deploy → Deploy latest commit" -ForegroundColor Cyan
Write-Host "4. Test API: https://ev-rental-be-2.onrender.com/" -ForegroundColor Cyan

Write-Host "`n💡 Lưu ý:" -ForegroundColor Yellow
Write-Host "- Thay [YOUR_TERMINAL_ID] bằng Terminal ID thật" -ForegroundColor Red
Write-Host "- Thay [YOUR_HASH_SECRET] bằng Hash Secret thật" -ForegroundColor Red
Write-Host "- Thay [SQL Server Connection String] bằng connection string thật" -ForegroundColor Red
Write-Host "- Return URL đã được set sẵn cho Render" -ForegroundColor Green

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Green
Write-Host "🎉 Bây giờ có thể deploy với VNPay Production!" -ForegroundColor Green
