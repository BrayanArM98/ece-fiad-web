using System.Security.Claims;
using Aplicacion.Abstracciones;
using Dominio.Enumeraciones;

namespace Presentacion.Seguridad;

/// <summary>
/// Lee los datos del usuario de la petición a partir de los claims del token.
/// </summary>
public sealed class UsuarioActual : IUsuarioActual
{
    private readonly IHttpContextAccessor _accesor;

    public UsuarioActual(IHttpContextAccessor accesor)
    {
        _accesor = accesor;
    }

    private ClaimsPrincipal? Principal => _accesor.HttpContext?.User;

    public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true;

    public int? IdUsuario => LeerEntero(ClaimsJwt.Sujeto);

    public RolUsuario? Rol =>
        Enum.TryParse<RolUsuario>(Principal?.FindFirst(ClaimsJwt.Rol)?.Value, out var rol) ? rol : null;

    public int? IdDoctor => LeerEntero(ClaimsJwt.IdDoctor);

    public int? IdPaciente => LeerEntero(ClaimsJwt.IdPaciente);

    private int? LeerEntero(string tipo) =>
        int.TryParse(Principal?.FindFirst(tipo)?.Value, out var valor) ? valor : null;
}
