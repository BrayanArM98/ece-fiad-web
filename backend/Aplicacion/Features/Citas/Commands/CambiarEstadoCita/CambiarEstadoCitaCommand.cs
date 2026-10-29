using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CambiarEstadoCita
{
    /// <summary>
    /// Command que registra el resultado de una cita: confirmada, completada o no asistió.
    /// </summary>
    public record CambiarEstadoCitaCommand(int Id, CambiarEstadoCitaDTO Datos)
        : IRequest<ResultadoAccion<CitaDTO>>;
}
