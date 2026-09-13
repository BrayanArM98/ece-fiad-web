using Dominio.Entidades.Doctores;

namespace Aplicacion.Abstracciones
{
    public interface IDoctorRepositorio : IRepositorioGenerico<Doctor>
    {
        // Obtiene un doctor junto con su especialidad asociada.
        // Útil para mostrar el nombre de la especialidad en la vista de detalles.
        Task<Doctor?> ObtenerConEspecialidadAsync(int id);

        // Obtiene todos los doctores activos con su especialidad incluida.
        // Necesario para que NombreEspecialidad se llene en el listado.
        Task<IEnumerable<Doctor>> ObtenerTodosConEspecialidadAsync();

        // Obtiene un doctor con sus citas asociadas.
        // Se usa para validar antes de eliminar (regla 20 del manual).
        Task<Doctor?> ObtenerConCitasAsync(int id);

        // Busca un doctor por email exacto (para validar duplicados).
        // El idExcluir sirve al editar (para no chocar con el propio registro).
        Task<Doctor?> ObtenerPorEmailAsync(string email, int? idExcluir = null);
    }
}