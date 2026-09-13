using Aplicacion.Abstracciones;
using Dominio.Entidades.Evoluciones;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class EvolucionRepositorio : RepositorioGenerico<Evolucion>, IEvolucionRepositorio
    {
        public EvolucionRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        public async Task<Evolucion?> ObtenerConRelacionesAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(e => e.HistoriaClinica)
                    .ThenInclude(h => h.Paciente)
                .Include(e => e.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Evolucion>> ObtenerTodasConRelacionesAsync()
        {
            return await ObtenActivosNoEliminados()
                .Include(e => e.HistoriaClinica)
                    .ThenInclude(h => h.Paciente)
                .Include(e => e.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evolucion>> ObtenerPorHistoriaClinicaAsync(int idHistoriaClinica)
        {
            return await ObtenActivosNoEliminados()
                .Where(e => e.IdHistoriaClinica == idHistoriaClinica)
                .Include(e => e.HistoriaClinica)
                    .ThenInclude(h => h.Paciente)
                .Include(e => e.Doctor)
                    .ThenInclude(d => d.Especialidad)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<int> ContarPorPacienteAsync(int idPaciente)
        {
            return await ObtenActivosNoEliminados()
                .Where(e => e.HistoriaClinica.IdPaciente == idPaciente)
                .CountAsync();
        }
    }
}