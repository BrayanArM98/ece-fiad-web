using Aplicacion.DTOs.Reportes;
using Dominio.Enumeraciones;

namespace Aplicacion.Abstracciones
{
    /// <summary>
    /// Consultas de solo lectura para los reportes.
    /// Filtran y proyectan en la base de datos; desde es inclusivo y hastaExclusivo no lo es.
    /// </summary>
    public interface IConsultasReportes
    {
        Task<List<FilaCitaReporte>> ObtenerCitasAsync(
            DateTime desde, DateTime hastaExclusivo, int? idDoctor, int? idEspecialidad, EstadoCita? estado);

        Task<List<FilaEvolucionReporte>> ObtenerEvolucionesAsync(
            DateTime desde, DateTime hastaExclusivo, int? idDoctor);

        Task<List<FilaDoctorReporte>> ObtenerDoctoresAsync(int? idDoctor, int? idEspecialidad);

        Task<List<FilaPacienteReporte>> ObtenerPacientesActivosAsync();

        Task<int> ContarPacientesNuevosAsync(DateTime desde, DateTime hastaExclusivo);
    }
}
