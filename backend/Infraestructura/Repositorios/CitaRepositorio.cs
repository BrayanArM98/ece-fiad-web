using Aplicacion.Abstracciones;
using Dominio.Entidades.Citas;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class CitaRepositorio : RepositorioGenerico<Cita>, ICitaRepositorio
    {
        public CitaRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        // Trae la cita junto con su paciente y doctor cargados.
        // Útil para mostrar nombres en detalles, edición y al confirmar al usuario.
        public async Task<Cita?> ObtenerConRelacionesAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Trae todas las citas activas con paciente y doctor incluidos.
        // Necesario para que NombrePaciente y NombreDoctor se llenen en el listado.
        public async Task<IEnumerable<Cita>> ObtenerTodasConRelacionesAsync()
        {
            return await ObtenActivosNoEliminados()
                .Include(c => c.Paciente)
                .Include(c => c.Doctor)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();
        }

        // Verifica si un doctor ya tiene una cita en el mismo horario exacto.
        // Implementa la regla 21 del manual: no permitir doble reserva del mismo doctor.
        // El idExcluir sirve al editar (para no chocar con la propia cita).
        public async Task<bool> ExisteCitaEnHorarioAsync(int idDoctor, DateTime fechaHora, int? idExcluir = null)
        {
            var consulta = ObtenActivosNoEliminados()
                .Where(c => c.IdDoctor == idDoctor && c.FechaHora == fechaHora);

            if (idExcluir.HasValue)
            {
                consulta = consulta.Where(c => c.Id != idExcluir.Value);
            }

            return await consulta.AnyAsync();
        }
    }
}