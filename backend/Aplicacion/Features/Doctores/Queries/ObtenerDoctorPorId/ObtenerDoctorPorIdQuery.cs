using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ObtenerDoctorPorId
{
    /// <summary>
    /// Query que solicita un doctor específico por su identificador.
    /// </summary>
    public record ObtenerDoctorPorIdQuery(int Id)
        : IRequest<ResultadoAccion<DoctorDTO>>;
}