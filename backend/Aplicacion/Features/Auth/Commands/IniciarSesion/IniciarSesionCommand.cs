using Aplicacion.DTOs.Auth;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Auth.Commands.IniciarSesion
{
    /// <summary>
    /// Command que valida las credenciales y emite el token de acceso.
    /// Es un command (no una query) porque registra la fecha del último acceso.
    /// </summary>
    public record IniciarSesionCommand(IniciarSesionDTO Credenciales)
        : IRequest<ResultadoAccion<SesionDTO>>;
}
