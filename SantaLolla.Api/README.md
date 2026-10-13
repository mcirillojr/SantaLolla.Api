# Santa Lolla - API de Integração -- SANTA LOLLA

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
│   ├── AuthController.cs           # Gerenciamento de autenticação (token JWT)
│   ├── VendasController.cs         # Operações de vendas
│   ├── VendasProdutosController.cs # Operações de produtos/itens das vendas
│   ├── ClientesVarejoController.cs # Operações de clientes de varejo
│   ├── EstoquesController.cs       # Operações de estoques
│   ├── LojasController.cs          # Operações de lojas
│   ├── VendedoresController.cs     # Operações de vendedores
│   ├── HealthController.cs         # Health check
│   └── DevController.cs            # Endpoints de desenvolvimento
│
├── Models/                         # Modelos de Dados (DTOs)
│   ├── Auth/
│   │   ├── TokenRequest.cs
│   │   ├── TokenFormRequest.cs
│   │   ├── TokenResponse.cs
│   │   └── TerceiroApi.cs
│   ├── Vendas/
│   │   ├── VendaFiltroRequest.cs
│   │   ├── VendaResponse.cs
│   │   ├── VendaParcelaResponse.cs
│   │   └── VendaPagamentoResponse.cs
│   ├── VendasProdutos/
│   │   ├── VendaProdutoFiltroRequest.cs
│   │   ├── VendaProdutoResponse.cs
│   │   └── VendaProdutoItemResponse.cs
│   ├── ClientesVarejo/
│   │   ├── ClienteVarejoFiltroRequest.cs
│   │   └── ClienteVarejoResponse.cs
│   ├── Estoques/
│   │   ├── EstoqueFiltroRequest.cs
│   │   ├── EstoqueResponse.cs
│   │   ├── EstoqueTotalAgrupadoFiltroRequest.cs
│   │   └── EstoqueTotalAgrupadoResponse.cs
│   ├── Lojas/
│   ├── Vendedores/
│   └── PagedResponse/
│       └── PagedResponse.cs
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
│   │   ├── IVendaProdutoRepository.cs
│   │   ├── IClienteVarejoRepository.cs
│   │   ├── IEstoqueRepository.cs
│   │   ├── ILojaRepository.cs
│   │   ├── IVendedorRepository.cs
│   │   └── ITerceiroRepository.cs
│   ├── VendaRepository.cs
│   ├── VendaProdutoRepository.cs
│   ├── ClienteVarejoRepository.cs
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

### Controle de Acesso por Endpoint

A API implementa um sistema de **controle de acesso granular por endpoint**, garantindo que cada terceiro (integrador) possa acessar apenas os endpoints para os quais tem permissão explícita.

#### Como Funciona

```
┌─────────────────────────────────────────────────────────┐
│         Requisição HTTP com JWT Token                   │
│  GET /api/vendas                                        │
│  Authorization: Bearer {JWT}                            │
└──────────────────┬──────────────────────────────────────┘
                   │
                   ▼
        ┌──────────────────────┐
        │ PermissaoTerceiro    │
        │ Middleware           │
        └──────────┬───────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
    ✓ Endpoint               │
    autorizado?              ▼
         │          ┌─────────────────────┐
         ▼          │ Consultar Banco de  │
    [Continuar]     │ Dados               │
                    │ VerificaPermissao() │
                    └────────┬────────────┘
                             │
                    ┌────────┴────────┐
                    │                 │
                ✓ Permitido       ✗ Negado
                    │                 │
                    ▼                 ▼
              [Continuar]        [403 Forbidden]
                Requisição
```

#### O que é Validado

## 🔒 Controle de Acesso por Rede

Foi adicionado suporte para filtrar acessos por "rede" (identificador de PDV/loja) para terceiros integrados. A validação é feita pelo middleware PermissaoTerceiroMiddleware que, após validar token e permissão de endpoint, carrega a configuração de redes permitidas do terceiro e aplica as regras abaixo.

Comportamento principal:

- Se o terceiro tem `ACESSO_TODAS_REDES = 1`, pode acessar todas as redes sem restrição.
- Caso contrário, apenas as redes listadas em `SETA_API_TERCEIROS_REDES` com `ATIVO = 1` são permitidas.
- O cliente pode informar explicitamente a rede via query string: `?Rede=rede000184`. Quando informada, a rede é validada imediatamente pelo middleware.
- Se a rede informada não estiver permitida, o middleware retorna 403 (Forbidden).

