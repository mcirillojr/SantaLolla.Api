# 📚 Documentação Funcional - APIs SantaLolla

## Índice
1. [Visão Geral](#visão-geral)
2. [Autenticação](#autenticação)
3. [Endpoints por Módulo](#endpoints-por-módulo)
4. [Exemplos Funcionais](#exemplos-funcionais)
5. [Códigos de Status](#códigos-de-status)
6. [Tratamento de Erros](#tratamento-de-erros)

---

## Visão Geral

A **SantaLolla API** fornece acesso integrado a dados de vendas, estoques, lojas e vendedores através de endpoints REST com autenticação JWT.

### Informações Base
- **URL Base**: `http://localhost:5000/api` (desenvolvimento)
- **Versão API**: v1
- **Content-Type**: `application/json`
- **Autenticação**: JWT Bearer Token
- **Documentação Interativa**: `http://localhost:5000/swagger`

---

## Autenticação

### 🔐 Fluxo de Autenticação

Toda requisição a endpoints protegidos requer um token JWT válido.

#### 1️⃣ Obter Token

**Endpoint**: `POST /api/auth/token`

Gera um novo token JWT a partir das credenciais do cliente.

```http
POST /api/auth/token HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "clientId": "client-001",
  "clientSecret": "super-secret-key-123"
}
```

**Resposta - 200 OK**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjbGllbnQtMDAxIiwiaWF0IjoxNzA1MzIzMDAwLCJleHAiOjE3MDUzMjY2MDB9.XYZ...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "expirationTime": "2024-01-15T15:30:00Z"
}
```

**Resposta - 401 Unauthorized**:
```json
{
  "mensagem": "ClientId ou ClientSecret inválido."
}
```

---

#### 2️⃣ Usar Token em Requisições

Adicione o token no header `Authorization`:

```http
GET /api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Token Expirado - 401 Unauthorized**:
```json
{
  "mensagem": "Token expirado ou inválido."
}
```

---

## Endpoints por Módulo

### 🏥 Módulo Health Check

#### GET /api/health - Verificar Status da API

Verifica se a API está funcionando corretamente.

```http
GET /api/health HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "status": "OK",
  "sistema": "SantaLolla.Api",
  "mensagem": "API Santa Lolla em execução",
  "dataHora": "2024-01-15T12:30:45.123Z"
}
```

---

### 🔐 Módulo Autenticação

#### POST /api/auth/token - Gerar Token (JSON)

```http
POST /api/auth/token HTTP/1.1
Host: localhost:5000
Content-Type: application/json
Content-Length: 65

{
  "clientId": "terceiroapi-001",
  "clientSecret": "chave-secreta-muito-segura-12345"
}
```

**Resposta - 200 OK**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRJZCI6InRlcmNlaXJvYXBpLTAwMSIsImlhdCI6MTcwNTMyMzAwMH0.abc123xyz789...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "expirationTime": "2024-01-15T15:30:00Z"
}
```

---

#### POST /api/auth/token-form - Gerar Token (Form)

Alternativa usando formulário URL-encoded.

```http
POST /api/auth/token-form HTTP/1.1
Host: localhost:5000
Content-Type: application/x-www-form-urlencoded
Content-Length: 80

clientId=terceiroapi-001&clientSecret=chave-secreta-muito-segura-12345
```

**Resposta - 200 OK**: (mesma resposta anterior)

---

### 📊 Módulo Vendas

#### GET /api/vendas - Listar Vendas

Retorna lista de vendas com filtros avançados.

**Query Parameters**:
- `dataInicio` (datetime, opcional): Data inicial da venda (formato: YYYY-MM-DD ou ISO 8601)
- `dataFim` (datetime, opcional): Data final da venda
- `lastUpdateInicio` (datetime, opcional): Data inicial de última atualização
- `lastUpdateFim` (datetime, opcional): Data final de última atualização
- `rede` (string, opcional): Código da rede comercial
- `loja` (string, opcional): Código da loja
- `pagina` (int, default: 1): Número da página
- `itensPorPagina` (int, default: 50): Quantidade de itens por página

**Observação Importante**: Pelo menos um período deve ser informado:
- `dataInicio` E/OU `dataFim` OU
- `lastUpdateInicio` E/OU `lastUpdateFim`

```http
GET /api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31&rede=RD01&pagina=1&itensPorPagina=10 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "total": 250,
  "pagina": 1,
  "itensPorPagina": 10,
  "totalPaginas": 25,
  "vendas": [
    {
      "id": "V20240115001",
      "data": "2024-01-15T10:30:00Z",
      "rede": "RD01",
      "loja": "L001",
      "valor": 1500.50,
      "itens": 5,
      "lastUpdate": "2024-01-15T10:45:00Z",
      "status": "finalizada"
    },
    {
      "id": "V20240115002",
      "data": "2024-01-15T11:15:00Z",
      "rede": "RD01",
      "loja": "L002",
      "valor": 2350.75,
      "itens": 8,
      "lastUpdate": "2024-01-15T11:20:00Z",
      "status": "finalizada"
    }
  ]
}
```

**Resposta - 400 Bad Request** (Sem período informado):
```json
{
  "mensagem": "Informe pelo menos um período: dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim."
}
```

---

#### GET /api/vendas/{id} - Obter Detalhes de uma Venda

```http
GET /api/vendas/V20240115001 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "id": "V20240115001",
  "data": "2024-01-15T10:30:00Z",
  "rede": "RD01",
  "loja": "L001",
  "valor": 1500.50,
  "itens": 5,
  "lastUpdate": "2024-01-15T10:45:00Z",
  "status": "finalizada",
  "produtos": [
    {
      "id": "P001",
      "nome": "Produto A",
      "quantidade": 2,
      "valorUnitario": 500.00,
      "valorTotal": 1000.00
    },
    {
      "id": "P002",
      "nome": "Produto B",
      "quantidade": 3,
      "valorUnitario": 166.83,
      "valorTotal": 500.50
    }
  ]
}
```

**Resposta - 404 Not Found**:
```json
{
  "mensagem": "Venda com ID V20240115001 não encontrada."
}
```

---

#### POST /api/vendas - Criar Nova Venda

```http
POST /api/vendas HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
Content-Type: application/json

{
  "rede": "RD01",
  "loja": "L001",
  "data": "2024-01-15T14:30:00Z",
  "valor": 1500.50,
  "itens": 5,
  "status": "pendente"
}
```

**Resposta - 201 Created**:
```json
{
  "id": "V20240115003",
  "rede": "RD01",
  "loja": "L001",
  "data": "2024-01-15T14:30:00Z",
  "valor": 1500.50,
  "itens": 5,
  "status": "pendente",
  "criadoEm": "2024-01-15T14:32:00Z"
}
```

**Resposta - 400 Bad Request** (Dados inválidos):
```json
{
  "mensagem": "Rede é obrigatória.",
  "erros": {
    "rede": ["O campo rede é obrigatório"]
  }
}
```

---

### 📦 Módulo Estoques

#### GET /api/estoques - Listar Estoques

Lista estoques disponíveis com filtros.

**Query Parameters**:
- `rede` (string, opcional): Código da rede
- `loja` (string, opcional): Código da loja
- `produto` (string, opcional): Código do produto
- `pagina` (int, default: 1): Página
- `itensPorPagina` (int, default: 50): Itens por página

```http
GET /api/estoques?rede=RD01&loja=L001&pagina=1&itensPorPagina=20 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "total": 120,
  "pagina": 1,
  "itensPorPagina": 20,
  "estoques": [
    {
      "id": "E001",
      "rede": "RD01",
      "loja": "L001",
      "produto": "P001",
      "nomeProduto": "Produto A",
      "quantidade": 100,
      "quantidadeMinima": 10,
      "quantidadeMaxima": 500,
      "ultimaAtualizacao": "2024-01-15T10:00:00Z"
    },
    {
      "id": "E002",
      "rede": "RD01",
      "loja": "L001",
      "produto": "P002",
      "nomeProduto": "Produto B",
      "quantidade": 50,
      "quantidadeMinima": 5,
      "quantidadeMaxima": 200,
      "ultimaAtualizacao": "2024-01-15T10:15:00Z"
    }
  ]
}
```

---

#### GET /api/estoques/total - Estoques Agrupados por Rede/Loja

```http
GET /api/estoques/total?rede=RD01 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "totais": [
    {
      "rede": "RD01",
      "loja": "L001",
      "quantidadeTotal": 5000,
      "quantidadeProdutos": 45,
      "ultimaAtualizacao": "2024-01-15T10:00:00Z"
    },
    {
      "rede": "RD01",
      "loja": "L002",
      "quantidadeTotal": 3500,
      "quantidadeProdutos": 42,
      "ultimaAtualizacao": "2024-01-15T10:15:00Z"
    }
  ]
}
```

---

### 🏪 Módulo Lojas

#### GET /api/lojas - Listar Lojas

```http
GET /api/lojas?rede=RD01&pagina=1 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "total": 12,
  "pagina": 1,
  "itensPorPagina": 50,
  "lojas": [
    {
      "id": "L001",
      "rede": "RD01",
      "nome": "Loja Centro - São Paulo",
      "endereco": "Rua Principal, 123",
      "cidade": "São Paulo",
      "estado": "SP",
      "cep": "01234-567",
      "telefone": "(11) 3123-4567",
      "email": "loja.centro@santLolla.com.br",
      "status": "ativa",
      "proximaAtualizacao": "2024-01-20T10:00:00Z"
    },
    {
      "id": "L002",
      "rede": "RD01",
      "nome": "Loja Vila - São Paulo",
      "endereco": "Avenida Secundária, 456",
      "cidade": "São Paulo",
      "estado": "SP",
      "cep": "02345-678",
      "telefone": "(11) 3234-5678",
      "email": "loja.vila@santLolla.com.br",
      "status": "ativa",
      "proximaAtualizacao": "2024-01-20T10:30:00Z"
    }
  ]
}
```

---

#### GET /api/lojas/{id} - Obter Detalhes de uma Loja

```http
GET /api/lojas/L001 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "id": "L001",
  "rede": "RD01",
  "nome": "Loja Centro - São Paulo",
  "endereco": "Rua Principal, 123",
  "cidade": "São Paulo",
  "estado": "SP",
  "cep": "01234-567",
  "telefone": "(11) 3123-4567",
  "email": "loja.centro@santLolla.com.br",
  "status": "ativa",
  "gerente": {
    "nome": "João Silva",
    "email": "joao.silva@santLolla.com.br",
    "telefone": "(11) 98765-4321"
  },
  "horarioFuncionamento": {
    "segunda": "09:00-20:00",
    "terca": "09:00-20:00",
    "quarta": "09:00-20:00",
    "quinta": "09:00-20:00",
    "sexta": "09:00-21:00",
    "sabado": "09:00-18:00",
    "domingo": "fechado"
  },
  "proximaAtualizacao": "2024-01-20T10:00:00Z"
}
```

---

### 👥 Módulo Vendedores

#### GET /api/vendedores - Listar Vendedores

```http
GET /api/vendedores?rede=RD01&status=ativo&pagina=1 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "total": 45,
  "pagina": 1,
  "itensPorPagina": 50,
  "vendedores": [
    {
      "id": "V001",
      "nome": "João Silva Santos",
      "email": "joao.silva@santLolla.com.br",
      "telefone": "(11) 98765-4321",
      "rede": "RD01",
      "loja": "L001",
      "cpf": "123.456.789-00",
      "status": "ativo",
      "dataAdmissao": "2020-03-15T00:00:00Z",
      "meta": 50000.00,
      "vendas": 52350.75,
      "comissao": 7852.61
    },
    {
      "id": "V002",
      "nome": "Maria Santos Oliveira",
      "email": "maria.santos@santLolla.com.br",
      "telefone": "(11) 98765-4322",
      "rede": "RD01",
      "loja": "L001",
      "cpf": "987.654.321-00",
      "status": "ativo",
      "dataAdmissao": "2021-06-20T00:00:00Z",
      "meta": 45000.00,
      "vendas": 43750.50,
      "comissao": 6187.21
    }
  ]
}
```

---

#### GET /api/vendedores/{id} - Obter Detalhes de um Vendedor

```http
GET /api/vendedores/V001 HTTP/1.1
Host: localhost:5000
Authorization: Bearer {TOKEN}
```

**Resposta - 200 OK**:
```json
{
  "id": "V001",
  "nome": "João Silva Santos",
  "email": "joao.silva@santLolla.com.br",
  "telefone": "(11) 98765-4321",
  "rede": "RD01",
  "loja": "L001",
  "cpf": "123.456.789-00",
  "status": "ativo",
  "dataAdmissao": "2020-03-15T00:00:00Z",
  "meta": 50000.00,
  "vendas": 52350.75,
  "comissao": 7852.61,
  "historico": [
    {
      "mes": "Janeiro 2024",
      "vendas": 5230.50,
      "comissao": 784.58,
      "meta": 50000.00
    },
    {
      "mes": "Dezembro 2023",
      "vendas": 4890.25,
      "comissao": 733.54,
      "meta": 50000.00
    }
  ]
}
```

---

## Exemplos Funcionais

### 🔄 Fluxo Completo: Do Acesso ao Relatório

**Passo 1: Obter Token**

```bash
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "terceiroapi-001",
    "clientSecret": "chave-secreta-12345"
  }'
