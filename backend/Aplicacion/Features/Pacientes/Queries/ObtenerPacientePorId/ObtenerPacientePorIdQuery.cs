using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ObtenerPacientePorId
{
    /// <summary>
    /// Query que solicita un paciente específico por su identificador.
    /// A diferencia de la query de listado, esta transporta un parámetro.
    /// </summary>
    public record ObtenerPacientePorIdQuery(int Id)
        : IRequest<ResultadoAccion<PacienteDTO>>;
}