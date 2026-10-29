using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Helpers;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteDiagnosticos
{
    /// <summary>
    /// Los 10 diagnósticos más frecuentes en las evoluciones del periodo.
    /// Un médico solo ve los diagnósticos de sus evoluciones.
    /// </summary>
    public class ObtenerReporteDiagnosticosHandler
        : IRequestHandler<ObtenerReporteDiagnosticosQuery, ResultadoAccion<ReporteDiagnosticosDTO>>
    {
        public const int Limite = 10;

        private readonly IConsultasReportes _consultas;
        private readonly IValidator<FiltroReporteDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerReporteDiagnosticosHandler(
            IConsultasReportes consultas,
            IValidator<FiltroReporteDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ReporteDiagnosticosDTO>> Handle(
            ObtenerReporteDiagnosticosQuery request,
            CancellationToken cancellationToken)
        {
            var validacion = await _validador.ValidateAsync(request.Filtro, cancellationToken);
            if (!validacion.IsValid)
                return ResultadoAccion<ReporteDiagnosticosDTO>.FallaValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var alcance = AlcanceReporte.Crear(request.Filtro, _usuarioActual, DateTime.Now);
            var evoluciones = await _consultas.ObtenerEvolucionesAsync(
                alcance.Periodo.Desde, Periodos.HastaExclusivo(alcance.Periodo), alcance.IdDoctor);

            return ResultadoAccion<ReporteDiagnosticosDTO>.Exito(new ReporteDiagnosticosDTO
            {
                Periodo = alcance.Periodo,
                TotalEvoluciones = evoluciones.Count,
                Diagnosticos = CalculadoraReportes.DiagnosticosFrecuentes(evoluciones, Limite)
            });
        }
    }
}
