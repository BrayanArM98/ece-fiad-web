using Aplicacion.DTOs.Pacientes;
using FluentValidation;

namespace Aplicacion.Validaciones.Pacientes
{
    public class ActualizarPacienteValidacion : AbstractValidator<ActualizarPacienteDTO>
    {
        public ActualizarPacienteValidacion()
        {
            RuleFor(p => p.Id).GreaterThan(0);
            RuleFor(p => p.Nombres).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Apellidos).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Email).EmailAddress().MaximumLength(150);
        }
    }
}