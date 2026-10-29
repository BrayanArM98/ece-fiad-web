using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Queries.ContarEvolucionesPorPaciente
{
    /// <summary>
    /// Manejador que cuenta las evoluciones registradas para un paciente.
    /// </summary>
    public class ContarEvolucionesPorPacienteHandler
        : IRequestHandler<ContarEvolucionesPorPacienteQuery, ResultadoAccion<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContarEvolucionesPorPacienteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion<int>> Handle(
            ContarEvolucionesPorPacienteQuery request,
            CancellationToken cancellationToken)
        {
            var cantidad = await _unitOfWork.Evoluciones.ContarPorPacienteAsync(request.IdPaciente);
            return ResultadoAccion<int>.Exito(cantidad);
        }
    }
}