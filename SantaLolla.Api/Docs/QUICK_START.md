# 🚀 Guia Rápido de Deployment - SantaLolla API

## Fluxo Completo: Do Desenvolvimento para Produção

```
┌─────────────────────────────────┐
│  1. Desenvolvimento Local        │ ← Você está aqui
│  dotnet run                      │
│  http://localhost:6000/swagger   │
└──────────────┬──────────────────┘

                 │

┌────────────────▼──────────────────────┐
│  2. Gerar Release                      │
│  .\build-release.ps1 -Version 1.0.0   │
│  Saída: SantaLolla-API-v1.0.0.zip    │
└────────────────┬──────────────────────┘

                 │

┌────────────────▼──────────────────────┐
│  3. Copiar para Servidor              │
│  Enviar ZIP para Windows Server       │
└────────────────┬──────────────────────┘

                 │

┌────────────────▼──────────────────────┐
│  4. Deploy no Servidor                │
│  .\deploy.ps1 -ZipPath "..."         │
│  Configurar appsettings.Production.json│
└────────────────┬──────────────────────┘

                 │

┌────────────────▼──────────────────────┐
│  5. Acessar em Produção               │
│  http://servidor:6000/swagger         │
│  https://servidor:6001 (com SSL)      │
└──────────────────────────────────────┘
```

---

## ⚡ Passos Rápidos

### 1️⃣ DESENVOLVER LOCALMENTE

```powershell
# Navegar para o projeto
cd C:\C#\SantaLolla.Api

# Executar em modo desenvolvimento
dotnet run

# Acessar Swagger
# http://localhost:6000/swagger
```

### 2️⃣ GERAR PACOTE DE RELEASE

```powershell
# No diretório do projeto
.\build-release.ps1 -Version 1.0.0

# Saída:
# ✓ C:\Publish\SantaLolla-API-v1.0.0-framework-dependent.zip
# ✓ C:\Publish\SantaLolla-API-v1.0.0-self-contained-win-x64.zip
# ✓ C:\Publish\SantaLolla_v1.0.0\manifest.json
```

### 3️⃣ TRANSFERIR PARA SERVIDOR

**Opção A: Via rede (RDP)**
```powershell
# Copiar arquivo via UNC
Copy-Item "C:\Publish\SantaLolla-API-v1.0.0-*.zip" `
  -Destination "\\SERVIDOR\C$\Temp\"
```

**Opção B: Via FTP/SFTP**
```powershell
# Usar WinSCP ou similar
```

**Opção C: Via GitHub Releases**
```powershell
# Fazer upload como release no GitHub
# Depois baixar no servidor
```

### 4️⃣ INSTALAR NO SERVIDOR

```powershell
# No servidor, como Administrator:

# 1. Extrair arquivo
$zip = "C:\Temp\SantaLolla-API-v1.0.0-self-contained-win-x64.zip"
Expand-Archive -Path $zip -DestinationPath "C:\SantaLolla\App" -Force

# 2. Executar deploy script
.\deploy.ps1 -ZipPath $zip -CreateService $true

# 3. Editar configuração
notepad "C:\SantaLolla\App\appsettings.Production.json"

# 4. Iniciar serviço
Start-Service -Name "SantaLollaAPI"

# 5. Verificar
Get-Service "SantaLollaAPI"
```

### 5️⃣ ACESSAR EM PRODUÇÃO

```
HTTP:   http://seu-servidor.com:6000
HTTPS:  https://seu-servidor.com:6001
Swagger: http://seu-servidor.com:6000/swagger
Health:  http://seu-servidor.com:6000/api/health
```

---

## 📋 CHECKLIST PRÉ-DEPLOYMENT

Antes de fazer deploy em produção:

- [ ] Código commitado no Git  
- [ ] Testes locais passando  
- [ ] Versão bumpada (git tag)  
- [ ] `build-release.ps1` executado com sucesso  
- [ ] ZIP gerado sem erros  
- [ ] Backup do servidor realizado  
- [ ] Certificado SSL (.pfx) preparado  
- [ ] SQL Server acessível e database criado  
- [ ] Firewall configurado para portas 6000/6001  
- [ ] appsettings.Production.json preenchido corretamente  
- [ ] .NET 8 Runtime instalado no servidor (se usando framework-dependent)  
- [ ] Fuso horário do servidor correto  
- [ ] Backups automatizados configurados  
- [ ] Monitoramento ativo  

---

## 🆘 TROUBLESHOOTING RÁPIDO

### ❌ Swagger não abre em produção

```powershell
# Verificar se API está rodando
Get-Process | Where-Object {$_.Name -like "*SantaLolla*"}

