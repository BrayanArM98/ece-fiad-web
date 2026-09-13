using System.Linq.Expressions;

namespace Aplicacion.Abstracciones
{
    public interface IRepositorioGenerico<T> where T : class
    {
        Task<T?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro);
        Task AgregarAsync(T entidad);
        void Actualizar(T entidad);
        void Eliminar(T entidad);
        Task<int> ContarAsync(Expression<Func<T, bool>>? filtro = null);
    }
}