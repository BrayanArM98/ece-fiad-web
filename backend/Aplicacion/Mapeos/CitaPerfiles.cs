using Aplicacion.DTOs.Citas;
using AutoMapper;
using Dominio.Entidades.Citas;

namespace Aplicacion.Mapeos
{
    public class CitaPerfiles : Profile
    {
        public CitaPerfiles()
        {
            // De DTO a Entidad (para crear)
            CreateMap<CrearCitaDTO, Cita>().ReverseMap();

            // Para actualización
            CreateMap<ActualizarCitaDTO, Cita>().ReverseMap();

            // Para consultar - mapea NombrePaciente y NombreDoctor desde las propiedades
            // calculadas NombreCompleto de cada relación, y EstadoTexto desde el enum.
            CreateMap<Cita, CitaDTO>()
                .ForMember(
                    dest => dest.NombrePaciente,
                    opt => opt.MapFrom(src => src.Paciente != null ? src.Paciente.NombreCompleto : string.Empty))
                .ForMember(
                    dest => dest.NombreDoctor,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.NombreCompleto : string.Empty))
                .ForMember(
                    dest => dest.EstadoTexto,
                    opt => opt.MapFrom(src => src.Estado.ToString()))
                .ReverseMap();
        }
    }
}