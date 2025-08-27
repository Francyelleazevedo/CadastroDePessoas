using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infra.Cache;
using CadastroDePessoas.Infra.Contexto;
using CadastroDePessoas.Infra.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CadastroDePessoas.Infra.IoC
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContexto>(options =>
                 options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPessoaRepositorio, PessoaRepositorio>();
            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            services.AddScoped<IServiceToken, ServiceToken>();

            if (bool.Parse(configuration["UseRedisCache"] ?? "false"))
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
                services.AddSingleton<IServiceCache, ServiceRedisCache>();
            }
            else
            {
                services.AddDistributedMemoryCache();
                services.AddSingleton<IServiceCache, ServicoCache>();
            }

            return services;
        }
    }
}