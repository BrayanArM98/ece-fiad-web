using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.CancelarCita
{
    /// <summary>
    /// Command que solicita la cancelación de una cita.
    /// Antes esta lógica vivía en el controlador; ahora es una operación de negocio propia.
    /// </summary>
    public record CancelarCitaCommand(int Id)
        : IRequest<ResultadoAccion<CitaDTO>>;
}