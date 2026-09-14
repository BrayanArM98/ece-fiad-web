using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.CrearPaciente
{
    /// <summary>
    /// Command que solicita el registro de un nuevo paciente.
    /// A diferencia de las Queries, este modifica el estado del sistema.
    /// </summary>
    public record CrearPacienteCommand(CrearPacienteDTO Paciente)
        : IRequest<ResultadoAccion<PacienteDTO>>;
}