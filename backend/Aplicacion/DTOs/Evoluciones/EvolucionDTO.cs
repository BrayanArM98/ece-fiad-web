namespace Aplicacion.DTOs.Evoluciones
{
    public class EvolucionDTO
    {
        public int Id { get; set; }

        // Llaves foráneas
        public int IdHistoriaClinica { get; set; }
        public int IdDoctor { get; set; }

        // Datos de las relaciones (mapeados con AutoMapper)
        // El nombre del paciente viaja desde Evolucion.HistoriaClinica.Paciente.NombreCompleto
        public string NombrePaciente { get; set; } = string.Empty;

        // El nombre del doctor viaja desde Evolucion.Doctor.NombreCompleto
        public string NombreDoctor { get; set; } = string.Empty;

        // La especialidad viaja desde Evolucion.Doctor.NombreEspecialidad
        public string NombreEspecialidad { get; set; } = string.Empty;

        // Datos propios de la evolución
        public DateTime Fecha { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        // Estado de la evolución (heredado de EntidadBase)
        public bool Activo { get; set; }

        // Campos de auditoría (heredados de EntidadBase) — necesarios para la vista de detalles
        public DateTime FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public DateTime? FechaDeEliminacion { get; set; }
    }
}