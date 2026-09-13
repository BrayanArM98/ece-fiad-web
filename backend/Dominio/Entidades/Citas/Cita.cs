using Dominio.Entidades.Bases;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Pacientes;
using Dominio.Enumeraciones;

namespace Dominio.Entidades.Citas
{
    public class Cita : EntidadBase
    {
        // Llaves foráneas
        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }

        // Propiedades propias
        public DateTime FechaHora { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
        public string Notas { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Paciente Paciente { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;

        // Métodos de negocio
        public void Confirmar()
        {
            Estado = EstadoCita.Confirmada;
            FechaDeModificacion = DateTime.UtcNow;
        }

        public void Cancelar()
        {
            Estado = EstadoCita.Cancelada;
            FechaDeModificacion = DateTime.UtcNow;
        }

        public void Completar()
        {
            Estado = EstadoCita.Completada;
            FechaDeModificacion = DateTime.UtcNow;
        }

        public void RegistrarNoAsistencia()
        {
            Estado = EstadoCita.NoAsistio;
            FechaDeModificacion = DateTime.UtcNow;
        }
    }
}