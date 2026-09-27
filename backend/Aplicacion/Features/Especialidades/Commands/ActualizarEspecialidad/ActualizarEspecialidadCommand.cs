using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.ActualizarEspecialidad
{
    /// <summary>
    /// Command que solicita la actualización de una especialidad existente.
    /// </summary>
    public record ActualizarEspecialidadCommand(ActualizarEspecialidadDTO Especialidad)
        : IRequest<ResultadoAccion<EspecialidadDTO>>;
}