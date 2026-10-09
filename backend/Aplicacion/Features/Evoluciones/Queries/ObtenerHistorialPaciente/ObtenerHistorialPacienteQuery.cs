using Aplicacion.DTOs.Evoluciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ObtenerHistorialPaciente
{
    /// <summary>
    /// Query que solicita el historial clínico completo de un paciente:
    /// su historia clínica activa y todas sus evoluciones.
    /// </summary>
    public record ObtenerHistorialPacienteQuery(int IdPaciente)
        : IRequest<ResultadoAccion<HistorialPacienteDTO>>;
}