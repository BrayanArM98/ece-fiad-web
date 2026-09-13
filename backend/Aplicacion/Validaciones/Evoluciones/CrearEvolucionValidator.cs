using Aplicacion.DTOs.Evoluciones;
using FluentValidation;

namespace Aplicacion.Validaciones.Evoluciones
{
    public class CrearEvolucionValidator : AbstractValidator<CrearEvolucionDTO>
    {
        public CrearEvolucionValidator()
        {
            RuleFor(e => e.IdHistoriaClinica)
                .GreaterThan(0).WithMessage("Debe seleccionar una historia clínica");

            RuleFor(e => e.IdDoctor)
                .GreaterThan(0).WithMessage("Debe seleccionar un doctor");

            RuleFor(e => e.Fecha)
                .NotEmpty().WithMessage("La fecha es obligatoria")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura");

            RuleFor(e => e.Diagnostico)
                .NotEmpty().WithMessage("El diagnóstico es obligatorio")
                .MaximumLength(500).WithMessage("El diagnóstico no puede superar los 500 caracteres");

            RuleFor(e => e.Tratamiento)
                .NotEmpty().WithMessage("El tratamiento es obligatorio")
                .MaximumLength(500).WithMessage("El tratamiento no puede superar los 500 caracteres");

            RuleFor(e => e.Notas)
                .MaximumLength(1000).WithMessage("Las notas no pueden superar los 1000 caracteres");
        }
    }
}