using Aplicacion.DTOs.Reportes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Reportes.Queries.ExportarReporte
{
    /// <summary>
    /// Genera el archivo PDF o Excel de un reporte con los mismos filtros que la consulta.
    /// </summary>
    public record ExportarReporteQuery(TipoReporte Tipo, FormatoExportacion Formato, FiltroReporteDTO Filtro)
        : IRequest<ResultadoAccion<ArchivoReporteDTO>>;
}
