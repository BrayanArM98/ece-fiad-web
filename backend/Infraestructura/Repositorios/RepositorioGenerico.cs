using Aplicacion.Abstracciones;
using Dominio.Entidades.Bases;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infraestructura.Repositorios
{
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : EntidadBase
    {
        protected readonly ContextoECE _contexto;
        protected readonly DbSet<T> _dbSet;

        public RepositorioGenerico(ContextoECE contexto)
        {
            _contexto = contexto;
            _dbSet = contexto.Set<T>();
        }

        // Filtro base: solo entidades activas y no eliminadas (borrado lógico)
        protected IQueryable<T> ObtenActivosNoEliminados()
        {
            return _dbSet.Where(e => e.Activo && !e.Eliminado);
        }

        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await ObtenActivosNoEliminados().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            return await ObtenActivosNoEliminados().ToListAsync();
        }

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro)
        {
            return await ObtenActivosNoEliminados().Where(filtro).ToListAsync();
        }

        public async Task AgregarAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
        }

        public void Actualizar(T entidad)
        {
            _dbSet.Update(entidad);
        }

        public void Eliminar(T entidad)
        {
            _dbSet.Remove(entidad);
        }

        public async Task<int> ContarAsync(Expression<Func<T, bool>>? filtro = null)
        {
            var query = ObtenActivosNoEliminados();
            if (filtro != null)
                query = query.Where(filtro);
            return await query.CountAsync();
        }
    }
}