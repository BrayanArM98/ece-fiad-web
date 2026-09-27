using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.CrearEspecialidad
{
    /// <summary>
    /// Command que solicita el registro de una nueva especialidad médica.
    /// </summary>
    public record CrearEspecialidadCommand(CrearEspecialidadDTO Especialidad)
        : IRequest<ResultadoAccion<EspecialidadDTO>>;
}