using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteCitas
{
    /// <summary>
    /// Citas por estado, en el tiempo, por doctor y por especialidad.
    /// </summary>
    public record ObtenerReporteCitasQuery(FiltroReporteDTO Filtro) : IRequest<ResultadoAccion<ReporteCitasDTO>>;
}
