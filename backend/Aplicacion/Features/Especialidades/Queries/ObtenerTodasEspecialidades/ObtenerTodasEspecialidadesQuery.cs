using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ObtenerTodasEspecialidades
{
    /// <summary>
    /// Query que solicita la lista completa de especialidades médicas.
    /// </summary>
    public record ObtenerTodasEspecialidadesQuery
        : IRequest<ResultadoAccion<IEnumerable<EspecialidadDTO>>>;
}