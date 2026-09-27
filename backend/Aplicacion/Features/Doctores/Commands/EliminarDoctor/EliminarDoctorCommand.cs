using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.EliminarDoctor
{
    /// <summary>
    /// Command que solicita la eliminación lógica de un doctor.
    /// </summary>
    public record EliminarDoctorCommand(int Id)
        : IRequest<ResultadoAccion>;
}