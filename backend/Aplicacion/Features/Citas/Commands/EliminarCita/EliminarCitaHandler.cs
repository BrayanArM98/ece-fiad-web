using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Citas.Commands.EliminarCita
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de una cita.
    /// </summary>
    public class EliminarCitaHandler
        : IRequestHandler<EliminarCitaCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarCitaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarCitaCommand request,
            CancellationToken cancellationToken)
        {
            var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(request.Id);
            if (cita == null)
                return ResultadoAccion.Falla("Cita no encontrada.");

            // Borrado lógico: la cita queda inactiva con fecha de eliminación
            _unitOfWork.Citas.Eliminar(cita);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Cita eliminada correctamente.");
        }
    }
}