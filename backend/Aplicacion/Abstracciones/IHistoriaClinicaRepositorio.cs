using Dominio.Entidades.HistoriasClinicas;

namespace Aplicacion.Abstracciones
{
    public interface IHistoriaClinicaRepositorio : IRepositorioGenerico<HistoriaClinica>
    {
        // Obtiene una historia clínica junto con su paciente cargado.
        // Útil para mostrar el nombre del paciente en detalles y edición.
        Task<HistoriaClinica?> ObtenerConRelacionesAsync(int id);

        // Obtiene todas las historias clínicas activas con paciente incluido.
        // Necesario para que NombrePaciente se llene en el listado.
        Task<IEnumerable<HistoriaClinica>> ObtenerTodasConRelacionesAsync();

        // Verifica si el paciente ya tiene una historia clínica activa.
        // Se usa para implementar la regla 22 del manual: unicidad por paciente.
        // El idExcluir sirve al editar para no chocar con la propia historia.
        Task<bool> ExisteHistoriaParaPacienteAsync(int idPaciente, int? idExcluir = null);
    }
}