using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Especialidades.Commands.EliminarEspecialidad
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de una especialidad.
    /// Regla de negocio: no se permite eliminar si tiene doctores asociados.
    /// </summary>
    public class EliminarEspecialidadHandler
        : IRequestHandler<EliminarEspecialidadCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarEspecialidadHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarEspecialidadCommand request,
            CancellationToken cancellationToken)
        {
            // Se trae con sus doctores para poder verificar la restricción
            var especialidad = await _unitOfWork.Especialidades.ObtenerConDoctoresAsync(request.Id);
            if (especialidad == null)
                return ResultadoAccion.Falla("Especialidad no encontrada");

            // Restricción: no eliminar si tiene doctores asociados
            if (especialidad.Doctores != null && especialidad.Doctores.Any())
            {
                return ResultadoAccion.Falla(
                    "No se puede eliminar la especialidad porque tiene doctores asociados. " +
                    "Primero debe reasignar o eliminar los doctores asociados.");
            }

            // Borrado lógico
            especialidad.Eliminado = true;
            especialidad.FechaDeEliminacion = DateTime.UtcNow;
            especialidad.Activo = false;

            _unitOfWork.Especialidades.Actualizar(especialidad);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Especialidad eliminada (borrado lógico)");
        }
    }
}