using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.CrearEvolucion
{
    /// <summary>
    /// Command que solicita el registro de una nueva evolución clínica.
    /// </summary>
    public record CrearEvolucionCommand(CrearEvolucionDTO Evolucion)
        : IRequest<ResultadoAccion<EvolucionDTO>>;
}