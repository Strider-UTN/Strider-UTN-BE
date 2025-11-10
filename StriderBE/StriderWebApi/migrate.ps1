# Script PowerShell para ejecutar migraciones de Entity Framework Core
# Uso: .\migrate.ps1 -ConnectionString "postgresql://user:pass@host:port/db"

param(
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = $env:ConnectionStrings__StriderConnectionString
)

if ([string]::IsNullOrEmpty($ConnectionString)) {
    Write-Host "Error: Se requiere una connection string" -ForegroundColor Red
    Write-Host "Uso: .\migrate.ps1 -ConnectionString 'postgresql://user:pass@host:port/db'" -ForegroundColor Yellow
    exit 1
}

Write-Host "Ejecutando migraciones..." -ForegroundColor Green

dotnet ef database update --project StriderWebApi.csproj --connection $ConnectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migraciones completadas exitosamente!" -ForegroundColor Green
} else {
    Write-Host "Error al ejecutar migraciones" -ForegroundColor Red
    exit $LASTEXITCODE
}

