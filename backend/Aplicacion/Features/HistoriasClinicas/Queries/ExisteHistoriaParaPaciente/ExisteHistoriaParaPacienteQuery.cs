using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ExisteHistoriaParaPaciente
{
    /// <summary>
    /// Query que verifica si un paciente ya tiene una historia clínica activa (regla 22).
    /// IdExcluir permite ignorar la propia historia al momento de editarla.
    /// </summary>
    public record ExisteHistoriaParaPacienteQuery(int IdPaciente, int? IdExcluir = null)
        : IRequest<bool>;
}