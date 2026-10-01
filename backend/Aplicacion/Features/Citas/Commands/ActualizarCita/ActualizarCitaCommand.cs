using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.ActualizarCita
{
    /// <summary>
    /// Command que solicita la actualización de una cita existente.
    /// </summary>
    public record ActualizarCitaCommand(ActualizarCitaDTO Cita)
        : IRequest<ResultadoAccion<CitaDTO>>;
}