using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    public class ActualizarCitaDTO
    {
        // El Id es necesario para identificar qué cita se va a modificar.
        public int Id { get; set; }

        // Llaves foráneas (permitimos cambiar paciente y doctor al editar).
        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }

        // Datos editables de la cita.
        public DateTime FechaHora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        // El estado se puede cambiar al editar (regla 19 del manual).
        // Permite pasar de Pendiente a Confirmada/Completada/Cancelada/NoAsistio.
        public EstadoCita Estado { get; set; }
    }
}