using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.EliminarEspecialidad
{
    /// <summary>
    /// Command que solicita la eliminación lógica de una especialidad.
    /// </summary>
    public record EliminarEspecialidadCommand(int Id)
        : IRequest<ResultadoAccion>;
}