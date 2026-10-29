using Aplicacion.DTOs.Auth;
using FluentValidation;

namespace Aplicacion.Validaciones.Auth
{
    public class IniciarSesionValidator : AbstractValidator<IniciarSesionDTO>
    {
        public IniciarSesionValidator()
        {
            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("El correo no tiene un formato válido");

            RuleFor(s => s.Contrasena)
                .NotEmpty().WithMessage("La contraseña es obligatoria");
        }
    }
}
