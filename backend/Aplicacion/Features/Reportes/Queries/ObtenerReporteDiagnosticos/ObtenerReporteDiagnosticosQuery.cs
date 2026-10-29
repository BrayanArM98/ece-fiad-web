using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteDiagnosticos
{
    /// <summary>
    /// Diagnósticos más frecuentes en las evoluciones del periodo.
    /// </summary>
    public record ObtenerReporteDiagnosticosQuery(FiltroReporteDTO Filtro) : IRequest<ResultadoAccion<ReporteDiagnosticosDTO>>;
}
