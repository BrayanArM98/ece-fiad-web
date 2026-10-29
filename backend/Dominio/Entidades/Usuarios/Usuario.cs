using Dominio.Entidades.Bases;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Pacientes;
using Dominio.Enumeraciones;

namespace Dominio.Entidades.Usuarios
{
    /// <summary>
    /// Cuenta de acceso al sistema.
    /// Un médico se vincula con su registro de Doctor y un paciente con su registro de Paciente,
    /// para que la API sepa qué agenda o qué expediente le corresponde.
    /// </summary>
    public class Usuario : EntidadBase
    {
        public string Email { get; set; } = string.Empty;

        // Nunca se guarda la contraseña, solo su hash (PBKDF2 con sal)
        public string ContrasenaHash { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }

        // Vínculos opcionales según el rol
        public int? IdDoctor { get; set; }
        public int? IdPaciente { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        // Propiedades de navegación
        public virtual Doctor? Doctor { get; set; }
        public virtual Paciente? Paciente { get; set; }
    }
}
