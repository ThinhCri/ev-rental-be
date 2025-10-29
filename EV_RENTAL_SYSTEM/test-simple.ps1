# Test đơn giản
$body = @{
    startTime = "2025-10-15T08:00:00"
    endTime = "2025-10-17T18:00:00"
    vehicleIds = @(1)
    depositAmount = 300000
    notes = "Test"
    isBookingForOthers = $false
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "https://localhost:7181/api/rental" -Method POST -Body $body -ContentType "application/json" -Headers @{"Authorization" = "Bearer test"}
    Write-Host "SUCCESS: $($response.message)"
} catch {
    Write-Host "ERROR: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $stream = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody"
    }
}
