# Santa Lolla - API de Integração

## 📋 Visão Geral

**Santa Lolla Integração API** é uma API REST desenvolvida em **.NET 8** para fornecer funcionalidades de integração com terceiros, permitindo acesso a dados de vendas, estoques, lojas, vendedores e gerenciamento de autenticação via tokens JWT.

## 🏗️ Arquitetura Técnica

### Stack de Tecnologia

| Camada | Tecnologia | Versão |
|--------|-----------|--------|
| **Framework** | ASP.NET Core Web API | .NET 8 |
| **Linguagem** | C# | 12+ (Nullable: `enable`, ImplicitUsings: `enable`) |
| **Autenticação** | JWT Bearer Token | - |
| **ORM/Query** | Dapper | 2.1.66 |
| **Banco de Dados** | SQL Server | via Microsoft.Data.SqlClient 6.0.2 |
| **Hash/Criptografia** | BCrypt.Net-Next | 4.0.3 |
| **Documentação API** | Swagger/OpenAPI | Swashbuckle.AspNetCore 6.6.2 |

### Estrutura de Camadas

```
SantaLolla.Api/
│
├── Controllers/                    # Camada de Apresentação
│   ├── AuthController.cs          # Gerenciamento de autenticação
│   ├── VendasController.cs        # Operações de vendas
│   ├── EstoquesController.cs      # Operações de estoques
│   ├── LojasController.cs         # Operações de lojas
│   ├── VendedoresController.cs    # Operações de vendedores
│   ├── HealthController.cs        # Health check
│   └── DevController.cs           # Endpoints de desenvolvimento
│
├── Models/                         # Modelos de Dados (DTOs)
│   ├── Auth/
│   │   ├── TokenRequest.cs
│   │   ├── TokenFormRequest.cs
│   │   ├── TokenResponse.cs
│   │   └── TerceiroApi.cs
│   ├── Vendas/
│   │   ├── VendaFiltroRequest.cs
│   │   └── VendaResponse.cs
│   ├── Estoques/
│   ├── Lojas/
│   └── Vendedores/
│
├── Services/                       # Lógica de Negócio
│   ├── Interfaces/
│   │   └── ITokenService.cs
│   ├── TokenService.cs
│   └── [Outros Serviços]
│
├── Repositories/                   # Camada de Acesso a Dados
│   ├── Interfaces/
│   │   ├── IVendaRepository.cs
│   │   ├── IEstoqueRepository.cs
│   │   ├── ILojaRepository.cs
│   │   ├── IVendedorRepository.cs
│   │   └── ITerceiroRepository.cs
│   ├── VendaRepository.cs
│   ├── EstoqueRepository.cs
│   ├── LojaRepository.cs
│   ├── VendedorRepository.cs
│   └── TerceiroRepository.cs
│
├── Data/                           # Infraestrutura de Dados
│   └── SqlConnectionFactory.cs     # Factory para conexões SQL
│
├── Configurations/                 # Configurações da Aplicação
│   ├── JwtSettings.cs
│   └── SantaLollaSettings.cs
│
├── Workers/                        # Serviços em Background
│   └── SantaLollaWorker.cs        # HostedService para processamento async
│
├── Docs/                           # Documentação
│   └── [Documentos adicionais]
│
├── Program.cs                      # Inicialização e configuração de DI
├── appsettings.json               # Configurações
└── SantaLolla.Api.csproj          # Arquivo de projeto
```

## 🔐 Autenticação e Autorização

### Fluxo JWT Bearer Token

```
┌─────────────────────────────────────────────────────────────┐
│                      Cliente Terceiro                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ POST /api/auth/token
                              │ {ClientId, ClientSecret}
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    AuthController                           │
│  ┌────────────────────────────────────────────────────┐    │
│  │  GerarToken(TokenRequest)                          │    │
│  │  GerarTokenForm(TokenFormRequest)                  │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    TokenService                             │
│  ┌────────────────────────────────────────────────────┐    │
│  │  GerarTokenAsync(TokenRequest): TokenResponse     │    │
│  │  - Valida ClientId/ClientSecret                   │    │
│  │  - Gera JWT Token assinado                        │    │
│  │  - Retorna token com expiração                    │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ TokenResponse (JWT)
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Cliente Terceiro                        │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Authorization: Bearer {JWT_TOKEN}                │    │
│  │                                                    │    │
│  │  Requisições com TOKEN para outros endpoints      │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                JwtBearer Middleware                         │
│  ┌────────────────────────────────────────────────────┐    │
│  │  ValidateToken()                                   │    │
│  │  - Verifica assinatura                            │    │
│  │  - Valida Issuer                                  │    │
│  │  - Valida Audience                                │    │
│  │  - Verifica expiração (lifetime)                  │    │
│  │  - ClockSkew: 1 minuto                           │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
     │ ✓ Token válido          │ ✗ Token inválido/expirado
     ▼                         ▼
  [Autorizado]            [401 Unauthorized]
     │
     ▼
  Acessa recurso protegido ([Authorize])
```

