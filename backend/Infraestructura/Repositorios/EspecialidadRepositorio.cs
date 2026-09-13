using Aplicacion.Abstracciones;
using Dominio.Entidades.Especialidades;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class EspecialidadRepositorio : RepositorioGenerico<Especialidad>, IEspecialidadRepositorio
    {
        public EspecialidadRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        // Trae la especialidad junto con sus doctores asociados.
        // Útil para validar antes de eliminar (no se puede borrar si tiene doctores).
        public async Task<Especialidad?> ObtenerConDoctoresAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(e => e.Doctores)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Busca una especialidad por nombre exacto.
        // Se usa para evitar nombres duplicados.
        public async Task<Especialidad?> ObtenerPorNombreAsync(string nombre)
        {
            return await ObtenActivosNoEliminados()
                .FirstOrDefaultAsync(e => e.Nombre == nombre);
        }

        // Trae todas las especialidades activas con sus doctores incluidos.
        // Necesario para que CantidadDoctores funcione en el listado.
        public async Task<IEnumerable<Especialidad>> ObtenerTodosConDoctoresAsync()
        {
            return await ObtenActivosNoEliminados()
                .Include(e => e.Doctores)
                .ToListAsync();
        }
    }
}