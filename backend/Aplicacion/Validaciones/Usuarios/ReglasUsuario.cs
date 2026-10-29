using FluentValidation;

namespace Aplicacion.Validaciones.Usuarios
{
    /// <summary>
    /// Reglas compartidas por la creación y la edición de usuarios.
    /// </summary>
    internal static class ReglasUsuario
    {
        public const int LongitudMinimaContrasena = 8;

        /// <summary>Mínimo 8 caracteres, con al menos una letra y un número.</summary>
        public static IRuleBuilderOptions<T, string> ContrasenaSegura<T>(this IRuleBuilder<T, string> regla)
        {
            return regla
                .MinimumLength(LongitudMinimaContrasena)
                    .WithMessage($"La contraseña debe tener al menos {LongitudMinimaContrasena} caracteres")
                .MaximumLength(100).WithMessage("La contraseña no puede superar los 100 caracteres")
                .Matches("[A-Za-z]").WithMessage("La contraseña debe incluir al menos una letra")
                .Matches("[0-9]").WithMessage("La contraseña debe incluir al menos un número");
        }
    }
}
