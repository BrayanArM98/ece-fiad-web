using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ObtenerTodosDoctores
{
    /// <summary>
    /// Query que solicita la lista completa de doctores.
    /// </summary>
    public record ObtenerTodosDoctoresQuery
        : IRequest<ResultadoAccion<IEnumerable<DoctorDTO>>>;
}