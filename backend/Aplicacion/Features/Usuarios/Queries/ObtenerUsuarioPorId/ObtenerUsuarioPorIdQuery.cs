using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Usuarios.Queries.ObtenerUsuarioPorId
{
    public record ObtenerUsuarioPorIdQuery(int Id) : IRequest<ResultadoAccion<UsuarioDTO>>;
}
