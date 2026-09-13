using Aplicacion.Abstracciones;
using Dominio.Entidades.Doctores;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class DoctorRepositorio : RepositorioGenerico<Doctor>, IDoctorRepositorio
    {
        public DoctorRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        // Trae el doctor junto con su especialidad asociada.
        // Útil para mostrar el nombre de la especialidad en detalles y edición.
        public async Task<Doctor?> ObtenerConEspecialidadAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(d => d.Especialidad)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Trae todos los doctores activos con su especialidad y citas incluidas.
        // Necesario para que NombreEspecialidad y CantidadCitas se calculen bien en el listado.
        public async Task<IEnumerable<Doctor>> ObtenerTodosConEspecialidadAsync()
        {
            return await ObtenActivosNoEliminados()
                .Include(d => d.Especialidad)
                .Include(d => d.Citas)
                .ToListAsync();
        }

        // Trae el doctor con sus citas asociadas.
        // Se usa para validar antes de eliminar (no se puede borrar si tiene citas).
        public async Task<Doctor?> ObtenerConCitasAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(d => d.Citas)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Busca un doctor por email exacto.
        // El idExcluir sirve al editar para no chocar con el propio registro.
        public async Task<Doctor?> ObtenerPorEmailAsync(string email, int? idExcluir = null)
        {
            var consulta = ObtenActivosNoEliminados()
                .Where(d => d.Email == email);

            if (idExcluir.HasValue)
            {
                consulta = consulta.Where(d => d.Id != idExcluir.Value);
            }

            return await consulta.FirstOrDefaultAsync();
        }
    }
}