using Dominio.Entidades.Pacientes;

namespace Aplicacion.Abstracciones
{
    public interface IPacienteRepositorio : IRepositorioGenerico<Paciente>
    {
        Task<Paciente?> ObtenerConHistoriaAsync(int id);
        Task<Paciente?> ObtenerPorDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<Paciente?>> ObtenerPaginadoAsync(int pagina, int tamanioPagina, string? filtro = null);
    }
}