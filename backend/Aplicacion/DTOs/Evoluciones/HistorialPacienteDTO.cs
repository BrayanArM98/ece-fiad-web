using Aplicacion.DTOs.HistoriasClinicas;

namespace Aplicacion.DTOs.Evoluciones
{
    /// <summary>
    /// Representa el historial clínico completo de un paciente:
    /// su historia clínica junto con todas sus evoluciones.
    /// </summary>
    public class HistorialPacienteDTO
    {
        public bool TieneHistoria { get; set; }
        public HistoriaClinicaDTO? Historia { get; set; }
        public List<EvolucionDTO> Evoluciones { get; set; } = new();
        public int TotalEvoluciones { get; set; }
    }
}