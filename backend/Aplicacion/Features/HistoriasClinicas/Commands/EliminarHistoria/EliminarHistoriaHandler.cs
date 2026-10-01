using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.HistoriasClinicas.Commands.EliminarHistoria
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de una historia clínica.
    /// </summary>
    public class EliminarHistoriaHandler
        : IRequestHandler<EliminarHistoriaCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarHistoriaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarHistoriaCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var historia = await _unitOfWork.HistoriasClinicas.ObtenerPorIdAsync(request.Id);
                if (historia == null)
                    return ResultadoAccion.Falla("La historia clínica no existe o ya fue eliminada.");

                // Borrado lógico
                historia.Eliminado = true;
                historia.FechaDeEliminacion = DateTime.UtcNow;

                _unitOfWork.HistoriasClinicas.Actualizar(historia);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Historia clínica eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar historia clínica: {ex.Message}");
            }
        }
    }
}