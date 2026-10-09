using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerTodasEvoluciones
{
    /// <summary>
    /// Query que solicita la lista completa de evoluciones clínicas.
    /// </summary>
    public record ObtenerTodasEvolucionesQuery
        : IRequest<ResultadoAccion<IEnumerable<EvolucionDTO>>>;
}