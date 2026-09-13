using Aplicacion.DTOs.Citas;
using FluentValidation;

namespace Aplicacion.Validaciones.Citas
{
    public class CrearCitaValidator : AbstractValidator<CrearCitaDTO>
    {
        public CrearCitaValidator()
        {
            RuleFor(c => c.IdPaciente)
                .GreaterThan(0).WithMessage("Debe seleccionar un paciente");

            RuleFor(c => c.IdDoctor)
                .GreaterThan(0).WithMessage("Debe seleccionar un doctor");

            RuleFor(c => c.FechaHora)
                .NotEmpty().WithMessage("La fecha y hora son obligatorias")
                .GreaterThan(DateTime.Now).WithMessage("La fecha y hora deben ser futuras");

            RuleFor(c => c.Motivo)
                .NotEmpty().WithMessage("El motivo de la cita es obligatorio")
                .MaximumLength(500).WithMessage("El motivo no puede superar los 500 caracteres");

            RuleFor(c => c.Notas)
                .MaximumLength(1000).WithMessage("Las notas no pueden superar los 1000 caracteres");

            RuleFor(c => c.Estado)
                .IsInEnum().WithMessage("Debe seleccionar un estado válido");
        }
    }
}