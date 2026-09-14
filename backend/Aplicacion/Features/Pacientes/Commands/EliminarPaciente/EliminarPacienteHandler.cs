using Aplicacion.Abstracciones;
using Aplicacion.Helpers;
using MediatR;

namespace Aplicacion.Features.Pacientes.Commands.EliminarPaciente
{
    /// <summary>
    /// Manejador que realiza el borrado lógico de un paciente.
    /// El registro permanece en la base de datos marcado como eliminado e inactivo.
    /// </summary>
    public class EliminarPacienteHandler
        : IRequestHandler<EliminarPacienteCommand, ResultadoAccion>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPacienteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoAccion> Handle(
            EliminarPacienteCommand request,
            CancellationToken cancellationToken)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(request.Id);
            if (paciente == null)
                return ResultadoAccion.Falla("Paciente no encontrado");

            // Borrado lógico: el registro se conserva pero queda marcado como eliminado
            paciente.Eliminado = true;
            paciente.FechaDeEliminacion = DateTime.UtcNow;
            paciente.Activo = false;

            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Paciente eliminado (borrado lógico)");
        }
    }
}