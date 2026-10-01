using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerTodasCitas
{
    /// <summary>
    /// Query que solicita la lista completa de citas médicas.
    /// </summary>
    public record ObtenerTodasCitasQuery
        : IRequest<ResultadoAccion<IEnumerable<CitaDTO>>>;
}