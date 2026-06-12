using AlterVision.Api.Configurations;
using Microsoft.Extensions.Options;

namespace AlterVision.Api.Workers
{
    public class AlterVisionWorker : BackgroundService
    {
        private readonly ILogger<AlterVisionWorker> _logger;
        private readonly AlterVisionSettings _settings;

        public AlterVisionWorker(
            ILogger<AlterVisionWorker> logger,
            IOptions<AlterVisionSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AlterVisionWorker iniciado em: {DataHora}", DateTime.Now);

            if (!_settings.ExecutarWorkerAutomaticamente)
            {
                _logger.LogWarning("AlterVisionWorker está desativado pelo appsettings.json.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "AlterVisionWorker executando em: {DataHora}. Intervalo: {Intervalo} segundos",
                    DateTime.Now,
                    _settings.IntervaloExecucaoSegundos
                );

                await Task.Delay(
                    TimeSpan.FromSeconds(_settings.IntervaloExecucaoSegundos),
                    stoppingToken
                );
            }

            _logger.LogInformation("AlterVisionWorker finalizado em: {DataHora}", DateTime.Now);
        }
    }
}