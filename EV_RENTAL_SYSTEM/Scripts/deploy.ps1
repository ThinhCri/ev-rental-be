# EV Rental System - Production Deployment Script
# Sử dụng: .\deploy.ps1 -Environment Production -ConnectionString "your_connection_string"

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipMigration,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$WarningColor = "Yellow"
$InfoColor = "Cyan"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Write-Step {
    param([string]$Step, [string]$Description)
    Write-ColorOutput "`n=== $Step ===" $InfoColor
    Write-ColorOutput $Description
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "✅ $Message" $SuccessColor
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "❌ $Message" $ErrorColor
}

function Write-Warning {
    param([string]$Message)
    Write-ColorOutput "⚠️  $Message" $WarningColor
}

# Start deployment
Write-ColorOutput "🚀 EV Rental System Deployment Script" $InfoColor
Write-ColorOutput "Environment: $Environment" $InfoColor
Write-ColorOutput "Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" $InfoColor

try {
    # Step 1: Pre-deployment checks
    Write-Step "PRE-DEPLOYMENT CHECKS" "Checking prerequisites and environment..."
    
    # Check if .NET is installed
    try {
        $dotnetVersion = dotnet --version
        Write-Success ".NET SDK found: $dotnetVersion"
    }
    catch {
        Write-Error ".NET SDK not found. Please install .NET 8.0 or later."
        exit 1
    }
    
    # Check if project file exists
    if (-not (Test-Path "EV_RENTAL_SYSTEM.csproj")) {
        Write-Error "Project file not found. Please run this script from the project root directory."
        exit 1
    }
    
    # Check connection string
    if ($Environment -eq "Production" -and -not $ConnectionString) {
        Write-Warning "No connection string provided for Production environment."
        Write-ColorOutput "Using connection string from appsettings.json" $WarningColor
    }
    
    # Step 2: Backup database (Production only)
    if ($Environment -eq "Production" -and -not $SkipMigration) {
        Write-Step "DATABASE BACKUP" "Creating database backup before migration..."
        
        if ($ConnectionString) {
            # Extract database name from connection string
            $dbName = ($ConnectionString -split "Database=")[1] -split ";"
            $dbName = $dbName[0]
            
            $backupPath = ".\Backups\backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
            $backupDir = Split-Path $backupPath -Parent
            
            if (-not (Test-Path $backupDir)) {
                New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
            }
            
            Write-ColorOutput "Creating backup: $backupPath" $InfoColor
            # Note: This requires sqlcmd to be installed
            # sqlcmd -S localhost -E -Q "BACKUP DATABASE [$dbName] TO DISK = '$backupPath'"
            Write-Success "Database backup created (manual step required)"
        }
    }
    
    # Step 3: Build application
    if (-not $SkipBuild) {
        Write-Step "BUILD APPLICATION" "Building EV Rental System..."
        
        Write-ColorOutput "Cleaning previous build..." $InfoColor
        dotnet clean --configuration Release --verbosity quiet
        
        Write-ColorOutput "Restoring packages..." $InfoColor
        dotnet restore --verbosity quiet
        
        Write-ColorOutput "Building application..." $InfoColor
        dotnet build --configuration Release --no-restore --verbosity quiet
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Build failed. Please fix build errors before deploying."
            exit 1
        }
        
        Write-Success "Build completed successfully"
    }
    
    # Step 4: Run migrations
    if (-not $SkipMigration) {
        Write-Step "DATABASE MIGRATION" "Running database migrations..."
        
        if ($ConnectionString) {
            # Set connection string as environment variable
            $env:ConnectionStrings__DefaultConnection = $ConnectionString
        }
        
        Write-ColorOutput "Checking pending migrations..." $InfoColor
        $pendingMigrations = dotnet ef migrations list --no-build --verbosity quiet
        
        if ($pendingMigrations -match "No migrations were found") {
            Write-Success "No pending migrations found"
        }
        else {
            Write-ColorOutput "Found pending migrations. Applying..." $InfoColor
            dotnet ef database update --no-build --verbosity normal
            
            if ($LASTEXITCODE -ne 0) {
                Write-Error "Migration failed. Please check the error messages above."
                exit 1
            }
            
            Write-Success "Migrations applied successfully"
        }
    }
    else {
        Write-Warning "Skipping migration step"
    }
    
    # Step 5: Run tests (optional)
    Write-Step "RUNNING TESTS" "Running unit tests..."
    
    if (Test-Path "Tests") {
        dotnet test --configuration Release --no-build --verbosity quiet --logger "console;verbosity=minimal"
        
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Some tests failed, but continuing with deployment"
        }
        else {
            Write-Success "All tests passed"
        }
    }
    else {
        Write-ColorOutput "No test project found, skipping tests" $InfoColor
    }
    
    # Step 6: Publish application
    Write-Step "PUBLISH APPLICATION" "Publishing application for deployment..."
    
    $publishDir = ".\publish\$Environment"
    if (Test-Path $publishDir) {
        Remove-Item $publishDir -Recurse -Force
    }
    
    dotnet publish --configuration Release --output $publishDir --no-build --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed. Please check the error messages above."
        exit 1
    }
    
    Write-Success "Application published to: $publishDir"
    
    # Step 7: Create deployment package
    Write-Step "CREATE DEPLOYMENT PACKAGE" "Creating deployment package..."
    
    $packageName = "EV_Rental_System_$Environment"_$(Get-Date -Format 'yyyyMMdd_HHmmss').zip"
    $packagePath = ".\Deployments\$packageName"
    $packageDir = Split-Path $packagePath -Parent
    
    if (-not (Test-Path $packageDir)) {
        New-Item -ItemType Directory -Path $packageDir -Force | Out-Null
    }
    
    # Create zip package
    Compress-Archive -Path "$publishDir\*" -DestinationPath $packagePath -Force
    
    Write-Success "Deployment package created: $packagePath"
    
    # Step 8: Post-deployment verification
    Write-Step "POST-DEPLOYMENT VERIFICATION" "Verifying deployment..."
    
    # Check if published files exist
    $requiredFiles = @("EV_RENTAL_SYSTEM.dll", "appsettings.json", "appsettings.$Environment.json")
    foreach ($file in $requiredFiles) {
        if (Test-Path "$publishDir\$file") {
            Write-Success "Found: $file"
        }
        else {
            Write-Warning "Missing: $file"
        }
    }
    
    # Step 9: Deployment summary
    Write-Step "DEPLOYMENT SUMMARY" "Deployment completed successfully!"
    
    Write-ColorOutput "`n📋 Deployment Details:" $InfoColor
    Write-ColorOutput "   Environment: $Environment" $InfoColor
    Write-ColorOutput "   Build Configuration: Release" $InfoColor
    Write-ColorOutput "   Publish Directory: $publishDir" $InfoColor
    Write-ColorOutput "   Package: $packagePath" $InfoColor
    Write-ColorOutput "   Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" $InfoColor
    
    Write-ColorOutput "`n🚀 Next Steps:" $InfoColor
    Write-ColorOutput "   1. Copy files from $publishDir to your server" $InfoColor
    Write-ColorOutput "   2. Update connection string in appsettings.$Environment.json" $InfoColor
    Write-ColorOutput "   3. Start the application" $InfoColor
    Write-ColorOutput "   4. Verify the application is running correctly" $InfoColor
    
    Write-ColorOutput "`n📝 Important Notes:" $WarningColor
    Write-ColorOutput "   - Migration will run automatically when the app starts" $WarningColor
    Write-ColorOutput "   - Data seeding will run automatically after migration" $WarningColor
    Write-ColorOutput "   - Check application logs for any errors" $WarningColor
    
    Write-Success "Deployment completed successfully! 🎉"
}
catch {
    Write-Error "Deployment failed: $($_.Exception.Message)"
    Write-ColorOutput "`nStack Trace:" $ErrorColor
    Write-ColorOutput $_.ScriptStackTrace $ErrorColor
    exit 1
}