### Configuração JWT

```csharp
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-muito-segura",
    "Issuer": "SantaLolla.Api",
    "Audience": "SantaLolla.Cliente",
    "ExpirationMinutes": 60
  }
}
```

## 📡 API Endpoints

### 1. Autenticação (`/api/auth`)

#### POST `/api/auth/token` - Gerar Token JWT
Gera um token JWT para acesso protegido à API.

```
Request (JSON):
POST /api/auth/token HTTP/1.1
Content-Type: application/json

{
  "clientId": "seu-client-id",
  "clientSecret": "seu-client-secret"
}

Response (200 OK):
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "expirationTime": "2024-01-15T14:30:00Z"
}

Response (401 Unauthorized):
{
  "mensagem": "ClientId ou ClientSecret inválido."
}
```

#### POST `/api/auth/token-form` - Gerar Token via Form
Alternativa para gerar token via formulário encoded.

```
Request (Form-Data):
POST /api/auth/token-form HTTP/1.1
Content-Type: application/x-www-form-urlencoded

clientId=seu-client-id&clientSecret=seu-client-secret

Response: (igual ao endpoint anterior)
```

---

### 2. Saúde da API (`/api/health`)

#### GET `/api/health` - Health Check
Verifica o status e disponibilidade da API.

```
Request:
GET /api/health HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Response (200 OK):
{
  "status": "OK",
  "sistema": "SantaLolla.Api",
  "mensagem": "API Santa Lolla em execução",
  "dataHora": "2024-01-15T12:30:45Z"
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

---

### 3. Vendas (`/api/vendas`)

#### GET `/api/vendas` - Listar Vendas
Lista vendas filtradas por período, rede, loja e paginação.

```
Request:
GET /api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31&pagina=1&itensPorPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- dataInicio (DateTime, opcional): Data inicial da venda
- dataFim (DateTime, opcional): Data final da venda
- lastUpdateInicio (DateTime, opcional): Data inicial de atualização
- lastUpdateFim (DateTime, opcional): Data final de atualização
- rede (string, opcional): Código da rede
- loja (string, opcional): Código da loja
- pagina (int): Página desejada
- itensPorPagina (int): Itens por página

Validação: Pelo menos um período deve ser informado:
  - dataInicio/dataFim OU
  - lastUpdateInicio/lastUpdateFim

