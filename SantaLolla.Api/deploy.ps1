#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Script de deployment automático para SantaLolla API em Windows Server

.DESCRIPTION
  Copia, configura e inicia a aplicação em Windows Server

.PARAMETER ZipPath
  Caminho do arquivo ZIP publicado

.PARAMETER BasePath
  Caminho base de instalação (padrão: C:\SantaLolla)

.PARAMETER StartService
  Iniciar aplicação após instalação (padrão: $true)

.PARAMETER CreateService
  Criar como Windows Service (padrão: $false)

.EXAMPLE
  .\deploy.ps1 -ZipPath "\\share\SantaLolla-API-v1.0.0.zip"

.EXAMPLE
  .\deploy.ps1 -ZipPath "C:\Publish\SantaLolla-API-v1.0.0.zip" -CreateService $true
#>

param(
  [Parameter(Mandatory=$true)]
  [string]$ZipPath,

  [Parameter(Mandatory=$false)]
  [string]$BasePath = "C:\SantaLolla",

  [Parameter(Mandatory=$false)]
  [bool]$StartService = $true,

  [Parameter(Mandatory=$false)]
  [bool]$CreateService = $false
)

# Cores
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-Success {
  param([string]$Message)
  Write-Host "$Green✅ $Message$Reset"
}

function Write-Warning {
  param([string]$Message)
  Write-Host "$Yellow⚠️  $Message$Reset"
}

function Write-Error {
  param([string]$Message)
  Write-Host "$Red❌ $Message$Reset"
}

function Write-Info {
  param([string]$Message)
  Write-Host "$Blue ℹ️  $Message$Reset"
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

Write-Section "🚀 DEPLOYMENT - SantaLolla API para Windows Server"

Write-Info "ZIP: $ZipPath"
Write-Info "Local: $BasePath"
Write-Info "Service: $(if ($CreateService) { 'Sim' } else { 'Não' })"
Write-Host ""

# 1. Validar permissões de Admin
Write-Section "1. VALIDANDO PERMISSÕES"

$isAdmin = [Security.Principal.WindowsIdentity]::GetCurrent().Groups -contains [Security.Principal.SecurityIdentifier]'S-1-5-32-544'
if (-not $isAdmin) {
  Write-Error "Este script deve ser executado como Administrator"
  exit 1
}
Write-Success "Executando como Administrator"

# 2. Validar arquivo ZIP
Write-Section "2. VALIDANDO ARQUIVO"

if (-not (Test-Path $ZipPath)) {
  Write-Error "Arquivo não encontrado: $ZipPath"
  exit 1
}

$zipFile = Get-Item $ZipPath
Write-Success "Arquivo encontrado"
Write-Info "Tamanho: $($zipFile.Length / 1MB -as [int])MB"
Write-Info "Data: $($zipFile.LastWriteTime)"

# 3. Fazer backup
Write-Section "3. BACKUP DA VERSÃO ANTERIOR"

if (Test-Path "$BasePath\App") {
  $backupPath = "$BasePath\Backups\App_backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
  Write-Host "Criando backup em: $backupPath"
  Copy-Item -Path "$BasePath\App" -Destination $backupPath -Recurse -Force | Out-Null
  Write-Success "Backup realizado"
} else {
  Write-Warning "Nenhuma versão anterior encontrada (primeira instalação)"
}

# 4. Preparar diretórios
Write-Section "4. PREPARANDO DIRETÓRIOS"

@(
  "$BasePath\App",
  "$BasePath\Logs",
  "$BasePath\Data",
  "$BasePath\Backups",
  "$BasePath\Certificados"
) | ForEach-Object {
  if (-not (Test-Path $_)) {
    mkdir $_ -Force | Out-Null
    Write-Success "Criado: $_"
  } else {
    Write-Info "Já existe: $_"
  }
}

# 5. Extrair arquivo ZIP
Write-Section "5. EXTRAINDO APLICAÇÃO"

# Limpar diretório de app (exceto configurações)
$appPath = "$BasePath\App"
if (Test-Path $appPath) {
  Write-Host "Removendo versão anterior..."
  Get-ChildItem -Path $appPath -Exclude "appsettings.Production.json" -Force | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Extraindo $($zipFile.Name)..."
Expand-Archive -Path $ZipPath -DestinationPath $appPath -Force

Write-Success "Aplicação extraída"

# 6. Copiar configuração
Write-Section "6. CONFIGURANDO APLICAÇÃO"

$prodConfig = "$appPath\appsettings.Production.json"
if (Test-Path $prodConfig) {
  Write-Success "Arquivo de configuração encontrado"
  Write-Warning "⚠️  IMPORTANTE: Editar $prodConfig com dados de produção:"
  Write-Host ""
  Write-Host "   - Connection String do SQL Server"
  Write-Host "   - JWT Secret Key"
  Write-Host "   - Caminho do certificado SSL"
  Write-Host "   - Senha do certificado"
  Write-Host ""
} else {
  Write-Error "Arquivo de configuração não encontrado!"
  Write-Info "Criando template..."
  $template = @'
{
  "ConnectionStrings": {
    "SantaLollaDb": "Server=SERVIDOR_SQL;Database=SantaLollaIntegracao;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "CHAVE-SECRETA-SUPER-SEGURA-MINIMO-32-CARACTERES"
  },
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:6000"
      },
      "Https": {
        "Url": "https://0.0.0.0:6001",
        "Certificate": {
          "Path": "C:\\SantaLolla\\Certificados\\certificado.pfx",
          "Password": "SENHA_DO_CERTIFICADO"
        }
      }
    }
  },
  "AllowedHosts": "*",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
'@
  $template | Out-File $prodConfig -Encoding UTF8
  Write-Success "Template criado em: $prodConfig"
}

