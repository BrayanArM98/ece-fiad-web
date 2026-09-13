using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    public class CrearCitaDTO
    {
        // Llaves foráneas: el usuario selecciona desde dos dropdowns.
        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }

        // Fecha y hora de la cita (capturada con InputDate + InputTime).
        public DateTime FechaHora { get; set; }

        // Motivo de la consulta (obligatorio).
        public string Motivo { get; set; } = string.Empty;

        // Notas adicionales (opcional).
        public string Notas { get; set; } = string.Empty;

        // Estado inicial de la cita.
        // Por defecto Pendiente, pero el usuario puede cambiarlo en el formulario.
        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
    }
}