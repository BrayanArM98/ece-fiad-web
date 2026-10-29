using Aplicacion.Abstracciones;
using Dominio.Entidades.Usuarios;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class UsuarioRepositorio : RepositorioGenerico<Usuario>, IUsuarioRepositorio
    {
        public UsuarioRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        // Los correos se guardan en minúsculas, así que se compara en minúsculas
        public async Task<Usuario?> ObtenerActivoPorEmailAsync(string email)
        {
            var normalizado = email.Trim().ToLowerInvariant();
            return await ObtenActivosNoEliminados()
                .FirstOrDefaultAsync(u => u.Email == normalizado);
        }

        // Incluye inactivos, pero nunca eliminados
        public async Task<IEnumerable<Usuario>> ObtenerTodosIncluyendoInactivosAsync()
        {
            return await _dbSet
                .Where(u => !u.Eliminado)
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdIncluyendoInactivoAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Id == id && !u.Eliminado);
        }

        // Un correo usado por un usuario inactivo tampoco se puede repetir
        public async Task<bool> ExisteEmailAsync(string email, int? idExcluir = null)
        {
            var normalizado = email.Trim().ToLowerInvariant();
            return await _dbSet.AnyAsync(u =>
                u.Email == normalizado && !u.Eliminado && (idExcluir == null || u.Id != idExcluir));
        }

        public async Task<bool> ExisteCuentaParaDoctorAsync(int idDoctor, int? idExcluir = null)
        {
            return await _dbSet.AnyAsync(u =>
                u.IdDoctor == idDoctor && !u.Eliminado && (idExcluir == null || u.Id != idExcluir));
        }

        public async Task<bool> ExisteCuentaParaPacienteAsync(int idPaciente, int? idExcluir = null)
        {
            return await _dbSet.AnyAsync(u =>
                u.IdPaciente == idPaciente && !u.Eliminado && (idExcluir == null || u.Id != idExcluir));
        }

        public async Task<bool> HayUsuariosAsync()
        {
            return await _dbSet.AnyAsync();
        }
    }
}
