using Dominio.Entidades.Usuarios;

namespace Aplicacion.Abstracciones
{
    /// <summary>Token de acceso emitido al iniciar sesión.</summary>
    public record TokenGenerado(string Token, DateTime ExpiraUtc);

    /// <summary>
    /// Genera el token de acceso (JWT) con el id, el rol y los vínculos del usuario.
    /// </summary>
    public interface IGeneradorTokens
    {
        TokenGenerado Generar(Usuario usuario);
    }
}
