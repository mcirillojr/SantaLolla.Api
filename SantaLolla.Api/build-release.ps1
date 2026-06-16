#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Script de build e packaging automático para SantaLolla API

.DESCRIPTION
  Compila, publica e gera pacotes de instalação para Windows Server

.PARAMETER Version
  Versão a publicar (ex: 1.0.0)

.PARAMETER OutputPath
  Caminho de saída dos pacotes (padrão: C:\Publish)

.PARAMETER BuildType
  Tipo de build: Release, Debug (padrão: Release)

.EXAMPLE
  .\build-release.ps1 -Version 1.0.0 -OutputPath D:\Deploy
#>

param(
  [Parameter(Mandatory=$true)]
  [string]$Version,

  [Parameter(Mandatory=$false)]
  [string]$OutputPath = "C:\Publish",

  [Parameter(Mandatory=$false)]
  [string]$BuildType = "Release"
)

# Cores para output
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Reset = "`e[0m"

function Write-Success {
  param([string]$Message)
  Write-Host "$Green✅ $Message$Reset" -ForegroundColor Green
}

function Write-Warning {
  param([string]$Message)
  Write-Host "$Yellow⚠️  $Message$Reset" -ForegroundColor Yellow
}

function Write-Error {
  param([string]$Message)
  Write-Host "$Red❌ $Message$Reset" -ForegroundColor Red
}

function Write-Section {
  param([string]$Title)
  Write-Host ""
  Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
  Write-Host "  $Title"
  Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
  Write-Host ""
}

# ==================== INÍCIO ====================

Write-Section "🚀 BUILD E PACKAGING - SantaLolla API v$Version"

Write-Host "Versão:       $Version"
Write-Host "Build Type:   $BuildType"
Write-Host "Output:       $OutputPath"
Write-Host ""

# 1. Validar diretório do projeto
Write-Section "1. VALIDANDO AMBIENTE"

$projectPath = "C:\C#\SantaLolla.Api\SantaLolla.Api"
if (-not (Test-Path "$projectPath\SantaLolla.Api.csproj")) {
  Write-Error "Arquivo .csproj não encontrado em $projectPath"
  exit 1
}
Write-Success "Projeto encontrado: $projectPath"

# 2. Limpar e preparar output
Write-Section "2. PREPARANDO DIRETÓRIOS"

$buildOutputPath = "$OutputPath\SantaLolla_v$Version"
$publishPath = "$buildOutputPath\publish"
$zipPath = "$OutputPath\SantaLolla-API-v$Version.zip"
$portablePath = "$buildOutputPath\portable"

Remove-Item -Path $buildOutputPath -Recurse -Force -ErrorAction SilentlyContinue
mkdir "$buildOutputPath" -Force | Out-Null
mkdir "$publishPath" -Force | Out-Null

Write-Success "Diretórios preparados"

# 3. Restaurar dependências
Write-Section "3. RESTAURANDO DEPENDÊNCIAS"

Push-Location $projectPath
dotnet restore
if ($LASTEXITCODE -ne 0) {
  Write-Error "Falha ao restaurar dependências"
  Pop-Location
  exit 1
}
Write-Success "Dependências restauradas"

# 4. Build
Write-Section "4. COMPILANDO PROJETO"

dotnet build -c $BuildType
if ($LASTEXITCODE -ne 0) {
  Write-Error "Build failed"
  Pop-Location
  exit 1
}
Write-Success "Build $BuildType concluído"

# 5. Publicar - Framework-dependent
Write-Section "5. PUBLICANDO - Framework-Dependent"

dotnet publish -c $BuildType -o "$publishPath\framework-dependent" `
  --no-restore --no-build `
  -p:PublishReadyToRun=true `
  -p:PublishTrimmed=true

if ($LASTEXITCODE -ne 0) {
  Write-Error "Publicação Framework-dependent falhou"
  Pop-Location
  exit 1
}
Write-Success "Framework-dependent publicado"

# 6. Publicar - Self-contained
Write-Section "6. PUBLICANDO - Self-Contained (Windows x64)"

dotnet publish -c $BuildType -o "$publishPath\self-contained-win-x64" `
  --no-restore --no-build `
  -r win-x64 `
  --self-contained `
  -p:PublishReadyToRun=true `
  -p:PublishSingleFile=true `
  -p:PublishTrimmed=true `
  -p:SelfContainedRuntimeIdentifier=win-x64

