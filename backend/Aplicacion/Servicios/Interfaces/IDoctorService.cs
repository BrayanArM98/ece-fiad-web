using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;

namespace Aplicacion.Servicios.Interfaces
{
    public interface IDoctorService
    {
        // Obtiene un doctor por su Id (para detalles y edición).
        Task<ResultadoAccion<DoctorDTO>> ObtenerPorIdAsync(int id);

        // Obtiene todos los doctores con su especialidad incluida (para el listado).
        Task<ResultadoAccion<IEnumerable<DoctorDTO>>> ObtenerTodosAsync();

        // Crea un nuevo doctor validando los datos y el email único.
        Task<ResultadoAccion<DoctorDTO>> CrearAsync(CrearDoctorDTO dto);

        // Actualiza un doctor existente validando datos y email único.
        Task<ResultadoAccion<DoctorDTO>> ActualizarAsync(ActualizarDoctorDTO dto);

        // Elimina un doctor (con borrado lógico desde el repositorio genérico).
        // Antes de eliminar valida que no tenga citas asociadas.
        Task<ResultadoAccion> EliminarAsync(int id);

        // Verifica si ya existe un doctor con el mismo email.
        // Se usa antes de crear o actualizar para evitar duplicados.
        Task<bool> ExistePorEmailAsync(string email, int? idExcluir = null);
    }
}