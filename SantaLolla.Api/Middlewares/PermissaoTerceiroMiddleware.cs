using SantaLolla.Api.Repositories.Interfaces;
using System.Security.Claims;

namespace SantaLolla.Api.Middlewares
{
    public class PermissaoTerceiroMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissaoTerceiroMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITerceiroRepository terceiroRepository
        )
        {
            var path =
                context.Request.Path.Value ??
                string.Empty;

            var metodo =
                context.Request.Method.ToUpper();

            // Libera Swagger sem validar permissão
            if (path.StartsWith(
                "/swagger",
                StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Libera geração de token
            if (path.StartsWith(
                "/api/Auth",
                StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Se não estiver autenticado,
            // deixa o próprio [Authorize] tratar
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var idTerceiroClaim =
                context.User.FindFirst("id_terceiro")?.Value ??
                context.User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (!long.TryParse(
                idTerceiroClaim,
                out var idTerceiro))
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        mensagem =
                            "Terceiro não identificado no token."
                    }
                );

                return;
            }

            // -----------------------------------------------------
            // VALIDA PERMISSÃO DO ENDPOINT
            // -----------------------------------------------------
            var permitido =
                await terceiroRepository
                    .TerceiroTemPermissaoAsync(
                        idTerceiro,
                        path,
                        metodo
                    );

            if (!permitido)
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        mensagem =
                            "Acesso não autorizado para este endpoint.",
                        endpoint = path,
                        metodo
                    }
                );

                return;
            }

            // -----------------------------------------------------
            // CARREGA ACESSO ÀS REDES
            // -----------------------------------------------------
            var acessoRedes =
                await terceiroRepository
                    .ObterAcessoRedesAsync(idTerceiro);

            if (acessoRedes == null)
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        mensagem =
                            "Configuração de acesso do terceiro não encontrada."
                    }
                );

                return;
            }

            // -----------------------------------------------------
            // GUARDA INFORMAÇÕES DURANTE A REQUISIÇÃO
            // -----------------------------------------------------
            context.Items["IdTerceiro"] =
                idTerceiro;

            context.Items["AcessoTodasRedes"] =
                acessoRedes.AcessoTodasRedes;

            context.Items["RedesPermitidas"] =
                acessoRedes.RedesPermitidas;

            // -----------------------------------------------------
            // SE INFORMOU REDE EXPLICITAMENTE,
            // JÁ VALIDAMOS AQUI
            // -----------------------------------------------------
            if (!acessoRedes.AcessoTodasRedes)
            {
                var redeInformada =
                    context.Request.Query["Rede"]
                        .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(redeInformada))
                {
                    var redePermitida =
                        acessoRedes.RedesPermitidas.Any(
                            r => string.Equals(
                                r,
                                redeInformada,
                                StringComparison.OrdinalIgnoreCase
                            )
                        );

                    if (!redePermitida)
                    {
                        context.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        await context.Response.WriteAsJsonAsync(
                            new
                            {
                                mensagem =
                                    "Acesso não autorizado para a rede informada.",
                                rede = redeInformada
                            }
                        );

                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}