using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CrearCita
{
    /// <summary>
    /// Command que solicita el registro de una nueva cita médica.
    /// </summary>
    public record CrearCitaCommand(CrearCitaDTO Cita)
        : IRequest<ResultadoAccion<CitaDTO>>;
}