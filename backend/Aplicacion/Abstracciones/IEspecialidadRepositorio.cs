using Dominio.Entidades.Especialidades;

namespace Aplicacion.Abstracciones
{
    public interface IEspecialidadRepositorio : IRepositorioGenerico<Especialidad>
    {
        // Obtiene una especialidad junto con la lista de doctores asociados.
        // Se usa para verificar si tiene doctores antes de permitir eliminarla.
        Task<Especialidad?> ObtenerConDoctoresAsync(int id);

        // Obtiene una especialidad por su nombre (para validar que no se duplique).
        Task<Especialidad?> ObtenerPorNombreAsync(string nombre);

        // Obtiene todas las especialidades activas con sus doctores incluidos.
        // Necesario para que CantidadDoctores se calcule en el listado.
        Task<IEnumerable<Especialidad>> ObtenerTodosConDoctoresAsync();
    }
}