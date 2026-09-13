using Aplicacion.Servicios.Implementaciones;
using Aplicacion.Servicios.Interfaces;
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

            // Servicios de aplicación
            services.AddScoped<IPacienteService, PacienteService>();
            services.AddScoped<IEspecialidadService, EspecialidadService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ICitaService, CitaService>();
            services.AddScoped<IHistoriaClinicaService, HistoriaClinicaService>();
            services.AddScoped<IEvolucionService, EvolucionService>();

            return services;
        }
    }
}