if ($LASTEXITCODE -ne 0) {
  Write-Error "Self-contained publishing falhou"
  Pop-Location
  exit 1
}
Write-Success "Self-contained (win-x64) publicado"

# 7. Criar pacotes ZIP
Write-Section "7. CRIANDO PACOTES"

Write-Host "Criando Framework-Dependent ZIP..."
$frameworkZip = "$OutputPath\SantaLolla-API-v$Version-framework-dependent.zip"
Compress-Archive -Path "$publishPath\framework-dependent\*" -DestinationPath $frameworkZip -Force
Write-Success "Framework-Dependent: $frameworkZip"

Write-Host "Criando Self-Contained ZIP..."
$selfContainedZip = "$OutputPath\SantaLolla-API-v$Version-self-contained-win-x64.zip"
Compress-Archive -Path "$publishPath\self-contained-win-x64\*" -DestinationPath $selfContainedZip -Force
Write-Success "Self-Contained: $selfContainedZip"

# 8. Criar arquivo de manifesto
Write-Section "8. CRIANDO MANIFESTO"

$manifest = @"
{
  "version": "$Version",
  "buildDate": "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
  "buildType": "$BuildType",
  "dotnetVersion": "$(dotnet --version)",
  "packages": {
    "framework-dependent": {
      "file": "SantaLolla-API-v$Version-framework-dependent.zip",
      "requiresRuntime": true,
      "description": "Requer .NET 8 Runtime instalado no servidor"
    },
    "self-contained": {
      "file": "SantaLolla-API-v$Version-self-contained-win-x64.zip",
      "requiresRuntime": false,
      "description": "Inclui .NET 8 Runtime (pacote maior)"
    }
  },
  "requirements": {
    "windows": "Windows Server 2019 ou superior",
    "memory": "2GB mínimo, 4GB recomendado",
    "disk": "500MB espaço livre",
    "ports": ["6000 (HTTP)", "6001 (HTTPS)"]
  },
  "deployment": {
    "manual": "Copiar arquivos para C:\\SantaLolla\\App",
    "asService": "Configurar como Windows Service",
    "documentation": "Consultar DEPLOYMENT_WINDOWS_SERVER.md"
  }
}
"@

$manifest | Out-File "$buildOutputPath\manifest.json" -Encoding UTF8
Write-Success "Manifesto criado: $buildOutputPath\manifest.json"

# 9. Copiar arquivos de configuração
Write-Section "9. COPIANDO ARQUIVOS DE CONFIGURAÇÃO"

Copy-Item -Path "$projectPath\appsettings.json" -Destination "$buildOutputPath\appsettings.json" -Force
Copy-Item -Path "$projectPath\appsettings.Production.json" -Destination "$buildOutputPath\appsettings.Production.json" -Force
Copy-Item -Path "$projectPath\..\Docs\DEPLOYMENT_WINDOWS_SERVER.md" -Destination "$buildOutputPath\DEPLOYMENT_WINDOWS_SERVER.md" -Force
Copy-Item -Path "$projectPath\..\Docs\API_FUNCIONAMENTO.md" -Destination "$buildOutputPath\API_FUNCIONAMENTO.md" -Force
Copy-Item -Path "$projectPath\..\README.md" -Destination "$buildOutputPath\README.md" -Force

Write-Success "Arquivos de configuração copiados"

# 10. Criar script de instalação
Write-Section "10. CRIANDO SCRIPTS DE INSTALAÇÃO"

$installScript = @"
@echo off
REM Script de instalação automática para SantaLolla API
REM Executar como Administrator

setlocal enabledelayedexpansion

echo.
echo ================================================
echo  SantaLolla API - Instalador para Windows
echo ================================================
echo.

REM Verificar se rodando como admin
net session >nul 2>&1
if %errorlevel% neq 0 (
  echo ERRO: Este script deve ser executado como Administrator
  pause
  exit /b 1
)