# Criar pasta de logs se não existir
$logsPath = "$BasePath\Logs"
if (-not (Test-Path $logsPath)) {
  mkdir $logsPath -Force | Out-Null
}

# 7. Configurar Firewall
Write-Section "7. CONFIGURANDO FIREWALL"

@(
  @{
    Name = "SantaLolla API HTTP"
    Port = 6000
  },
  @{
    Name = "SantaLolla API HTTPS"
    Port = 6001
  }
) | ForEach-Object {
  $ruleName = $_.Name
  $port = $_.Port

  # Remover regra antiga se existir
  netsh advfirewall firewall delete rule name="$ruleName" 2>$null

  # Criar nova regra
  netsh advfirewall firewall add rule name="$ruleName" `
    dir=in action=allow protocol=tcp localport=$port enable=yes 2>$null

  Write-Success "Porta $port liberada"
}

# 8. Verificar .NET Runtime
Write-Section "8. VERIFICANDO .NET RUNTIME"

$runtimePath = "$appPath\shared\Microsoft.NETCore.App" 
if (Test-Path $runtimePath) {
  Write-Success "Self-contained: .NET runtime incluído no pacote"
} else {
  Write-Warning "Framework-dependent: Verificando .NET 8 Runtime no sistema..."

  try {
    $version = (dotnet --version)
    if ($version -match "8\.") {
      Write-Success ".NET 8 encontrado: $version"
    } else {
      Write-Error ".NET 8 não encontrado! Versão instalada: $version"
      Write-Error "Instale .NET 8 Runtime de: https://dotnet.microsoft.com/download/dotnet/8.0"
      exit 1
    }
  } catch {
    Write-Error "dotnet não encontrado no PATH"
    exit 1
  }
}

# 9. Tornar executável (se necessário)
Write-Section "9. PREPARANDO EXECUTÁVEL"

$exePath = "$appPath\SantaLolla.Api.exe"
if (Test-Path $exePath) {
  Write-Success "Executável encontrado"
  # Adicionar permissões de execução
  $acl = Get-Acl $exePath
  $acl.SetAccessRuleProtection($false, $false)
  Set-Acl -Path $exePath -AclObject $acl | Out-Null
} else {
  Write-Error "Executável não encontrado em: $exePath"
  exit 1
}

# 10. Criar arquivo de inicialização
Write-Section "10. CRIANDO SCRIPTS DE INICIALIZAÇÃO"

$startBatchPath = "$BasePath\start.bat"
$startBatchContent = @"
@echo off
REM SantaLolla API - Script de inicialização
setlocal enabledelayedexpansion

echo Iniciando SantaLolla API...
cd /d "$appPath"

REM Definir variáveis de ambiente
set ASPNETCORE_ENVIRONMENT=Production
set ASPNETCORE_URLS=http://0.0.0.0:6000;https://0.0.0.0:6001

REM Iniciar aplicação
"$exePath"

REM Se sair, aguardar antes de fechar
if !ERRORLEVEL! neq 0 (
  echo.
  echo ERRO: Aplicacao finalizou com erro code !ERRORLEVEL!
  echo Verifique os logs em: $logsPath
  pause
)
"@
$startBatchContent | Out-File $startBatchPath -Encoding ASCII
Write-Success "Script de inicialização criado"

$startPsPath = "$BasePath\start.ps1"
$startPsContent = @"
# SantaLolla API - Script PowerShell
`$env:ASPNETCORE_ENVIRONMENT = 'Production'
`$env:ASPNETCORE_URLS = 'http://0.0.0.0:6000;https://0.0.0.0:6001'

Push-Location '$appPath'
Write-Host 'Iniciando SantaLolla API em porta 6000...' -ForegroundColor Green

& '.\SantaLolla.Api.exe'

if (`$LASTEXITCODE -ne 0) {
  Write-Host "Erro ao executar. Code: `$LASTEXITCODE" -ForegroundColor Red
  Write-Host "Verifique logs em: $logsPath"
}
"@
$startPsContent | Out-File $startPsPath -Encoding UTF8
Write-Success "Script PowerShell criado"

# 11. Criar serviço (opcional)
Write-Section "11. CONFIGURAÇÃO DE SERVIÇO"

$serviceName = "SantaLollaAPI"

