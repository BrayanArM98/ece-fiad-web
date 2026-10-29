using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.EliminarUsuario
{
    public record EliminarUsuarioCommand(int Id) : IRequest<ResultadoAccion>;
}
