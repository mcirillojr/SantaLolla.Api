# 📦 Guia de Build e Release - SantaLolla API

## Índice
1. [Visão Geral](#visão-geral)
2. [Requisitos de Build](#requisitos-de-build)
3. [Build Local](#build-local)
4. [Gerar Release](#gerar-release)
5. [Estrutura de Pacotes](#estrutura-de-pacotes)
6. [CI/CD Integração](#cicd-integração)
7. [Versionamento](#versionamento)

---

## Visão Geral

O processo de build e release da SantaLolla API foi completamente automatizado através do script `build-release.ps1`.

### Fluxo de Publication:

```
Código Fonte
    ↓
   [dotnet restore]
    ↓
   [dotnet build -c Release]
    ↓
   [dotnet publish] ──→ Framework-Dependent
    ↓                             ↓
   [dotnet publish]        (requer .NET 8)
    ↓              ↓
Self-Contained   Compactar ZIP
    ↓              ↓
(inclui runtime) [Gerar Checksums]
    ↓              ↓
                 📦 Pacote Final
```

---

## Requisitos de Build

### Software Necessário

```powershell
# Verificar versões
dotnet --version           # Deve ser 8.0.x
dotnet --list-sdks        # Deve incluir SDK 8.0

# Se não tiver, instalar:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### Requisitos de Sistema

- **Windows**: 10, 11 ou Windows Server 2019+
- **Memória**: 4GB (recomendado 8GB para compilação)
- **Disco**: 2GB espaço livre
- **PowerShell**: 5.0+ (ou PowerShell Core)

### Verificar Ambiente

```powershell
# Abrir PowerShell como Administrator
# Verificar politica de execução
Get-ExecutionPolicy

# Se retornar "Restricted", permitir scripts:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## Build Local

### 1. Build de Desenvolvimento

```powershell
cd C:\C#\SantaLolla.Api

# Build Debug (rápido, com symbols)
dotnet build

# Build Release (otimizado)
dotnet build -c Release
```

### 2. Executar Localmente

```powershell
# Modo Development
dotnet run

# Modo Produção
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --configuration Release
```

### 3. Executar Testes (se houver)

```powershell
dotnet test
```

---

## Gerar Release

### 1. Primeira Vez - Preparar

```powershell
# Clone ou navegue para a pasta do projeto
cd C:\C#\SantaLolla.Api

# Verificar se script existe
Test-Path .\build-release.ps1

# Se não existir, copie o arquivo
```

### 2. Executar Build Release

```powershell
# Sintaxe básica
.\build-release.ps1 -Version 1.0.0

# Com caminho customizado
.\build-release.ps1 -Version 1.0.0 -OutputPath D:\Deploy

# Exemplo completo
.\build-release.ps1 -Version 1.0.0 -OutputPath C:\Publish -BuildType Release
```

### 3. Entender os Parâmetros

| Parâmetro | Requisito | Exemplo | Descrição |
|-----------|-----------|---------|-----------|
| `-Version` | Obrigatório | `1.0.0` | Versão semântica (MAJOR.MINOR.PATCH) |
| `-OutputPath` | Opcional | `C:\Publish` | Local de saída (padrão: C:\Publish) |
| `-BuildType` | Opcional | `Release` | Tipo de build: Release ou Debug |

### 4. Resultado da Publicação

Ao executar `.\build-release.ps1 -Version 1.0.0`, você obtém:

```
C:\Publish\
├── SantaLolla_v1.0.0/
│   ├── publish/
│   │   ├── framework-dependent/     ← .NET 8 deve estar instalado
│   │   │   ├── SantaLolla.Api.exe
│   │   │   ├── SantaLolla.Api.dll
│   │   │   ├── appsettings.json
│   │   │   └── [outras DLLs]
│   │   │
│   │   └── self-contained-win-x64/  ← Inclui .NET 8 runtime
│   │       ├── SantaLolla.Api.exe (executável único)
│   │       ├── appsettings.json
│   │       └── [runtime files]
│   │
│   ├── appsettings.json
│   ├── appsettings.Production.json
│   ├── README.md
│   ├── API_FUNCIONAMENTO.md
│   ├── DEPLOYMENT_WINDOWS_SERVER.md
│   ├── install.bat
│   ├── manifest.json
│   └── checksums.txt
│
├── SantaLolla-API-v1.0.0-framework-dependent.zip
└── SantaLolla-API-v1.0.0-self-contained-win-x64.zip
```

### 5. Verificar Saída

```powershell
# Listar arquivos gerados
Get-ChildItem C:\Publish | Sort-Object Name

# Verificar tamanho dos ZIPs
(Get-Item C:\Publish\SantaLolla-API-*.zip | Measure-Object -Property Length -Sum).Sum / 1MB
```

---

## Estrutura de Pacotes

### Framework-Dependent vs Self-Contained

| Aspecto | Framework-Dependent | Self-Contained |
|---------|-------------------|-----------------|
| **Tamanho** | ~50MB | ~200MB |
| **Runtime** | Requer .NET 8 instalado | Incluso |
| **Instalação** | Rápida | Mais lenta |
| **Atualizações** | Via .NET runtime | Via app |
| **Caso de Uso** | Servidor com .NET | Servidor limpo |
| **Produção** | Recomendado | Fallback |

### Escolher Qual Usar

**Use Framework-Dependent se:**
- ✅ Servidor já tem .NET 8 Runtime
- ✅ Quer atualizações de segurança do runtime separado
- ✅ Disco/largura de banda limitados

**Use Self-Contained se:**
- ✅ Servidor é novo/limpo
- ✅ Não pode instalar .NET no servidor
- ✅ Quer independência total de versão
- ✅ Quer garantir compatibilidade

---

## CI/CD Integração

### GitHub Actions

```yaml
# .github/workflows/release.yml
name: Release Build

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET 8
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x

      - name: Build Release
        run: |
          $version = "${{ github.ref }}".Replace('refs/tags/v', '')
          .\build-release.ps1 -Version $version

      - name: Upload Artifacts
        uses: actions/upload-artifact@v3
        with:
          name: SantaLolla-Release-${{ github.ref }}
          path: C:\Publish\SantaLolla-API-*.zip
```

### Azure DevOps

```yaml
# azure-pipelines.yml
trigger:
  tags:
    include:
      - 'v*'

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'

steps:
  - task: UseDotNet@2
    inputs:
      version: $(dotnetVersion)

  - task: PowerShell@2
    inputs:
      targetType: filePath
      filePath: 'build-release.ps1'
      arguments: '-Version 1.0.0'
    displayName: 'Build Release Package'

  - task: PublishBuildArtifacts@1
    inputs:
      pathToPublish: $(System.DefaultWorkingDirectory)
```

### GitLab CI

```yaml
# .gitlab-ci.yml
stages:
  - build
  - release

release:
  stage: release
  image: mcr.microsoft.com/windows/servercore:ltsc2022
  script:
    - pwsh -Command ".\build-release.ps1 -Version '1.0.0'"
  artifacts:
    paths:
      - 'C:\Publish\SantaLolla-API-*.zip'
    expire_in: 30 days
  only:
    - tags
```

---

## Versionamento

### Semantic Versioning

Usar formato `MAJOR.MINOR.PATCH`:

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Novas funcionalidades (compatível)
- **PATCH**: Correções de bugs

### Exemplos

```powershell
.\build-release.ps1 -Version 1.0.0    # Release inicial
.\build-release.ps1 -Version 1.0.1    # Patch (bug fix)
.\build-release.ps1 -Version 1.1.0    # Minor (nova feature)
.\build-release.ps1 -Version 2.0.0    # Major (breaking change)
```

### Criar Tag no Git

```powershell
# Criar tag local
git tag -a v1.0.0 -m "Release v1.0.0: Descrição das mudanças"

# Enviar para GitHub
git push origin v1.0.0

# Listar tags
git tag -l
```

---

## Troubleshooting de Build

### ❌ Erro: "dotnet command not found"

```powershell
# Adicionar .NET ao PATH
$env:Path += ";C:\Program Files\dotnet"

# Ou instalar .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### ❌ Erro: "Execution of scripts is disabled"

```powershell
# Permitir execução de scripts
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### ❌ Erro: "Project file not found"

```powershell
# Verificar caminho
cd C:\C#\SantaLolla.Api
Test-Path .\SantaLolla.Api\SantaLolla.Api.csproj
```

### ❌ Erro: "Build failed with errors"

```powershell
# Limpar cache e tentar novamente
dotnet clean
dotnet restore
dotnet build -c Release
```

---

## Performance de Build

### Tempos Estimados

- **Restore**: 30-60s (primeira vez), 5-10s (cache)
- **Build**: 30-45s
- **Publish**: 30-60s

**Tempo total esperado**: ~3-5 minutos

### Otimizações

```powershell
# Build paralelo (padrão)
dotnet build -m

# Publicar apenas runtime necessário
dotnet publish -c Release -r win-x64 --self-contained
```

---

## Checklist de Release

- [ ] Código commitado no Git
- [ ] Todos os testes passando
- [ ] Versão documentada (CHANGELOG)
- [ ] Tag Git criada
- [ ] Build bem-sucedido localmente
- [ ] Checksums verificados
- [ ] ZIPs compactados corretamente
- [ ] Documentação incluída
- [ ] Manifesto válido
- [ ] Script de instalação testado
- [ ] Upload para servidor/repositório
- [ ] Backup do build armazenado

---

## Backup e Retenção

```powershell
# Manter últimas 5 releases
Get-ChildItem C:\Publish\SantaLolla-API-*.zip | 
  Sort-Object LastWriteTime -Descending | 
  Select-Object -Skip 5 | 
  Remove-Item -Force

# Ou arquivar antigas
$archive = "C:\Publish\Archive"
mkdir $archive -Force
Get-ChildItem C:\Publish\SantaLolla-API-*.zip | 
  Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-30) } |
  Move-Item -Destination $archive
```

---

## Próximos Passos

1. ✅ Executar `build-release.ps1 -Version 1.0.0`
2. ✅ Copiar ZIP para servidor
3. ✅ Executar `install.bat` no servidor
4. ✅ Configurar `appsettings.Production.json`
5. ✅ Iniciar aplicação
6. ✅ Verificar em http://localhost:6000/swagger

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0