```

**Resposta**:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresIn": 3600
}
```

---

**Passo 2: Usar Token para Consultar Vendas**

```bash
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET "http://localhost:5000/api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31&rede=RD01" \
  -H "Authorization: Bearer $TOKEN"
```

---

**Passo 3: Verificar Estoques de uma Loja**

```bash
curl -X GET "http://localhost:5000/api/estoques?rede=RD01&loja=L001" \
  -H "Authorization: Bearer $TOKEN"
```

---

**Passo 4: Consultar Informações de Lojas**

```bash
curl -X GET "http://localhost:5000/api/lojas?rede=RD01" \
  -H "Authorization: Bearer $TOKEN"
```

---

**Passo 5: Analisar Performance de Vendedores**

```bash
curl -X GET "http://localhost:5000/api/vendedores?rede=RD01&status=ativo" \
  -H "Authorization: Bearer $TOKEN"
```

---

### 📱 Integração em C# (.NET)

```csharp
// 1. Obter token
var client = new HttpClient();
var authRequest = new
{
    clientId = "terceiroapi-001",
    clientSecret = "chave-secreta-12345"
};

var response = await client.PostAsJsonAsync(
    "http://localhost:5000/api/auth/token",
    authRequest
);

var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
var token = tokenResponse.AccessToken;

// 2. Usar token em requisições
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

// 3. Consultar vendas
var vendas = await client.GetFromJsonAsync(
    "http://localhost:5000/api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31",
    typeof(VendaResponse[])
);

// 4. Processar dados
foreach (var venda in (VendaResponse[])vendas)
{
    Console.WriteLine($"Venda {venda.Id}: R$ {venda.Valor}");
}
```

