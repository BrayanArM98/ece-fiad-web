using Dominio.Entidades.Evoluciones;

namespace Aplicacion.Abstracciones
{
    public interface IEvolucionRepositorio : IRepositorioGenerico<Evolucion>
    {
        // Obtiene una evolución junto con su historia clínica (incluyendo el paciente)
        // y el doctor cargados. Útil para mostrar nombres en detalles y edición.
        Task<Evolucion?> ObtenerConRelacionesAsync(int id);

        // Obtiene todas las evoluciones activas con historia clínica (paciente) y doctor incluidos.
        // Necesario para que en el listado se muestren los nombres correctamente.
        Task<IEnumerable<Evolucion>> ObtenerTodasConRelacionesAsync();

        // Obtiene todas las evoluciones de una historia clínica específica.
        // Se usa para la vista filtrada por paciente (regla 27 del manual).
        Task<IEnumerable<Evolucion>> ObtenerPorHistoriaClinicaAsync(int idHistoriaClinica);

        // Cuenta cuántas evoluciones activas tiene un paciente específico.
        // Se usa para el reporte resumido (regla 29 del manual).
        Task<int> ContarPorPacienteAsync(int idPaciente);
    }
}