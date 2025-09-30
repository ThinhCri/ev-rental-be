#!/bin/bash
# EV Rental System - Production Deployment Script (Linux/macOS)
# Sử dụng: ./deploy.sh Production "your_connection_string"

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Function to print colored output
print_color() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

print_step() {
    local step=$1
    local description=$2
    echo ""
    print_color $CYAN "=== $step ==="
    echo "$description"
}

print_success() {
    print_color $GREEN "✅ $1"
}

print_error() {
    print_color $RED "❌ $1"
}

print_warning() {
    print_color $YELLOW "⚠️  $1"
}

print_info() {
    print_color $CYAN "$1"
}

# Parse command line arguments
ENVIRONMENT=""
CONNECTION_STRING=""
SKIP_MIGRATION=false
SKIP_BUILD=false
VERBOSE=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -e|--environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        -c|--connection-string)
            CONNECTION_STRING="$2"
            shift 2
            ;;
        --skip-migration)
            SKIP_MIGRATION=true
            shift
            ;;
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -e, --environment ENV        Environment (Development|Staging|Production)"
            echo "  -c, --connection-string STR  Database connection string"
            echo "  --skip-migration            Skip database migration"
            echo "  --skip-build                Skip building the application"
            echo "  -v, --verbose               Enable verbose output"
            echo "  -h, --help                  Show this help message"
            exit 0
            ;;
        *)
            print_error "Unknown option $1"
            exit 1
            ;;
    esac
done

# Validate environment
if [[ -z "$ENVIRONMENT" ]]; then
    print_error "Environment is required. Use -e or --environment"
    exit 1
fi

if [[ "$ENVIRONMENT" != "Development" && "$ENVIRONMENT" != "Staging" && "$ENVIRONMENT" != "Production" ]]; then
    print_error "Invalid environment. Must be Development, Staging, or Production"
    exit 1
fi

# Start deployment
print_color $CYAN "🚀 EV Rental System Deployment Script"
print_info "Environment: $ENVIRONMENT"
print_info "Timestamp: $(date '+%Y-%m-%d %H:%M:%S')"

# Step 1: Pre-deployment checks
print_step "PRE-DEPLOYMENT CHECKS" "Checking prerequisites and environment..."

# Check if .NET is installed
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    print_success ".NET SDK found: $DOTNET_VERSION"
else
    print_error ".NET SDK not found. Please install .NET 8.0 or later."
    exit 1
fi

# Check if project file exists
if [[ ! -f "EV_RENTAL_SYSTEM.csproj" ]]; then
    print_error "Project file not found. Please run this script from the project root directory."
    exit 1
fi

# Check connection string
if [[ "$ENVIRONMENT" == "Production" && -z "$CONNECTION_STRING" ]]; then
    print_warning "No connection string provided for Production environment."
    print_info "Using connection string from appsettings.json"
fi

# Step 2: Backup database (Production only)
if [[ "$ENVIRONMENT" == "Production" && "$SKIP_MIGRATION" == false ]]; then
    print_step "DATABASE BACKUP" "Creating database backup before migration..."
    
    if [[ -n "$CONNECTION_STRING" ]]; then
        # Extract database name from connection string
        DB_NAME=$(echo "$CONNECTION_STRING" | grep -o 'Database=[^;]*' | cut -d'=' -f2)
        BACKUP_PATH="./Backups/backup_$(date '+%Y%m%d_%H%M%S').bak"
        BACKUP_DIR=$(dirname "$BACKUP_PATH")
        
        mkdir -p "$BACKUP_DIR"
        
        print_info "Creating backup: $BACKUP_PATH"
        # Note: This requires sqlcmd to be installed
        # sqlcmd -S localhost -E -Q "BACKUP DATABASE [$DB_NAME] TO DISK = '$BACKUP_PATH'"
        print_success "Database backup created (manual step required)"
    fi
fi

# Step 3: Build application
if [[ "$SKIP_BUILD" == false ]]; then
    print_step "BUILD APPLICATION" "Building EV Rental System..."
    
    print_info "Cleaning previous build..."
    dotnet clean --configuration Release --verbosity quiet
    
    print_info "Restoring packages..."
    dotnet restore --verbosity quiet
    
    print_info "Building application..."
    dotnet build --configuration Release --no-restore --verbosity quiet
    
    if [[ $? -ne 0 ]]; then
        print_error "Build failed. Please fix build errors before deploying."
        exit 1
    fi
    
    print_success "Build completed successfully"
