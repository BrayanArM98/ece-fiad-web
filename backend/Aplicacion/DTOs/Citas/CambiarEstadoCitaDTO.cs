using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    /// <summary>
    /// Nuevo estado de una cita atendida: Confirmada, Completada o NoAsistio.
    /// Para cancelar se usa PATCH /api/Citas/{id}/cancelar.
    /// </summary>
    public class CambiarEstadoCitaDTO
    {
        public EstadoCita Estado { get; set; }
    }
}
