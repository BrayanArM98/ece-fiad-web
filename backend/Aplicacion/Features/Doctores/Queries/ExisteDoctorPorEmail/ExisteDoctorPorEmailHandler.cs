using Aplicacion.Abstracciones;
using MediatR;

namespace Aplicacion.Features.Doctores.Queries.ExisteDoctorPorEmail
{
    /// <summary>
    /// Manejador que responde si existe un doctor con el correo indicado.
    /// </summary>
    public class ExisteDoctorPorEmailHandler
        : IRequestHandler<ExisteDoctorPorEmailQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExisteDoctorPorEmailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ExisteDoctorPorEmailQuery request,
            CancellationToken cancellationToken)
        {
            var doctor = await _unitOfWork.Doctores
                .ObtenerPorEmailAsync(request.Email, request.IdExcluir);

            return doctor != null;
        }
    }
}