Variáveis disponibilizadas no contexto da requisição (HttpContext.Items):

- `IdTerceiro` — id do terceiro extraído do token.
- `AcessoTodasRedes` — booleano indicando se o terceiro tem acesso global.
- `RedesPermitidas` — lista de strings com as redes ativas permitidas para o terceiro.

### SQL de Migração (exemplos)

Os scripts a seguir mostram as alterações de esquema necessárias para suportar o controle por rede (exemplos que já foram aplicados no ambiente de desenvolvimento):

```sql
USE [SetaIntegracao];
GO

IF COL_LENGTH('dbo.SETA_API_TERCEIROS','ACESSO_TODAS_REDES') IS NULL
BEGIN
    ALTER TABLE dbo.SETA_API_TERCEIROS
    ADD ACESSO_TODAS_REDES BIT NOT NULL
        CONSTRAINT DF_SETA_API_TERCEIROS_ACESSO_TODAS_REDES
        DEFAULT 0;
END;
GO

IF OBJECT_ID('dbo.SETA_API_TERCEIROS_REDES','U') IS NULL
BEGIN
    CREATE TABLE dbo.SETA_API_TERCEIROS_REDES
    (
        ID_TERCEIRO_REDE BIGINT IDENTITY(1,1) NOT NULL,
        ID_TERCEIRO BIGINT NOT NULL,
        REDE VARCHAR(20) NOT NULL,
        ATIVO BIT NOT NULL DEFAULT 1,
        DATA_CRIACAO DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_SETA_API_TERCEIROS_REDES PRIMARY KEY (ID_TERCEIRO_REDE),
        CONSTRAINT FK_SETA_API_TERCEIROS_REDES_TERCEIROS FOREIGN KEY (ID_TERCEIRO) REFERENCES dbo.SETA_API_TERCEIROS(ID_TERCEIRO)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_SETA_API_TERCEIROS_REDES_TERCEIRO_REDE' AND object_id = OBJECT_ID('dbo.SETA_API_TERCEIROS_REDES'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_SETA_API_TERCEIROS_REDES_TERCEIRO_REDE ON dbo.SETA_API_TERCEIROS_REDES (ID_TERCEIRO, REDE);
END;
GO

-- Exemplo de inserção
INSERT INTO dbo.SETA_API_TERCEIROS_REDES (ID_TERCEIRO, REDE, ATIVO) VALUES (1, 'rede000184', 1);
```

### Exemplo de Requisição

Exemplo de chamada informando a rede explicitamente:

```
GET /api/Vendas?Rede=rede000184
Authorization: Bearer {JWT_TOKEN}
```

Comportamento esperado:

- Se o token pertence a um terceiro com `ACESSO_TODAS_REDES = 1`, a requisição segue normalmente.
- Se `ACESSO_TODAS_REDES = 0` e `rede000184` está na lista de `RedesPermitidas`, a requisição segue normalmente.
- Caso contrário, o middleware retorna 403 com mensagem explicativa.

### Implementação / Observações

- O middleware `PermissaoTerceiroMiddleware` chama `ITerceiroRepository.ObterAcessoRedesAsync(long idTerceiro)` para obter `AcessoTodasRedes` e `RedesPermitidas`.
- Os controllers e serviços que precisarem dessa informação durante a requisição podem acessar `HttpContext.Items["IdTerceiro"]`, `HttpContext.Items["AcessoTodasRedes"]` e `HttpContext.Items["RedesPermitidas"]`.
- Se houver discrepância de versões binárias (ex.: interface modificada mas projeto não recompilado), limpe e reconstrua a solução.

> Observação: aplique os scripts de migração em ambiente de homologação antes de produção e siga o processo de deploy da sua equipe.


| Aspecto | Descrição |
|---------|-----------|
| **ID do Terceiro** | Extraído do JWT (claim: `id_terceiro`) |
| **Endpoint** | Caminho da requisição (ex: `/api/vendas`) |
| **Método HTTP** | GET, POST, PUT, DELETE, etc. |
| **Permissão** | Verificada na tabela de permissões do banco |

#### Endpoints Liberados (Sem Validação)

Os seguintes endpoints **não requerem permissão explícita**:

- `GET /swagger/*` - Documentação Swagger
- `POST /api/auth/token` - Geração de token JWT
- `POST /api/auth/token-form` - Geração de token (formulário)