---

### 🐍 Integração em Python

```python
import requests
import json
from datetime import datetime

# 1. Obter token
auth_url = "http://localhost:5000/api/auth/token"
auth_data = {
    "clientId": "terceiroapi-001",
    "clientSecret": "chave-secreta-12345"
}

auth_response = requests.post(auth_url, json=auth_data)
token = auth_response.json()["accessToken"]

# 2. Configurar headers
headers = {
    "Authorization": f"Bearer {token}",
    "Content-Type": "application/json"
}

# 3. Consultar vendas
vendas_url = "http://localhost:5000/api/vendas"
params = {
    "dataInicio": "2024-01-01",
    "dataFim": "2024-01-31",
    "rede": "RD01"
}

vendas_response = requests.get(vendas_url, headers=headers, params=params)
vendas = vendas_response.json()

# 4. Processar dados
for venda in vendas["vendas"]:
    print(f"Venda {venda['id']}: R$ {venda['valor']}")
```

---

### 🔗 Integração em JavaScript/Node.js

```javascript
// 1. Obter token
const getToken = async () => {
  const response = await fetch('http://localhost:5000/api/auth/token', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      clientId: 'terceiroapi-001',
      clientSecret: 'chave-secreta-12345'
    })
  });

  const data = await response.json();
  return data.accessToken;
};

// 2. Consultar vendas
const getVendas = async (token) => {
  const params = new URLSearchParams({
    dataInicio: '2024-01-01',
    dataFim: '2024-01-31',
    rede: 'RD01'
  });

  const response = await fetch(
    `http://localhost:5000/api/vendas?${params}`,
    {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );

  return await response.json();
};

