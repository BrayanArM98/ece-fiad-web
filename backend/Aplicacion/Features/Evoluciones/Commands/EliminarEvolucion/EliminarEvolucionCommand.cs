using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.EliminarEvolucion
{
    /// <summary>
    /// Command que solicita la eliminación lógica de una evolución.
    /// </summary>
    public record EliminarEvolucionCommand(int Id)
        : IRequest<ResultadoAccion>;
}