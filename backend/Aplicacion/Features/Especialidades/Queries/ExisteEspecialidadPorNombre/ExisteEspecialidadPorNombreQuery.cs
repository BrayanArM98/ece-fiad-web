using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ExisteEspecialidadPorNombre
{
    /// <summary>
    /// Query que verifica si ya existe una especialidad con el nombre dado.
    /// IdExcluir permite ignorar la propia especialidad al momento de editarla.
    /// </summary>
    public record ExisteEspecialidadPorNombreQuery(string Nombre, int? IdExcluir = null)
        : IRequest<bool>;
}