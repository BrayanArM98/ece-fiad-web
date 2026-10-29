using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Citas.Queries.ObtenerTodasCitas
{
    /// <summary>
    /// Manejador que devuelve las citas con sus relaciones (paciente y doctor),
    /// aplicando en la base de datos los filtros recibidos.
    /// </summary>
    public class ObtenerTodasCitasHandler
        : IRequestHandler<ObtenerTodasCitasQuery, ResultadoAccion<IEnumerable<CitaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<FiltroCitasDTO> _validador;

        public ObtenerTodasCitasHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<FiltroCitasDTO> validador)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
        }

        public async Task<ResultadoAccion<IEnumerable<CitaDTO>>> Handle(
            ObtenerTodasCitasQuery request,
            CancellationToken cancellationToken)
        {
            var filtro = request.Filtro ?? new FiltroCitasDTO();

            // 1. Validar los filtros
            var validacion = await _validador.ValidateAsync(filtro, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<IEnumerable<CitaDTO>>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. "Hasta" incluye todo el día: se consulta hasta el inicio del día siguiente
            DateTime? hastaExclusivo = filtro.Hasta?.Date.AddDays(1);

            // 3. Consultar con los filtros aplicados en la base de datos
            var citas = await _unitOfWork.Citas.ObtenerFiltradasAsync(
                filtro.IdDoctor,
                filtro.IdPaciente,
                filtro.Desde,
                hastaExclusivo,
                filtro.Estado);

            var lista = _mapper.Map<IEnumerable<CitaDTO>>(citas);
            return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(lista);
        }
    }
}
