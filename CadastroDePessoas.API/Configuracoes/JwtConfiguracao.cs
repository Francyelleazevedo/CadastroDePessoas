using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class JwtConfiguracao
    {
        public static IServiceCollection AdicionarJwtAutenticacao(this IServiceCollection services, IConfiguration configuration)
        {
            var chave = Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Emissor"],
                    ValidAudience = configuration["Jwt:Audiencia"],
                    IssuerSigningKey = new SymmetricSecurityKey(chave)
                };
            });

            return services;
        }
    }
}

