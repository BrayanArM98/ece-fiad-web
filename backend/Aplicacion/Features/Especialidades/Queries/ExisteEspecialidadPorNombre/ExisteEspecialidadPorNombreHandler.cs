using Aplicacion.Abstracciones;
using MediatR;

namespace Aplicacion.Features.Especialidades.Queries.ExisteEspecialidadPorNombre
{
    /// <summary>
    /// Manejador que responde si existe una especialidad con el nombre indicado.
    /// </summary>
    public class ExisteEspecialidadPorNombreHandler
        : IRequestHandler<ExisteEspecialidadPorNombreQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExisteEspecialidadPorNombreHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ExisteEspecialidadPorNombreQuery request,
            CancellationToken cancellationToken)
        {
            var especialidades = await _unitOfWork.Especialidades.BuscarAsync(e =>
                e.Nombre == request.Nombre &&
                (request.IdExcluir == null || e.Id != request.IdExcluir));

            return especialidades.Any();
        }
    }
}