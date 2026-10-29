using Dominio.Enumeraciones;

namespace Aplicacion.Abstracciones
{
    /// <summary>
    /// Datos del usuario que hace la petición, tomados del token.
    /// Los handlers lo usan para aplicar reglas por rol en el servidor
    /// (por ejemplo, que un paciente solo vea sus propias citas).
    /// </summary>
    public interface IUsuarioActual
    {
        bool EstaAutenticado { get; }
        int? IdUsuario { get; }
        RolUsuario? Rol { get; }
        int? IdDoctor { get; }
        int? IdPaciente { get; }

        bool EsMedico => Rol == RolUsuario.Medico;
        bool EsPaciente => Rol == RolUsuario.Paciente;
    }
}
