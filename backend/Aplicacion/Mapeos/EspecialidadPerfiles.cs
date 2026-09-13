using Aplicacion.DTOs.Especialidades;
using AutoMapper;
using Dominio.Entidades.Especialidades;

namespace Aplicacion.Mapeos
{
    public class EspecialidadPerfiles : Profile
    {
        public EspecialidadPerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearEspecialidadDTO, Especialidad>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarEspecialidadDTO, Especialidad>().ReverseMap();

            // Para consultar - mapea CantidadDoctores desde el conteo de la colección Doctores
            CreateMap<Especialidad, EspecialidadDTO>()
                .ForMember(
                    dest => dest.CantidadDoctores,
                    opt => opt.MapFrom(src => src.Doctores.Count))
                .ReverseMap();
        }
    }
}