#!/bin/bash

# VMG Portal - Complete Database Setup
# This script runs all database setup steps

set -e

echo "========================================="
echo "VMG Portal - Complete Database Setup"
echo "========================================="
echo ""

# Configuration
CONTAINER_NAME="vmg-sqlserver"
SA_PASSWORD="YourStrong@Passw0rd"
DATABASE_NAME="Vmg.BillingDB"

# Step 1: Setup SQL Server
echo "Step 1/4: Setting up SQL Server..."
chmod +x setup-sqlserver.sh
./setup-sqlserver.sh

echo ""
echo "Step 2/4: Waiting for SQL Server to be ready..."
sleep 5

# Step 2: Create Schema
echo ""
echo "Step 3/4: Creating database schema..."
docker exec -i ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${SA_PASSWORD}" -C \
    -i /dev/stdin < database/create-schema.sql

echo ""
echo "✅ Schema created successfully"

# Step 3: Seed Data
echo ""
echo "Step 4/4: Seeding sample data..."
docker exec -i ${CONTAINER_NAME} /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "${SA_PASSWORD}" -C \
    -i /dev/stdin < database/seed-data.sql

echo ""
echo "✅ Data seeded successfully"

# Step 4: Update appsettings.Development.json
echo ""
echo "========================================="
echo "Updating appsettings.Development.json"
echo "========================================="

CONNECTION_STRING="Server=localhost,1433;Database=${DATABASE_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=true;Encrypt=false"

# Update all connection strings in appsettings.Development.json
if [ -f "NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json" ]; then
    echo "Updating connection strings..."
    
    # Backup original file
    cp NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json \
       NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json.backup
    
    # Update connection strings using sed
    sed -i '' "s|\"CardGameBettingAPIConnectionString\":.*|\"CardGameBettingAPIConnectionString\": \"${CONNECTION_STRING}\",|" \
        NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json
    
    sed -i '' "s|\"BillingAuthenticationAPIConnectionString\":.*|\"BillingAuthenticationAPIConnectionString\": \"${CONNECTION_STRING}\",|" \
        NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json
    
    sed -i '' "s|\"BillingDatabaseAPIConnectionString\":.*|\"BillingDatabaseAPIConnectionString\": \"${CONNECTION_STRING}\",|" \
        NetCore.PortalAPI/ServerCore.PortalAPI/appsettings.Development.json
    
    echo "✅ Connection strings updated"
    echo "   Backup saved to: appsettings.Development.json.backup"
else
    echo "⚠️  appsettings.Development.json not found"
    echo "   Please update connection strings manually:"
    echo "   ${CONNECTION_STRING}"
fi

echo ""
echo "========================================="
echo "🎉 Database Setup Complete!"
echo "========================================="
echo ""
echo "Database Information:"
echo "  Server: localhost,1433"
echo "  Database: ${DATABASE_NAME}"
echo "  Username: sa"
echo "  Password: ${SA_PASSWORD}"
echo ""
echo "Test Accounts:"
echo "  admin/admin     - Balance: 10,000,000 (VIP 10)"
echo "  testuser/admin  - Balance: 1,000,000"
echo "  player001/admin - Balance: 500,000"
echo "  player002/admin - Balance: 750,000"
echo ""
echo "Next Steps:"
echo "  1. Restart your API server (Ctrl+C then 'dotnet run')"
echo "  2. Test login: ./test-api.sh"
echo "  3. Or use Swagger: http://localhost:64327/swagger/index.html"
echo ""
echo "Database Management:"
echo "  Stop:   docker stop ${CONTAINER_NAME}"
echo "  Start:  docker start ${CONTAINER_NAME}"
echo "  Remove: docker rm -f ${CONTAINER_NAME}"
echo ""
