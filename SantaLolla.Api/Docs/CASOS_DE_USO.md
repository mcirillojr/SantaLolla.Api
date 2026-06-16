# 📖 Guia de Casos de Uso - SantaLolla API

## Índice
1. [Caso 1: Relatório de Vendas Mensal](#caso-1-relatório-de-vendas-mensal)
2. [Caso 2: Monitoramento de Estoques](#caso-2-monitoramento-de-estoques)
3. [Caso 3: Performance de Vendedores](#caso-3-performance-de-vendedores)
4. [Caso 4: Sincronização de Dados](#caso-4-sincronização-de-dados)
5. [Caso 5: Dashboard em Tempo Real](#caso-5-dashboard-em-tempo-real)
6. [Caso 6: Integração com ERP](#caso-6-integração-com-erp)

---

## Caso 1: Relatório de Vendas Mensal

### 🎯 Objetivo
Gerar relatório de vendas do mês de janeiro de 2024, agrupado por rede e loja.

### 📊 Fluxo

```
1. Obter Token JWT
   ↓
2. Consultar Vendas (dataInicio=2024-01-01, dataFim=2024-01-31)
   ↓
3. Agrupar por Rede/Loja
   ↓
4. Calcular Totais
   ↓
5. Gerar Relatório
```

### 🔧 Implementação em cURL

```bash
#!/bin/bash

# Configuração
API_URL="http://localhost:5000/api"
CLIENT_ID="terceiroapi-001"
CLIENT_SECRET="chave-secreta-12345"

# Step 1: Obter Token
echo "⏳ Obtendo token..."
TOKEN_RESPONSE=$(curl -s -X POST "$API_URL/auth/token" \
  -H "Content-Type: application/json" \
  -d "{
    \"clientId\": \"$CLIENT_ID\",
    \"clientSecret\": \"$CLIENT_SECRET\"
  }")

TOKEN=$(echo $TOKEN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
echo "✅ Token obtido: ${TOKEN:0:20}..."

# Step 2: Consultar Vendas
echo ""
echo "⏳ Consultando vendas..."
VENDAS=$(curl -s -X GET "$API_URL/vendas?dataInicio=2024-01-01&dataFim=2024-01-31" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Vendas consultadas:"
echo $VENDAS | jq '.'

# Step 3: Gerar Relatório
echo ""
echo "📊 RELATÓRIO DE VENDAS - JANEIRO 2024"
echo "======================================"
echo $VENDAS | jq '.vendas | group_by(.rede) | map({
  rede: .[0].rede,
  totalVendas: map(.valor) | add,
  qtdVendas: length,
  lojas: map(.loja) | unique
})'
```

### 💻 Implementação em C#

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class RelatorioVendas
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "http://localhost:5000/api";
    private readonly string _clientId = "terceiroapi-001";
    private readonly string _clientSecret = "chave-secreta-12345";

    public RelatorioVendas()
    {
        _httpClient = new HttpClient();
    }

    public async Task GerarRelatorioMensal()
    {
        try
        {
            // Step 1: Obter Token
            var token = await ObterToken();
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Step 2: Consultar Vendas
            var vendas = await ConsultarVendas(
                dataInicio: new DateTime(2024, 1, 1),
                dataFim: new DateTime(2024, 1, 31)
            );

            // Step 3: Processar Dados
            var relatorio = ProcessarRelatorio(vendas);

            // Step 4: Exibir Relatório
            ExibirRelatorio(relatorio);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }

    private async Task<string> ObterToken()
    {
        var request = new
        {
            clientId = _clientId,
            clientSecret = _clientSecret
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiUrl}/auth/token",
            request
        );

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsAsync<dynamic>();
        return result.accessToken;
    }

    private async Task<VendaResponse[]> ConsultarVendas(
        DateTime dataInicio, DateTime dataFim)
    {
        var url = $"{_apiUrl}/vendas" +
                  $"?dataInicio={dataInicio:yyyy-MM-dd}" +
                  $"&dataFim={dataFim:yyyy-MM-dd}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<dynamic>();
        return result.vendas;
    }

    private Dictionary<string, RelatorioRede> ProcessarRelatorio(VendaResponse[] vendas)
    {
        var relatorio = new Dictionary<string, RelatorioRede>();

        foreach (var venda in vendas)
        {
            if (!relatorio.ContainsKey(venda.Rede))
            {
                relatorio[venda.Rede] = new RelatorioRede { Rede = venda.Rede };
            }

            var redeReport = relatorio[venda.Rede];
            redeReport.TotalVendas += venda.Valor;
            redeReport.QtdVendas++;

            if (!redeReport.LojasTotais.ContainsKey(venda.Loja))
            {
                redeReport.LojasTotais[venda.Loja] = 0;
            }
            redeReport.LojasTotais[venda.Loja] += venda.Valor;
        }

        return relatorio;
    }

    private void ExibirRelatorio(Dictionary<string, RelatorioRede> relatorio)
    {
        Console.WriteLine("\n📊 RELATÓRIO DE VENDAS - JANEIRO 2024");
        Console.WriteLine("=====================================\n");

        decimal totalGeral = 0;

        foreach (var rede in relatorio.Values)
        {
            Console.WriteLine($"🏢 Rede: {rede.Rede}");
            Console.WriteLine($"   Total: R$ {rede.TotalVendas:F2}");
            Console.WriteLine($"   Qtd Vendas: {rede.QtdVendas}");
            Console.WriteLine($"   Lojas:");

            foreach (var loja in rede.LojasTotais)
            {
                Console.WriteLine($"      • {loja.Key}: R$ {loja.Value:F2}");
            }

            Console.WriteLine();
            totalGeral += rede.TotalVendas;
        }

        Console.WriteLine($"💰 TOTAL GERAL: R$ {totalGeral:F2}");
    }
}

public class RelatorioRede
{
    public string Rede { get; set; }
    public decimal TotalVendas { get; set; }
    public int QtdVendas { get; set; }
    public Dictionary<string, decimal> LojasTotais { get; set; } = new();
}
```

---

## Caso 2: Monitoramento de Estoques

### 🎯 Objetivo
Monitorar estoques críticos (abaixo da quantidade mínima) e alertar sobre possíveis rupturas.

### 📊 Fluxo

```
1. Obter Token
   ↓
2. Consultar Estoques
   ↓
3. Identificar Críticos (quantidade < mínimo)
   ↓
4. Agrupar por Gravidade
   ↓
5. Enviar Alertas
```

### 🔧 Implementação com Python

```python
import requests
import json
from datetime import datetime
from typing import List, Dict

class MonitorEstoques:
    def __init__(self, api_url: str, client_id: str, client_secret: str):
        self.api_url = api_url
        self.client_id = client_id
        self.client_secret = client_secret
        self.token = None
        self.headers = {}

    def executar_monitoramento(self):
        """Executa monitoramento completo de estoques"""
        try:
            # Step 1: Obter Token
            self.obter_token()
            print("✅ Token obtido")

            # Step 2: Obter Estoques Críticos
            criticos = self.get_estoques_criticos()
            print(f"⚠️  Encontrados {len(criticos)} estoques críticos")

            # Step 3: Processar e Alertar
            self.processar_alertas(criticos)

        except Exception as e:
            print(f"❌ Erro: {e}")

    def obter_token(self):
        """Obter JWT token"""
        url = f"{self.api_url}/auth/token"
        payload = {
            "clientId": self.client_id,
            "clientSecret": self.client_secret
        }

        response = requests.post(url, json=payload)
        response.raise_for_status()

        self.token = response.json()["accessToken"]
        self.headers = {
            "Authorization": f"Bearer {self.token}",
            "Content-Type": "application/json"
        }

    def get_estoques_criticos(self) -> List[Dict]:
        """Obter estoques críticos (quantidade < mínimo)"""
        url = f"{self.api_url}/estoques"
        params = {
            "pagina": 1,
            "itensPorPagina": 100
        }

        response = requests.get(url, headers=self.headers, params=params)
        response.raise_for_status()

        todos_estoques = response.json()["estoques"]

        # Filtrar estoques críticos
        criticos = [
            e for e in todos_estoques
            if e["quantidade"] < e["quantidadeMinima"]
        ]

        return criticos

    def processar_alertas(self, criticos: List[Dict]):
        """Processar e categorizar alertas"""

        # Categorizar por gravidade
        criticos_graves = []      # 0 a 25% do mínimo
        criticos_moderados = []   # 25% a 75% do mínimo
        criticos_leves = []       # 75% a 100% do mínimo

        for estoque in criticos:
            percentual = (estoque["quantidade"] / estoque["quantidadeMinima"]) * 100

            if percentual < 25:
                criticos_graves.append((estoque, percentual))
            elif percentual < 75:
                criticos_moderados.append((estoque, percentual))
            else:
                criticos_leves.append((estoque, percentual))

        # Exibir alertas
        print("\n🚨 ESTOQUES CRÍTICOS - MONITORAMENTO")
        print("====================================\n")

        if criticos_graves:
            print("🔴 CRÍTICOS GRAVES (ação imediata):")
            for estoque, pct in criticos_graves:
                print(f"  • {estoque['nomeProduto']}")
                print(f"    Loja: {estoque['loja']}")
                print(f"    Qtd: {estoque['quantidade']} ({pct:.1f}% do mínimo)")
                print(f"    Mínimo: {estoque['quantidadeMinima']}")
                print()

        if criticos_moderados:
            print("🟠 CRÍTICOS MODERADOS (ação em 24h):")
            for estoque, pct in criticos_moderados:
                print(f"  • {estoque['nomeProduto']}")
                print(f"    Qtd: {estoque['quantidade']} ({pct:.1f}% do mínimo)")
                print()

        if criticos_leves:
            print("🟡 ATENÇÃO (ação em 48h):")
            for estoque, pct in criticos_leves:
                print(f"  • {estoque['nomeProduto']}")
                print(f"    Qtd: {estoque['quantidade']} ({pct:.1f}% do mínimo)")
                print()

        # Resumo
        print("\n📊 RESUMO:")
        print(f"Total Críticos: {len(criticos)}")
        print(f"  - Graves: {len(criticos_graves)}")
        print(f"  - Moderados: {len(criticos_moderados)}")
        print(f"  - Leves: {len(criticos_leves)}")

# Usar
if __name__ == "__main__":
    monitor = MonitorEstoques(
        api_url="http://localhost:5000/api",
        client_id="terceiroapi-001",
        client_secret="chave-secreta-12345"
    )
    monitor.executar_monitoramento()
```

---

## Caso 3: Performance de Vendedores

### 🎯 Objetivo
Analisar performance de vendedores comparando meta vs realizado.

### 🔧 Implementação em JavaScript/Node.js

```javascript
const axios = require('axios');

class AnalisadorPerformanceVendedores {
    constructor(apiUrl, clientId, clientSecret) {
        this.apiUrl = apiUrl;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
        this.client = axios.create({
            baseURL: apiUrl
        });
    }

    async analisar() {
        try {
            // Step 1: Obter Token
            const token = await this.obterToken();
            console.log('✅ Token obtido\n');

            // Step 2: Obter Vendedores
            const vendedores = await this.obterVendedores(token);
            console.log(`📊 Analisando ${vendedores.length} vendedores\n`);

            // Step 3: Calcular Performance
            const performance = this.calcularPerformance(vendedores);

            // Step 4: Exibir Ranking
            this.exibirRanking(performance);

        } catch (error) {
            console.error('❌ Erro:', error.message);
        }
    }

    async obterToken() {
        const response = await this.client.post('/auth/token', {
            clientId: this.clientId,
            clientSecret: this.clientSecret
        });

        return response.data.accessToken;
    }

    async obterVendedores(token) {
        const response = await this.client.get('/vendedores?status=ativo', {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        return response.data.vendedores;
    }

    calcularPerformance(vendedores) {
        return vendedores.map(v => ({
            id: v.id,
            nome: v.nome,
            loja: v.loja,
            meta: v.meta,
            vendas: v.vendas,
            comissao: v.comissao,
            percentualMeta: (v.vendas / v.meta * 100).toFixed(2),
            diferenca: (v.vendas - v.meta).toFixed(2),
            status: v.vendas >= v.meta ? '✅ Atingiu' : '⚠️  Abaixo'
        })).sort((a, b) => b.percentualMeta - a.percentualMeta);
    }

    exibirRanking(performance) {
        console.log('🏆 RANKING DE PERFORMANCE - JANEIRO 2024');
        console.log('=========================================\n');

        console.log('┌─────┬──────────────────────┬────────┬────────┬───────────┬──────────┐');
        console.log('│ Pos │ Nome                 │ Meta   │ Vendas │ % Meta    │ Status   │');
        console.log('├─────┼──────────────────────┼────────┼────────┼───────────┼──────────┤');

        performance.forEach((v, i) => {
            const pos = String(i + 1).padEnd(4);
            const nome = v.nome.padEnd(20);
            const meta = `R$${v.meta}`.padEnd(7);
            const vendas = `R$${v.vendas}`.padEnd(7);
            const pct = `${v.percentualMeta}%`.padEnd(10);
            const status = v.status.padEnd(8);

            console.log(`│ ${pos} │ ${nome} │ ${meta} │ ${vendas} │ ${pct}│ ${status}│`);
        });

        console.log('└─────┴──────────────────────┴────────┴────────┴───────────┴──────────┘');

        // Resumo
        const atingiramMeta = performance.filter(v => v.status.includes('✅')).length;
        console.log(`\n📈 Resumo:`);
        console.log(`   • Total de Vendedores: ${performance.length}`);
        console.log(`   • Atingiram Meta: ${atingiramMeta}`);
        console.log(`   • Taxa de Sucesso: ${(atingiramMeta / performance.length * 100).toFixed(1)}%`);
    }
}

// Usar
const analisador = new AnalisadorPerformanceVendedores(
    'http://localhost:5000/api',
    'terceiroapi-001',
    'chave-secreta-12345'
);

analisador.analisar();
```

---

## Caso 4: Sincronização de Dados

### 🎯 Objetivo
Sincronizar dados entre SantaLolla API e sistema local a cada hora.

### 🔧 Implementação com Scheduler

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SincronizadorSantaLolla : BackgroundService
{
    private readonly ILogger<SincronizadorSantaLolla> _logger;
    private readonly HttpClient _httpClient;
    private readonly ILocalDatabase _database;
    private readonly TimeSpan _intervaloSincronizacao = TimeSpan.FromHours(1);

    public SincronizadorSantaLolla(
        ILogger<SincronizadorSantaLolla> logger,
        HttpClient httpClient,
        ILocalDatabase database)
    {
        _logger = logger;
        _httpClient = httpClient;
        _database = database;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Sincronizador SantaLolla iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecutarSincronizacao();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Erro na sincronização: {ex.Message}");
            }

            await Task.Delay(_intervaloSincronizacao, stoppingToken);
        }
    }

    private async Task ExecutarSincronizacao()
    {
        _logger.LogInformation("⏳ Iniciando sincronização...");

        // 1. Obter Token
        var token = await ObterToken();

        // 2. Sincronizar Vendas
        await SincronizarVendas(token);

        // 3. Sincronizar Estoques
        await SincronizarEstoques(token);

        // 4. Sincronizar Lojas
        await SincronizarLojas(token);

        // 5. Sincronizar Vendedores
        await SincronizarVendedores(token);

        _logger.LogInformation("✅ Sincronização concluída");
    }

    private async Task<string> ObterToken()
    {
        var request = new
        {
            clientId = "terceiroapi-001",
            clientSecret = "chave-secreta-12345"
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:5000/api/auth/token",
            request
        );

        var result = await response.Content.ReadAsAsync<dynamic>();
        return result.accessToken;
    }

    private async Task SincronizarVendas(string token)
    {
        _logger.LogInformation("  📊 Sincronizando vendas...");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var dataInicio = DateTime.Now.AddHours(-1);
        var dataFim = DateTime.Now;

        var url = $"http://localhost:5000/api/vendas" +
                  $"?dataInicio={dataInicio:yyyy-MM-dd}T{dataInicio:HH:mm:ss}" +
                  $"&dataFim={dataFim:yyyy-MM-dd}T{dataFim:HH:mm:ss}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<dynamic>();
        var vendas = result.vendas;

        // Salvar no banco local
        await _database.SalvarVendas(vendas);

        _logger.LogInformation($"  ✅ {vendas.Length} vendas sincronizadas");
    }

    private async Task SincronizarEstoques(string token)
    {
        _logger.LogInformation("  📦 Sincronizando estoques...");

        var response = await _httpClient.GetAsync(
            "http://localhost:5000/api/estoques?pagina=1&itensPorPagina=1000"
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<dynamic>();
        var estoques = result.estoques;

        await _database.SalvarEstoques(estoques);

        _logger.LogInformation($"  ✅ {estoques.Length} estoques sincronizados");
    }

    private async Task SincronizarLojas(string token)
    {
        _logger.LogInformation("  🏪 Sincronizando lojas...");

        var response = await _httpClient.GetAsync(
            "http://localhost:5000/api/lojas?pagina=1&itensPorPagina=500"
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<dynamic>();
        var lojas = result.lojas;

        await _database.SalvarLojas(lojas);

        _logger.LogInformation($"  ✅ {lojas.Length} lojas sincronizadas");
    }

    private async Task SincronizarVendedores(string token)
    {
        _logger.LogInformation("  👥 Sincronizando vendedores...");

        var response = await _httpClient.GetAsync(
            "http://localhost:5000/api/vendedores?pagina=1&itensPorPagina=500"
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsAsync<dynamic>();
        var vendedores = result.vendedores;

        await _database.SalvarVendedores(vendedores);

        _logger.LogInformation($"  ✅ {vendedores.Length} vendedores sincronizados");
    }
}

// Registrar no Startup
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient();
    services.AddScoped<ILocalDatabase, LocalDatabase>();
    services.AddHostedService<SincronizadorSantaLolla>();
}
```

---

## Caso 5: Dashboard em Tempo Real

### 🎯 Objetivo
Criar dashboard com dados atualizados em tempo real.

### 🔧 Implementação com WebSocket

```html
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard SantaLolla</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 20px;
            padding: 20px;
        }
        .card {
            background: #f5f5f5;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .card h2 {
            margin-top: 0;
            color: #333;
        }
        .value {
            font-size: 32px;
            font-weight: bold;
            color: #00aa00;
            margin: 10px 0;
        }
        .status-ok { color: #00aa00; }
        .status-warning { color: #ffaa00; }
        .status-error { color: #ff0000; }
    </style>
</head>
<body>
    <div class="card">
        <h2>💰 Vendas do Dia</h2>
        <div class="value" id="vendas-dia">R$ 0,00</div>
        <small>Última atualização: <span id="ultima-vendas">-</span></small>
    </div>

    <div class="card">
        <h2>📦 Estoques Críticos</h2>
        <div class="value status-warning" id="estoques-criticos">0</div>
        <small>Produtos abaixo do mínimo</small>
    </div>

    <div class="card">
        <h2>🏆 Performance</h2>
        <div class="value status-ok" id="performance">0%</div>
        <small>Meta atingida</small>
    </div>

    <script>
        class DashboardSantaLolla {
            constructor(apiUrl, clientId, clientSecret) {
                this.apiUrl = apiUrl;
                this.clientId = clientId;
                this.clientSecret = clientSecret;
                this.token = null;
            }

            async iniciar() {
                console.log('🚀 Iniciando Dashboard...');
                await this.obterToken();
                await this.atualizarDados();

                // Atualizar a cada 5 minutos
                setInterval(() => this.atualizarDados(), 5 * 60 * 1000);
            }

            async obterToken() {
                const response = await fetch(`${this.apiUrl}/auth/token`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        clientId: this.clientId,
                        clientSecret: this.clientSecret
                    })
                });

                const data = await response.json();
                this.token = data.accessToken;
            }

            async atualizarDados() {
                try {
                    await this.atualizarVendas();
                    await this.atualizarEstoques();
                    await this.atualizarPerformance();
                } catch (error) {
                    console.error('❌ Erro:', error);
                }
            }

            async atualizarVendas() {
                const hoje = new Date().toISOString().split('T')[0];
                const response = await fetch(
                    `${this.apiUrl}/vendas?dataInicio=${hoje}&dataFim=${hoje}`,
                    { headers: { Authorization: `Bearer ${this.token}` } }
                );

                const data = await response.json();
                const total = data.vendas.reduce((acc, v) => acc + v.valor, 0);

                document.getElementById('vendas-dia').textContent = 
                    `R$ ${total.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`;
                document.getElementById('ultima-vendas').textContent = 
                    new Date().toLocaleTimeString('pt-BR');
            }

            async atualizarEstoques() {
                const response = await fetch(
                    `${this.apiUrl}/estoques`,
                    { headers: { Authorization: `Bearer ${this.token}` } }
                );

                const data = await response.json();
                const criticos = data.estoques.filter(e => e.quantidade < e.quantidadeMinima);

                document.getElementById('estoques-criticos').textContent = criticos.length;
            }

            async atualizarPerformance() {
                const response = await fetch(
                    `${this.apiUrl}/vendedores?status=ativo`,
                    { headers: { Authorization: `Bearer ${this.token}` } }
                );

                const data = await response.json();
                const atingiram = data.vendedores.filter(v => v.vendas >= v.meta).length;
                const percentual = (atingiram / data.vendedores.length * 100).toFixed(0);

                document.getElementById('performance').textContent = percentual + '%';
            }
        }

        // Iniciar
        const dashboard = new DashboardSantaLolla(
            'http://localhost:5000/api',
            'terceiroapi-001',
            'chave-secreta-12345'
        );

        dashboard.iniciar();
    </script>
</body>
</html>
```

---

## Caso 6: Integração com ERP

### 🎯 Objetivo
Sincronizar dados bidirecionais com sistema ERP existente.

### 🔧 Implementação

```csharp
public class IntegradorERP
{
    private readonly SantaLollaClient _santaLollaClient;
    private readonly ERPClient _erpClient;
    private readonly ILogger<IntegradorERP> _logger;

    public async Task SincronizarVendas()
    {
        // 1. Obter últimas vendas do ERP
        var vendasERP = await _erpClient.ObterUltimasVendas();
        _logger.LogInformation($"📊 {vendasERP.Count} vendas obtidas do ERP");

        // 2. Enviar para SantaLolla
        foreach (var venda in vendasERP)
        {
            var resultado = await _santaLollaClient.CriarVenda(new VendaRequest
            {
                Rede = venda.Rede,
                Loja = venda.Loja,
                Data = venda.Data,
                Valor = venda.Valor,
                Itens = venda.Itens
            });

            _logger.LogInformation($"✅ Venda criada: {resultado.Id}");
        }

        // 3. Atualizar status no ERP
        foreach (var venda in vendasERP)
        {
            await _erpClient.AtualizarVendaSincronizada(venda.Id);
        }

        _logger.LogInformation("🔄 Sincronização de vendas concluída");
    }

    public async Task SincronizarEstoques()
    {
        // 1. Obter estoques da SantaLolla
        var estoquesSantaLolla = await _santaLollaClient.ObterEstoques();

        // 2. Atualizar no ERP
        foreach (var estoque in estoquesSantaLolla)
        {
            await _erpClient.AtualizarEstoque(new EstoqueUpdate
            {
                Produto = estoque.Produto,
                Loja = estoque.Loja,
                Quantidade = estoque.Quantidade
            });
        }

        _logger.LogInformation("🔄 Sincronização de estoques concluída");
    }
}
```

---

**Última atualização**: 2024-01-15 | **Versão**: 1.0