#### Exemplo de Resposta Negada

```json
HTTP/1.1 403 Forbidden
Content-Type: application/json

{
  "mensagem": "Acesso não autorizado para este endpoint.",
  "endpoint": "/api/vendas",
  "metodo": "GET"
}
```

#### Implementação

O controle é implementado através do **`PermissaoTerceiroMiddleware`**:

```csharp
// Program.cs
app.UseMiddleware<PermissaoTerceiroMiddleware>();
```

O middleware:
1. Extrai o `id_terceiro` do JWT token
2. Consulta o banco de dados para verificar permissão
3. Retorna **403 Forbidden** se não autorizado
4. Continua normalmente se autorizado

#### Configurar Permissões

As permissões são armazenadas na tabela `SETA_TERCEIRO_PERMISSOES` e devem ser configuradas via:

- Dashboard administrativo (recomendado)
- Scripts SQL diretos (apenas para administradores)
- API de configuração (se disponível)

**Exemplo de estrutura:**

| IdTerceiro | Endpoint | Metodo | Ativo |
|-----------|----------|--------|-------|
| 1 | /api/vendas | GET | Sim |
| 1 | /api/vendas | POST | Sim |
| 1 | /api/estoques | GET | Sim |
| 2 | /api/vendas | GET | Sim |
| 2 | /api/lojas | GET | Sim |

---

### 📊 Logs de Auditoria de Acesso

A API registra automaticamente **todos os acessos e operações** realizadas pelos terceiros (integradores), criando um histórico completo para auditoria, análise de performance e troubleshooting.

#### O que é Registrado

| Campo | Descrição | Exemplo |
|-------|-----------|---------|
| **ID Terceiro** | Identificador do integrador | `123` |
| **Client ID** | Identificador do cliente dentro do JWT | `app-vendas-001` |
| **Nome Terceiro** | Nome do integrador (do token) | `SantaLolla Vendas` |
| **Método HTTP** | GET, POST, PUT, DELETE, PATCH | `GET` |
| **Endpoint** | Caminho da requisição | `/api/vendas?filtro=pendentes` |
| **Query String** | Parâmetros da URL | `filtro=pendentes&limite=10` |
| **Status Code** | Código HTTP da resposta | `200`, `400`, `403`, `500` |
| **Tempo de Resposta** | Duração em milissegundos | `125` ms |
| **IP de Origem** | Endereço IP do cliente | `192.168.1.100` |
| **User Agent** | Navegador/cliente usado | `Mozilla/5.0 (Windows...)` |
| **Mensagem de Erro** | Se houver erro | `Connection timeout` |
| **Data/Hora** | Timestamp da requisição | `2024-01-15 10:30:45` |

#### Fluxo de Captura

```
┌─────────────────────────────────────────────────┐
│         Requisição HTTP                         │
│  GET /api/vendas                                │
│  Authorization: Bearer {JWT}                    │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
    ┌────────────────────────┐
    │  ApiLogMiddleware      │
    │  (inicia cronômetro)   │
    └────────────┬───────────┘
                 │
                 ▼
    ┌────────────────────────┐
    │  Outros Middlewares    │
    │  Controllers           │
    │  Lógica de Negócio     │
    └────────────┬───────────┘
                 │
                 ▼
    ┌────────────────────────┐
    │  Resposta HTTP         │
    │  (cronômetro para)     │
    └────────────┬───────────┘
                 │
                 ▼
    ┌────────────────────────────────────────┐
    │  Compor Log (tudo que foi capturado)   │
    │  - ID Terceiro (do JWT)                │
    │  - Endpoint, Método, Status Code       │
    │  - Tempo de resposta                   │
    │  - IP, User Agent                      │
    │  - Erros (se houver)                   │
    └────────────┬─────────────────────────┘
                 │
                 ▼
    ┌────────────────────────────────────────┐
    │  Gravar em Banco de Dados              │
    │  Tabela: SETA_API_LOG_ACESSOS          │
    └────────────────────────────────────────┘
```

#### Modelo de Dados - ApiLogAcesso

