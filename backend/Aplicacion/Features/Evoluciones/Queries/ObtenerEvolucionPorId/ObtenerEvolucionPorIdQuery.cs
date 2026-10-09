using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionPorId
{
    /// <summary>
    /// Query que solicita una evolución específica por su identificador.
    /// </summary>
    public record ObtenerEvolucionPorIdQuery(int Id)
        : IRequest<ResultadoAccion<EvolucionDTO>>;
}