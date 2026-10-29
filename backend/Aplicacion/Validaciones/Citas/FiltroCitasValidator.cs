using Aplicacion.DTOs.Citas;
using FluentValidation;

namespace Aplicacion.Validaciones.Citas
{
    /// <summary>
    /// Reglas de los filtros de citas: ids positivos y rango de fechas coherente.
    /// </summary>
    public class FiltroCitasValidator : AbstractValidator<FiltroCitasDTO>
    {
        public FiltroCitasValidator()
        {
            RuleFor(f => f.IdDoctor)
                .GreaterThan(0).WithMessage("El doctor seleccionado no es válido.")
                .When(f => f.IdDoctor.HasValue);

            RuleFor(f => f.IdPaciente)
                .GreaterThan(0).WithMessage("El paciente seleccionado no es válido.")
                .When(f => f.IdPaciente.HasValue);

            RuleFor(f => f.Estado)
                .IsInEnum().WithMessage("El estado de la cita no es válido.")
                .When(f => f.Estado.HasValue);

            RuleFor(f => f)
                .Must(f => f.Desde!.Value.Date <= f.Hasta!.Value.Date)
                .WithMessage("La fecha inicial no puede ser posterior a la fecha final.")
                .When(f => f.Desde.HasValue && f.Hasta.HasValue);
        }
    }
}
