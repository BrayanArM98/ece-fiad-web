using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Aplicacion.Features.Reportes.Comun;
using Aplicacion.Helpers;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteCitas
{
    /// <summary>
    /// Reporte de citas: por estado, en el tiempo, por doctor y por especialidad.
    /// Un médico solo ve sus citas.
    /// </summary>
    public class ObtenerReporteCitasHandler
        : IRequestHandler<ObtenerReporteCitasQuery, ResultadoAccion<ReporteCitasDTO>>
    {
        private static readonly string[] OrdenEstados =
            Enum.GetValues<EstadoCita>().Select(CalculadoraReportes.TextoEstado).ToArray();

        private readonly IConsultasReportes _consultas;
        private readonly IValidator<FiltroReporteDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerReporteCitasHandler(
            IConsultasReportes consultas,
            IValidator<FiltroReporteDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _validador = validador;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<ReporteCitasDTO>> Handle(
            ObtenerReporteCitasQuery request,
            CancellationToken cancellationToken)
        {
            var validacion = await _validador.ValidateAsync(request.Filtro, cancellationToken);
            if (!validacion.IsValid)
                return ResultadoAccion<ReporteCitasDTO>.FallaValidacion(validacion.Errors.Select(e => e.ErrorMessage));

            var alcance = AlcanceReporte.Crear(request.Filtro, _usuarioActual, DateTime.Now);
            var citas = await _consultas.ObtenerCitasAsync(
                alcance.Periodo.Desde,
                Periodos.HastaExclusivo(alcance.Periodo),
                alcance.IdDoctor,
                alcance.IdEspecialidad,
                request.Filtro.Estado);

            var (agrupacion, serie) = CalculadoraReportes.Serie(citas, alcance.Periodo);

            return ResultadoAccion<ReporteCitasDTO>.Exito(new ReporteCitasDTO
            {
                Periodo = alcance.Periodo,
                Total = citas.Count,
                Agrupacion = agrupacion,
                PorEstado = CalculadoraReportes.Contar(citas.Select(c => CalculadoraReportes.TextoEstado(c.Estado)), OrdenEstados),
                Serie = serie,
                PorDoctor = CalculadoraReportes.Contar(citas.Select(c => c.NombreDoctor)),
                PorEspecialidad = CalculadoraReportes.Contar(citas.Select(c => c.NombreEspecialidad))
            });
        }
    }
}
