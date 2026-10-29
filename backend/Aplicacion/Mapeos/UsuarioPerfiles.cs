using Aplicacion.DTOs.Usuarios;
using AutoMapper;
using Dominio.Entidades.Usuarios;

namespace Aplicacion.Mapeos
{
    public class UsuarioPerfiles : Profile
    {
        public UsuarioPerfiles()
        {
            // El hash de la contraseña nunca sale de la API: UsuarioDTO no lo tiene
            CreateMap<Usuario, UsuarioDTO>();

            // La contraseña se cifra en el handler, no se mapea
            CreateMap<CrearUsuarioDTO, Usuario>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim().ToLowerInvariant()))
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.NombreCompleto.Trim()))
                .ForMember(dest => dest.ContrasenaHash, opt => opt.Ignore());
        }
    }
}
