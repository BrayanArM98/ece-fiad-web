using Dominio.Entidades.Usuarios;

namespace Aplicacion.Abstracciones
{
    public interface IUsuarioRepositorio : IRepositorioGenerico<Usuario>
    {
        // Busca un usuario activo por correo, sin distinguir mayúsculas. Se usa al iniciar sesión.
        Task<Usuario?> ObtenerActivoPorEmailAsync(string email);

        // El administrador necesita ver también a los usuarios desactivados para poder reactivarlos.
        Task<IEnumerable<Usuario>> ObtenerTodosIncluyendoInactivosAsync();
        Task<Usuario?> ObtenerPorIdIncluyendoInactivoAsync(int id);

        // Reglas de unicidad: un correo, un doctor y un paciente solo pueden tener una cuenta.
        Task<bool> ExisteEmailAsync(string email, int? idExcluir = null);
        Task<bool> ExisteCuentaParaDoctorAsync(int idDoctor, int? idExcluir = null);
        Task<bool> ExisteCuentaParaPacienteAsync(int idPaciente, int? idExcluir = null);

        // Indica si la base ya tiene algún usuario (para crear el administrador inicial).
        Task<bool> HayUsuariosAsync();
    }
}
