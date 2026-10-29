using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerTodasCitas
{
    /// <summary>
    /// Query que solicita las citas médicas, opcionalmente filtradas
    /// por doctor, paciente, rango de fechas y estado.
    /// Sin filtros devuelve todas las citas activas.
    /// </summary>
    public record ObtenerTodasCitasQuery(FiltroCitasDTO? Filtro = null)
        : IRequest<ResultadoAccion<IEnumerable<CitaDTO>>>;
}
