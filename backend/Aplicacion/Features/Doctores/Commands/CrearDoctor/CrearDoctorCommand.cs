using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.CrearDoctor
{
    /// <summary>
    /// Command que solicita el registro de un nuevo doctor.
    /// </summary>
    public record CrearDoctorCommand(CrearDoctorDTO Doctor)
        : IRequest<ResultadoAccion<DoctorDTO>>;
}