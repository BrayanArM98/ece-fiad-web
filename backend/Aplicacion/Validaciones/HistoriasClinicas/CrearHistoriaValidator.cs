using Aplicacion.DTOs.HistoriasClinicas;
using FluentValidation;

namespace Aplicacion.Validaciones.HistoriasClinicas
{
    public class CrearHistoriaValidator : AbstractValidator<CrearHistoriaDTO>
    {
        public CrearHistoriaValidator()
        {
            RuleFor(h => h.IdPaciente)
                .GreaterThan(0).WithMessage("Debe seleccionar un paciente");

            RuleFor(h => h.FechaApertura)
                .NotEmpty().WithMessage("La fecha de apertura es obligatoria")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de apertura no puede ser futura");

            RuleFor(h => h.Alergias)
                .MaximumLength(500).WithMessage("Las alergias no pueden superar los 500 caracteres");

            RuleFor(h => h.AntecedentesFamiliares)
                .MaximumLength(500).WithMessage("Los antecedentes familiares no pueden superar los 500 caracteres");

            RuleFor(h => h.AntecedentesPersonales)
                .MaximumLength(500).WithMessage("Los antecedentes personales no pueden superar los 500 caracteres");
        }
    }
}