```csharp
public class ApiLogAcesso
{
    public long? IdTerceiro { get; set; }        // ID do integrador
    public string? ClientId { get; set; }        // Client ID do JWT
    public string? NomeTerceiro { get; set; }    // Nome do integrador

    public string MetodoHttp { get; set; }       // GET, POST, etc
    public string Endpoint { get; set; }         // /api/vendas
    public string? QueryString { get; set; }     // Parâmetros da URL

    public int? StatusCode { get; set; }         // 200, 404, 500
    public int? TempoRespostaMs { get; set; }    // Tempo em ms

    public string? IpOrigem { get; set; }        // IP do cliente
    public string? UserAgent { get; set; }       // Browser/Cliente

    public string? MensagemErro { get; set; }    // Erro se houver
}
```

#### Armazenamento em Banco de Dados

Os logs são armazenados na tabela **`SETA_API_LOG_ACESSOS`** com a seguinte estrutura:

```sql
CREATE TABLE SETA_API_LOG_ACESSOS (
    ID_LOG_ACESSO BIGINT PRIMARY KEY IDENTITY(1,1),
    ID_TERCEIRO BIGINT NULL,
    CLIENT_ID NVARCHAR(100) NULL,
    NOME_TERCEIRO NVARCHAR(100) NULL,
    METODO_HTTP NVARCHAR(10),
    ENDPOINT NVARCHAR(300),
    QUERY_STRING NVARCHAR(MAX) NULL,
    STATUS_CODE INT,
    TEMPO_RESPOSTA_MS INT,
    IP_ORIGEM NVARCHAR(100),
    USER_AGENT NVARCHAR(500),
    MENSAGEM_ERRO NVARCHAR(MAX) NULL,
    DATA_REQUISICAO DATETIME DEFAULT GETDATE()
);
```

#### Extracting Claims do JWT

Os dados do log são extraídos automaticamente do JWT token através dos seguintes claims:

| Claim | Campo no Log | Descrição |
|-------|-------------|-----------|
| `id_terceiro` ou `sub` | IdTerceiro | Identificador único do integrador |
| `client_id` ou `name` | ClientId | Identificador do cliente/aplicação |
| `nome` | NomeTerceiro | Nome amigável do terceiro |

#### Exclusões de Log

Os seguintes requisições **NÃO são registradas** para evitar poluição de logs:

- `GET /swagger/*` - Documentação da API
- Requisições de healthcheck (se configurado)

#### Casos de Uso

**Auditoria e Compliance:**
- Rastrear quem acessou quais dados e quando
- Identificar tentativas de acesso não autorizado
- Gerar relatórios de conformidade

**Troubleshooting:**
- Analisar erros específicos de um cliente
- Identificar endpoints com problemas de performance
- Debugar integrações problemáticas

**Análise de Performance:**
- Monitorar tempo de resposta por endpoint
- Identificar gargalos
- Otimizar recursos

**Segurança:**
- Detectar padrões suspeitos (múltiplas 403, velocidade anormal)
- Rastrear atividades após mudanças de acesso
- Validar cumprimento de políticas de segurança

---

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
Lista vendas filtradas por período de venda, período de atualização, rede, loja, nota fiscal, observação e paginação.

