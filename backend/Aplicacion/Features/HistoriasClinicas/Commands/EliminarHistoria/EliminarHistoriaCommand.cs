using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.EliminarHistoria
{
    /// <summary>
    /// Command que solicita la eliminación lógica de una historia clínica.
    /// </summary>
    public record EliminarHistoriaCommand(int Id)
        : IRequest<ResultadoAccion>;
}