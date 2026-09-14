using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerTodosPacientes
{
    /// <summary>
    /// Query que solicita la lista completa de pacientes.
    /// Al ser una operación de lectura, no modifica el estado del sistema.
    /// </summary>
    public record ObtenerTodosPacientesQuery : IRequest<ResultadoAccion<IEnumerable<PacienteDTO>>>;
}