using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientesSinHistoria
{
    /// <summary>
    /// Query que solicita los pacientes que aún no tienen historia clínica.
    /// Se usa para alimentar el selector al crear una nueva historia clínica.
    /// </summary>
    public record ObtenerPacientesSinHistoriaQuery
        : IRequest<ResultadoAccion<IEnumerable<PacienteDTO>>>;
}