REM Criar pastas
echo [1/5] Criando diretórios...
if not exist C:\SantaLolla\App mkdir C:\SantaLolla\App
if not exist C:\SantaLolla\Logs mkdir C:\SantaLolla\Logs
if not exist C:\SantaLolla\Backups mkdir C:\SantaLolla\Backups

REM Copiar arquivos
echo [2/5] Copiando arquivos...
xcopy /E /I /Y "self-contained-win-x64\*" "C:\SantaLolla\App\"

REM Copiar config
echo [3/5] Configurando appsettings...
copy appsettings.Production.json "C:\SantaLolla\App\"

REM Configurar firewall
echo [4/5] Configurando firewall...
netsh advfirewall firewall add rule name="SantaLolla API HTTP" dir=in action=allow protocol=tcp localport=6000 enable=yes >nul 2>&1
netsh advfirewall firewall add rule name="SantaLolla API HTTPS" dir=in action=allow protocol=tcp localport=6001 enable=yes >nul 2>&1

REM Criar serviço (usando Task Scheduler como alternativa)
echo [5/5] Preparando para inicialização...
echo Pronto! A aplicação está em C:\SantaLolla\App

echo.
echo PRÓXIMOS PASSOS:
echo 1. Editar: C:\SantaLolla\App\appsettings.Production.json
echo    - Alterar connection string
echo    - Alterar JWT Secret
echo    - Alterar caminho do certificado
echo.
echo 2. Iniciar manualmente:
echo    cd C:\SantaLolla\App
echo    .\SantaLolla.Api.exe
echo.
echo 3. Ou configurar como serviço:
echo    Consultar DEPLOYMENT_WINDOWS_SERVER.md
echo.
echo Para mais informações, abra: DEPLOYMENT_WINDOWS_SERVER.md
pause
"@

$installScript | Out-File "$buildOutputPath\install.bat" -Encoding ASCII
Write-Success "Script de instalação criado: $buildOutputPath\install.bat"

# 11. Gerar checksums
Write-Section "11. GERANDO CHECKSUMS (SHA256)"

$checksums = @()
Get-ChildItem "$OutputPath\SantaLolla-API-v$Version*.zip" | ForEach-Object {
  $hash = (Get-FileHash -Path $_.FullName -Algorithm SHA256).Hash
  $checksums += "$($_.Name): $hash"
  Write-Host "  $($_.Name)"
  Write-Host "    SHA256: $hash"
}

"# SantaLolla API v$Version - Checksums`n`n" + ($checksums -join "`n") | `
  Out-File "$buildOutputPath\checksums.txt" -Encoding UTF8

Write-Success "Checksums gerados"

Pop-Location

# 12. Resumo Final
Write-Section "✅ BUILD CONCLUÍDO COM SUCESSO"

Write-Host "📦 PACOTES GERADOS:"
Write-Host ""
Write-Host "Local de saída: $OutputPath"
Write-Host ""
Write-Host "  📁 Framework-Dependent (requer .NET 8 no servidor):"
Write-Host "     $frameworkZip"
Write-Host ""
Write-Host "  📁 Self-Contained (inclui runtime .NET 8):"
Write-Host "     $selfContainedZip"
Write-Host ""
Write-Host "📋 ARQUIVOS INCLUSOS:"
Write-Host "   ✓ appsettings.json"
Write-Host "   ✓ appsettings.Production.json"
Write-Host "   ✓ README.md"
Write-Host "   ✓ API_FUNCIONAMENTO.md"
Write-Host "   ✓ DEPLOYMENT_WINDOWS_SERVER.md"
Write-Host "   ✓ install.bat"
Write-Host "   ✓ manifest.json"
Write-Host "   ✓ checksums.txt"
Write-Host ""

Write-Host "🚀 PRÓXIMOS PASSOS:"
Write-Host "   1. Copiar o arquivo ZIP para o servidor Windows"
Write-Host "   2. Extrair o arquivo"
Write-Host "   3. Executar install.bat como Administrator"
Write-Host "   4. Editar appsettings.Production.json com dados de produção"
Write-Host "   5. Iniciar a aplicação"
Write-Host ""

Write-Host "📚 MAIS INFORMAÇÕES:"
Write-Host "   Consulte: $buildOutputPath\DEPLOYMENT_WINDOWS_SERVER.md"
Write-Host ""
