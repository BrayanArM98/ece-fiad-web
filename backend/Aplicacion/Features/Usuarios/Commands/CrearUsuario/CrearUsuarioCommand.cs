using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.CrearUsuario
{
    public record CrearUsuarioCommand(CrearUsuarioDTO Usuario) : IRequest<ResultadoAccion<UsuarioDTO>>;
}