# Verificar logs
Get-Content -Path "C:\SantaLolla\Logs\*.log" -Tail 50

# Testar porta
Test-NetConnection -ComputerName localhost -Port 6000
```

### ❌ Connection String errada

```powershell
# Corrigir em appsettings.Production.json
notepad "C:\SantaLolla\App\appsettings.Production.json"

# Reiniciar serviço
Restart-Service -Name "SantaLollaAPI"
```

### ❌ Certificado SSL não funciona

```powershell
# Verificar certificado instalado
Get-ChildItem Cert:\LocalMachine\My

# Importar novo certificado
$pwd = ConvertTo-SecureString "SENHA" -AsPlainText -Force
Import-PfxCertificate -FilePath "C:\SantaLolla\Certificados\cert.pfx" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -Password $pwd
```

### ❌ Porta já em uso

```powershell
# Encontrar o que está usando porta 6000
Get-NetTcpConnection -LocalPort 6000 | Select-Object OwningProcess

# Matar o processo
Stop-Process -Id 1234 -Force
```

---

## 📊 MONITORAMENTO

### Verificar Status

```powershell
# Status do serviço
Get-Service -Name "SantaLollaAPI" | Select-Object Status, DisplayName

# Processos ativos
Get-Process | Where-Object {$_.Name -like "*SantaLolla*"}

# Uso de memória
Get-Process SantaLolla.Api | Select-Object Name, WorkingSet
```

### Verificar Logs

```powershell
# Logs em tempo real
Get-Content -Path "C:\SantaLolla\Logs\*.log" -Tail 20 -Wait

# Últimas linhas
Get-Content -Path "C:\SantaLolla\Logs\*.log" | Select-Object -Last 100
```

### Health Check

```powershell
# Verificar saúde da API
$response = Invoke-RestMethod -Uri "http://localhost:6000/api/health" -ErrorAction SilentlyContinue
$response | ConvertTo-Json
```

---

## 🔄 ROLLBACK (Voltar para versão anterior)

```powershell
# 1. Parar serviço
Stop-Service -Name "SantaLollaAPI"

# 2. Restaurar backup
$backup = "C:\SantaLolla\Backups\App_backup_YYYYMMDD_HHMMSS"
Copy-Item -Path $backup -Destination "C:\SantaLolla\App" -Recurse -Force

# 3. Iniciar serviço
Start-Service -Name "SantaLollaAPI"

# 4. Verificar
Get-Service -Name "SantaLollaAPI"
```

---

## 📚 DOCUMENTAÇÃO COMPLETA

Consulte os arquivos de documentação para mais detalhes:

| Documento | Conteúdo |
|-----------|----------|
| **BUILD_AND_RELEASE.md** | Como gerar releases, CI/CD, versionamento |
| **DEPLOYMENT_WINDOWS_SERVER.md** | Guia detalhado para Windows Server |
| **API_FUNCIONAMENTO.md** | Endpoints e como integrar com a API |
| **CASOS_DE_USO.md** | Exemplos de integração |
| **TROUBLESHOOTING.md** | Solução de problemas operacionais |
| **README.md** | Visão geral técnica do projeto |

---

## 🎯 RESUMO DOS SCRIPTS

| Script | Propósito | Execução |
|--------|-----------|----------|
| **build-release.ps1** | Gerar pacotes de release | `.\build-release.ps1 -Version 1.0.0` |
| **deploy.ps1** | Instalar em servidor Windows | `.\deploy.ps1 -ZipPath "..." ` |
| **start.bat** | Iniciar aplicação manualmente | Duplo clique ou cmd |
| **start.ps1** | Iniciar via PowerShell | `. .\start.ps1` |

---

## 💡 DICAS IMPORTANTES

1. **Sempre fazer backup** antes de atualizar versão
2. **Testar em staging** antes de ir para produção
3. **Monitorar logs** nos primeiros minutos após deploy
4. **Documentar mudanças** de configuração
5. **Usar versioning semântico** (1.0.0, 1.0.1, 1.1.0, etc)
6. **Manter certificados SSL** atualizados
7. **Rotacionar logs** periodicamente
8. **Fazer health checks** regularmente

---

## ❓ SUPORTE

Para mais informações:
- 📖 Consulte os guias na pasta `Docs/`
- 🐛 Verifique `TROUBLESHOOTING.md`
- 💻 Acesse `http://localhost:6000/swagger` (modo desenvolvimento)

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0

