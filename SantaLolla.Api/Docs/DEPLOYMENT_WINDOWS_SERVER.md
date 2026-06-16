# 🚀 Guia de Deployment - SantaLolla API para Windows Server

## Índice
1. [Pré-Requisitos](#pré-requisitos)
2. [Preparação do Servidor](#preparação-do-servidor)
3. [Instalação Manual](#instalação-manual)
4. [Instalação com Serviço Windows](#instalação-com-serviço-windows)
5. [Configuração de Produção](#configuração-de-produção)
6. [Verificação e Testes](#verificação-e-testes)
7. [Troubleshooting](#troubleshooting)

---

## Pré-Requisitos

### Software Necessário
- ✅ Windows Server 2019 ou superior
- ✅ .NET 8 Runtime (ou .NET 8 SDK para desenvolvimento)
- ✅ SQL Server 2019 ou superior
- ✅ Certificado SSL (.pfx) para HTTPS

### Download de Dependências

```powershell
# 1. Instalar .NET 8 Runtime
# Baixar de: https://dotnet.microsoft.com/download/dotnet/8.0
# Versão: ASP.NET Core Runtime 8.0

# 2. SQL Server
# Já deve estar instalado e configurado

# 3. Certificado SSL
# Deve estar disponível em formato .pfx com senha
```

### Requisitos de Sistema
- **CPU**: Mínimo 2 cores, recomendado 4+
- **RAM**: Mínimo 2GB, recomendado 4GB+
- **Disco**: Mínimo 500MB espaço livre
- **Porta**: 6000 (HTTP) e 6001 (HTTPS)

---

## Preparação do Servidor

### 1. Criar Pastas de Instalação

```powershell
# Abrir PowerShell como Admin

# Criar estrutura de pastas
$basePath = "C:\SantaLolla"
mkdir "$basePath\App" -Force
mkdir "$basePath\Certificados" -Force
mkdir "$basePath\Logs" -Force
mkdir "$basePath\Backups" -Force

# Definir permissões
$acl = Get-Acl "$basePath"
$acl.SetAccessRuleProtection($false, $false)
Set-Acl -Path "$basePath" -AclObject $acl

Write-Host "✅ Pastas criadas em $basePath"
```

### 2. Copiar Certificado SSL

```powershell
# Copiar arquivo .pfx para pasta de certificados
Copy-Item -Path "C:\Certificados\seudominio.pfx" -Destination "C:\SantaLolla\Certificados\certificado.pfx"

Write-Host "✅ Certificado copiado"
```

### 3. Configurar Firewall

```powershell
# Permitir porta 6000 (HTTP)
netsh advfirewall firewall add rule name="SantaLolla API HTTP" `
  dir=in action=allow protocol=tcp localport=6000 enable=yes

# Permitir porta 6001 (HTTPS)
netsh advfirewall firewall add rule name="SantaLolla API HTTPS" `
  dir=in action=allow protocol=tcp localport=6001 enable=yes

# Verificar regras
netsh advfirewall firewall show rule name="SantaLolla*"
```

---

## Instalação Manual

### 1. Publicar Aplicação

```bash
# No seu computador de desenvolvimento
cd C:\C#\SantaLolla.Api

# Publicar Release
dotnet publish -c Release -o "C:\Publish\SantaLolla"

# Resultado: arquivos publicados em C:\Publish\SantaLolla
```

### 2. Copiar para Servidor

```powershell
# Via RDP copiar pasta C:\Publish\SantaLolla para:
# \\servidor\C$\SantaLolla\App

# Ou via UNC
Copy-Item -Path "C:\Publish\SantaLolla\*" `
  -Destination "\\SERVIDOR\C$\SantaLolla\App" `
  -Recurse -Force
```

### 3. Editar appsettings.Production.json

```powershell
# Abrir arquivo no editor
notepad "C:\SantaLolla\App\appsettings.Production.json"
```

Alterar:
```json
{
  "ConnectionStrings": {
    "SantaLollaDb": "Server=SERVIDOR_SQL;Database=SantaLollaIntegracao;User Id=sa;Password=SENHA_SA;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "CHAVE-SECRETA-SUPER-SEGURA-MINIMO-32-CARACTERES"
  },
  "Kestrel": {
    "EndPoints": {
      "Https": {
        "Certificate": {
          "Password": "SENHA_DO_CERTIFICADO"
        }
      }
    }
  },
  "AllowedHosts": "seudominio.com.br"
}
```

### 4. Testar Manualmente

```powershell
cd "C:\SantaLolla\App"

# Executar
.\SantaLolla.Api.exe

# Acessar
# HTTP:  http://localhost:6000/swagger
# HTTPS: https://localhost:6001/swagger

# Parar: Ctrl+C
```

---

## Instalação com Serviço Windows

### 1. Instalar como Serviço

```powershell
# Criar arquivo de configuração do serviço
# Arquivo: C:\SantaLolla\App\web.config

$webConfig = @'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <aspNetCore processPath="SantaLolla.Api.exe" 
                arguments="" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout">
      <environmentVariables>
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        <environmentVariable name="ASPNETCORE_URLS" value="http://0.0.0.0:6000;https://0.0.0.0:6001" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>
'@

$webConfig | Out-File "C:\SantaLolla\App\web.config" -Encoding UTF8
```

### 2. Usar ServiceMonitor (Recomendado)

```powershell
# Baixar Windows Hosting Bundle que inclui Services
# https://dotnet.microsoft.com/download/dotnet/8.0

# Instalar .NET Hosting Bundle

# Depois, criar o serviço via PowerShell:
$serviceName = "SantaLollaAPI"
$appPath = "C:\SantaLolla\App"
$exePath = "$appPath\SantaLolla.Api.exe"

# Criar arquivo batch para iniciar
$batchContent = @"
@echo off
cd /d "$appPath"
set ASPNETCORE_ENVIRONMENT=Production
"$exePath"
"@

$batchContent | Out-File "$appPath\start.bat" -Encoding ASCII

# Criar serviço usando NSSM (Non-Sucking Service Manager)
# Baixar: https://nssm.cc/download

# Ou usar sc.exe nativo:
sc.exe create SantaLollaAPI `
  binPath= "$appPath\start.bat" `
  DisplayName= "SantaLolla API Service" `
  start= auto

# Iniciar serviço
Start-Service -Name "SantaLollaAPI"

# Verificar status
Get-Service "SantaLollaAPI"
```

### 3. Alternativa: Usar Publish como Executável

```powershell
# Criar script de inicialização PS1
$psScript = @"
`$env:ASPNETCORE_ENVIRONMENT = "Production"
Push-Location "C:\SantaLolla\App"
.\SantaLolla.Api.exe
"@

$psScript | Out-File "C:\SantaLolla\App\run.ps1" -Encoding UTF8

# Agendar no Task Scheduler
# Ação: powershell -ExecutionPolicy Bypass -File "C:\SantaLolla\App\run.ps1"
```

### 4. Configurar Task Scheduler (Ou verificação periódica)

```powershell
# Criar verificação automática e reinício se cair

$taskName = "SantaLolla-Monitor"
$taskAction = New-ScheduledTaskAction `
  -Execute "PowerShell.exe" `
  -Argument "-NoProfile -WindowStyle Hidden -Command `
    & { if (-not (Get-Process SantaLolla.Api -ErrorAction SilentlyContinue)) { `
      Start-Process -FilePath 'C:\SantaLolla\App\SantaLolla.Api.exe' -WorkingDirectory 'C:\SantaLolla\App' } }"

$taskTrigger = New-ScheduledTaskTrigger -AtStartup
$taskSettings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable

Register-ScheduledTask -TaskName $taskName `
  -Action $taskAction `
  -Trigger $taskTrigger `
  -Settings $taskSettings `
  -RunLevel Highest -Force

Write-Host "✅ Task Scheduler configurado para monitorar SantaLolla"
```

---

## Configuração de Produção

### 1. Validar Configurações

```powershell
# Verificar arquivo Production
$config = Get-Content "C:\SantaLolla\App\appsettings.Production.json" | ConvertFrom-Json
Write-Host "Banco de Dados: $($config.ConnectionStrings.SantaLollaDb)"
Write-Host "JWT Expiration: $($config.JwtSettings.ExpirationMinutes) minutos"
```

### 2. Verificar Certificado SSL

```powershell
# Validar certificado
$cert = Get-Item "C:\SantaLolla\Certificados\certificado.pfx"
Write-Host "✅ Certificado encontrado: $($cert.FullName)"

# Verificar validade (opcional, com senha)
# $pwd = ConvertTo-SecureString "SENHA" -AsPlainText -Force
# $cert = Import-PfxCertificate -FilePath "C:\SantaLolla\Certificados\certificado.pfx" -CertStoreLocation "Cert:\LocalMachine\My" -Password $pwd
```

### 3. Configurar Reverse Proxy (Recomendado)

```
Se usando IIS como reverse proxy:

Application Pool:
- .NET CLR Version: Sem Código Gerenciado
- Managed Pipeline Mode: Integrated

Binding:
- HTTP: Port 80 → 6000
- HTTPS: Port 443 → 6001 (com certificado)
```

---

## Verificação e Testes

### 1. Testar Conectividade

```powershell
# Verificar se portas estão listening
netstat -ano | findstr :6000
netstat -ano | findstr :6001

# Testar localhost
$response = Invoke-WebRequest -Uri "http://localhost:6000/swagger" -ErrorAction SilentlyContinue
if ($response.StatusCode -eq 200) {
  Write-Host "✅ API respondendo em HTTP"
} else {
  Write-Host "❌ Erro na porta 6000"
}
```

### 2. Testar Autenticação

```powershell
# Obter token
$auth = @{
  "clientId" = "terceiroapi-001"
  "clientSecret" = "SUA_CHAVE"
}

$response = Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:6000/api/auth/token" `
  -Body ($auth | ConvertTo-Json) `
  -ContentType "application/json"

Write-Host "Token: $($response.accessToken)"
```

### 3. Testar Health Check

```powershell
$token = "SEU_TOKEN_AQUI"
$response = Invoke-RestMethod `
  -Method Get `
  -Uri "http://localhost:6000/api/health" `
  -Headers @{ Authorization = "Bearer $token" }

Write-Host $response | ConvertTo-Json
```

### 4. Verificar Logs

```powershell
# Verificar logs em tempo real
Get-Content -Path "C:\SantaLolla\Logs\*.log" -Tail 20 -Wait

# Ou via Event Viewer
Get-EventLog -LogName Application -Source ".NET Runtime" -Newest 50
```

---

## Troubleshooting

### ❌ Erro: "Port Already in Use"

```powershell
# Encontrar processo usando a porta
$process = Get-Process | Where-Object { $_.MainWindowHandle -ne 0 } | 
  ForEach-Object { Get-NetTcpConnection -OwningProcess $_.Id -ErrorAction SilentlyContinue } |
  Where-Object { $_.LocalPort -eq 6000 }

# Matar processo (cuidado!)
Stop-Process -Id $process.OwningProcess -Force
```

### ❌ Erro: "Connection String Invalid"

```powershell
# Testar conexão SQL Server
$cs = "Server=SERVIDOR_SQL;Database=SantaLollaIntegracao;User Id=sa;Password=SENHA;"
$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = $cs
try {
  $conn.Open()
  Write-Host "✅ Conexão OK"
  $conn.Close()
} catch {
  Write-Host "❌ Erro: $_"
}
```

### ❌ Erro: "Certificate Not Found"

```powershell
# Verificar certificados no store
Get-ChildItem Cert:\LocalMachine\My

# Importar certificado manualmente
$pwd = ConvertTo-SecureString "SENHA" -AsPlainText -Force
$cert = Import-PfxCertificate `
  -FilePath "C:\SantaLolla\Certificados\certificado.pfx" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -Password $pwd
```

### ❌ Erro: "Access Denied"

```powershell
# Verificar permissões da pasta
icacls "C:\SantaLolla" /grant "NT AUTHORITY\NETWORK SERVICE":F /T /C

# Ou criar usuário dedicado
New-LocalUser -Name "SantaLollaAPI" -Password (ConvertTo-SecureString "SENHA_FORTE" -AsPlainText -Force)
Add-LocalGroupMember -Group "Administrators" -Member "SantaLollaAPI"
```

---

## Manutenção

### Backup Automático

```powershell
# Script de backup diário
$date = Get-Date -Format "yyyyMMdd_HHmmss"
$source = "C:\SantaLolla"
$destination = "C:\SantaLolla\Backups\backup_$date"

Copy-Item -Path $source -Destination $destination -Recurse -Force
Write-Host "✅ Backup criado: $destination"
```

### Atualizar Versão

```powershell
# 1. Parar serviço
Stop-Service -Name "SantaLollaAPI"

# 2. Backup versão atual
Copy-Item "C:\SantaLolla\App" "C:\SantaLolla\App_backup_$(Get-Date -Format yyyyMMdd)" -Recurse

# 3. Copiar nova versão
Copy-Item -Path "C:\Publish\SantaLolla_v2.0\*" -Destination "C:\SantaLolla\App" -Recurse -Force

# 4. Iniciar serviço
Start-Service -Name "SantaLollaAPI"

# 5. Verificar
Start-Sleep -Seconds 5
Get-Service "SantaLollaAPI" | Select-Object Status
```

---

## Monitoramento

### Verificação de Saúde

```powershell
# Script de monitoramento
$scriptBlock = {
  $response = Invoke-WebRequest -Uri "http://localhost:6000/api/health" -ErrorAction SilentlyContinue
  if ($response.StatusCode -ne 200) {
    Send-AlertEmail -Subject "SantaLolla API Down" -Body "API não respondendo"
  }
}

# Agendar a cada 5 minutos
$trigger = New-JobTrigger -RepeatIndefinitely -At (Get-Date) -RepetitionInterval (New-TimeSpan -Minutes 5)
Register-ScheduledJob -Name "SantaLolla-Monitor" -ScriptBlock $scriptBlock -Trigger $trigger
```

---

## Checklist Pré-Deployment

- [ ] .NET 8 Runtime instalado
- [ ] SQL Server acessível
- [ ] Certificado SSL válido (.pfx)
- [ ] Portas 6000 e 6001 liberadas no firewall
- [ ] Pasta "C:\SantaLolla" criada com permissões corretas
- [ ] appsettings.Production.json configurado
- [ ] Connection string testada
- [ ] JWT Secret Key gerada e configurada
- [ ] Backup do banco de dados realizado
- [ ] Registros de log habilitados
- [ ] Monitoramento configurado
- [ ] Plano de rollback documentado

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0
