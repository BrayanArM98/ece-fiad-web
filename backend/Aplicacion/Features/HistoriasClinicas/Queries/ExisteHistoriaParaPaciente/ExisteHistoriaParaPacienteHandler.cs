using Aplicacion.Abstracciones;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Queries.ExisteHistoriaParaPaciente
{
    /// <summary>
    /// Manejador que responde si el paciente indicado ya tiene historia clínica.
    /// </summary>
    public class ExisteHistoriaParaPacienteHandler
        : IRequestHandler<ExisteHistoriaParaPacienteQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExisteHistoriaParaPacienteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ExisteHistoriaParaPacienteQuery request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.HistoriasClinicas
                .ExisteHistoriaParaPacienteAsync(request.IdPaciente, request.IdExcluir);
        }
    }
}