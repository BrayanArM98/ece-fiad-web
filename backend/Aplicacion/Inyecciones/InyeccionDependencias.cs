using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aplicacion.Inyecciones
{
    public static class InyeccionDependencias
    {
        /// <summary>
        /// Registra los servicios de la capa de Aplicación.
        /// </summary>
        /// <param name="licenciaAutoMapper">
        /// Clave de licencia de AutoMapper (versión 15 en adelante). Es opcional:
        /// sin ella AutoMapper funciona igual y solo escribe una advertencia en el log.
        /// </param>
        public static IServiceCollection AgregarAplicacion(
            this IServiceCollection services,
            string? licenciaAutoMapper = null)
        {
            // AutoMapper: escanea el ensamblado en busca de clases que hereden de Profile
            services.AddAutoMapper(
                cfg => cfg.LicenseKey = licenciaAutoMapper,
                typeof(InyeccionDependencias).Assembly);

            // FluentValidation: registra todos los validadores del ensamblado actual
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // MediatR: registra todos los Commands y Queries del ensamblado.
            // Toda la lógica de aplicación opera con el patrón CQRS.
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(InyeccionDependencias).Assembly));

            return services;
        }
    }
}
