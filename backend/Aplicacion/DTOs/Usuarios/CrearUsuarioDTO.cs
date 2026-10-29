using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Usuarios
{
    public class CrearUsuarioDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }

        // Obligatorio cuando el rol es Medico
        public int? IdDoctor { get; set; }

        // Obligatorio cuando el rol es Paciente
        public int? IdPaciente { get; set; }
    }
}
