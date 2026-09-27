using Aplicacion.Abstracciones;
using MediatR;

namespace Aplicacion.Features.Pacientes.Queries.ExistePacientePorDocumento
{
    /// <summary>
    /// Manejador que responde si existe un paciente con el número de documento indicado.
    /// </summary>
    public class ExistePacientePorDocumentoHandler
        : IRequestHandler<ExistePacientePorDocumentoQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExistePacientePorDocumentoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ExistePacientePorDocumentoQuery request,
            CancellationToken cancellationToken)
        {
            var pacientes = await _unitOfWork.Pacientes
                .BuscarAsync(p => p.NumeroDocumento == request.NumeroDocumento);

            return pacientes.Any();
        }
    }
}