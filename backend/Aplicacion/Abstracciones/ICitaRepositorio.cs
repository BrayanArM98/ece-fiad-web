using Dominio.Entidades.Citas;

namespace Aplicacion.Abstracciones
{
    public interface ICitaRepositorio : IRepositorioGenerico<Cita>
    {
        // Obtiene una cita junto con su paciente y doctor cargados.
        // Útil para mostrar nombres en detalles y edición.
        Task<Cita?> ObtenerConRelacionesAsync(int id);

        // Obtiene todas las citas activas con paciente y doctor incluidos.
        // Necesario para que NombrePaciente y NombreDoctor se llenen en el listado.
        Task<IEnumerable<Cita>> ObtenerTodasConRelacionesAsync();

        // Verifica si un doctor ya tiene una cita programada en el mismo horario.
        // Se usa para la regla de disponibilidad horaria (regla 21 del manual).
        // El idExcluir sirve al editar para no chocar con la propia cita.
        Task<bool> ExisteCitaEnHorarioAsync(int idDoctor, DateTime fechaHora, int? idExcluir = null);
    }
}