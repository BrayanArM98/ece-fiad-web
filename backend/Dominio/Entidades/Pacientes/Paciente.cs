using Dominio.Entidades.Bases;
using Dominio.Entidades.Citas;
using Dominio.Entidades.HistoriasClinicas;
using Dominio.Enumeraciones;

namespace Dominio.Entidades.Pacientes
{
    public class Paciente : EntidadBase
    {
        // Propiedades personales
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public TipoDeDocumento TipoDocumento { get; set; }
        public string? Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public Genero Genero { get; set; }
        public GrupoSanguineo GrupoSanguineo { get; set; }
        public string? Direccion { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual HistoriaClinica HistoriaClinica { get; set; } = null!;
        public virtual ICollection<Cita> Citas { get; set; } = new List<Cita>();

        // Propiedad calculada
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}