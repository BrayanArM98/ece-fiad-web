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
    /// Un paciente solo recibe sus citas y un médico solo las de su agenda,
    /// sin importar los filtros que envíe.
    /// </summary>
    public class ObtenerTodasCitasHandler
        : IRequestHandler<ObtenerTodasCitasQuery, ResultadoAccion<IEnumerable<CitaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<FiltroCitasDTO> _validador;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerTodasCitasHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<FiltroCitasDTO> validador,
            IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _usuarioActual = usuarioActual;
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

            // 2. Restricción por rol: se impone el paciente o el doctor de la sesión
            if (_usuarioActual.EsPaciente)
            {
                if (_usuarioActual.IdPaciente == null)
                    return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(Enumerable.Empty<CitaDTO>());
                filtro.IdPaciente = _usuarioActual.IdPaciente;
            }
            else if (_usuarioActual.EsMedico)
            {
                if (_usuarioActual.IdDoctor == null)
                    return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(Enumerable.Empty<CitaDTO>());
                filtro.IdDoctor = _usuarioActual.IdDoctor;
            }

            // 3. "Hasta" incluye todo el día: se consulta hasta el inicio del día siguiente
            DateTime? hastaExclusivo = filtro.Hasta?.Date.AddDays(1);

            // 4. Consultar con los filtros aplicados en la base de datos
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
