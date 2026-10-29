using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Auth;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Auth.Commands.IniciarSesion
{
    /// <summary>
    /// Manejador del inicio de sesión.
    /// Ante cualquier falla de credenciales responde el mismo mensaje,
    /// para no revelar si el correo existe.
    /// </summary>
    public class IniciarSesionHandler
        : IRequestHandler<IniciarSesionCommand, ResultadoAccion<SesionDTO>>
    {
        private const string MensajeCredencialesInvalidas = "Correo o contraseña incorrectos.";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<IniciarSesionDTO> _validador;
        private readonly IServicioContrasenas _contrasenas;
        private readonly IGeneradorTokens _tokens;

        public IniciarSesionHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<IniciarSesionDTO> validador,
            IServicioContrasenas contrasenas,
            IGeneradorTokens tokens)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _contrasenas = contrasenas;
            _tokens = tokens;
        }

        public async Task<ResultadoAccion<SesionDTO>> Handle(
            IniciarSesionCommand request,
            CancellationToken cancellationToken)
        {
            var credenciales = request.Credenciales;

            // 1. Validar formato
            var validacion = await _validador.ValidateAsync(credenciales, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<SesionDTO>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Buscar al usuario activo y verificar la contraseña
            var usuario = await _unitOfWork.Usuarios.ObtenerActivoPorEmailAsync(credenciales.Email);
            if (usuario == null || !_contrasenas.Verificar(usuario.ContrasenaHash, credenciales.Contrasena))
                return ResultadoAccion<SesionDTO>.Falla(MensajeCredencialesInvalidas);

            // 3. Registrar el acceso
            usuario.UltimoAcceso = DateTime.UtcNow;
            _unitOfWork.Usuarios.Actualizar(usuario);
            await _unitOfWork.GuardarCambiosAsync();

            // 4. Emitir el token
            var token = _tokens.Generar(usuario);
            var sesion = new SesionDTO
            {
                Token = token.Token,
                ExpiraUtc = token.ExpiraUtc,
                Usuario = _mapper.Map<UsuarioDTO>(usuario)
            };

            return ResultadoAccion<SesionDTO>.Exito(sesion, "Sesión iniciada correctamente.");
        }
    }
}
