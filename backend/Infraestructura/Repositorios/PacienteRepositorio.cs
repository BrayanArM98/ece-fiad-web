using Aplicacion.Abstracciones;
using Dominio.Entidades.Pacientes;
using Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorios
{
    public class PacienteRepositorio : RepositorioGenerico<Paciente>, IPacienteRepositorio
    {
        public PacienteRepositorio(ContextoECE contexto) : base(contexto)
        {
        }

        public async Task<Paciente?> ObtenerConHistoriaAsync(int id)
        {
            return await ObtenActivosNoEliminados()
                .Include(p => p.HistoriaClinica)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paciente?> ObtenerPorDocumentoAsync(string numeroDocumento)
        {
            return await ObtenActivosNoEliminados()
                .FirstOrDefaultAsync(p => p.NumeroDocumento == numeroDocumento);
        }

        public async Task<IEnumerable<Paciente?>> ObtenerPaginadoAsync(int pagina, int tamanioPagina, string? filtro = null)
        {
            var query = ObtenActivosNoEliminados();

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(p =>
                    p.Nombres.Contains(filtro) ||
                    p.Apellidos.Contains(filtro) ||
                    p.NumeroDocumento.Contains(filtro));
            }

            return await query
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();
        }
    }
}