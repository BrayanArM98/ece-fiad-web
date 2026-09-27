using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Doctores.Commands.EliminarDoctor
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de un doctor.
    /// Regla de negocio: no se permite eliminar si tiene citas asociadas.
    /// </summary>
    public class EliminarDoctorHandler
        : IRequestHandler<EliminarDoctorCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarDoctorHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarDoctorCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Traer el doctor con sus citas para validar la restricción
                var doctor = await _unitOfWork.Doctores.ObtenerConCitasAsync(request.Id);
                if (doctor == null)
                    return ResultadoAccion.Falla("Doctor no encontrado.");

                // 2. Restricción: no eliminar si tiene citas asociadas
                if (doctor.Citas != null && doctor.Citas.Any())
                {
                    return ResultadoAccion.Falla(
                        $"No se puede eliminar el doctor porque tiene {doctor.Citas.Count} cita(s) asociada(s).");
                }

                // 3. Borrado lógico
                _unitOfWork.Doctores.Eliminar(doctor);
                await _unitOfWork.GuardarCambiosAsync();

                return ResultadoAccion.Exito("Doctor eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return ResultadoAccion.Falla($"Error al eliminar doctor: {ex.Message}");
            }
        }
    }
}