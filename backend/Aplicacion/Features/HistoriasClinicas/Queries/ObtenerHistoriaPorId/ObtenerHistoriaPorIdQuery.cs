using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ObtenerHistoriaPorId
{
    /// <summary>
    /// Query que solicita una historia clínica específica por su identificador.
    /// </summary>
    public record ObtenerHistoriaPorIdQuery(int Id)
        : IRequest<ResultadoAccion<HistoriaClinicaDTO>>;
}