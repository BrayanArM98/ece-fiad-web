using Aplicacion.DTOs.Doctores;
using FluentValidation;

namespace Aplicacion.Validaciones.Doctores
{
    public class ActualizarDoctorValidator : AbstractValidator<ActualizarDoctorDTO>
    {
        public ActualizarDoctorValidator()
        {
            RuleFor(d => d.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0");

            RuleFor(d => d.Nombres)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres");

            RuleFor(d => d.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100).WithMessage("Los apellidos no pueden superar los 100 caracteres");

            RuleFor(d => d.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio")
                .MaximumLength(20).WithMessage("El teléfono no puede superar los 20 caracteres");

            RuleFor(d => d.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio")
                .EmailAddress().WithMessage("El formato del correo electrónico no es válido")
                .MaximumLength(100).WithMessage("El correo no puede superar los 100 caracteres");

            RuleFor(d => d.HorarioAtencion)
                .MaximumLength(200).WithMessage("El horario de atención no puede superar los 200 caracteres");

            RuleFor(d => d.IdEspecialidad)
                .GreaterThan(0).WithMessage("Debe seleccionar una especialidad");
        }
    }
}