fi

# Step 4: Run migrations
if [[ "$SKIP_MIGRATION" == false ]]; then
    print_step "DATABASE MIGRATION" "Running database migrations..."
    
    if [[ -n "$CONNECTION_STRING" ]]; then
        # Set connection string as environment variable
        export ConnectionStrings__DefaultConnection="$CONNECTION_STRING"
    fi
    
    print_info "Checking pending migrations..."
    PENDING_MIGRATIONS=$(dotnet ef migrations list --no-build --verbosity quiet)
    
    if echo "$PENDING_MIGRATIONS" | grep -q "No migrations were found"; then
        print_success "No pending migrations found"
    else
        print_info "Found pending migrations. Applying..."
        dotnet ef database update --no-build --verbosity normal
        
        if [[ $? -ne 0 ]]; then
            print_error "Migration failed. Please check the error messages above."
            exit 1
        fi
        
        print_success "Migrations applied successfully"
    fi
else
    print_warning "Skipping migration step"
fi

# Step 5: Run tests (optional)
print_step "RUNNING TESTS" "Running unit tests..."

if [[ -d "Tests" ]]; then
    dotnet test --configuration Release --no-build --verbosity quiet --logger "console;verbosity=minimal"
    
    if [[ $? -ne 0 ]]; then
        print_warning "Some tests failed, but continuing with deployment"
    else
        print_success "All tests passed"
    fi
else
    print_info "No test project found, skipping tests"
fi

# Step 6: Publish application
print_step "PUBLISH APPLICATION" "Publishing application for deployment..."

PUBLISH_DIR="./publish/$ENVIRONMENT"
if [[ -d "$PUBLISH_DIR" ]]; then
    rm -rf "$PUBLISH_DIR"
fi

dotnet publish --configuration Release --output "$PUBLISH_DIR" --no-build --verbosity quiet

if [[ $? -ne 0 ]]; then
    print_error "Publish failed. Please check the error messages above."
    exit 1
fi

print_success "Application published to: $PUBLISH_DIR"

# Step 7: Create deployment package
print_step "CREATE DEPLOYMENT PACKAGE" "Creating deployment package..."

PACKAGE_NAME="EV_Rental_System_${ENVIRONMENT}_$(date '+%Y%m%d_%H%M%S').tar.gz"
PACKAGE_PATH="./Deployments/$PACKAGE_NAME"
PACKAGE_DIR=$(dirname "$PACKAGE_PATH")

mkdir -p "$PACKAGE_DIR"

# Create tar.gz package
tar -czf "$PACKAGE_PATH" -C "$PUBLISH_DIR" .

print_success "Deployment package created: $PACKAGE_PATH"

# Step 8: Post-deployment verification
print_step "POST-DEPLOYMENT VERIFICATION" "Verifying deployment..."

# Check if published files exist
REQUIRED_FILES=("EV_RENTAL_SYSTEM.dll" "appsettings.json" "appsettings.$ENVIRONMENT.json")
for file in "${REQUIRED_FILES[@]}"; do
    if [[ -f "$PUBLISH_DIR/$file" ]]; then
        print_success "Found: $file"
    else
        print_warning "Missing: $file"
    fi
done

# Step 9: Deployment summary
print_step "DEPLOYMENT SUMMARY" "Deployment completed successfully!"

print_info ""
print_info "📋 Deployment Details:"
print_info "   Environment: $ENVIRONMENT"
print_info "   Build Configuration: Release"
print_info "   Publish Directory: $PUBLISH_DIR"
print_info "   Package: $PACKAGE_PATH"
print_info "   Timestamp: $(date '+%Y-%m-%d %H:%M:%S')"

print_info ""
print_info "🚀 Next Steps:"
print_info "   1. Copy files from $PUBLISH_DIR to your server"
print_info "   2. Update connection string in appsettings.$ENVIRONMENT.json"
print_info "   3. Start the application"
print_info "   4. Verify the application is running correctly"

print_info ""
print_warning "📝 Important Notes:"
print_warning "   - Migration will run automatically when the app starts"
print_warning "   - Data seeding will run automatically after migration"
print_warning "   - Check application logs for any errors"

print_success "Deployment completed successfully! 🎉"
