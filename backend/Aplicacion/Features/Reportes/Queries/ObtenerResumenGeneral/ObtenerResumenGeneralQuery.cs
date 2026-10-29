using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ObtenerResumenGeneral
{
    /// <summary>
    /// Indicadores generales del periodo comparados con el periodo anterior.
    /// </summary>
    public record ObtenerResumenGeneralQuery(FiltroReporteDTO Filtro) : IRequest<ResultadoAccion<ResumenGeneralDTO>>;
}
