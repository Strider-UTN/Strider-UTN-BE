#!/bin/bash
# Script para ejecutar migraciones de Entity Framework Core
# Uso: ./migrate.sh [connection_string]

set -e

CONNECTION_STRING="${1:-$ConnectionStrings__StriderConnectionString}"

if [ -z "$CONNECTION_STRING" ]; then
    echo "Error: Se requiere una connection string"
    echo "Uso: ./migrate.sh 'postgresql://user:pass@host:port/db'"
    exit 1
fi

echo "Ejecutando migraciones con connection string: ${CONNECTION_STRING:0:30}..."

dotnet ef database update --project StriderWebApi.csproj --connection "$CONNECTION_STRING"

echo "Migraciones completadas exitosamente!"

