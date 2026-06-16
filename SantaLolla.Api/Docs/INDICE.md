# 📚 Índice de Documentação - SantaLolla API

## Documentos Disponíveis

### 1. 📖 README.md
**Localização**: `SantaLolla.Api/README.md`
**Tamanho**: ~25KB
**Descrição**: Documentação técnica completa do projeto incluindo:
- ✅ Visão geral da API
- ✅ Stack de tecnologia (.NET 8)
- ✅ Arquitetura em camadas
- ✅ Fluxo de autenticação JWT
- ✅ Todos os endpoints disponíveis
- ✅ Exemplo de integração completo
- ✅ Configuração e inicialização

**Quando usar**: Para entender a estrutura geral do projeto e ter uma visão macro.

---

### 2. 🔐 API_FUNCIONAMENTO.md
**Localização**: `SantaLolla.Api/Docs/API_FUNCIONAMENTO.md`
**Tamanho**: ~20KB
**Descrição**: Documentação funcional com exemplos práticos de todos os endpoints:

#### Seções:
- **Autenticação**: Fluxo JWT Bearer completo
- **Endpoints**: GET/POST detalhados para cada módulo
- **Resposta de Sucesso**: Exemplos JSON de 200 OK
- **Resposta de Erro**: Exemplos JSON de 400/401/404
- **Exemplos em Múltiplas Linguagens**:
  - 🔧 cURL
  - 🟩 C#
  - 🐍 Python
  - 🟨 JavaScript/Node.js

#### Endpoints Documentados:
| Módulo | Endpoints |
|--------|-----------|
| 🔐 Auth | POST /token, POST /token-form |
| 🏥 Health | GET /health |
| 📊 Vendas | GET /, GET /{id}, POST / |
| 📦 Estoques | GET /, GET /total |
| 🏪 Lojas | GET /, GET /{id} |
| 👥 Vendedores | GET /, GET /{id} |

**Quando usar**: Para implementar integração com a API ou validar requisições/respostas.

---

### 3. 📖 CASOS_DE_USO.md
**Localização**: `SantaLolla.Api/Docs/CASOS_DE_USO.md`
**Tamanho**: ~28KB
**Descrição**: Seis casos de uso práticos do mundo real:

#### Casos Inclusos:

**Caso 1: Relatório de Vendas Mensal**
- 🎯 Gerar relatório de vendas por rede/loja
- 📊 Implementações em cURL e C#
- 📈 Agregação e processamento de dados

**Caso 2: Monitoramento de Estoques**
- ⚠️ Identificar produtos críticos
- 🟢🟡🔴 Categorização por gravidade
- 🐍 Implementação em Python

**Caso 3: Performance de Vendedores**
- 🏆 Ranking de performance
- 📊 Comparação meta vs realizado
- 🟨 Implementação em JavaScript

**Caso 4: Sincronização de Dados**
- 🔄 Sincronização periódica (hourly)
- 📦 BackgroundService .NET
- 💾 Sincronização bidirecional

**Caso 5: Dashboard em Tempo Real**
- 📊 Dados atualizados em tempo real
- 🌐 Implementação com WebSocket HTML5
- 📈 Visualização de métricas

**Caso 6: Integração com ERP**
- 🔄 Sincronização entre SantaLolla e ERP
- 🔀 Bidirectional sync
- 📝 Padrão de integração

**Quando usar**: Para ver exemplos práticos de como usar a API em cenários reais.

---

### 4. 🔧 TROUBLESHOOTING.md
**Localização**: `SantaLolla.Api/Docs/TROUBLESHOOTING.md`
**Tamanho**: ~11KB
**Descrição**: Guia completo de troubleshooting e respostas para problemas comuns:

#### Seções:

**Problemas de Autenticação**
- ❌ Token ausente
- ❌ Token expirado
- ❌ ClientId/ClientSecret inválido
- ✅ Soluções para cada problema

**Problemas de Conexão**
- ❌ Connection refused
- ❌ Connection timeout
- ❌ Certificate error
- ✅ Troubleshooting passo a passo

**Erros de Validação**
- ❌ Campo obrigatório
- ❌ Período não informado
- ❌ Validação de tipo
- ✅ Como corrigir

**Performance**
- 🐌 Requisições lentas
- 💾 Out of memory
- 📊 Paginação correta
- ✅ Otimizações

