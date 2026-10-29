using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.ActualizarUsuario
{
    public record ActualizarUsuarioCommand(ActualizarUsuarioDTO Usuario) : IRequest<ResultadoAccion<UsuarioDTO>>;
}