Response (200 OK):
{
  "total": 250,
  "pagina": 1,
  "itensPorPagina": 50,
  "vendas": [
    {
      "id": "V123456",
      "data": "2024-01-15T10:30:00Z",
      "rede": "RD01",
      "loja": "L001",
      "valor": 1500.00,
      "itens": 5,
      "lastUpdate": "2024-01-15T10:30:00Z"
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "Informe pelo menos um período: dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

#### POST `/api/vendas` - Criar Venda
Cria um novo registro de venda.

```
Request:
POST /api/vendas HTTP/1.1
Content-Type: application/json
Authorization: Bearer {JWT_TOKEN}

{
  "rede": "RD01",
  "loja": "L001",
  "data": "2024-01-15T10:30:00Z",
  "valor": 1500.00,
  "itens": 5
}

Response (201 Created):
{
  "id": "V123456",
  "rede": "RD01",
  "loja": "L001",
  "data": "2024-01-15T10:30:00Z",
  "valor": 1500.00,
  "itens": 5
}
```

---

### 4. Estoques (`/api/estoques`)

#### GET `/api/estoques` - Listar Estoques
Lista estoques com filtros.

```
Request:
GET /api/estoques?rede=RD01&loja=L001 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Response (200 OK):
{
  "estoques": [
    {
      "id": "E123",
      "rede": "RD01",
      "loja": "L001",
      "produto": "P001",
      "quantidade": 100,
      "ultimaAtualizacao": "2024-01-15T10:00:00Z"
    }
  ]
}
```

> ⚠️ Requer autenticação JWT

---

### 5. Lojas (`/api/lojas`)

#### GET `/api/lojas` - Listar Lojas
Lista todas as lojas cadastradas.

```
Request:
GET /api/lojas HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Response (200 OK):
{
  "lojas": [
    {
      "id": "L001",
      "rede": "RD01",
      "nome": "Loja Centro",
      "endereco": "Rua Principal, 123",
      "cidade": "São Paulo",
      "estado": "SP"
    }
  ]
}
```

> ⚠️ Requer autenticação JWT

---

### 6. Vendedores (`/api/vendedores`)

#### GET `/api/vendedores` - Listar Vendedores
Lista todos os vendedores cadastrados.

```
Request:
GET /api/vendedores HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Response (200 OK):
{
  "vendedores": [
    {
      "id": "V001",
      "nome": "João Silva",
      "email": "joao@example.com",
      "rede": "RD01",
      "status": "ativo"
    }
  ]
}
```

> ⚠️ Requer autenticação JWT

---

## 🔄 Fluxo Típico de Uso

```
┌──────────────────────────────────────────────────────────────┐
│                    INÍCIO - Cliente Terceiro                 │
└──────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┴────────────────────┐
         │                                         │
         ▼                                         ▼
┌─────────────────────────┐         ┌────────────────────────┐
│  POST /api/auth/token   │         │  GET /api/health (opt) │
│  [Sem autenticação]     │         │  [Sem autenticação]    │
│                         │         │                        │
│ ClientId + ClientSecret │         │ Verificar API OK       │
│      ➜ JWT Token        │         │    ➜ Status OK         │
└─────────────────────────┘         └────────────────────────┘
         │                                         │
         │ (salvar JWT)                           │
         └────────────────────┬────────────────────┘
                              │
         ┌────────────────────┴────────────────────┐
         │                                         │
         ▼                                         ▼
┌──────────────────────────────┐    ┌──────────────────────────────┐
│  GET /api/vendas             │    │  GET /api/estoques           │
│  [Com JWT Authorization]     │    │  [Com JWT Authorization]     │
│                              │    │                              │
│  Filtrar por período         │    │  Filtrar por rede/loja       │
│  Rede, Loja, Paginação       │    │  Retorna stocks              │
│      ➜ Lista de Vendas       │    │      ➜ Estoques disponíveis  │
└──────────────────────────────┘    └──────────────────────────────┘
         │                                         │
         │ (processar dados)                       │
         │                                         │
         └────────────────────┬────────────────────┘
                              │
         ┌────────────────────┴────────────────────┐
         │                                         │
         ▼                                         ▼
┌──────────────────────────────┐    ┌──────────────────────────────┐
│  GET /api/lojas              │    │  GET /api/vendedores         │
│  [Com JWT Authorization]     │    │  [Com JWT Authorization]     │
│                              │    │                              │
│  Retorna lojas cadastradas   │    │  Retorna vendedores          │
│      ➜ Lista de Lojas        │    │      ➜ Lista de Vendedores   │
└──────────────────────────────┘    └──────────────────────────────┘
         │                                         │
         │ (processamento completo)               │
         └────────────────────┬────────────────────┘
                              │
                              ▼
                   ┌────────────────────┐
                   │   FIM - Dados      │
                   │   Processados      │
                   └────────────────────┘
```

---

## 🔄 Fluxo de Processamento em Background

A API inclui um **HostedService** (`SantaLollaWorker`) que executa processamento assíncrono em background:

```
┌─────────────────────────────────────────┐
│   Inicialização da Aplicação            │
│   (Program.cs: AddHostedService)        │
└─────────────────────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────┐
│   SantaLollaWorker StartAsync()         │
│   - Inicia loop de processamento        │
│   - Aguarda sinais de parada            │
└─────────────────────────────────────────┘
              │
              ▼
        ┌─────────────┐
        │ Loop Ativo  │
        └─────────────┘
         │           │
         │           └──→ [Processamento Periódico]
         │                 - Sincronizar dados
         │                 - Atualizar caches
         │                 - Executar jobs
         │
         └──→ [Shutdown/Cancelamento]
              - Liberar recursos
              - Finalizar operações
```

---

## ⚙️ Configuração e Inicialização

### Program.cs - Dependency Injection

```csharp
// Configurações
builder.Services.Configure<SantaLollaSettings>(
    builder.Configuration.GetSection("SantaLollaSettings"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Infraestrutura
builder.Services.AddSingleton<SqlConnectionFactory>();

// Repositories
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();
builder.Services.AddScoped<ILojaRepository, LojaRepository>();
builder.Services.AddScoped<IVendedorRepository, VendedorRepository>();
builder.Services.AddScoped<ITerceiroRepository, TerceiroRepository>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();

// Autenticação JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* validação */ });

// Worker
builder.Services.AddHostedService<SantaLollaWorker>();
```

### Swagger Documentation

- **URL**: `http://localhost:5000/swagger`
- **Especificação**: OpenAPI v1
- **Autenticação**: JWT Bearer integrado na UI

---

## 📊 Padrões Utilizados

| Padrão | Descrição | Localização |
|--------|-----------|-------------|
| **Repository** | Abstrai acesso a dados | `Repositories/Interfaces/*` |
| **Dependency Injection** | IoC container ASP.NET Core | `Program.cs` |
| **Async/Await** | Operações não-bloqueantes | Controllers, Services, Repositories |
| **DTO (Data Transfer Object)** | Modelos para API/Banco | `Models/*` |
| **Factory Pattern** | Criação de conexões | `Data/SqlConnectionFactory.cs` |
| **HostedService** | Background processing | `Workers/SantaLollaWorker.cs` |

---

## 🔒 Segurança

### Implementações

- ✅ **JWT Bearer Token**: Autenticação stateless
- ✅ **BCrypt**: Hash seguro de senhas (via BCrypt.Net-Next)
- ✅ **HTTPS Redirection**: Aplicado na pipeline
- ✅ **Token Validation**: Issuer, Audience, SigningKey verificados
- ✅ **JWT Expiration**: Validação de lifetime com ClockSkew
- ✅ **[Authorize]**: Proteção de endpoints

### Headers Obrigatórios

Todos os endpoints protegidos requerem:

```
Authorization: Bearer {JWT_TOKEN}
```

---

## 🔧 Variáveis de Ambiente Necessárias

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=seu-servidor;Database=SantaLolla;User Id=sa;Password=sua-senha;"
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-muito-segura-com-miximo-de-caracteres",
    "Issuer": "SantaLolla.Api",
    "Audience": "SantaLolla.Cliente",
    "ExpirationMinutes": 60
  },
  "SantaLollaSettings": {
    "ApiUrl": "http://localhost:5000",
    "Environment": "Development"
  }
}
```

---

## 📦 Dependências

```xml
<ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Dapper" Version="2.1.66" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="6.0.2" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
    <PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="6.6.2" />
</ItemGroup>
```

---

## 🚀 Executando a Aplicação

### Desenvolvimento

```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Executar
dotnet run

# Acessar Swagger
# http://localhost:5000/swagger
```

### Produção

```bash
# Publicar
dotnet publish -c Release

# Executar DLL publicada
dotnet SantaLolla.Api.dll
```

---

## 📝 Exemplo Completo de Integração

```bash
# 1. Obter Token
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "seu-client-id",
    "clientSecret": "seu-client-secret"
  }'

# Response:
# {
#   "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
#   "tokenType": "Bearer",
#   "expiresIn": 3600
# }

# 2. Usar Token para Consultar Vendas
JWT_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET "http://localhost:5000/api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31" \
  -H "Authorization: Bearer $JWT_TOKEN"

# 3. Verificar Health
curl -X GET http://localhost:5000/api/health \
  -H "Authorization: Bearer $JWT_TOKEN"
```

---

## 📞 Contato e Suporte

- **Repositório**: https://github.com/mcirillojr/AlterVision.Api
- **Desenvolvedor**: mcirillojr
- **Ambiente**: ASP.NET Core 8.0

---

**Última atualização**: 2024-01-15 | **Versão API**: v1
