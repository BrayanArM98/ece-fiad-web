using Aplicacion.Abstracciones;
using Aplicacion.DTOs.Usuarios;
using Aplicacion.Helpers;
using AutoMapper;
using Dominio.Enumeraciones;
using FluentValidation;
using MediatR;

namespace Aplicacion.Features.Usuarios.Commands.ActualizarUsuario
{
    /// <summary>
    /// Manejador que edita una cuenta: nombre, rol, vínculos, estado y, opcionalmente, contraseña.
    /// Un administrador no puede quitarse a sí mismo el rol ni desactivarse,
    /// para que el sistema nunca se quede sin quien lo administre.
    /// </summary>
    public class ActualizarUsuarioHandler
        : IRequestHandler<ActualizarUsuarioCommand, ResultadoAccion<UsuarioDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<ActualizarUsuarioDTO> _validador;
        private readonly IServicioContrasenas _contrasenas;
        private readonly IUsuarioActual _usuarioActual;

        public ActualizarUsuarioHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<ActualizarUsuarioDTO> validador,
            IServicioContrasenas contrasenas,
            IUsuarioActual usuarioActual)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validador = validador;
            _contrasenas = contrasenas;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoAccion<UsuarioDTO>> Handle(
            ActualizarUsuarioCommand request,
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

            // 2. Verificar que exista (aunque esté inactivo)
            var usuario = await _unitOfWork.Usuarios.ObtenerPorIdIncluyendoInactivoAsync(dto.Id);
            if (usuario == null)
                return ResultadoAccion<UsuarioDTO>.Falla("Usuario no encontrado.");

            // 3. Protección de la propia cuenta del administrador
            if (_usuarioActual.IdUsuario == usuario.Id &&
                (dto.Rol != RolUsuario.Administrador || !dto.Activo))
            {
                return ResultadoAccion<UsuarioDTO>.Falla(
                    "No puedes quitarte el rol de administrador ni desactivar tu propia cuenta.");
            }

            // 4. Vínculo con doctor o paciente
            var errorVinculo = await VinculosUsuario.ValidarAsync(_unitOfWork, dto.IdDoctor, dto.IdPaciente, usuario.Id);
            if (errorVinculo != null)
                return ResultadoAccion<UsuarioDTO>.Falla(errorVinculo);

            // 5. Aplicar cambios
            usuario.NombreCompleto = dto.NombreCompleto.Trim();
            usuario.Rol = dto.Rol;
            usuario.IdDoctor = dto.IdDoctor;
            usuario.IdPaciente = dto.IdPaciente;
            usuario.Activo = dto.Activo;
            usuario.FechaDeModificacion = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.NuevaContrasena))
                usuario.ContrasenaHash = _contrasenas.Cifrar(dto.NuevaContrasena);

            _unitOfWork.Usuarios.Actualizar(usuario);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<UsuarioDTO>.Exito(
                _mapper.Map<UsuarioDTO>(usuario), "Usuario actualizado exitosamente.");
        }
    }
}
