using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;

namespace Aplicacion.Servicios.Interfaces
{
    public interface IEvolucionService
    {
        // Obtiene una evolución por su Id (para detalles y edición).
        Task<ResultadoAccion<EvolucionDTO>> ObtenerPorIdAsync(int id);

        // Obtiene todas las evoluciones con sus relaciones (para el listado).
        Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> ObtenerTodasAsync();

        // Obtiene todas las evoluciones de una historia clínica específica (regla 27).
        // Usado para la vista filtrada cuando llega desde "Ver Evoluciones" del paciente.
        Task<ResultadoAccion<IEnumerable<EvolucionDTO>>> ObtenerPorHistoriaClinicaAsync(int idHistoriaClinica);

        // Cuenta cuántas evoluciones activas tiene un paciente (regla 29 del manual).
        // Usado para el reporte resumido en la pantalla principal del módulo.
        Task<ResultadoAccion<int>> ContarPorPacienteAsync(int idPaciente);

        // Crea una nueva evolución validando los datos del formulario.
        Task<ResultadoAccion<EvolucionDTO>> CrearAsync(CrearEvolucionDTO dto);

        // Actualiza una evolución existente validando los datos del formulario.
        Task<ResultadoAccion<EvolucionDTO>> ActualizarAsync(ActualizarEvolucionDTO dto);

        // Elimina una evolución (borrado lógico).
        Task<ResultadoAccion> EliminarAsync(int id);
    }
}