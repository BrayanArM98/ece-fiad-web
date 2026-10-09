using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aplicacion.Inyecciones
{
    public static class InyeccionDependencias
    {
        public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
        {
            // AutoMapper: escanea el ensamblado en busca de clases que hereden de Profile
            services.AddAutoMapper(typeof(InyeccionDependencias).Assembly);

            // FluentValidation: registra todos los validadores del ensamblado actual
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // MediatR: registra todos los Commands y Queries del ensamblado.
            // Toda la lógica de aplicación opera ahora con el patrón CQRS.
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(InyeccionDependencias).Assembly));

            return services;
        }
    }
}