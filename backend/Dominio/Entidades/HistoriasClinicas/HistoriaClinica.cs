using Dominio.Entidades.Bases;
using Dominio.Entidades.Pacientes;
using Dominio.Entidades.Evoluciones;

namespace Dominio.Entidades.HistoriasClinicas
{
    public class HistoriaClinica : EntidadBase
    {
        // Llave foránea
        public int IdPaciente { get; set; }

        // Propiedades propias
        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
        public string Alergias { get; set; } = string.Empty;
        public string AntecedentesFamiliares { get; set; } = string.Empty;
        public string AntecedentesPersonales { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Paciente Paciente { get; set; } = null!;
        public virtual ICollection<Evolucion> Evoluciones { get; set; } = new List<Evolucion>();
    }
}