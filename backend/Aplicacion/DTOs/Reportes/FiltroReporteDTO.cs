using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Reportes
{
    /// <summary>
    /// Filtros comunes de los reportes. Todos son opcionales:
    /// sin fechas, el reporte cubre el mes actual completo.
    /// </summary>
    public class FiltroReporteDTO
    {
        /// <summary>Fecha inicial, inclusiva.</summary>
        public DateTime? Desde { get; set; }

        /// <summary>Fecha final, inclusiva (incluye todo ese día).</summary>
        public DateTime? Hasta { get; set; }

        public int? IdDoctor { get; set; }
        public int? IdEspecialidad { get; set; }
        public EstadoCita? Estado { get; set; }
    }
}