```
Request:
GET /api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- dataInicio (DateTime, opcional): Data inicial da venda
- dataFim (DateTime, opcional): Data final da venda
- lastUpdateInicio (DateTime, opcional): Data/hora inicial de atualização da venda
- lastUpdateFim (DateTime, opcional): Data/hora final de atualização da venda
- rede (string, opcional): Código da rede
- codigoLoja (string, opcional): Código da loja/empresa
- codigoVenda (string, opcional): Código da venda
- notaFiscal (string, opcional): Número da nota fiscal (suporta LIKE)
- obs (string, opcional): Observação da venda (suporta LIKE)
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Registros por página (máx: 5000)

Validação: Pelo menos um período deve ser informado:
  - dataInicio/dataFim OU
  - lastUpdateInicio/lastUpdateFim

Response (200 OK):
{
  "total": 250,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 5,
  "dados": [
    {
      "rede": "rede000001",
      "codigoLoja": "00000001",
      "marca": "SANTA LOLLA",
      "cnpj": "28.803.454/0001-10",
      "aliasId": "LOJA001",
      "apelido": "SL - OSCAR FREIRE",
      "nome": "SANTA LOLLA FRANQUIAS LTDA",
      "codigoVenda": "123456",
      "vendaVinculada": "01162820",
      "tipoOperacao": "VENDA",
      "dataVenda": "2026-06-11",
      "horaVenda": "14:30",
      "notaFiscal": "00012345",
      "serie": "1",
      "emissaoNf": "2026-06-11",
      "dataAtualizacao": "2026-06-11T09:30:00",
      "codigoCliente": "00012345",
      "cpfCnpjCliente": "030.508.974-90",
      "cliente": "MARIA SILVA",
      "codigoVendedor": "00084745",
      "vendedor": "THAIS MARTINS",
      "condicoes": "CARTAO",
      "qtdeParcelas": 2,
      "parcelas": [
        {
          "parcela": "01",
          "vencimento": "2026-08-24",
          "valor": 119.40
        }
      ],
      "pagamentos": [
        {
          "codigoVenda": "123456",
          "codigoTitulo": "T0001",
          "status": "PAGO",
          "lancamento": "2026-06-11",
          "vencimento": "2026-07-11",
          "pagamento": "2026-06-11",
          "valor": 199.90,
          "valorPago": 199.90,
          "numeroParcela": "01",
          "totalParcelas": "02",
          "posTef": "POS",
          "forma": "CC",
          "instituicao": "123",
          "nomeInstituicao": "BANCO EXEMPLO",
          "operadora": "OP01",
          "bandeira": "VISA",
          "nomeOperadora": "REDE",
          "autorizacao": "123456",
          "nsu": "654321"
        }
      ],
      "total": 238.80,
      "frete": 10.00,
      "custo": 150.00,
      "vendaImportada": "Sim",
      "status": "S",
      "obs": "VENDA IMPORTADA EM 2026-06-17"
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

---

### 4. Vendas Produtos (`/api/vendasprodutos`)

#### GET `/api/vendasprodutos` - Consultar Produtos das Vendas
Retorna os itens/produtos agrupados por venda, com filtros por venda, rede, vendedor, referência, loja, cliente, data da venda e data de atualização.

A paginação é realizada por venda, e não por item.

```
Request:
GET /api/vendasprodutos?dataVendaInicio=2024-01-01&dataVendaFim=2024-01-31&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- rede (string, opcional): Código da rede
- venda (string, opcional): Código da venda
- codVendedor (string, opcional): Código do vendedor
- referencia (string, opcional): Referência do produto (suporta LIKE)
- apelido (string, opcional): Apelido/nome fantasia da loja (suporta LIKE)
- cliente (string, opcional): Nome do cliente (suporta LIKE)
- dataVendaInicio (DateTime, opcional): Data inicial da venda
- dataVendaFim (DateTime, opcional): Data final da venda
- lastUpdateInicio (DateTime, opcional): Data/hora inicial de atualização
- lastUpdateFim (DateTime, opcional): Data/hora final de atualização
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Vendas por página (máx: 5000)

