using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerCitaPorId
{
    /// <summary>
    /// Query que solicita una cita específica por su identificador.
    /// </summary>
    public record ObtenerCitaPorIdQuery(int Id)
        : IRequest<ResultadoAccion<CitaDTO>>;
}