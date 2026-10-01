using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.CrearHistoria
{
    /// <summary>
    /// Command que solicita el registro de una nueva historia clínica.
    /// </summary>
    public record CrearHistoriaCommand(CrearHistoriaDTO Historia)
        : IRequest<ResultadoAccion<HistoriaClinicaDTO>>;
}