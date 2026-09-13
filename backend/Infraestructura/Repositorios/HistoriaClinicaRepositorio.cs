using Aplicacion.Abstracciones;
using Dominio.Entidades.HistoriasClinicas;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class HistoriaClinicaRepositorio : RepositorioGenerico<HistoriaClinica>, IHistoriaClinicaRepositorio
    {
        public HistoriaClinicaRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        // Trae la historia clínica junto con su paciente cargado.
        // Útil para mostrar el nombre en detalles y edición.
        public async Task<HistoriaClinica?> ObtenerConRelacionesAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(h => h.Paciente)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        // Trae todas las historias activas con paciente incluido.
        // Necesario para que NombrePaciente se llene en el listado.
        public async Task<IEnumerable<HistoriaClinica>> ObtenerTodasConRelacionesAsync()
        {
            return await ObtenActivosNoEliminados()
                .Include(h => h.Paciente)
                .OrderByDescending(h => h.FechaApertura)
                .ToListAsync();
        }

        // Verifica si el paciente ya tiene una historia clínica activa (regla 22 del manual).
        // El idExcluir sirve al editar para no chocar con la propia historia.
        public async Task<bool> ExisteHistoriaParaPacienteAsync(int idPaciente, int? idExcluir = null)
        {
            var consulta = ObtenActivosNoEliminados()
                .Where(h => h.IdPaciente == idPaciente);

            if (idExcluir.HasValue)
            {
                consulta = consulta.Where(h => h.Id != idExcluir.Value);
            }

            return await consulta.AnyAsync();
        }
    }
}