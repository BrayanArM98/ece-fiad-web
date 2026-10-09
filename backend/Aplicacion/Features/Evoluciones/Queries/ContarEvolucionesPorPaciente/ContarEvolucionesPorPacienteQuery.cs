using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ContarEvolucionesPorPaciente
{
    /// <summary>
    /// Query que solicita el número de evoluciones activas de un paciente (regla 29).
    /// </summary>
    public record ContarEvolucionesPorPacienteQuery(int IdPaciente)
        : IRequest<ResultadoAccion<int>>;
}