if ($CreateService) {
  Write-Host "Criando Windows Service..."

  # Verificar se já existe
  if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
    Write-Warning "Serviço já existe. Removendo..."
    Stop-Service -Name $serviceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Remove-Item "HKLM:\SYSTEM\CurrentControlSet\Services\$serviceName" -Force -ErrorAction SilentlyContinue
  }

  # Criar serviço com sc.exe
  Write-Host "Criando serviço com sc.exe..."

  $scPath = "C:\SantaLolla\start.bat"
  sc.exe create $serviceName `
    binPath= "cmd.exe /c `"$scPath`"" `
    displayName= "SantaLolla API Service" `
    start= auto `
    error= critical 2>$null

  if ($LASTEXITCODE -eq 0) {
    Write-Success "Serviço criado"
    Write-Info "Para gerenciar:"
    Write-Host "   Start:   Start-Service -Name '$serviceName'"
    Write-Host "   Stop:    Stop-Service -Name '$serviceName'"
    Write-Host "   Status:  Get-Service -Name '$serviceName'"
    Write-Host "   Remove:  sc.exe delete '$serviceName'"
  } else {
    Write-Error "Erro ao criar serviço"
    Write-Warning "Alternativa: Use Task Scheduler ou nssm"
  }
} else {
  Write-Info "Service não será criado (use -CreateService $true para criar)"
  Write-Info "Alternativas:"
  Write-Host "   1. Executar manualmente: $startBatchPath"
  Write-Host "   2. Criar Task Scheduler: schtasks /create /tn SantaLolla /tr \"$startBatchPath\" /sc onstart /ru SYSTEM"
  Write-Host "   3. Instalar nssm: https://nssm.cc/"
}

# 12. Teste de conectividade
Write-Section "12. TESTE DE CONECTIVIDADE"

Write-Host "Testando conexão com banco de dados..."
Write-Warning "⚠️  Você deve editar appsettings.Production.json ANTES de iniciar"

# 13. Iniciar aplicação
Write-Section "13. INICIANDO APLICAÇÃO"

if ($StartService) {
  Write-Host "Aguarde enquanto a aplicação inicia..."
  Start-Sleep -Seconds 2

  try {
    $process = Start-Process -FilePath $exePath `
      -WorkingDirectory $appPath `
      -UserName $env:USERNAME `
      -NoNewWindow `
      -PassThru

    Write-Success "Processo iniciado (PID: $($process.Id))"
    Write-Info "Aguarde 3-5 segundos para inicialização..."
    Start-Sleep -Seconds 5

    # Verificar se ainda está rodando
    if (Get-Process -Id $process.Id -ErrorAction SilentlyContinue) {
      Write-Success "Aplicação rodando"
    } else {
      Write-Error "Processo finalizou inesperadamente"
      Write-Info "Verifique configurações em: $prodConfig"
    }
  } catch {
    Write-Error "Erro ao iniciar: $_"
  }
} else {
  Write-Info "Inicialização manual. Execute:"
  Write-Host "   $startBatchPath"
}

# 14. Resumo e instruções
Write-Section "✅ DEPLOYMENT CONCLUÍDO"

Write-Host "📍 INFORMAÇÕES DE ACESSO:"
Write-Host ""
Write-Host "   HTTP:           http://$(hostname):6000"
Write-Host "   HTTPS:          https://$(hostname):6001"
Write-Host "   Swagger:        http://$(hostname):6000/swagger"
Write-Host "   Health Check:   http://$(hostname):6000/api/health"
Write-Host ""

Write-Host "📁 DIRETÓRIOS IMPORTANTES:"
Write-Host ""
Write-Host "   Aplicação:      $appPath"
Write-Host "   Configuração:   $prodConfig"
Write-Host "   Logs:           $logsPath"
Write-Host "   Backups:        $BasePath\Backups"
Write-Host ""

Write-Host "⚙️  PRÓXIMOS PASSOS:"
Write-Host ""
Write-Host "   1. EDITAR CONFIGURAÇÃO:"
Write-Host "      notepad '$prodConfig'"
Write-Host ""
Write-Host "   2. INICIAR APLICAÇÃO (se não iniciou):"
Write-Host "      $startBatchPath"
Write-Host ""
Write-Host "   3. VERIFICAR LOGS:"
Write-Host "      Get-Content -Path '$logsPath\*.log' -Tail 50 -Wait"
Write-Host ""
Write-Host "   4. CRIAR CERTIFICADO SSL (para HTTPS):"
Write-Host "      Copie seu .pfx para: $BasePath\Certificados\certificado.pfx"
Write-Host ""
Write-Host "   5. MONITORAR:"
Write-Host "      Get-Service -Name '$serviceName' | Select-Object Status, Name"
Write-Host ""

Write-Host "📚 DOCUMENTAÇÃO:"
Write-Host "   $appPath\DEPLOYMENT_WINDOWS_SERVER.md"
Write-Host "   $appPath\API_FUNCIONAMENTO.md"
Write-Host ""

Write-Host "🆘 TROUBLESHOOTING:"
Write-Host "   http://localhost:6000/api/health"
Write-Host ""
