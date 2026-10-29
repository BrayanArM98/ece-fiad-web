using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    /// <summary>
    /// Filtros opcionales para consultar citas (GET /api/Citas?idDoctor=1&amp;desde=2026-10-01...).
    /// Los filtros que no se envían no se aplican.
    /// </summary>
    public class FiltroCitasDTO
    {
        public int? IdDoctor { get; set; }
        public int? IdPaciente { get; set; }

        /// <summary>Fecha y hora inicial, inclusiva.</summary>
        public DateTime? Desde { get; set; }

        /// <summary>Fecha final, inclusiva: incluye todas las citas de ese día.</summary>
        public DateTime? Hasta { get; set; }

        public EstadoCita? Estado { get; set; }
    }
}
