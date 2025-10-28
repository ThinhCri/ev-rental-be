# Script để setup ngrok cho VNPay testing
Write-Host "Installing ngrok..." -ForegroundColor Green

# Download ngrok
Invoke-WebRequest -Uri "https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip" -OutFile "ngrok.zip"

# Extract ngrok
Expand-Archive -Path "ngrok.zip" -DestinationPath "." -Force

# Remove zip file
Remove-Item "ngrok.zip"

Write-Host "ngrok installed successfully!" -ForegroundColor Green
Write-Host "Now run: .\ngrok.exe http 5228" -ForegroundColor Yellow
Write-Host "Then update your VnPayService with the ngrok URL" -ForegroundColor Yellow