// 3. Usar
async function main() {
  const token = await getToken();
  const vendas = await getVendas(token);

  vendas.vendas.forEach(v => {
    console.log(`Venda ${v.id}: R$ ${v.valor}`);
  });
}

main();
```

---

## Códigos de Status

| Código | Descrição | Exemplo |
|--------|-----------|---------|
| **200** | OK - Requisição bem-sucedida | Listar vendas |
| **201** | Created - Recurso criado com sucesso | Criar nova venda |
| **204** | No Content - Sucesso sem retorno | Deletar recurso |
| **400** | Bad Request - Dados inválidos | Período não informado |
| **401** | Unauthorized - Token ausente/inválido | Token expirado |
| **403** | Forbidden - Acesso negado | Sem permissão para recurso |
| **404** | Not Found - Recurso não encontrado | Venda inexistente |
| **409** | Conflict - Conflito de dados | ID duplicado |
| **422** | Unprocessable Entity - Validação falhou | Campo obrigatório faltando |
| **429** | Too Many Requests - Rate limit excedido | Muitas requisições |
| **500** | Internal Server Error - Erro no servidor | Exception não tratada |
| **503** | Service Unavailable - Serviço indisponível | BD offline |

---

## Tratamento de Erros

### Formato Padrão de Erro

```json
{
  "mensagem": "Descrição do erro",
  "codigo": "CODIGO_ERRO",
  "detalhes": {
    "campo": ["mensagem de validação"]
  },
  "timestamp": "2024-01-15T12:30:45Z"
}
```

### Exemplos Comuns

#### ❌ Token Expirado

**Status**: 401 Unauthorized

```json
{
  "mensagem": "Token expirado. Gere um novo token.",
  "codigo": "TOKEN_EXPIRED"
}
```

#### ❌ Período de Filtro Não Informado

**Status**: 400 Bad Request

```json
{
  "mensagem": "Informe pelo menos um período: dataInicio/dataFim ou lastUpdateInicio/lastUpdateFim.",
  "codigo": "PERIODO_OBRIGATORIO",
  "detalhes": {
    "periodo": ["Período não informado"]
  }
}
```

#### ❌ Validação de Campo

**Status**: 422 Unprocessable Entity

```json
{
  "mensagem": "Erro de validação",
  "codigo": "VALIDATION_ERROR",
  "detalhes": {
    "rede": ["Rede é obrigatória"],
    "valor": ["Valor deve ser maior que 0"]
  }
}
```

#### ❌ Recurso Não Encontrado

**Status**: 404 Not Found

```json
{
  "mensagem": "Venda V20240115001 não encontrada.",
  "codigo": "VENDA_NAO_ENCONTRADA"
}
```

---

## 📋 Resumo de Endpoints

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/token` | Gerar token JWT | ❌ |
| POST | `/api/auth/token-form` | Gerar token (form) | ❌ |
| GET | `/api/health` | Health check | ✅ |
| GET | `/api/vendas` | Listar vendas | ✅ |
| GET | `/api/vendas/{id}` | Detalhes venda | ✅ |
| POST | `/api/vendas` | Criar venda | ✅ |
| GET | `/api/estoques` | Listar estoques | ✅ |
| GET | `/api/estoques/total` | Estoques agrupados | ✅ |
| GET | `/api/lojas` | Listar lojas | ✅ |
| GET | `/api/lojas/{id}` | Detalhes loja | ✅ |
| GET | `/api/vendedores` | Listar vendedores | ✅ |
| GET | `/api/vendedores/{id}` | Detalhes vendedor | ✅ |

---

## 🔧 Configuração Recomendada

### Variáveis de Ambiente

```bash
# .env
SANTA_LOLLA_API_URL=http://localhost:5000
SANTA_LOLLA_CLIENT_ID=terceiroapi-001
SANTA_LOLLA_CLIENT_SECRET=chave-secreta-12345
SANTA_LOLLA_TOKEN_EXPIRATION=3600
```

### Timeout de Requisição

- **Padrão**: 30 segundos
- **Recomendado**: 30-60 segundos
- **Máximo**: 120 segundos

### Rate Limiting

- Limite: 1000 requisições por hora
- Retry-After: Aguarde header informado

---

## 📞 Suporte

Para dúvidas ou reportar problemas:
- 📧 Email: api-support@santLolla.com.br
- 🐛 Issues: https://github.com/mcirillojr/SantaLolla.Api/issues
- 📚 Swagger UI: http://localhost:5000/swagger

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0
