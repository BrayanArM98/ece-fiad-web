using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerEvolucionesPorHistoria
{
    /// <summary>
    /// Query que solicita las evoluciones de una historia clínica (regla 27).
    /// </summary>
    public record ObtenerEvolucionesPorHistoriaQuery(int IdHistoriaClinica)
        : IRequest<ResultadoAccion<IEnumerable<EvolucionDTO>>>;
}