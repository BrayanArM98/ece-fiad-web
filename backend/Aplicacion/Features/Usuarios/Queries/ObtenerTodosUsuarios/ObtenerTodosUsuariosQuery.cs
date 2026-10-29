using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Queries.ObtenerTodosUsuarios
{
    /// <summary>
    /// Query que devuelve todos los usuarios, activos e inactivos.
    /// </summary>
    public record ObtenerTodosUsuariosQuery : IRequest<ResultadoAccion<IEnumerable<UsuarioDTO>>>;
}
