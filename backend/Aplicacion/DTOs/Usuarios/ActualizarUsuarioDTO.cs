using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Usuarios
{
    /// <summary>
    /// Datos editables de un usuario. El correo no se cambia.
    /// La contraseña solo se cambia si se envía NuevaContrasena.
    /// </summary>
    public class ActualizarUsuarioDTO
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public int? IdDoctor { get; set; }
        public int? IdPaciente { get; set; }
        public bool Activo { get; set; } = true;
        public string? NuevaContrasena { get; set; }
    }
}
