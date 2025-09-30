# ========================================
# EV RENTAL SYSTEM - SETUP SCRIPT
# ========================================
# Script tu dong setup project cho team members
# Chi can chay: .\setup.ps1
# ========================================

Write-Host "EV RENTAL SYSTEM - SETUP SCRIPT" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green

# Kiem tra xem co dang o dung thu muc project khong
if (-not (Test-Path "EV_RENTAL_SYSTEM.csproj")) {
    Write-Host "Loi: Khong tim thay file EV_RENTAL_SYSTEM.csproj" -ForegroundColor Red
    Write-Host "Vui long chay script nay trong thu muc goc cua project" -ForegroundColor Yellow
    exit 1
}

Write-Host "Da xac nhan dang o dung thu muc project" -ForegroundColor Green

# Buoc 1: Restore packages
Write-Host "`nDang restore NuGet packages..." -ForegroundColor Yellow
try {
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Restore packages that bai"
    }
    Write-Host "Restore packages thanh cong" -ForegroundColor Green
}
catch {
    Write-Host "Loi khi restore packages: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Buoc 2: Build project
Write-Host "`nDang build project..." -ForegroundColor Yellow
try {
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        throw "Build project that bai"
    }
    Write-Host "Build project thanh cong" -ForegroundColor Green
}
catch {
    Write-Host "Loi khi build project: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Buoc 3: Xu ly database
Write-Host "`nDang setup database..." -ForegroundColor Yellow

# Kiem tra xem co database cu khong va xoa neu can
Write-Host "Kiem tra database hien tai..." -ForegroundColor Cyan
try {
    $dbExists = dotnet ef database update --dry-run 2>&1 | Select-String "There is already an object named"
    if ($dbExists) {
        Write-Host "Phat hien database cu co the gay xung dot" -ForegroundColor Yellow
        Write-Host "Dang xoa database cu..." -ForegroundColor Cyan
        dotnet ef database drop --force
        Write-Host "Da xoa database cu" -ForegroundColor Green
    }
}
catch {
    Write-Host "Database chua ton tai hoac khong co van de" -ForegroundColor Blue
}

# Tao database moi va chay migration
Write-Host "Dang tao database moi va chay migration..." -ForegroundColor Cyan
try {
    dotnet ef database update
    if ($LASTEXITCODE -ne 0) {
        throw "Migration that bai"
    }
    Write-Host "Database setup thanh cong" -ForegroundColor Green
}
catch {
    Write-Host "Loi khi setup database: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Dang thu phuong phap du phong..." -ForegroundColor Yellow
    
    # Phuong phap du phong: Xoa database va tao lai
    try {
        dotnet ef database drop --force
        dotnet ef database update
        Write-Host "Database setup thanh cong voi phuong phap du phong" -ForegroundColor Green
    }
    catch {
        Write-Host "Khong the setup database. Vui long kiem tra SQL Server va connection string" -ForegroundColor Red
        exit 1
    }
}

# Buoc 4: Chay ung dung de test data seeding
Write-Host "`nDang test data seeding..." -ForegroundColor Yellow
Write-Host "Khoi dong ung dung de chay data seeding..." -ForegroundColor Cyan

# Chay ung dung trong background de test data seeding
$job = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run --urls "http://localhost:5228" 2>&1
}

# Cho 10 giay de ung dung khoi dong va chay data seeding
Start-Sleep -Seconds 10

# Dung job
Stop-Job $job
Remove-Job $job

Write-Host "Data seeding da hoan thanh" -ForegroundColor Green

# Buoc 5: Kiem tra ket qua
Write-Host "`nDang kiem tra ket qua..." -ForegroundColor Yellow

try {
    # Kiem tra xem co data trong database khong
    $brandCount = sqlcmd -S localhost -d EV_Rental_System -E -Q "SELECT COUNT(*) FROM Brand" -h -1 2>$null
    $roleCount = sqlcmd -S localhost -d EV_Rental_System -E -Q "SELECT COUNT(*) FROM Role" -h -1 2>$null
    
    if ($brandCount -gt 0 -and $roleCount -gt 0) {
        Write-Host "Database da duoc setup thanh cong voi data mau" -ForegroundColor Green
        Write-Host "   - Brands: $brandCount records" -ForegroundColor Cyan
        Write-Host "   - Roles: $roleCount records" -ForegroundColor Cyan
    } else {
        Write-Host "Database da duoc tao nhung chua co data mau" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "Khong the kiem tra database, nhung setup da hoan thanh" -ForegroundColor Blue
}

# Buoc 6: Huong dan su dung
Write-Host "`nSETUP HOAN THANH!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host "De chay ung dung:" -ForegroundColor Yellow
Write-Host "  dotnet run" -ForegroundColor Cyan
Write-Host "`nTruy cap Swagger UI:" -ForegroundColor Yellow
Write-Host "  http://localhost:5228/swagger" -ForegroundColor Cyan
Write-Host "`nAPI Endpoints:" -ForegroundColor Yellow
Write-Host "  POST http://localhost:5228/api/Auth/register" -ForegroundColor Cyan
Write-Host "  POST http://localhost:5228/api/Auth/login" -ForegroundColor Cyan
Write-Host "`nLuu y:" -ForegroundColor Yellow
Write-Host "  - Su dung HTTP (port 5228) thay vi HTTPS" -ForegroundColor White
Write-Host "  - API register can upload file anh bang lai xe" -ForegroundColor White
Write-Host "  - Test API tren Swagger UI de tranh loi CORS" -ForegroundColor White

Write-Host "`nChuc ban code vui ve!" -ForegroundColor Green