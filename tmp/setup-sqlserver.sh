#!/bin/bash

# VMG Portal - SQL Server Setup Script
# This script sets up SQL Server in Docker and creates the database

set -e

echo "========================================="
echo "VMG Portal - Database Setup"
echo "========================================="
echo ""

# Configuration
CONTAINER_NAME="vmg-sqlserver"
SA_PASSWORD="YourStrong@Passw0rd"
SQL_PORT="1433"
DATABASE_NAME="Vmg.BillingDB"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop first."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Check if container already exists
if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo "📦 Container '${CONTAINER_NAME}' already exists"
    
    # Check if it's running
    if docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
        echo "✅ Container is already running"
    else
        echo "🔄 Starting existing container..."
        docker start ${CONTAINER_NAME}
        echo "✅ Container started"
    fi
else
    echo "🚀 Creating new SQL Server container..."
    docker run -e "ACCEPT_EULA=Y" \
        -e "MSSQL_SA_PASSWORD=${SA_PASSWORD}" \
        -p ${SQL_PORT}:1433 \
        --name ${CONTAINER_NAME} \
        --hostname ${CONTAINER_NAME} \
        -d mcr.microsoft.com/mssql/server:2022-latest
    
    echo "✅ Container created successfully"
    echo "⏳ Waiting for SQL Server to start (30 seconds)..."
    sleep 30
fi

echo ""
echo "========================================="
echo "SQL Server Information"
echo "========================================="
echo "Container: ${CONTAINER_NAME}"
echo "Host: localhost"
echo "Port: ${SQL_PORT}"
echo "Username: sa"
echo "Password: ${SA_PASSWORD}"
echo "Database: ${DATABASE_NAME}"
echo ""

# Connection string for appsettings.Development.json
CONNECTION_STRING="Server=localhost,${SQL_PORT};Database=${DATABASE_NAME};User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=true;Encrypt=false"

echo "========================================="
echo "Connection String"
echo "========================================="
echo "${CONNECTION_STRING}"
echo ""

echo "========================================="
echo "Next Steps"
echo "========================================="
echo "1. Update appsettings.Development.json with the connection string above"
echo "2. Run: ./setup-database-schema.sh to create tables"
echo "3. Run: ./seed-database.sh to insert sample data"
echo ""
echo "✅ SQL Server setup complete!"
echo ""
echo "To stop SQL Server: docker stop ${CONTAINER_NAME}"
echo "To start SQL Server: docker start ${CONTAINER_NAME}"
echo "To remove SQL Server: docker rm -f ${CONTAINER_NAME}"
