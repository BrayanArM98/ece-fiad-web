using Aplicacion.DTOs.Reportes;
using FluentValidation;

namespace Aplicacion.Validaciones.Reportes
{
    public class FiltroReporteValidator : AbstractValidator<FiltroReporteDTO>
    {
        public const int DiasMaximos = 366;

        public FiltroReporteValidator()
        {
            RuleFor(f => f)
                .Must(f => f.Desde!.Value.Date <= f.Hasta!.Value.Date)
                .WithMessage("La fecha inicial no puede ser posterior a la fecha final.")
                .When(f => f.Desde.HasValue && f.Hasta.HasValue);

            RuleFor(f => f)
                .Must(f => (f.Hasta!.Value.Date - f.Desde!.Value.Date).Days < DiasMaximos)
                .WithMessage($"El periodo de un reporte no puede superar {DiasMaximos} días.")
                .When(f => f.Desde.HasValue && f.Hasta.HasValue);

            RuleFor(f => f.IdDoctor)
                .GreaterThan(0).WithMessage("El doctor seleccionado no es válido.")
                .When(f => f.IdDoctor.HasValue);

            RuleFor(f => f.IdEspecialidad)
                .GreaterThan(0).WithMessage("La especialidad seleccionada no es válida.")
                .When(f => f.IdEspecialidad.HasValue);

            RuleFor(f => f.Estado)
                .IsInEnum().WithMessage("El estado de la cita no es válido.")
                .When(f => f.Estado.HasValue);
        }
    }
}
