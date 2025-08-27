using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class SwaggerConfiguracao
    {
        public static IServiceCollection AdicionarSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, description) =>
                {
                    if (docName == "v1")
                    {
                        return description.RelativePath.Contains("/v1/") ||
                               (!description.RelativePath.Contains("/v2/") && !description.RelativePath.Contains("/v1/"));
                    }
                    else if (docName == "v2")
                    {
                        return description.RelativePath.Contains("/v2/");
                    }
                    return false;
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cadastro de Pessoas API",
                    Version = "v1.0",
                    Description = @"**Versão 1 da API** para cadastro de pessoas com operações CRUD e autenticação JWT.

                    Funcionalidades:
                    - Cadastro de pessoas (endereço opcional)
                    - Autenticação JWT",
                    
                    Contact = new OpenApiContact
                    {
                        Name = "Stefanini - V1"
                    }
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Cadastro de Pessoas API",
                    Version = "v2.0",
                    Description = @"**Versão 2 da API** para cadastro de pessoas com operações CRUD e autenticação JWT.
                    
                    Funcionalidades:
                    - Cadastro de pessoas (endereço obrigatório)
                    - Autenticação JWT",

                    Contact = new OpenApiContact
                    {
                        Name = "Stefanini - V2"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header usando o esquema Bearer. 
                                   
                    **Passo a Passo:**
                    1. Faça login em `/auth/login`
                    2. Copie o token retornado
                    3. Cole aqui no formato: `Bearer {seu-token}`
                    
                    **Exemplo:** `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static IApplicationBuilder UsarSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2.0");

                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}
