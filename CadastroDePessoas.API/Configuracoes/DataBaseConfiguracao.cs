using CadastroDePessoas.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class DatabaseConfiguracao
    {
        public static IServiceCollection AdicionarDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContexto>(options =>
            {
                options.UseSqlite(connectionString);

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            return services;
        }

        public static IApplicationBuilder InicializarDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContexto>();

                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}
