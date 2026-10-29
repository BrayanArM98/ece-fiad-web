using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Entidades.Usuarios;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.CrearUsuario
{
    /// <summary>
    /// Manejador que da de alta una cuenta de acceso.
    /// Verifica que el correo no exista y que el doctor o paciente vinculado exista
    /// y no tenga ya otra cuenta.
    /// </summary>
    public class CrearUsuarioHandler
        : IRequestHandler<CrearUsuarioCommand, ResultadoAccion<UsuarioDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearUsuarioDTO> _validador;
        private readonly IServicioContrasenas _contrasenas;

        public CrearUsuarioHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CrearUsuarioDTO> validador,
            IServicioContrasenas contrasenas)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _contrasenas = contrasenas;
        }

        public async Task<ResultadoAccion<UsuarioDTO>> Handle(
            CrearUsuarioCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Usuario;

            // 1. Validar datos de entrada
            var validacion = await _validador.ValidateAsync(dto, cancellationToken);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<UsuarioDTO>.FallaValidacion(
                    validacion.Errors.Select(e => e.ErrorMessage));
            }

            // 2. Correo único
            if (await _unitOfWork.Usuarios.ExisteEmailAsync(dto.Email))
                return ResultadoAccion<UsuarioDTO>.Falla("Ya existe un usuario con ese correo.");

            // 3. Vínculo con doctor o paciente
            var errorVinculo = await VinculosUsuario.ValidarAsync(_unitOfWork, dto.IdDoctor, dto.IdPaciente, null);
            if (errorVinculo != null)
                return ResultadoAccion<UsuarioDTO>.Falla(errorVinculo);

            // 4. Crear con la contraseña cifrada
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.ContrasenaHash = _contrasenas.Cifrar(dto.Contrasena);

            await _unitOfWork.Usuarios.AgregarAsync(usuario);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<UsuarioDTO>.Exito(
                _mapper.Map<UsuarioDTO>(usuario), "Usuario creado exitosamente.");
        }
    }
}
