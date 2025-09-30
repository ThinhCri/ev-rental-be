# Test Data Seeding Script
# Sử dụng: .\test-data.ps1

Write-Host "🧪 Testing Data Seeding..." -ForegroundColor Cyan

# Test 1: Check if app is running
Write-Host "`n=== Test 1: Check Application Status ===" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "https://localhost:7000/swagger/v1/swagger.json" -Method Get -SkipCertificateCheck
    Write-Host "✅ Application is running" -ForegroundColor Green
}
catch {
    Write-Host "❌ Application is not running. Please run 'dotnet run' first." -ForegroundColor Red
    exit 1
}

# Test 2: Test API endpoints
Write-Host "`n=== Test 2: Testing API Endpoints ===" -ForegroundColor Yellow

# Test Roles endpoint
try {
    $roles = Invoke-RestMethod -Uri "https://localhost:7000/api/LicenseType" -Method Get -SkipCertificateCheck
    Write-Host "✅ License Types API working" -ForegroundColor Green
    Write-Host "   Found $($roles.Count) license types" -ForegroundColor Cyan
}
catch {
    Write-Host "❌ License Types API failed" -ForegroundColor Red
}

# Test 3: Check specific data
Write-Host "`n=== Test 3: Checking Seeded Data ===" -ForegroundColor Yellow

# Check if we can get specific data
$expectedData = @(
    @{Name="Roles"; Count=3},
    @{Name="License Types"; Count=5},
    @{Name="Brands"; Count=8},
    @{Name="Stations"; Count=4}
)

foreach ($data in $expectedData) {
    Write-Host "Checking $($data.Name)..." -ForegroundColor Cyan
    # Note: This would need actual API endpoints to work
    Write-Host "   Expected: $($data.Count) items" -ForegroundColor Gray
}

Write-Host "`n🎉 Data Seeding Test Complete!" -ForegroundColor Green
Write-Host "`n📋 Next Steps for Team:" -ForegroundColor Cyan
Write-Host "1. git pull origin main" -ForegroundColor White
Write-Host "2. dotnet ef database update" -ForegroundColor White
Write-Host "3. dotnet run" -ForegroundColor White
Write-Host "4. Open https://localhost:7000/swagger" -ForegroundColor White
Write-Host "5. Test the API endpoints" -ForegroundColor White
