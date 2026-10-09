using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Evoluciones.Commands.EliminarEvolucion
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de una evolución.
    /// </summary>
    public class EliminarEvolucionHandler
        : IRequestHandler<EliminarEvolucionCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarEvolucionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarEvolucionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var evolucion = await _unitOfWork.Evoluciones.ObtenerPorIdAsync(request.Id);
                if (evolucion == null)
                    return ResultadoAccion.Falla("La evolución no existe o ya fue eliminada.");

                // Borrado lógico
                evolucion.Eliminado = true;
                evolucion.FechaDeEliminacion = DateTime.UtcNow;

                _unitOfWork.Evoluciones.Actualizar(evolucion);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Evolución eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar evolución: {ex.Message}");
            }
        }
    }
}