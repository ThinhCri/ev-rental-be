@echo off
echo Starting EV Rental System on HTTP...
echo.
echo Application will be available at:
echo   - HTTP: http://localhost:5228
echo   - Swagger: http://localhost:5228/swagger
echo.
echo Press Ctrl+C to stop the application
echo.
dotnet run --launch-profile http
