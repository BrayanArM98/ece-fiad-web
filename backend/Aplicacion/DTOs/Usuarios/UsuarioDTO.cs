using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Usuarios
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
        public int? IdDoctor { get; set; }
        public int? IdPaciente { get; set; }
        public bool Activo { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public DateTime FechaDeCreacion { get; set; }
    }
}
