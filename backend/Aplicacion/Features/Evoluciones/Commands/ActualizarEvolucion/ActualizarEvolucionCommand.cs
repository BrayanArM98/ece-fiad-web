using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.ActualizarEvolucion
{
    /// <summary>
    /// Command que solicita la actualización de una evolución existente.
    /// </summary>
    public record ActualizarEvolucionCommand(ActualizarEvolucionDTO Evolucion)
        : IRequest<ResultadoAccion<EvolucionDTO>>;
}