Response (200 OK):
{
  "total": 120,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 3,
  "dados": [
    {
      "rede": "rede000001",
      "codigoVenda": "00247577",
      "vendaVinculada": "01162820",
      "tipoOperacao": "VENDA",
      "codCliente": "00012345",
      "codVendedor": "00084745",
      "codigoEmpresa": "00000001",
      "marca": "SANTA LOLLA",
      "cnpj": "28.803.454/0001-10",
      "apelido": "SL - OSCAR FREIRE",
      "nome": "SANTA LOLLA FRANQUIAS LTDA",
      "dataVenda": "2026-06-11",
      "lastUpdateOrigem": "2026-06-11T09:30:00",
      "cliente": "MARIA SILVA",
      "vendedor": "THAIS MARTINS",
      "condicoes": "CARTAO",
      "vendaImportada": "Sim",
      "status": "S",
      "produtos": [
        {
          "referencia": "038F.11E4.0048.0157",
          "barras": "7891234567890",
          "codigoProduto": "123681",
          "tamanho": "37",
          "quantidade": 1,
          "unitario": 199.90,
          "desconto": 0.00,
          "total": 199.90,
          "frete": 10.00,
          "totalFrete": 209.90,
          "custo": 120.00,
          "colecao": "VERAO 2027",
          "fornecedor": "12.345.678/0001-90"
        }
      ]
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "dataVendaInicio não pode ser maior que dataVendaFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

---

### 5. Clientes Varejo (`/api/clientesvarejo`)

#### GET `/api/clientesvarejo` - Consultar Clientes de Varejo
Retorna os clientes de varejo cadastrados, permitindo filtros por rede, código, CPF/CNPJ, nome e data de atualização.

O filtro de nome permite LIKE. Exemplo: `%Marcio%`.

```
Request:
GET /api/clientesvarejo?nome=%25Marcio%25&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- rede (string, opcional): Código da rede/schema de origem
- codigoCliente (string, opcional): Código único do cliente na origem
- nome (string, opcional): Nome do cliente (suporta LIKE)
- cpfCnpj (string, opcional): CPF ou CNPJ do cliente
- atualizadoInicio (DateTime, opcional): Data/hora inicial de atualização
- atualizadoFim (DateTime, opcional): Data/hora final de atualização
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 50): Registros por página (máx: 5000)

Response (200 OK):
{
  "total": 500,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 10,
  "dados": [
    {
      "rede": "rede000001",
      "codigoCliente": "00012345",
      "nome": "MARCIO JUNIOR",
      "apelido": "MARCIO",
      "pessoa": 1,
      "cpfCnpj": "123.456.789-00",
      "rgIe": "12.345.678-9",
      "docAuxiliar": "",
      "email": "marcio@email.com",
      "telefone1": "(11) 98765-4321",
      "telefone2": null,
      "telefone3": null,
      "telefone4": null,
      "cep": "01.426-003",
      "endereco": "RUA OSCAR FREIRE, 1000",
      "bairro": "JARDINS",
      "cidade": "SÃO PAULO",
      "uf": "SP",
      "complemento": "APTO 101",
      "enderecoCompleto": "RUA OSCAR FREIRE, 1000, APTO 101, JARDINS, SÃO PAULO, SP",
      "naturalidade": "SÃO PAULO/SP",
      "origem": "LOJA",
      "estadoCivil": "CASADO",
      "sexo": "M",
      "nascimento": "1985-03-10",
      "aniversario": "10/03",
      "status": "Ativo",
      "grupo": "VIP",
      "atividade": "001",
      "descricaoAtividade": "COMÉRCIO VAREJISTA",
      "responsavel": "00010",
      "nomeResponsavel": "JOÃO SILVA",
      "empresa": "MATRIZ",
      "cliente": true,
      "fornecedor": false,
      "funcionario": false,
      "transportadora": false,
      "conveniado": "Não",
      "credito": 5000.00,
      "bloqueia": 0.00,
      "cadastro": "2020-01-15",
      "atualizado": "2026-06-11T10:30:00",
      "obs": null,
      "marca": "SANTA LOLLA",
      "dataUltimaCompra": "2026-08-25"
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "atualizadoInicio não pode ser maior que atualizadoFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

---

### 6. Estoques (`/api/estoques`)

#### GET `/api/estoques` - Listar Estoque Atual Detalhado
Lista o estoque atual detalhado por loja, produto, tamanho, cor e referência.

```
Request:
GET /api/estoques?rede=rede000001&codigoLoja=00000001&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- rede (string, opcional): Código da rede
- codigoLoja (string, opcional): Código da loja/empresa
- codigoProduto (string, opcional): Código base do produto
- referencia (string, opcional): Referência do produto
- tamanho (string, opcional): Tamanho do produto
- cor (string, opcional): Cor do produto
- descricaoColecao (string, opcional): Descrição da coleção (suporta LIKE)
- descricaoLinha (string, opcional): Descrição da linha (suporta LIKE)
- dataAtualizacaoInicio (DateTime, opcional): Data/hora inicial de atualização
- dataAtualizacaoFim (DateTime, opcional): Data/hora final de atualização
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Registros por página (máx: 5000)

Response (200 OK):
{
  "total": 1500,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 30,
  "dados": [
    {
      "rede": "rede000001",
      "codigoEmpresa": "00000001",
      "cnpj": "28.803.454/0001-10",
      "apelidoEmpresa": "SL - OSCAR FREIRE",
      "nomeEmpresa": "SANTA LOLLA FRANQUIAS LTDA",
      "codigoProduto": "123681",
      "codigoLinx": "LINX123",
      "descricaoProduto": "TENIS SUEDE BAKED",
      "tamanho": "37",
      "cor": "PRETO",
      "grade": "ABC",
      "referencia": "038F.11E4.0048.0157",
      "marca": "SANTA LOLLA",
      "grupo": "CALÇADOS",
      "descricaoColecao": "VERAO 2027",
      "descricaoLinha": "CASUAL",
      "quantidade": 15,
      "custo": 45.50,
      "preco": 89.90,
      "preco1": 85.00,
      "preco2": 75.00,
      "dataEstoque": "2026-06-11",
      "dataCriacao": "2026-01-15",
      "dataAtualizacao": "2026-06-11T07:44:02"
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "dataAtualizacaoInicio não pode ser maior que dataAtualizacaoFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

#### GET `/api/estoques/total-agrupado` - Listar Estoque Total Agrupado
Lista estoques agrupados por produto, referência, marca, grupo, coleção e linha, considerando todas as lojas, com totalização de quantidade e médias de custo/preço.

```
Request:
GET /api/estoques/total-agrupado?nomeLoja=%25oscar%25&referencia=%25038F%25&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- nomeLoja (string, opcional): Nome da loja com suporte a LIKE (ex: %oscar%)
- referencia (string, opcional): Referência do produto com suporte a LIKE
- descricaoColecao (string, opcional): Descrição da coleção com suporte a LIKE
- descricaoLinha (string, opcional): Descrição da linha com suporte a LIKE
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Registros por página (máx: 5000)

Response (200 OK):
{
  "total": 320,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 7,
  "dados": [
    {
      "rede": "rede000001",
      "codigoProduto": "123681",
      "descricaoProduto": "TENIS SUEDE BAKED",
      "referencia": "038F.11E4.0048.0157",
      "marca": "SANTA LOLLA",
      "grupo": "CALÇADOS",
      "descricaoColecao": "VERAO 2027",
      "descricaoLinha": "CASUAL",
      "quantidadeTotal": 3,
      "custo": 45.50,
      "preco": 89.90,
      "preco1": 85.00,
      "preco2": 75.00
    }
  ]
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

---

### 7. Lojas (`/api/lojas`)

#### GET `/api/lojas` - Listar Lojas
Lista todas as lojas cadastradas com filtros por rede, código da loja e período de atualização.

```
Request:
GET /api/lojas?rede=rede000001&codigoLoja=00000001&lastUpdateInicio=2024-01-01T00:00:00&lastUpdateFim=2024-01-31T23:59:59&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- rede (string, opcional): Código da rede
- codigoLoja (string, opcional): Código da loja/empresa
- lastUpdateInicio (DateTime, opcional): Data/hora inicial de atualização
- lastUpdateFim (DateTime, opcional): Data/hora final de atualização
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Registros por página (máx: 5000)

Response (200 OK):
{
  "total": 245,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 5,
  "dados": [
    {
      "rede": "rede000001",
      "codigoLoja": "00000001",
      "nomeFantasia": "SL - OSCAR FREIRE",
      "cnpj": "28.803.454/0001-10",
      "cep": "01.426-003",
      "marca": "SANTA LOLLA",
      "dataAtualizacao": "2026-06-11T07:44:02"
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "lastUpdateInicio não pode ser maior que lastUpdateFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

---

### 8. Vendedores (`/api/vendedores`)

#### GET `/api/vendedores` - Listar Vendedores
Lista os vendedores cadastrados, permitindo filtros por rede, loja, código do vendedor e período de atualização.

```
Request:
GET /api/vendedores?rede=rede000001&codigoLoja=00000001&codigoVendedor=00084745&lastUpdateInicio=2024-01-01T00:00:00&lastUpdateFim=2024-01-31T23:59:59&pagina=1&tamanhoPagina=50 HTTP/1.1
Authorization: Bearer {JWT_TOKEN}

Query Parameters:
- rede (string, opcional): Código da rede
- codigoLoja (string, opcional): Código da loja/empresa
- codigoVendedor (string, opcional): Código do vendedor
- lastUpdateInicio (DateTime, opcional): Data/hora inicial de atualização
- lastUpdateFim (DateTime, opcional): Data/hora final de atualização
- pagina (int, padrão: 1): Número da página
- tamanhoPagina (int, padrão: 500): Registros por página (máx: 5000)

Response (200 OK):
{
  "total": 150,
  "pagina": 1,
  "tamanhoPagina": 50,
  "totalPaginas": 3,
  "dados": [
    {
      "rede": "rede000001",
      "codigoLoja": "00000001",
      "nomeLoja": "SL - OSCAR FREIRE",
      "marca": "SANTALOLLA",
      "codigoVendedor": "00084745",
      "nome": "THAIS MARTINS PEREIRA GODOIS",
      "cpf": "453.580.568-74",
      "cargo": "VENDEDORA",
      "apelido": "THAIS MARTINS",
      "dataAdmissao": "2019-02-01",
      "dataDemissao": null,
      "dataAtualizacao": "2026-06-11T10:09:32",
      "status": "Ativo"
    }
  ]
}

Response (400 Bad Request):
{
  "mensagem": "lastUpdateInicio não pode ser maior que lastUpdateFim."
}

Response (401 Unauthorized):
{
  "mensagem": "Token ausente, expirado ou inválido."
}
```

> ⚠️ Requer autenticação JWT (`[Authorize]`)

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
builder.Services.AddScoped<IVendaProdutoRepository, VendaProdutoRepository>();
builder.Services.AddScoped<IClienteVarejoRepository, ClienteVarejoRepository>();
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

## 📋 Alterações Recentes (Changelog)

### v1.2.0 - [2024-01-XX]

#### ✅ Novidades

1. **GET `/api/vendas`** - Novo campo `horaVenda`
   - Adicionado campo `horaVenda` (string, formato HH:MM) no response
   - Fonte: `SETA_VENDAS_DETALHE.HORA_VENDA`
   - Campo opcional, sem segundos

2. **Modelos de Vendas** - Estrutura de response enriquecida
   - `VendaResponse` agora inclui `vendaVinculada`, `tipoOperacao`, `marca`, `aliasId`, `apelido`, `nome`, `cpfCnpjCliente`
   - Suporte a listas de `parcelas` e `pagamentos`

3. **GET `/api/estoques/total-agrupado`** - Novo filtro e campo
   - Adicionado filtro `descricaoLinha`
   - Response passa a incluir `descricaoLinha`

#### 📚 Documentação

- README atualizado para refletir todos os endpoints, filtros e responses conforme os DTOs atuais
- Adicionadas seções completas para `Vendas Produtos`, `Clientes Varejo` e `Estoques`
- Removida documentação obsoleta (ex: POST `/api/vendas` inexistente)
- Padronização do nome `tamanhoPagina` em todos os endpoints

---

### v1.1.0 - [2024-01-XX]

#### ✅ Novos Endpoints Adicionados

1. **GET `/api/vendasprodutos`** - Operações de Produtos de Vendas
   - Retorna produtos/itens agrupados por venda
   - Filtros: dataVendaInicio/Fim, lastUpdateInicio/Fim, rede, venda, codVendedor, referência, apelido, cliente
   - Paginação por venda (não por item)
   - Padrão: 500 vendas por página (máx: 5000)

2. **GET `/api/clientesvarejo`** - Consulta de Clientes de Varejo
   - Filtra clientes por rede, código, CPF/CNPJ, nome
   - Suporte a filtro LIKE no campo nome
   - Validação de períodos de atualização
   - Padrão: 50 registros por página (máx: 5000)

3. **GET `/api/estoques/total-agrupado`** - Estoques Agrupados e Totalizados
   - Agrupa estoques por produto, referência, marca, grupo, coleção e linha
   - Retorna: SUM(quantidade) e AVG(preços)
   - Filtros disponíveis: nomeLoja (LIKE), referência (LIKE), descricaoColecao, descricaoLinha
   - Removido: Campo `codigoColecao` conforme requisição
   - Mantido: Campo `descricaoColecao`

#### ⚙️ Endpoints Atualizados

1. **GET `/api/lojas`** - Melhorias na Paginação
   - Adicionados filtros: `rede`, `codigoLoja`, `lastUpdateInicio`, `lastUpdateFim`
   - Paginação: 500 registros por página (máx: 5000)
   - Adicionada validação de períodos

2. **GET `/api/vendas`** - Mantém compatibilidade
   - Validação obrigatória de períodos (dataInicio/Fim ou lastUpdateInicio/Fim)
   - Suporta filtros de rede, loja, nota fiscal e observação

#### 🏗️ Infraestrutura

- Adicionados repositories:
  - `IVendaProdutoRepository` / `VendaProdutoRepository`
  - `IClienteVarejoRepository` / `ClienteVarejoRepository`

---

## 📞 Contato e Suporte

- **Repositório**: https://github.com/mcirillojr/SantaLolla.Api
- **Desenvolvedor**: mcirillojr
- **Ambiente**: ASP.NET Core 8.0

---

**Última atualização**: 2024-01-XX | **Versão API**: v1.2.0
