using Aplicacion.DTOs.Pacientes;
using FluentValidation;

namespace Aplicacion.Validaciones.Pacientes
{
    public class CrearPacienteValidator : AbstractValidator<CrearPacienteDTO>
    {
        public CrearPacienteValidator()
        {
            RuleFor(p => p.Nombres)
                .NotEmpty().WithMessage("Los nombres son obligatorios")
                .MaximumLength(100).WithMessage("Los nombres no pueden superar 100 caracteres");

            RuleFor(p => p.Apellidos)
                .NotEmpty().WithMessage("Los apellidos son obligatorios")
                .MaximumLength(100);

            RuleFor(p => p.NumeroDocumento)
                .NotEmpty().WithMessage("El número de identificación es obligatorio")
                .MaximumLength(20);

            RuleFor(p => p.FechaNacimiento)
                .LessThan(DateTime.Today).WithMessage("La fecha de nacimiento debe ser anterior a hoy")
                .Must(fecha => fecha > DateTime.Today.AddYears(-120))
                .WithMessage("Edad no válida (máximo 120 años)");

            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("Formato de email incorrecto")
                .MaximumLength(150);

            RuleFor(p => p.Telefono)
                .MaximumLength(15);
        }
    }
}