namespace Aplicacion.DTOs.HistoriasClinicas
{
    public class HistoriaClinicaDTO
    {
        public int Id { get; set; }

        // Llave foránea
        public int IdPaciente { get; set; }

        // Dato de la relación (mapeado con AutoMapper desde Paciente.NombreCompleto)
        public string NombrePaciente { get; set; } = string.Empty;

        // Datos propios de la historia clínica
        public DateTime FechaApertura { get; set; }
        public string Alergias { get; set; } = string.Empty;
        public string AntecedentesFamiliares { get; set; } = string.Empty;
        public string AntecedentesPersonales { get; set; } = string.Empty;

        // Estado de la historia (heredado de EntidadBase)
        public bool Activo { get; set; }

        // Campos de auditoría (heredados de EntidadBase) — necesarios para la vista de detalles
        public DateTime FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public DateTime? FechaDeEliminacion { get; set; }
    }
}