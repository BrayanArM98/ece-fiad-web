using Aplicacion.DTOs.Usuarios;

namespace Aplicacion.DTOs.Auth
{
    /// <summary>
    /// Respuesta del inicio de sesión: el token que el cliente envía en cada petición
    /// (encabezado Authorization: Bearer ...) y los datos del usuario.
    /// </summary>
    public class SesionDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraUtc { get; set; }
        public UsuarioDTO Usuario { get; set; } = new();
    }
}
