using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;

namespace Aplicacion.Servicios.Interfaces
{
    public interface ICitaService
    {
        // Obtiene una cita por su Id (para detalles y edición).
        Task<ResultadoAccion<CitaDTO>> ObtenerPorIdAsync(int id);

        // Obtiene todas las citas con paciente y doctor incluidos (para el listado).
        Task<ResultadoAccion<IEnumerable<CitaDTO>>> ObtenerTodasAsync();

        // Crea una nueva cita validando datos y disponibilidad horaria.
        Task<ResultadoAccion<CitaDTO>> CrearAsync(CrearCitaDTO dto);

        // Actualiza una cita existente validando datos y disponibilidad horaria.
        Task<ResultadoAccion<CitaDTO>> ActualizarAsync(ActualizarCitaDTO dto);

        // Elimina una cita (con borrado lógico).
        Task<ResultadoAccion> EliminarAsync(int id);

        // Verifica si un doctor ya tiene una cita programada en el mismo horario.
        // Se usa antes de crear o actualizar para validar disponibilidad (regla 21).
        Task<bool> ExisteCitaEnHorarioAsync(int idDoctor, DateTime fechaHora, int? idExcluir = null);
    }
}