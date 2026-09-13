using Aplicacion.DTOs.Especialidades;
using FluentValidation;

namespace Aplicacion.Validaciones.Especialidades
{
    public class CrearEspecialidadValidator : AbstractValidator<CrearEspecialidadDTO>
    {
        public CrearEspecialidadValidator()
        {
            RuleFor(e => e.Nombre)
                .NotEmpty().WithMessage("El nombre de la especialidad es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres");

            RuleFor(e => e.Descripcion)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres");
        }
    }
}