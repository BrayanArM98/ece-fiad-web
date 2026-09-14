using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.ActualizarPaciente
{
    /// <summary>
    /// Command que solicita la actualización de un paciente existente.
    /// </summary>
    public record ActualizarPacienteCommand(ActualizarPacienteDTO Paciente)
        : IRequest<ResultadoAccion<PacienteDTO>>;
}