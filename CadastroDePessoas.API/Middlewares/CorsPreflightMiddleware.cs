namespace CadastroDePessoas.API.Middlewares
{
    public class CorsPreflightMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CorsPreflightMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var corsOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            var origin = context.Request.Headers.Origin.ToString();
            
            bool isAllowedOrigin = (corsOrigins != null && corsOrigins.Contains(origin)) || 
                                  context.Request.Host.Value.Contains("localhost");

            if (context.Request.Method == "OPTIONS")
            {
                if (isAllowedOrigin)
                {
                    context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                    context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept");
                    context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                    context.Response.Headers.Append("Access-Control-Max-Age", "3600");
                }
                
                context.Response.StatusCode = 200;
                return;
            }
            else
            {
                if (isAllowedOrigin && !string.IsNullOrEmpty(origin))
                {
                    context.Response.OnStarting(() =>
                    {
                        context.Response.Headers.Append("Access-Control-Allow-Origin", origin);
                        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
                        return Task.CompletedTask;
                    });
                }
            }

            await _next(context);
        }
    }
}