**FAQ** (Frequently Asked Questions)
- ❓ Como aumentar expiração do token?
- ❓ É seguro enviar ClientSecret?
- ❓ Como habilitar logs?
- ❓ Qual é o rate limit?
- ❓ Como reportar bugs?

**Quando usar**: Quando encontrar um erro na integração ou durante testes.

---

## Mapa de Navegação Rápida

### Para Começar:
1. 📖 Leia **README.md** para entender arquitetura
2. 🔐 Vá para **API_FUNCIONAMENTO.md** seção "Autenticação"
3. 📝 Copie um exemplo em sua linguagem preferida

### Para Implementar um Caso de Uso:
1. 📖 Vá a **CASOS_DE_USO.md**
2. 🔍 Encontre o caso mais similar
3. 💻 Adapte o código para suas necessidades
4. 🧪 Teste com os exemplos de **API_FUNCIONAMENTO.md**

### Para Debugar um Erro:
1. 🔧 Abra **TROUBLESHOOTING.md**
2. 🔍 Procure pela mensagem de erro
3. ✅ Siga as soluções propostas
4. 📞 Se persistir, veja seção "Como reportar um bug"

---

## Fluxo Recomendado para Integração

```
┌─────────────────────────────────────────┐
│ 1. Ler README.md (30 min)               │
│    - Entender arquitetura               │
│    - Conhecer tecnologias               │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ 2. Ler API_FUNCIONAMENTO.md (20 min)    │
│    - Seção Autenticação                 │
│    - Endpoints básicos                  │
│    - Exemplos na sua linguagem           │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ 3. Implementar Autenticação (20 min)    │
│    - Gerar token JWT                    │
│    - Salvar token                       │
│    - Renovar quando necessário          │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ 4. Procurar Caso de Uso Similar (15...)│
│    em CASOS_DE_USO.md                   │
│    - Estudar implementação               │
│    - Adaptar para seus dados            │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ 5. Testar com API_FUNCIONAMENTO.md      │
│    - Fazer requisições                  │
│    - Validar respostas                  │
│    - Debugar com TROUBLESHOOTING.md     │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│ ✅ INTEGRAÇÃO COMPLETA!                 │
└─────────────────────────────────────────┘
```

---

## Recursos no GitHub

📦 **Repository**: https://github.com/mcirillojr/SantaLolla.Api

### Estrutura de Pastas:
```
SantaLolla.Api/
├── README.md                      # Documentação geral
├── SantaLolla.Api/
│   ├── Docs/
│   │   ├── API_FUNCIONAMENTO.md   # Endpoints e exemplos
│   │   ├── CASOS_DE_USO.md        # 6 casos práticos
│   │   ├── TROUBLESHOOTING.md     # Guia de troubleshooting
│   │   └── *.sql                  # Scripts SQL
│   ├── Controllers/               # Endpoints API
│   ├── Models/                    # DTOs
│   ├── Repositories/              # Acesso a dados
│   ├── Services/                  # Lógica negócio
│   └── Program.cs                 # Inicialização
```

---

## Versão dos Documentos

| Documento | Data | Versão | Status |
|-----------|------|--------|--------|
| README.md | 2024-01-15 | 1.0 | ✅ Final |
| API_FUNCIONAMENTO.md | 2024-01-15 | 1.0 | ✅ Final |
| CASOS_DE_USO.md | 2024-01-15 | 1.0 | ✅ Final |
| TROUBLESHOOTING.md | 2024-01-15 | 1.0 | ✅ Final |

---

## Suporte

### Dúvidas sobre Documentação?
- 📧 Email: api-support@santLolla.com.br
- 🐛 GitHub Issues: https://github.com/mcirillojr/SantaLolla.Api/issues
- 💬 Swagger UI: http://localhost:5000/swagger

### Encontrou um Erro na Documentação?
1. Crie uma issue no GitHub
2. Descreva qual documento e onde
3. Sugira uma correção

---

## Próximos Passos

- [ ] Instalar dependências: `dotnet restore`
- [ ] Compilar: `dotnet build`
- [ ] Executar: `dotnet run`
- [ ] Acessar Swagger: http://localhost:5000/swagger
- [ ] Testar endpoints com exemplos de **API_FUNCIONAMENTO.md**

---

**Última atualização**: 2024-01-15
**Documentação versão**: 1.0
**API versão**: v1
