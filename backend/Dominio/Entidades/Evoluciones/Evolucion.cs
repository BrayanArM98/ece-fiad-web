using Dominio.Entidades.Bases;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.HistoriasClinicas;

namespace Dominio.Entidades.Evoluciones
{
    public class Evolucion : EntidadBase
    {
        // Llaves foráneas
        public int IdHistoriaClinica { get; set; }
        public int IdDoctor { get; set; }

        // Propiedades propias
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;
        public string Notas { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual HistoriaClinica HistoriaClinica { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
    }
}