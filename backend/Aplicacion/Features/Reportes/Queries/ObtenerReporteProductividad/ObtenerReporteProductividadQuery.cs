using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerReporteProductividad
{
    /// <summary>
    /// Desempeño de cada doctor en el periodo.
    /// </summary>
    public record ObtenerReporteProductividadQuery(FiltroReporteDTO Filtro) : IRequest<ResultadoAccion<ReporteProductividadDTO>>;
}
