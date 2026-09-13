using System.ComponentModel.DataAnnotations;

namespace Dominio.Enumeraciones
{
    public enum EstadoCita
    {
        Pendiente = 1,
        Confirmada = 2,
        Cancelada = 3,
        Completada = 4,
        [Display(Name = "No asistió")]
        NoAsistio = 5
    }
}