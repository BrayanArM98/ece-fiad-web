using Aplicacion.DTOs.HistoriasClinicas;
using Aplicacion.Helpers;

namespace Aplicacion.Servicios.Interfaces
{
    public interface IHistoriaClinicaService
    {
        // Obtiene una historia clínica por su Id (para detalles y edición).
        Task<ResultadoAccion<HistoriaClinicaDTO>> ObtenerPorIdAsync(int id);

        // Obtiene todas las historias clínicas con paciente incluido (para el listado).
        Task<ResultadoAccion<IEnumerable<HistoriaClinicaDTO>>> ObtenerTodasAsync();

        // Crea una nueva historia clínica validando datos y unicidad por paciente.
        Task<ResultadoAccion<HistoriaClinicaDTO>> CrearAsync(CrearHistoriaDTO dto);

        // Actualiza una historia clínica existente validando datos y unicidad por paciente.
        Task<ResultadoAccion<HistoriaClinicaDTO>> ActualizarAsync(ActualizarHistoriaDTO dto);

        // Elimina una historia clínica (con borrado lógico).
        Task<ResultadoAccion> EliminarAsync(int id);

        // Verifica si un paciente ya tiene una historia clínica activa.
        // Se usa antes de crear o actualizar para validar la unicidad (regla 22).
        Task<bool> ExisteHistoriaParaPacienteAsync(int idPaciente, int? idExcluir = null);
    }
}