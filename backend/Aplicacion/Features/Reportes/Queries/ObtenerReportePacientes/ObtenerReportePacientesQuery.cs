using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReportePacientes
{
    /// <summary>
    /// Altas del periodo y composición de los pacientes activos.
    /// </summary>
    public record ObtenerReportePacientesQuery(FiltroReporteDTO Filtro) : IRequest<ResultadoAccion<ReportePacientesDTO>>;
}
