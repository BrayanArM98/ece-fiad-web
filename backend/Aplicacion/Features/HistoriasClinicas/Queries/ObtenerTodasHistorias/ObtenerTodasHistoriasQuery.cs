using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ObtenerTodasHistorias
{
    /// <summary>
    /// Query que solicita la lista completa de historias clínicas.
    /// </summary>
    public record ObtenerTodasHistoriasQuery
        : IRequest<ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>>;
}