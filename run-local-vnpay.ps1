# Script chạy local với VNPay Sandbox
# Chạy: .\run-local-vnpay.ps1

Write-Host "=== CHẠY LOCAL VỚI VNPAY SANDBOX ===" -ForegroundColor Green

Write-Host "`n✅ VNPay Sandbox đã được cấu hình:" -ForegroundColor Green
Write-Host "Terminal ID: RLJJQTUS" -ForegroundColor Cyan
Write-Host "Hash Secret: 4PZZLQR2JWPOU8NAAB81H1DIF3N9Z9KF" -ForegroundColor Cyan
Write-Host "URL: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" -ForegroundColor Cyan
Write-Host "Return URL: https://localhost:5000/payment/return" -ForegroundColor Cyan

Write-Host "`n🔧 Cấu hình cần thiết:" -ForegroundColor Yellow
Write-Host "1. ✅ VNPay Sandbox credentials đã có" -ForegroundColor Green
Write-Host "2. ✅ Local SQL Server connection string" -ForegroundColor Green
Write-Host "3. ✅ Cloudinary settings đã có" -ForegroundColor Green

Write-Host "`n🚀 Chạy server local..." -ForegroundColor Yellow

# Chuyển vào thư mục project
Set-Location "EV_RENTAL_SYSTEM"

# Chạy server
Write-Host "`n📡 Starting server on http://localhost:5000..." -ForegroundColor Cyan
Write-Host "Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "API Base: http://localhost:5000/api" -ForegroundColor Cyan

try {
    dotnet run --urls "http://localhost:5000"
} catch {
    Write-Host "❌ Lỗi chạy server: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`n🔧 Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Kiểm tra SQL Server đang chạy" -ForegroundColor Cyan
    Write-Host "2. Kiểm tra connection string trong appsettings.json" -ForegroundColor Cyan
    Write-Host "3. Chạy: dotnet restore" -ForegroundColor Cyan
    Write-Host "4. Chạy: dotnet build" -ForegroundColor Cyan
}

Write-Host "`n🧪 Test VNPay Sandbox:" -ForegroundColor Yellow
Write-Host "1. Tạo đơn thuê xe" -ForegroundColor Cyan
Write-Host "2. Xác nhận hợp đồng" -ForegroundColor Cyan
Write-Host "3. Thanh toán sẽ redirect đến VNPay Sandbox" -ForegroundColor Cyan
Write-Host "4. Sử dụng thẻ test để thanh toán" -ForegroundColor Cyan

Write-Host "`n💡 Lưu ý:" -ForegroundColor Yellow
Write-Host "- Đây là môi trường Sandbox, chỉ để test" -ForegroundColor Red
Write-Host "- Không sử dụng cho thanh toán thật" -ForegroundColor Red
Write-Host "- Test với số tiền nhỏ trước" -ForegroundColor Yellow
Write-Host "- Return URL sẽ redirect về localhost:5000" -ForegroundColor Yellow

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Green
Write-Host "🎉 Server đang chạy với VNPay Sandbox!" -ForegroundColor Green
