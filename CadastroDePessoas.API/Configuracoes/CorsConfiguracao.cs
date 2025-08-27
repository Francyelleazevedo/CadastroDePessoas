namespace CadastroDePessoas.API.Configuracoes
{
    public static class CorsConfiguracao
    {
        public static IServiceCollection AdicionarCorsConfiguracao(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();


            if (corsOrigins == null || corsOrigins.Length == 0)
            {
                corsOrigins = [
                    "http://localhost:3000",
                    "https://localhost:3000",
                    "http://localhost:3001",
                    "https://localhost:3001",
                    "https://cadastro-de-pessoas-web.onrender.com"
                ];
            }

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(corsOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .WithExposedHeaders("Content-Disposition")
                           .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });

                options.AddPolicy("Development", builder =>
                {
                    builder.SetIsOriginAllowed(origin =>
                        origin.StartsWith("http://localhost") ||
                        origin.StartsWith("https://localhost"))
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });

                // Política específica para produção com configurações mais restritivas
                options.AddPolicy("Production", builder =>
                {
                    builder.WithOrigins("https://cadastro-de-pessoas-web.onrender.com")
                           .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                           .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                           .AllowCredentials()
                           .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            });

            return services;
        }

        public static IApplicationBuilder UsarCorsConfiguracao(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("Development");
            }
            else
            {
                app.UseCors();
            }

            return app;
        }
    }
}
