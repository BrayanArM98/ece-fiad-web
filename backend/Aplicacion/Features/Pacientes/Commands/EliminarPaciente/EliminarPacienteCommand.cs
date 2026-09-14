using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.EliminarPaciente
{
    /// <summary>
    /// Command que solicita la eliminación lógica de un paciente.
    /// </summary>
    public record EliminarPacienteCommand(int Id)
        : IRequest<ResultadoAccion>;
}