using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ObtenerEspecialidadPorId
{
    /// <summary>
    /// Query que solicita una especialidad específica por su identificador.
    /// </summary>
    public record ObtenerEspecialidadPorIdQuery(int Id)
        : IRequest<ResultadoAccion<EspecialidadDTO>>;
}