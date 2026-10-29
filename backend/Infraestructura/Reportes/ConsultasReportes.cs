using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Reportes;
using Dominio.Enumeraciones;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Reportes
{
    /// <summary>
    /// Consultas de solo lectura para los reportes.
    /// Filtran en SQL Server y traen solo las columnas necesarias (sin seguimiento de cambios).
    /// </summary>
    public class ConsultasReportes : IConsultasReportes
    {
        private readonly ContextoECE _contexto;

        public ConsultasReportes(ContextoECE contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<FilaCitaReporte>> ObtenerCitasAsync(
            DateTime desde, DateTime hastaExclusivo, int? idDoctor, int? idEspecialidad, EstadoCita? estado)
        {
            var consulta = _contexto.Citas.AsNoTracking()
                .Where(c => c.Activo && !c.Eliminado && c.FechaHora >= desde && c.FechaHora < hastaExclusivo);

            if (idDoctor.HasValue)
                consulta = consulta.Where(c => c.IdDoctor == idDoctor.Value);

            if (idEspecialidad.HasValue)
                consulta = consulta.Where(c => c.Doctor.IdEspecialidad == idEspecialidad.Value);

            if (estado.HasValue)
                consulta = consulta.Where(c => c.Estado == estado.Value);

            return await consulta
                .Select(c => new FilaCitaReporte(
                    c.Id,
                    c.FechaHora,
                    c.Estado,
                    c.IdDoctor,
                    c.Doctor.Nombres + " " + c.Doctor.Apellidos,
                    c.Doctor.IdEspecialidad,
                    c.Doctor.Especialidad.Nombre,
                    c.IdPaciente))
                .ToListAsync();
        }

        public async Task<List<FilaEvolucionReporte>> ObtenerEvolucionesAsync(
            DateTime desde, DateTime hastaExclusivo, int? idDoctor)
        {
            var consulta = _contexto.Evoluciones.AsNoTracking()
                .Where(e => e.Activo && !e.Eliminado && e.Fecha >= desde && e.Fecha < hastaExclusivo);

            if (idDoctor.HasValue)
                consulta = consulta.Where(e => e.IdDoctor == idDoctor.Value);

            return await consulta
                .Select(e => new FilaEvolucionReporte(e.Fecha, e.IdDoctor, e.Diagnostico))
                .ToListAsync();
        }

        public async Task<List<FilaDoctorReporte>> ObtenerDoctoresAsync(int? idDoctor, int? idEspecialidad)
        {
            var consulta = _contexto.Doctores.AsNoTracking().Where(d => d.Activo && !d.Eliminado);

            if (idDoctor.HasValue)
                consulta = consulta.Where(d => d.Id == idDoctor.Value);

            if (idEspecialidad.HasValue)
                consulta = consulta.Where(d => d.IdEspecialidad == idEspecialidad.Value);

            return await consulta
                .OrderBy(d => d.Apellidos)
                .Select(d => new FilaDoctorReporte(d.Id, d.Nombres + " " + d.Apellidos, d.Especialidad.Nombre))
                .ToListAsync();
        }

        public async Task<List<FilaPacienteReporte>> ObtenerPacientesActivosAsync()
        {
            return await _contexto.Pacientes.AsNoTracking()
                .Where(p => p.Activo && !p.Eliminado)
                .Select(p => new FilaPacienteReporte(p.FechaNacimiento, p.Genero, p.GrupoSanguineo))
                .ToListAsync();
        }

        public async Task<int> ContarPacientesNuevosAsync(DateTime desde, DateTime hastaExclusivo)
        {
            return await _contexto.Pacientes
                .CountAsync(p => !p.Eliminado && p.FechaDeCreacion >= desde && p.FechaDeCreacion < hastaExclusivo);
        }
    }
}
