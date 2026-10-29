using Aplicacion.Abstracciones;

namespace Aplicacion.Features.Usuarios
{
    /// <summary>
    /// Regla compartida por crear y actualizar usuario: el doctor o paciente vinculado
    /// debe existir y no puede tener otra cuenta.
    /// </summary>
    internal static class VinculosUsuario
    {
        /// <returns>El mensaje de error, o null si los vínculos son válidos.</returns>
        public static async Task<string?> ValidarAsync(
            IUnitOfWork unitOfWork,
            int? idDoctor,
            int? idPaciente,
            int? idUsuarioExcluir)
        {
            if (idDoctor is int doctor)
            {
                if (await unitOfWork.Doctores.ObtenerPorIdAsync(doctor) == null)
                    return "El doctor seleccionado no existe.";

                if (await unitOfWork.Usuarios.ExisteCuentaParaDoctorAsync(doctor, idUsuarioExcluir))
                    return "El doctor seleccionado ya tiene una cuenta de usuario.";
            }

            if (idPaciente is int paciente)
            {
                if (await unitOfWork.Pacientes.ObtenerPorIdAsync(paciente) == null)
                    return "El paciente seleccionado no existe.";

                if (await unitOfWork.Usuarios.ExisteCuentaParaPacienteAsync(paciente, idUsuarioExcluir))
                    return "El paciente seleccionado ya tiene una cuenta de usuario.";
            }

            return null;
        }
    }
}
