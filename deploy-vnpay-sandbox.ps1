# Script deploy với VNPay Sandbox
# Chạy: .\deploy-vnpay-sandbox.ps1

Write-Host "=== DEPLOY VỚI VNPAY SANDBOX ===" -ForegroundColor Green

Write-Host "`n✅ Thông tin VNPay Sandbox đã được cấu hình:" -ForegroundColor Green
Write-Host "Terminal ID: RLJJQTUS" -ForegroundColor Cyan
Write-Host "Hash Secret: 4PZZLQR2JWPOU8NAAB81H1DIF3N9Z9KF" -ForegroundColor Cyan
Write-Host "URL: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" -ForegroundColor Cyan
Write-Host "Return URL: https://ev-rental-be-2.onrender.com/payment/return" -ForegroundColor Cyan

Write-Host "`n🔧 Environment Variables cần thêm vào Render:" -ForegroundColor Yellow
Write-Host "ASPNETCORE_ENVIRONMENT=Production" -ForegroundColor White
Write-Host "ASPNETCORE_URLS=http://0.0.0.0:10000" -ForegroundColor White
Write-Host "ConnectionStrings__DefaultConnection=[SQL Server Connection String]" -ForegroundColor White
Write-Host "VnPay__TmnCode=RLJJQTUS" -ForegroundColor White
Write-Host "VnPay__HashSecret=4PZZLQR2JWPOU8NAAB81H1DIF3N9Z9KF" -ForegroundColor White
Write-Host "VnPay__Url=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" -ForegroundColor White
Write-Host "VnPay__ReturnUrl=https://ev-rental-be-2.onrender.com/payment/return" -ForegroundColor White
Write-Host "VnPay__Command=pay" -ForegroundColor White
Write-Host "VnPay__CurrCode=VND" -ForegroundColor White
Write-Host "VnPay__Version=2.1.0" -ForegroundColor White
Write-Host "VnPay__Locale=vn" -ForegroundColor White

Write-Host "`n📤 Push code lên GitHub..." -ForegroundColor Yellow
try {
    git add .
    git commit -m "Configure VNPay sandbox with real credentials"
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
Write-Host "- Đây là môi trường Sandbox, chỉ để test" -ForegroundColor Red
Write-Host "- Không sử dụng cho thanh toán thật" -ForegroundColor Red
Write-Host "- Cần tạo IPN URL để VNPay callback" -ForegroundColor Yellow
Write-Host "- Test với số tiền nhỏ trước" -ForegroundColor Yellow

Write-Host "`n🧪 Test VNPay Sandbox:" -ForegroundColor Yellow
Write-Host "1. Tạo đơn thuê xe" -ForegroundColor Cyan
Write-Host "2. Xác nhận hợp đồng" -ForegroundColor Cyan
Write-Host "3. Thanh toán sẽ redirect đến VNPay Sandbox" -ForegroundColor Cyan
Write-Host "4. Sử dụng thẻ test để thanh toán" -ForegroundColor Cyan

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Green
Write-Host "🎉 Bây giờ có thể deploy với VNPay Sandbox!" -ForegroundColor Green
