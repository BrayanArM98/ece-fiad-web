using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.ActualizarDoctor
{
    /// <summary>
    /// Command que solicita la actualización de un doctor existente.
    /// </summary>
    public record ActualizarDoctorCommand(ActualizarDoctorDTO Doctor)
        : IRequest<ResultadoAccion<DoctorDTO>>;
}