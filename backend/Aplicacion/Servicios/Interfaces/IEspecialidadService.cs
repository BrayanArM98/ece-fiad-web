using Aplicacion.DTOs.Especialidades;
using Aplicacion.Helpers;

namespace Aplicacion.Servicios.Interfaces
{
    public interface IEspecialidadService
    {
        Task<ResultadoAccion<EspecialidadDTO>> ObtenerPorIdAsync(int id);
        Task<ResultadoAccion<IEnumerable<EspecialidadDTO>>> ObtenerTodosAsync();
        Task<ResultadoAccion<EspecialidadDTO>> CrearAsync(CrearEspecialidadDTO dto);
        Task<ResultadoAccion<EspecialidadDTO>> ActualizarAsync(ActualizarEspecialidadDTO dto);
        Task<ResultadoAccion> EliminarAsync(int id);
        Task<bool> ExistePorNombreAsync(string nombre, int? idExcluir = null);
    }
}