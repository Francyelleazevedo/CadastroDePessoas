using System.Security.Claims;

namespace CadastroDePessoas.API.Middlewares
{
    /// <summary>
    /// Middleware para adicionar camadas extras de segurança
    /// </summary>
    public class SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {

            if (context.Request.Method == "OPTIONS")
            {
                await next(context);
                return;
            }

            AdicionarHeadersSeguranca(context);
            LogarTentativaAcesso(context);

            if (!ValidarOrigemRequisicao(context))
            {
                logger.LogWarning("Origem não autorizada: {Origin}", context.Request.Headers.Origin.FirstOrDefault());
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Origem não autorizada");
                return;
            }

            if (!ValidarRateLimit(context))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Muitas requisições. Tente novamente em alguns minutos.");
                return;
            }


            await next(context);
        }

        private void AdicionarHeadersSeguranca(HttpContext context)
        {
            var response = context.Response;

            response.Headers.Append("X-Content-Type-Options", "nosniff");
            response.Headers.Append("X-Frame-Options", "DENY");
            response.Headers.Append("X-XSS-Protection", "1; mode=block");
            response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "connect-src 'self' http://localhost:* https://localhost:* https://cadastrodepessoas-web.onrender.com; " +
                "img-src 'self' data:; " +
                "style-src 'self' 'unsafe-inline'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval';");

            response.Headers.Remove("Server");
            response.Headers.Remove("X-Powered-By");
            response.Headers.Remove("X-AspNet-Version");
        }

        private void LogarTentativaAcesso(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.FirstOrDefault();
            var method = context.Request.Method;
            var path = context.Request.Path;
            var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            logger.LogInformation(
                "API Access: {Method} {Path} | IP: {IP} | UserAgent: {UserAgent} | Auth: {IsAuth} | User: {UserId}",
                method, path, ip, userAgent, isAuthenticated, userId ?? "Anonymous");

            if (!isAuthenticated && !IsEndpointPublico(path))
            {
                logger.LogWarning(
                    "Tentativa de acesso no autorizado: {Method} {Path} | IP: {IP}",
                    method, path, ip);
            }
        }

        private static bool ValidarOrigemRequisicao(HttpContext context)
        {
            var origin = context.Request.Headers.Origin.FirstOrDefault();
            var host = context.Request.Host.Host;

            if (host.Contains("localhost") || host.Contains("render.com"))
                return true;

            if (string.IsNullOrEmpty(origin))
                return true;

            var origensPermitidas = new[]
            {
                "https://cadastrodepessoas-web.onrender.com",
                "http://localhost:3001",
                "https://localhost:3001",
                "http://localhost:3000",
                "https://localhost:3000"
            };

            return origensPermitidas.Contains(origin);
        }

        private bool ValidarRateLimit(HttpContext context)
        {
            return true; 
        }

        private static bool IsEndpointPublico(string path)
        {
            var endpointsPublicos = new[]
            {
                "/api/v1/auth/login",
                "/api/v2/auth/login",
                "/api/v1/auth/logout",
                "/api/v2/auth/logout",
                "/api/v1/auth/registrar",
                "/api/v1/health",
                "/api/v2/health",
                "/swagger",
                "/health"
            };

            return endpointsPublicos.Any(endpoint => path.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase));
        }
    }
}