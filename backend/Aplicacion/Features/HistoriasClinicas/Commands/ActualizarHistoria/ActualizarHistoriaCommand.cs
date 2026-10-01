using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.ActualizarHistoria
{
    /// <summary>
    /// Command que solicita la actualización de una historia clínica existente.
    /// </summary>
    public record ActualizarHistoriaCommand(ActualizarHistoriaDTO Historia)
        : IRequest<ResultadoAccion<HistoriaClinicaDTO>>;
}