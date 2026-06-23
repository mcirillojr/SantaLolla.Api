using SantaLolla.Api.Models.Logs;
using SantaLolla.Api.Repositories.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace SantaLolla.Api.Middlewares
{
    public class ApiLogMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IApiLogRepository apiLogRepository
        )
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Evita poluir log com arquivos do Swagger
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            string? mensagemErro = null;
            int statusCodeLog = StatusCodes.Status200OK;

            try
            {
                await _next(context);

                statusCodeLog = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                statusCodeLog = StatusCodes.Status500InternalServerError;
                mensagemErro = ex.Message;

                throw;
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    var log = MontarLog(
                        context,
                        statusCodeLog,
                        Convert.ToInt32(stopwatch.ElapsedMilliseconds),
                        mensagemErro
                    );

                    await apiLogRepository.GravarAsync(log);
                }
                catch
                {
                    // Não deixa erro de gravação de log quebrar a API
                }
            }
        }

        private static ApiLogAcesso MontarLog(
            HttpContext context,
            int statusCode,
            int tempoRespostaMs,
            string? mensagemErro
        )
        {
            var idTerceiroClaim =
                context.User.FindFirst("id_terceiro")?.Value ??
                context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            long? idTerceiro = null;

            if (long.TryParse(idTerceiroClaim, out var idConvertido))
            {
                idTerceiro = idConvertido;
            }

            var clientId =
                context.User.FindFirst("client_id")?.Value ??
                context.User.FindFirst(ClaimTypes.Name)?.Value ??
                context.User.FindFirst("sub")?.Value;

            var nomeTerceiro =
                context.User.FindFirst("nome")?.Value;

            var ipOrigem =
                context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ipOrigem))
            {
                ipOrigem = context.Connection.RemoteIpAddress?.ToString();
            }

            return new ApiLogAcesso
            {
                IdTerceiro = idTerceiro,
                ClientId = clientId,
                NomeTerceiro = nomeTerceiro,

                MetodoHttp = context.Request.Method.ToUpper(),
                Endpoint = context.Request.Path.Value ?? string.Empty,
                QueryString = context.Request.QueryString.HasValue
                    ? context.Request.QueryString.Value
                    : null,

                StatusCode = statusCode,
                TempoRespostaMs = tempoRespostaMs,

                IpOrigem = ipOrigem,
                UserAgent = context.Request.Headers.UserAgent.ToString(),

                MensagemErro = mensagemErro
            };
        }
    }
}