using CadastroDePessoas.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CadastroDePessoas.Infra.IoC
{
    public static class InjecaoMediatR
    {
        public static IServiceCollection AdicionarMediatR(this IServiceCollection services)
        {
            var applicationAssembly = Assembly.Load("CadastroDePessoas.Application");

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

            services.AddValidatorsFromAssembly(applicationAssembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
