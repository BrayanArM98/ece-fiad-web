using Dominio.Entidades.Bases;
using Dominio.Entidades.Citas;
using Dominio.Entidades.Especialidades;

namespace Dominio.Entidades.Doctores
{
    public class Doctor : EntidadBase
    {
        // Llave foránea
        public int IdEspecialidad { get; set; }

        // Propiedades propias
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HorarioAtencion { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Especialidad Especialidad { get; set; } = null!;
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();

        // Propiedad calculada
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}