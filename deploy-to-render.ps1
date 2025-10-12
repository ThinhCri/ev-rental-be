# Script deploy lên Render
# Chạy: .\deploy-to-render.ps1

Write-Host "=== DEPLOY LÊN RENDER ===" -ForegroundColor Green

# Kiểm tra git status
Write-Host "`n1. Kiểm tra git status..." -ForegroundColor Yellow
try {
    $status = git status --porcelain
    if ($status) {
        Write-Host "Có thay đổi cần commit:" -ForegroundColor Cyan
        Write-Host $status
    } else {
        Write-Host "Không có thay đổi nào" -ForegroundColor Green
    }
} catch {
    Write-Host "❌ Lỗi kiểm tra git status: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Add tất cả file
Write-Host "`n2. Add files..." -ForegroundColor Yellow
try {
    git add .
    Write-Host "✅ Đã add tất cả files" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi add files: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Commit
Write-Host "`n3. Commit changes..." -ForegroundColor Yellow
try {
    git commit -m "Prepare for Render deployment - Add Dockerfile and config files"
    Write-Host "✅ Đã commit thành công" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi commit: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Push lên GitHub
Write-Host "`n4. Push lên GitHub..." -ForegroundColor Yellow
try {
    git push origin main
    Write-Host "✅ Đã push lên GitHub thành công" -ForegroundColor Green
} catch {
    Write-Host "❌ Lỗi push: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Green
Write-Host "✅ Code đã được push lên GitHub" -ForegroundColor Green
Write-Host "✅ Bây giờ có thể deploy trên Render:" -ForegroundColor Cyan
Write-Host "   1. Vào Render Dashboard" -ForegroundColor Cyan
Write-Host "   2. Tạo Web Service mới" -ForegroundColor Cyan
Write-Host "   3. Chọn repository và branch 'main'" -ForegroundColor Cyan
Write-Host "   4. Chọn Language: Docker" -ForegroundColor Cyan
Write-Host "   5. Deploy!" -ForegroundColor Cyan
