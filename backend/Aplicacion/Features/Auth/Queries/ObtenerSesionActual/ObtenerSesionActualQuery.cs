using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Auth.Queries.ObtenerSesionActual
{
    /// <summary>
    /// Query que devuelve los datos del usuario dueño del token.
    /// La usan los clientes al abrir la app para saber si la sesión sigue vigente.
    /// </summary>
    public record ObtenerSesionActualQuery : IRequest<ResultadoAccion<UsuarioDTO>>;
}
