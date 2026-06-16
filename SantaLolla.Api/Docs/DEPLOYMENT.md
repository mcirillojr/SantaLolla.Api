# 📦 DEPLOYMENT CONSOLIDADO - SantaLolla API

**Data**: 2024-01-15 | **Versão do Projeto**: .NET 8 | **Porta Swagger**: 6000

---

## 📋 Índice Completo

1. [Visão Geral](#visão-geral)
2. [Arquitetura de Deployment](#arquitetura-de-deployment)
3. [Preparação Local](#preparação-local)
4. [Gerar Release](#gerar-release)
5. [Deploy em Windows Server](#deploy-em-windows-server)
6. [Verificação Pós-Deployment](#verificação-pós-deployment)
7. [Manutenção e Monitoramento](#manutenção-e-monitoramento)
8. [Scripts Disponíveis](#scripts-disponíveis)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

A SantaLolla API agora possui um **pipeline de deployment completo e automatizado** para Windows Server.

### Componentes

- **Swagger**: Porta 6000 (HTTP) e 6001 (HTTPS)
- **Banco de Dados**: SQL Server (connection string em appsettings.Production.json)
- **Runtime**: .NET 8 (framework-dependent ou self-contained)
- **Execução**: Windows Service ou aplicação standalone

### Documentos Disponíveis

| Documento | Tamanho | Propósito |
|-----------|---------|----------|
| **QUICK_START.md** | 6 KB | Início rápido - copie e cole |
| **BUILD_AND_RELEASE.md** | 12 KB | Como gerar pacotes de release |
| **DEPLOYMENT_WINDOWS_SERVER.md** | 15 KB | Guia detalhado deployment Windows |
| **API_FUNCIONAMENTO.md** | 20 KB | Referência de endpoints |
| **TROUBLESHOOTING.md** | 10 KB | Solução de problemas |
| **CASOS_DE_USO.md** | 25 KB | Exemplos de integração |

---

## 🏗️ Arquitetura de Deployment

```
┌──────────────────────────────────────────────────┐
│         CÓDIGO FONTE (.NET 8)                    │
│    C:\C#\SantaLolla.Api\                        │
└────────────────────┬─────────────────────────────┘
                     │
                     ├─→ build-release.ps1
                     │   (compila + publica + empacotha)
                     │
┌────────────────────▼─────────────────────────────┐
│  ARTEFATOS DE RELEASE (ZIP)                      │
│  C:\Publish\SantaLolla-API-v*.zip               │
│                                                   │
│  ├─ framework-dependent (50 MB)                  │
│  │  ↳ Requer .NET 8 Runtime no servidor         │
│  │                                               │
│  └─ self-contained-win-x64 (200 MB)             │
│     ↳ Inclui .NET 8 Runtime                     │
└────────────────────┬─────────────────────────────┘
                     │
                     ├─→ Copiar para servidor
                     │
┌────────────────────▼─────────────────────────────┐
│    SERVIDOR WINDOWS                              │
│    C:\SantaLolla\App\                           │
│                                                   │
│  deploy.ps1                                      │
│  ├─ Extrai arquivos                             │
│  ├─ Configura firewall (portas 6000, 6001)      │
│  ├─ Cria Windows Service (opcional)              │
│  └─ Inicia aplicação                            │
└────────────────────┬─────────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────┐
│      APLICAÇÃO EM PRODUÇÃO                       │
│  http://servidor:6000/swagger                   │
│  https://servidor:6001/ (com SSL)               │
│                                                   │
│  ├─ Database: SQL Server (porta 1433)           │
│  ├─ Logs: C:\SantaLolla\Logs\                   │
│  ├─ Backups: C:\SantaLolla\Backups\             │
│  └─ Certificados: C:\SantaLolla\Certificados\   │
└──────────────────────────────────────────────────┘
```

---

## 🖥️ Preparação Local

### Requisitos

```powershell
# Verificar .NET SDK
dotnet --version          # Deve ser 8.0.x

# SDK instalado?
dotnet --list-sdks        # Deve conter 8.0.x

# Se não: https://dotnet.microsoft.com/download/dotnet/8.0
```

### Clone e Setup

```powershell
# Clone do repositório
git clone https://github.com/mcirillojr/SantaLolla.Api.git
cd SantaLolla.Api

# Restaurar dependências
dotnet restore

# Build
dotnet build -c Release

# Testar localmente
dotnet run
# → http://localhost:6000/swagger
```

---

## 📦 Gerar Release

### Passo 1: Executar Build Script

```powershell
# No diretório raiz do projeto
cd C:\C#\SantaLolla.Api

# Executar script (requer PowerShell 5.0+)
.\build-release.ps1 -Version 1.0.0 -OutputPath C:\Publish

# Parâmetros:
# -Version        (obrigatório) - Versão semântica (ex: 1.0.0)
# -OutputPath     (opcional)   - Local saída (padrão: C:\Publish)
# -BuildType      (opcional)   - Release ou Debug (padrão: Release)
```

### Passo 2: Verificar Output

```powershell
# Verificar arquivos gerados
Get-ChildItem C:\Publish\SantaLolla-API-*.zip

# Resultado esperado:
# - SantaLolla-API-v1.0.0-framework-dependent.zip (~50MB)
# - SantaLolla-API-v1.0.0-self-contained-win-x64.zip (~200MB)
```

### Passo 3: Escolher Pacote

```powershell
# Use self-contained se o servidor for novo/sem .NET
# Use framework-dependent se já tem .NET 8 Runtime

# Verificar tamanho
(Get-Item "C:\Publish\SantaLolla-API-v*.zip" | Measure-Object -Property Length -Sum).Sum / 1MB
```

---

## 🚀 Deploy em Windows Server

### Pré-Requisitos no Servidor

```powershell
# Executar COMO ADMINISTRATOR

# 1. Verificar S.O.
[System.Environment]::OSVersion

# 2. .NET Runtime (se usar framework-dependent)
dotnet --version

# 3. SQL Server acesso
# Verificar se consegue acessar o banco
```

### Instalação Step-by-Step

```powershell
# ===== NO SERVIDOR COMO ADMIN =====

# 1. Criar estrutura de pastas
$basePath = "C:\SantaLolla"
mkdir "$basePath\App" -Force
mkdir "$basePath\Logs" -Force
mkdir "$basePath\Backups" -Force
mkdir "$basePath\Certificados" -Force

# 2. Extrair arquivo ZIP
$zip = "C:\Temp\SantaLolla-API-v1.0.0-self-contained-win-x64.zip"
Expand-Archive -Path $zip -DestinationPath "$basePath\App" -Force

# 3. Copiar deploy script (se não estiver no ZIP)
Copy-Item "deploy.ps1" -Destination $basePath

# 4. Executar deploy script
cd $basePath
.\deploy.ps1 -ZipPath $zip -BasePath $basePath -CreateService $true

# 5. Editar configuração
notepad "$basePath\App\appsettings.Production.json"
```

### Configuração de Produção

Editar `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    // Alterar para seu SQL Server
    "SantaLollaDb": "Server=SEU_SERVIDOR_SQL;Database=SantaLollaIntegracao;User Id=sa;Password=SUA_SENHA;"
  },

  "JwtSettings": {
    // Gerar chave segura (mínimo 32 caracteres)
    "SecretKey": "SUA_CHAVE_SUPER_SEGURA_COM_32_CARACTERES_OU_MAIS"
  },

  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:6000"
      },
      "Https": {
        "Url": "https://0.0.0.0:6001",
        "Certificate": {
          // Certifcado SSL .pfx
          "Path": "C:\\SantaLolla\\Certificados\\certificado.pfx",
          "Password": "SENHA_DO_CERTIFICADO"
        }
      }
    }
  }
}
```

### Iniciar Aplicação

```powershell
# Opção 1: Como Service (recomendado)
Start-Service -Name "SantaLollaAPI"
Get-Service -Name "SantaLollaAPI"

# Opção 2: Manualmente (debug)
cd "C:\SantaLolla\App"
.\SantaLolla.Api.exe

# Opção 3: Via PowerShell
& 'C:\SantaLolla\start.ps1'
```

### Certificado SSL

```powershell
# Copiar certificado .pfx para servidor
Copy-Item -Path "C:\Certificados\seu_dominio.pfx" `
  -Destination "C:\SantaLolla\Certificados\certificado.pfx"

# Importar no Windows (opcional, para IIS)
$pwd = ConvertTo-SecureString "SENHA" -AsPlainText -Force
Import-PfxCertificate -FilePath "C:\SantaLolla\Certificados\certificado.pfx" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -Password $pwd
```

### Firewall

```powershell
# Liberar porta 6000 (HTTP)
netsh advfirewall firewall add rule name="SantaLolla HTTP" `
  dir=in action=allow protocol=tcp localport=6000

# Liberar porta 6001 (HTTPS)
netsh advfirewall firewall add rule name="SantaLolla HTTPS" `
  dir=in action=allow protocol=tcp localport=6001

# Verificar
netsh advfirewall firewall show rule name="SantaLolla*"
```

---

## ✅ Verificação Pós-Deployment

### 1. Verificar Processo

```powershell
# Processo ativo?
Get-Process | Where-Object {$_.Name -like "*SantaLolla*"}

# Service ativo?
Get-Service -Name "SantaLollaAPI" | Select-Object Status, DisplayName
```

### 2. Testar Portas

```powershell
# Portas listening?
netstat -ano | findstr :6000
netstat -ano | findstr :6001

# Ou com PowerShell
Get-NetTcpConnection -LocalPort 6000, 6001
```

### 3. Health Check

```powershell
# HTTP
Invoke-WebRequest -Uri "http://localhost:6000/api/health"

# HTTPS (com certificado auto-assinado)
$cert = New-Object System.Net.Http.HttpClientHandler
$cert.ServerCertificateCustomValidationCallback = {$true}
$client = New-Object System.Net.Http.HttpClient -ArgumentList $cert
$client.GetAsync("https://localhost:6001/api/health").Result
```

### 4. Acessar Swagger

```
Abra no navegador:
HTTP:  http://localhost:6000/swagger
HTTPS: https://localhost:6001/swagger
```

### 5. Verificar Logs

```powershell
# Logs em tempo real
Get-Content -Path "C:\SantaLolla\Logs\*.log" -Tail 20 -Wait

# Últimas 100 linhas
Get-Content -Path "C:\SantaLolla\Logs\*.log" | Select-Object -Last 100 | Out-Host
```

---

## 🔧 Manutenção e Monitoramento

### Monitoramento

```powershell
# Script de monitoramento (salvar como monitor.ps1)
$serviceName = "SantaLollaAPI"

while ($true) {
  $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

  if ($null -eq $service) {
    Write-Host "$(Get-Date): SERVICE NÃO ENCONTRADO!" -ForegroundColor Red
  } elseif ($service.Status -ne "Running") {
    Write-Host "$(Get-Date): Service parado. Reiniciando..." -ForegroundColor Yellow
    Start-Service -Name $serviceName
  } else {
    Write-Host "$(Get-Date): ✓ $serviceName rodando" -ForegroundColor Green
  }

  Start-Sleep -Seconds 60
}

# Executar: . .\monitor.ps1
```

### Backup

```powershell
# Backup diário automático via Task Scheduler
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
  -Argument "-NoProfile -Command `"Copy-Item -Path 'C:\SantaLolla\App' -Destination 'C:\SantaLolla\Backups\backup_\$(Get-Date -Format yyyyMMdd_HHmmss)' -Recurse`""

$trigger = New-ScheduledTaskTrigger -Daily -At 02:00AM

Register-ScheduledTask -TaskName "SantaLolla-Backup" `
  -Action $action `
  -Trigger $trigger `
  -RunLevel Highest
```

### Atualizar Versão

```powershell
# 1. Gerar nova release no desenvolvimento
.\build-release.ps1 -Version 1.0.1

# 2. Copiar ZIP para servidor

# 3. No servidor (como admin):
Stop-Service -Name "SantaLollaAPI"
Start-Sleep -Seconds 2

# Backup versão anterior
Copy-Item "C:\SantaLolla\App" `
  -Destination "C:\SantaLolla\Backups\App_backup_$(Get-Date -Format yyyyMMdd_HHmmss)" `
  -Recurse

# 4. Deploy nova versão
.\deploy.ps1 -ZipPath "C:\Temp\SantaLolla-API-v1.0.1.zip"

# 5. Reiniciar
Start-Service -Name "SantaLollaAPI"
Get-Service -Name "SantaLollaAPI"
```

### Rollback

```powershell
# Se algo der errado:

# 1. Parar serviço
Stop-Service -Name "SantaLollaAPI" -Force

# 2. Restaurar backup
$backup = "C:\SantaLolla\Backups\App_backup_YYYYMMDD_HHMMSS"
Remove-Item "C:\SantaLolla\App" -Recurse -Force
Copy-Item -Path $backup -Destination "C:\SantaLolla\App" -Recurse

# 3. Reiniciar
Start-Service -Name "SantaLollaAPI"

# 4. Verificar
Get-Service -Name "SantaLollaAPI"
```

---

## 🔧 Scripts Disponíveis

### build-release.ps1

**Localização**: `C:\C#\SantaLolla.Api\build-release.ps1`

```powershell
# Uso
.\build-release.ps1 -Version 1.0.0 -OutputPath C:\Publish

# O que faz:
# ✓ Restaura dependências
# ✓ Compila em Release
# ✓ Publica 2 variações (framework-dependent e self-contained)
# ✓ Gera ZIPs
# ✓ Cria manifesto e checksums
# ✓ Copia documentação

# Saída
# C:\Publish\
# ├── SantaLolla-API-v1.0.0-framework-dependent.zip
# ├── SantaLolla-API-v1.0.0-self-contained-win-x64.zip
# ├── SantaLolla_v1.0.0/
# │   ├── manifest.json
# │   ├── checksums.txt
# │   └── [documentação]
```

### deploy.ps1

**Localização**: `C:\C#\SantaLolla.Api\deploy.ps1`

```powershell
# Uso (no servidor como admin)
.\deploy.ps1 -ZipPath "C:\Temp\SantaLolla-API-v1.0.0.zip" -CreateService $true

# Parâmetros
# -ZipPath        (obrigatório) - Caminho do ZIP
# -BasePath       (opcional)   - Local instalação (padrão: C:\SantaLolla)
# -StartService   (opcional)   - Iniciar app (padrão: $true)
# -CreateService  (opcional)   - Criar Windows Service (padrão: $false)

# O que faz:
# ✓ Valida permissões de admin
# ✓ Faz backup da versão anterior
# ✓ Extrai arquivos
# ✓ Configura firewall
# ✓ Cria scripts de inicialização
# ✓ Configura Windows Service
# ✓ Inicia aplicação
```

### start.bat

**Localização**: `C:\SantaLolla\start.bat`

```cmd
# Iniciador simples da aplicação
# Duplo clique para rodar
# Ou: cmd /k "C:\SantaLolla\start. bat"
```

### start.ps1

**Localização**: `C:\SantaLolla\start.ps1`

```powershell
# Iniciador PowerShell
# . .\start.ps1
```

---

## ❌ Troubleshooting

### Swagger não abre

**Sintoma**: `http://localhost:6000/swagger` não responde

```powershell
# 1. Verificar se está rodando
Get-Process | Where-Object {$_.Name -like "*SantaLolla*"}

# 2. Verificar porta
netstat -ano | findstr :6000

# 3. Testar localhost
Invoke-WebRequest -Uri "http://localhost:6000/api/health" -ErrorAction Stop

# 4. Verificar logs
Get-Content -Path "C:\SantaLolla\Logs\*.log" -Tail 50

# 5. Se port 6000 em uso
Get-NetTcpConnection -LocalPort 6000 | Select-Object OwningProcess
Stop-Process -Id <PID> -Force
```

### Connection String errada

**Sintoma**: `SqlException: Cannot open server...`

```powershell
# 1. Verificar connection string
$config = Get-Content "C:\SantaLolla\App\appsettings.Production.json" | ConvertFrom-Json
$config.ConnectionStrings.SantaLollaDb

# 2. Testar conexão SQL
$cs = "Server=SEU_SERVIDOR;Database=SantaLollaIntegracao;User Id=sa;Password=SENHA;"
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = $cs
try {
  $conn.Open()
  Write-Host "✓ Conexão OK"
} catch {
  Write-Host "✗ Erro: $_"
}

# 3. Corrigir e reiniciar
Stop-Service -Name "SantaLollaAPI"
# Editar appsettings.Production.json
Start-Service -Name "SantaLollaAPI"
```

### Certificado SSL inválido

**Sintoma**: HTTPS não funciona

```powershell
# 1. Verificar certificado
Get-ChildItem Cert:\LocalMachine\My | Where-Object {$_.Subject -like "*seu_dominio*"}

# 2. Importar novo
$pwd = ConvertTo-SecureString "SENHA" -AsPlainText -Force
Import-PfxCertificate -FilePath "C:\Certificados\novo.pfx" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -Password $pwd

# 3. Copiar para pasta da app
Copy-Item "C:\Certificados\novo.pfx" `
  -Destination "C:\SantaLolla\Certificados\certificado.pfx" -Force

# 4. Editar appsettings.Production.json com senha correta
# 5. Reiniciar
Restart-Service -Name "SantaLollaAPI"
```

### Service não inicia

**Sintoma**: `Start-Service` falha

```powershell
# 1. Verificar status
Get-Service -Name "SantaLollaAPI" | Select-Object Status, DisplayName

# 2. Ver erro
$service = Get-Service -Name "SantaLollaAPI"
Get-EventLog -LogName Application -InstanceId $service.ServiceHandle -Newest 5

# 3. Tentar iniciar manualmente
cd "C:\SantaLolla\App"
.\SantaLolla.Api.exe

# 4. Se não funcionar, verificar logs
Get-Content -Path "C:\SantaLolla\Logs\*" -Tail 100

# 5. Recriar service
sc.exe delete "SantaLollaAPI"
# Depois executar deploy script novamente
```

### Memória alta / CPU alto

**Sintoma**: Processo consumindo muitos recursos

```powershell
# 1. Monitorar
Get-Process SantaLolla.Api | Select-Object Name, WorkingSet, CPU

# 2. Verificar threads
(Get-Process SantaLolla.Api).Threads.Count

# 3. Coletar logs detalhados
# Adicionar a appsettings.Production.json:
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft": "Warning"
  }
}

# 4. Reiniciar
Restart-Service -Name "SantaLollaAPI"

# 5. Se persistir, verificar banco de dados
# Queries longas? Índices?
```

---

## 📞 Checklist Final

Antes de colocar em produção:

- [ ] .NET 8 Runtime instalado (ou usando self-contained)
- [ ] SQL Server acesso testado
- [ ] Certificado SSL (.pfx) preparado
- [ ] Firewall configurado (portas 6000, 6001)
- [ ] appsettings.Production.json preenchido
- [ ] Pasta C:\SantaLolla criada com permissões corretas
- [ ] Backup do servidor realizado
- [ ] Deploy script testado
- [ ] Health check respondendo
- [ ] Swagger acessível
- [ ] Logs em funcionamento
- [ ] Monitoramento configurado
- [ ] Plano de rollback documentado
- [ ] Team é capaz de revertir em caso de problema

---

## 📚 Documentação Relacionada

| Documento | Link |
|-----------|------|
| Build e Release | `BUILD_AND_RELEASE.md` |
| Deployment Windows | `DEPLOYMENT_WINDOWS_SERVER.md` |
| Quick Start | `QUICK_START.md` |
| API Endpoints | `API_FUNCIONAMENTO.md` |
| Troubleshooting | `TROUBLESHOOTING.md` |
| Casos de Uso | `CASOS_DE_USO.md` |
| README Projeto | `README.md` |

---

**Versão**: 1.0 | **Última atualização**: 2024-01-15 | **Status**: ✅ Pronto para Produção

