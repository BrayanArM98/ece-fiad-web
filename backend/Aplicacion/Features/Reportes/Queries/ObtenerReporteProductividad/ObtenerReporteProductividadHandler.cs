using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Helpers;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteProductividad
{
    /// <summary>
    /// Productividad por doctor: citas por resultado, evoluciones registradas y tasa de asistencia.
    /// Incluye a los doctores sin citas en el periodo. Un médico solo se ve a sí mismo.
    /// </summary>
    public class ObtenerReporteProductividadHandler
        : IRequestHandler<ObtenerReporteProductividadQuery, ResultadoAccion<ReporteProductividadDTO>>
    {
        private readonly IConsultasReportes _consultas;
        private readonly IValidator<FiltroReporteDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerReporteProductividadHandler(
            IConsultasReportes consultas,
            IValidator<FiltroReporteDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ReporteProductividadDTO>> Handle(
            ObtenerReporteProductividadQuery request,
            CancellationToken cancellationToken)
        {
            var validacion = await _validador.ValidateAsync(request.Filtro, cancellationToken);
            if (!validacion.IsValid)
                return ResultadoAccion<ReporteProductividadDTO>.FallaValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var alcance = AlcanceReporte.Crear(request.Filtro, _usuarioActual, DateTime.Now);
            var hasta = Periodos.HastaExclusivo(alcance.Periodo);

            var doctores = await _consultas.ObtenerDoctoresAsync(alcance.IdDoctor, alcance.IdEspecialidad);
            var citas = await _consultas.ObtenerCitasAsync(alcance.Periodo.Desde, hasta, alcance.IdDoctor, alcance.IdEspecialidad, null);
            var evoluciones = await _consultas.ObtenerEvolucionesAsync(alcance.Periodo.Desde, hasta, alcance.IdDoctor);

            var citasPorDoctor = citas.ToLookup(c => c.IdDoctor);
            var evolucionesPorDoctor = evoluciones.ToLookup(e => e.IdDoctor);

            var filas = doctores.Select(doctor =>
            {
                var suyas = citasPorDoctor[doctor.Id].ToList();
                var completadas = suyas.Count(c => c.Estado == EstadoCita.Completada);
                var noAsistio = suyas.Count(c => c.Estado == EstadoCita.NoAsistio);

                return new ProductividadDoctorDTO
                {
                    IdDoctor = doctor.Id,
                    NombreDoctor = doctor.NombreCompleto,
                    Especialidad = doctor.Especialidad,
                    CitasTotales = suyas.Count,
                    Completadas = completadas,
                    NoAsistio = noAsistio,
                    Canceladas = suyas.Count(c => c.Estado == EstadoCita.Cancelada),
                    PorAtender = suyas.Count(c => c.Estado is EstadoCita.Pendiente or EstadoCita.Confirmada),
                    Evoluciones = evolucionesPorDoctor[doctor.Id].Count(),
                    TasaAsistencia = CalculadoraReportes.TasaAsistencia(completadas, noAsistio)
                };
            })
            .OrderByDescending(d => d.Completadas)
            .ThenByDescending(d => d.CitasTotales)
            .ThenBy(d => d.NombreDoctor)
            .ToList();

            return ResultadoAccion<ReporteProductividadDTO>.Exito(new ReporteProductividadDTO
            {
                Periodo = alcance.Periodo,
                Doctores = filas
            });
        }
    }
}
