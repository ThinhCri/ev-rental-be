@echo off
echo Starting EV Rental System on HTTPS...
echo.
echo Application will be available at:
echo   - HTTPS: https://localhost:7181
echo   - Swagger: https://localhost:7181/swagger
echo.
echo If you get certificate errors, run: dotnet dev-certs https --trust
echo Press Ctrl+C to stop the application
echo.
dotnet run --launch-profile https
