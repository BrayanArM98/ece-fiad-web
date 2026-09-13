using Aplicacion.DTOs.Doctores;
using AutoMapper;
using Dominio.Entidades.Doctores;

namespace Aplicacion.Mapeos
{
    public class DoctorPerfiles : Profile
    {
        public DoctorPerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearDoctorDTO, Doctor>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarDoctorDTO, Doctor>().ReverseMap();

            // Para consultar - mapea NombreEspecialidad desde Especialidad.Nombre
            // y CantidadCitas desde el conteo de la colección Citas
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(
                    dest => dest.NombreEspecialidad,
                    opt => opt.MapFrom(src => src.Especialidad != null ? src.Especialidad.Nombre : string.Empty))
                .ForMember(
                    dest => dest.CantidadCitas,
                    opt => opt.MapFrom(src => src.Citas.Count))
                .ReverseMap();
        }
    }
}