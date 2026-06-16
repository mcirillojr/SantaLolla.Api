using SantaLolla.Api.Configurations;
using Microsoft.Extensions.Options;

namespace SantaLolla.Api.Workers
{
    public class SantaLollaWorker : BackgroundService
    {
        private readonly ILogger<SantaLollaWorker> _logger;
        private readonly SantaLollaSettings _settings;

        public SantaLollaWorker(
            ILogger<SantaLollaWorker> logger,
            IOptions<SantaLollaSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SantaLollaWorker iniciado em: {DataHora}", DateTime.Now);

            if (!_settings.ExecutarWorkerAutomaticamente)
            {
                _logger.LogWarning("SantaLollaWorker está desativado pelo appsettings.json.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "SantaLollaWorker executando em: {DataHora}. Intervalo: {Intervalo} segundos",
                    DateTime.Now,
                    _settings.IntervaloExecucaoSegundos
                );

                await Task.Delay(
                    TimeSpan.FromSeconds(_settings.IntervaloExecucaoSegundos),
                    stoppingToken
                );
            }

            _logger.LogInformation("SantaLollaWorker finalizado em: {DataHora}", DateTime.Now);
        }
    }
}
