using Aplicacion.DTOs.Usuarios;
using Dominio.Enumeraciones;
using FluentValidation;

namespace Aplicacion.Validaciones.Usuarios
{
    public class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioDTO>
    {
        public ActualizarUsuarioValidator()
        {
            RuleFor(u => u.Id)
                .GreaterThan(0).WithMessage("El usuario no es válido");

            RuleFor(u => u.NombreCompleto)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres");

            RuleFor(u => u.Rol)
                .IsInEnum().WithMessage("El rol seleccionado no es válido");

            // La contraseña solo se valida si se quiere cambiar
            RuleFor(u => u.NuevaContrasena!)
                .ContrasenaSegura()
                .When(u => !string.IsNullOrEmpty(u.NuevaContrasena));

            // Un médico se vincula con un doctor, un paciente con un paciente y los demás roles con ninguno
            RuleFor(u => u.IdDoctor)
                .NotNull().WithMessage("Un usuario médico debe vincularse con un doctor")
                .GreaterThan(0).WithMessage("El doctor seleccionado no es válido")
                .When(u => u.Rol == RolUsuario.Medico);

            RuleFor(u => u.IdPaciente)
                .NotNull().WithMessage("Un usuario paciente debe vincularse con un paciente")
                .GreaterThan(0).WithMessage("El paciente seleccionado no es válido")
                .When(u => u.Rol == RolUsuario.Paciente);

            RuleFor(u => u.IdDoctor)
                .Null().WithMessage("Solo un usuario médico se vincula con un doctor")
                .When(u => u.Rol != RolUsuario.Medico);

            RuleFor(u => u.IdPaciente)
                .Null().WithMessage("Solo un usuario paciente se vincula con un paciente")
                .When(u => u.Rol != RolUsuario.Paciente);
        }
    }
}
