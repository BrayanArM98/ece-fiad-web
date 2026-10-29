using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerCitaPorId
{
    /// <summary>
    /// Manejador que recupera una cita por su Id, incluyendo paciente y doctor.
    /// Para un paciente o un médico, una cita ajena se responde como no encontrada.
    /// </summary>
    public class ObtenerCitaPorIdHandler
        : IRequestHandler<ObtenerCitaPorIdQuery, ResultadoAccion<CitaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerCitaPorIdHandler(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<CitaDTO>> Handle(
            ObtenerCitaPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var cita = await _unitOfWork.Citas.ObtenerConRelacionesAsync(request.Id);
            var esAjena =
                (_usuarioActual.EsPaciente && cita?.IdPaciente != _usuarioActual.IdPaciente) ||
                (_usuarioActual.EsMedico && cita?.IdDoctor != _usuarioActual.IdDoctor);

            if (cita == null || esAjena)
                return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada.");

            var dto = _mapper.Map<CitaDTO>(cita);
            return ResultadoAccion<CitaDTO>.Exito(dto);
        }
    }
}