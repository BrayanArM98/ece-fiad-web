using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientesActivos
{
    /// <summary>
    /// Query que solicita únicamente los pacientes activos.
    /// Se usa para alimentar selectores del frontend (citas, evoluciones, etc.).
    /// </summary>
    public record ObtenerPacientesActivosQuery
        : IRequest<ResultadoAccion<IEnumerable<PacienteDTO>>>;
}