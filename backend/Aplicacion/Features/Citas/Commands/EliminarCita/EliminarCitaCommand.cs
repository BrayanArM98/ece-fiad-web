using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.EliminarCita
{
    /// <summary>
    /// Command que solicita la eliminación lógica de una cita.
    /// </summary>
    public record EliminarCitaCommand(int Id)
        : IRequest<ResultadoAccion>;
}