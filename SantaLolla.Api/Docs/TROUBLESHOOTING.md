# 🔧 Guia de Troubleshooting - SantaLolla API

## Índice
1. [Problemas de Autenticação](#problemas-de-autenticação)
2. [Problemas de Conexão](#problemas-de-conexão)
3. [Erros de Validação](#erros-de-validação)
4. [Performance](#performance)
5. [FAQ](#faq)

---

## Problemas de Autenticação

### ❌ Erro: 401 Unauthorized - "Token Ausente"

**Sintoma**: Requisição retorna 401 com mensagem "Token ausente"

```
GET /api/vendas HTTP/1.1
Host: localhost:5000

→ 401 Unauthorized
  "mensagem": "Token ausente"
```

**Soluções**:

1. ✅ Verificar se o header Authorization foi adicionado:
```bash
curl -X GET http://localhost:5000/api/vendas \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

2. ✅ Validar formato do token:
```
Header Correto:   Authorization: Bearer eyJhbGciOi...
Header Errado:    Authorization: eyJhbGciOi...
Header Errado:    Authorization: Token eyJhbGciOi...
```

3. ✅ Verificar se o token foi gerado corretamente:
```bash
# Gerar novo token
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "seu-client-id",
    "clientSecret": "seu-client-secret"
  }'
```

---

### ❌ Erro: 401 Unauthorized - "Token Expirado"

**Sintoma**: Token funciona inicialmente mas depois falha

```
→ 401 Unauthorized
  "mensagem": "Token expirado ou inválido"
```

**Soluções**:

1. ✅ Verificar tempo de expiração (padrão: 3600 segundos):
```bash
# Na resposta do token, verificar:
{
  "expiresIn": 3600,           # 1 hora em segundos
  "expirationTime": "2024-01-15T15:30:00Z"
}
```

2. ✅ Implementar renovação automática de token:
```csharp
public class TokenManager
{
    private string _token;
    private DateTime _expiracaoToken;

    public async Task<string> ObterTokenValido()
    {
        if (_expiracaoToken < DateTime.UtcNow.AddMinutes(5))
        {
            _token = await RenovarToken();
            _expiracaoToken = DateTime.UtcNow.AddHours(1);
        }

        return _token;
    }
}
```

3. ✅ Implementar retry com novo token:
```bash
# Ao receber 401, gerar novo token e tentar novamente
MAX_RETRIES=1
for attempt in {1..MAX_RETRIES}; do
  if [ $attempt -gt 1 ]; then
    TOKEN=$(obter_novo_token)
  fi

  curl -X GET $URL \
    -H "Authorization: Bearer $TOKEN"

  if [ $? -ne 401 ]; then break; fi
done
```

---

### ❌ Erro: 401 Unauthorized - "ClientId ou ClientSecret Inválido"

**Sintoma**: Token não é gerado

```
POST /api/auth/token
{
  "clientId": "erro-na-credencial",
  "clientSecret": "chave-errada"
}

→ 401 Unauthorized
  "mensagem": "ClientId ou ClientSecret inválido."
```

**Soluções**:

1. ✅ Verificar credenciais na configuração:
```json
// appsettings.json
{
  "Authentication": {
    "ValidClients": [
      {
        "ClientId": "terceiroapi-001",
        "ClientSecret": "chave-secreta-muito-segura"
      }
    ]
  }
}
```

2. ✅ Verificar variáveis de ambiente:
```bash
# Listar variáveis
$env:SANTA_LOLLA_CLIENT_ID
$env:SANTA_LOLLA_CLIENT_SECRET
```

3. ✅ Testa com curl:
```bash
curl -X POST http://localhost:5000/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "terceiroapi-001",
    "clientSecret": "chave-secreta-muito-segura"
  }'
```

---

## Problemas de Conexão

### ❌ Erro: "Connection Refused"

**Sintoma**: Não consegue conectar à API

```
error: connection refused
Error: connect ECONNREFUSED 127.0.0.1:5000
```

**Soluções**:

1. ✅ Verificar se a API está rodando:
```bash
# Windows - Verificar porta 5000
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000

# Ou testar com curl
curl -v http://localhost:5000/api/health
```

2. ✅ Iniciar a API:
```bash
cd C:\C#\SantaLolla.Api
dotnet run

# Ou via Visual Studio
# F5 ou Debug > Start Debugging
```

3. ✅ Verificar porta correta:
```bash
# Padrão: http://localhost:5000
# Produção: https://seudominio.com/api

# Listar portas em uso (Windows)
netstat -ano
```

4. ✅ Verificar firewall:
```bash
# Windows Defender - Permitir porta 5000
netsh advfirewall firewall add rule name="Allow Port 5000" \
  dir=in action=allow protocol=tcp localport=5000
```

---

### ❌ Erro: "Connection Timeout"

**Sintoma**: Requisição demora muito e falha

```
Error: Operation timed out after 30000ms
```

**Soluções**:

1. ✅ Aumentar timeout:
```csharp
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(60); // 60 segundos
```

2. ✅ Verificar performance do servidor:
```bash
# Verificar CPU
tasklist /v

# Verificar memória
Get-Process dotnet | Select-Object ws, vm
```

3. ✅ Verificar conexão de rede:
```bash
# Testar conectividade
ping localhost
telnet localhost 5000

# Diagnóstico de rede
tracert localhost
```

---

### ❌ Erro: "Certificate Error"

**Sintoma**: HTTPS falha com erro de certificado

```
error: certificate verify failed
error: CERTIFICATE_VERIFY_FAILED
```

**Soluções**:

1. ✅ Verificar certificado SSL em produção:
```bash
# Testar certificado
openssl s_client -connect seudominio.com:443

# Verificar validade
openssl x509 -in cert.pem -text -noout
```

2. ✅ Permitir certificados auto-assinados (DEV ONLY):
```csharp
// ⚠️ APENAS PARA DESENVOLVIMENTO
using (var handler = new HttpClientHandler())
{
    handler.ServerCertificateCustomValidationCallback = 
        (message, cert, chain, errors) => true;

    var client = new HttpClient(handler);
    // ...
}
```

3. ✅ Usar correct protocol:
```
https://seudominio.com (produção)
http://localhost:5000    (desenvolvimento)
```

---

## Erros de Validação

### ❌ Erro: 400 Bad Request - "Campo Obrigatório"

**Sintoma**: 
```json
POST /api/vendas
{
  "rede": "RD01",
  // falta "loja"
}

→ 400 Bad Request
{
  "mensagem": "Erro de validação",
  "detalhes": {
    "loja": ["Loja é obrigatória"]
  }
}
```

**Soluções**:

1. ✅ Verificar schema da requisição:
```json
// Válido
{
  "rede": "RD01",
  "loja": "L001",
  "data": "2024-01-15T10:30:00Z",
  "valor": 1500.50,
  "itens": 5
}
```

2. ✅ Validar tipos de dados:
```json
// Campos esperados
{
  "rede": "string",     // obrigatório
  "loja": "string",     // obrigatório
  "data": "datetime",   // obrigatório
  "valor": "decimal",   // obrigatório
  "itens": "integer"    // obrigatório
}
```

---

### ❌ Erro: 400 Bad Request - "Período Não Informado"

**Sintoma**:
```
GET /api/vendas

→ 400 Bad Request
{
  "mensagem": "Informe pelo menos um período..."
}
```

**Soluções**:

1. ✅ Adicionar filtro de período:
```bash
# Opção 1: Por data
?dataInicio=2024-01-01&dataFim=2024-01-31

# Opção 2: Por atualização
?lastUpdateInicio=2024-01-01&lastUpdateFim=2024-01-31

# Opção 3: Combinado
?dataInicio=2024-01-01&lastUpdateFim=2024-01-31
```

2. ✅ Formato de data correto:
```
Correto:   2024-01-15
Correto:   2024-01-15T10:30:00Z
Errado:    15/01/2024
Errado:    01-15-2024
```

---

### ❌ Erro: 422 Unprocessable Entity

**Sintoma**: Validação falha
```json
→ 422 Unprocessable Entity
{
  "mensagem": "Erro de validação",
  "detalhes": {
    "valor": ["Valor deve ser maior que 0"]
  }
}
```

**Soluções**:

1. ✅ Verificar regras de validação:
```
Valor: mínimo 0.01
Quantidade: >= 0
Data: não pode ser futura
```

2. ✅ Validar antes de enviar:
```csharp
if (venda.Valor <= 0)
    throw new ArgumentException("Valor deve ser maior que 0");

if (venda.Data > DateTime.Now)
    throw new ArgumentException("Data não pode ser futura");
```

---

## Performance

### 🐌 Problema: Requisições Lentas

**Sintoma**: Requisições levam mais de 5 segundos

**Soluções**:

1. ✅ Usar paginação:
```bash
# Sem paginação (LENTO)
GET /api/vendas?dataInicio=2020-01-01&dataFim=2024-01-31

# Com paginação (RÁPIDO)
GET /api/vendas?dataInicio=2024-01-01&dataFim=2024-01-31&pagina=1&itensPorPagina=50
```

2. ✅ Filtrar específico:
```bash
# Vago
GET /api/estoques

# Específico
GET /api/estoques?rede=RD01&loja=L001&pagina=1
```

3. ✅ Monitorar com logs:
```csharp
var stopwatch = Stopwatch.StartNew();

var vendas = await _repository.ObterVendas(filtro);

stopwatch.Stop();
_logger.LogInformation($"Requisição levou {stopwatch.ElapsedMilliseconds}ms");
```

4. ✅ Implementar cache:
```csharp
[MemoryCache(Duration = 300)] // 5 minutos
public async Task<List<Loja>> ObterLojas()
{
    return await _repository.ObterLojas();
}
```

---

### 💾 Problema: "Out of Memory"

**Sintoma**: API falha com erro de memória

```
OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.
```

**Soluções**:

1. ✅ Processar em chunks:
```csharp
// ❌ Não fazer
var todasVendas = await _repository.ObterTodasVendas();

// ✅ Fazer
const int pageSize = 1000;
for (int page = 1; page <= totalPages; page++)
{
    var vendas = await _repository.ObterVendas(page, pageSize);
    ProcessarLote(vendas);
}
```

2. ✅ Limitar resultados:
```bash
# ❌ Sem limite
GET /api/vendas

# ✅ Com limite
GET /api/vendas?pagina=1&itensPorPagina=100
```

3. ✅ Monitorar memória:
```bash
# Ver uso de memória
Get-Process dotnet | Select-Object pm, vm
```

---

## FAQ

### ❓ Como aumentar ou diminuir tempo de expiração do token?

Edite `appsettings.json`:
```json
{
  "JwtSettings": {
    "ExpirationMinutes": 120  // 2 horas
  }
}
```

---

### ❓ É seguro enviar ClientSecret no header?

**NÃO!** Use apenas via POST (body):
```bash
# ✅ CORRETO
POST /api/auth/token
{
  "clientId": "...",
  "clientSecret": "..."
}

# ❌ ERRADO
GET /api/vendas?clientSecret=...
```

---

### ❓ Como habilitar logs detalhados?

Edit `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

---

### ❓ Qual é o rate limit da API?

Padrão: **1000 requisições por hora**

Se exceder:
```json
→ 429 Too Many Requests
{
  "mensagem": "Limite de requisições excedido",
  "retryAfter": 60
}
```

**Solução**: Implementar backoff exponencial:
```csharp
int delay = 1000; // 1 segundo
while (response.StatusCode == 429)
{
    await Task.Delay(delay);
    delay *= 2; // Duplicar a cada tentativa
    response = await client.GetAsync(url);
}
```

---

### ❓ Como reportar um bug?

1. Criar issue no GitHub: https://github.com/mcirillojr/SantaLolla.Api/issues
2. Incluir:
   - Descrição do problema
   - Passos para reproduzir
   - Resposta da API (JSON)
   - Logs relevantes
   - Versão do .NET usada

Exemplo:
```
Título: POST /api/vendas retorna 500 ao enviar valor decimal

Descrição:
Ao tentar criar uma venda com valor "1.500,50", a API retorna erro 500.

Passos:
1. POST /api/vendas
2. Enviar: { "rede": "RD01", "valor": 1500.50 }
3. Receber StatusCode: 500

Logs:
2024-01-15 12:30:45 ERR ArgumentException: ...
```

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0
