using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    public class CitaDTO
    {
        public int Id { get; set; }

        // Llaves foráneas
        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }

        // Datos de las relaciones (mapeados con AutoMapper desde las entidades)
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreDoctor { get; set; } = string.Empty;

        // Datos propios de la cita
        public DateTime FechaHora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        // Estado de la cita (Pendiente, Confirmada, Cancelada, Completada, NoAsistio)
        public EstadoCita Estado { get; set; }

        // Texto del estado para mostrarlo formateado en la interfaz
        public string EstadoTexto { get; set; } = string.Empty;

        // Estado del registro (borrado lógico)
        public bool Activo { get; set; }

        // Fechas de auditoría (para mostrar en la vista de detalles)
        public DateTime FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public DateTime? FechaDeEliminacion { get; set; }
    }
}